/*
	Written by: guzuligo@gmail.com
	Licence: https://opensource.org/licenses/MIT
	
	Modified by Fhiz
 */
 
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;

public class Patreon :MonoBehaviour {

	[Header("Client & API Keys")]
	[Tooltip("Get it from\n\n https://www.patreon.com/portal  ")]
	public string client_id;
	private string client_secret="";
	[Tooltip("Make sure you write it on patreon client page including the port. Example: http://localhost:8080/")]
	public string redirect_uri="http://localhost";
	public int port=8080;
	
	//listener
	public delegate void onStatus(string text);
	public delegate void onPageReturn(JsonValue jsonData_);
	/// <summary>
	/// If there is an error, onError(string errorName) will be called
	/// </summary>
	public onStatus onError;
	/// <summary>
	/// If connection successful, onConnect(string successMessage) will be called
	/// </summary>
	public onStatus onConnect;
	//public onStatus onToken;
	void _onerror(string text){
		if (onError!=null)
			onError(text);
		status=text;
	}

	void _onconnect(string text){
		if (onConnect!=null)
			onConnect(text);
		status=text;
	}

	[Header("User Data")]
	public string email="";
	public float pledge=0;
	
	public List<string> rewards=new List<string>();
	[Header("Debugging")]
	
	[Tooltip("Status message for debugging")]
	public string status;
	[Tooltip("Will make hasReward function always return true"+
	 		 " if current patrion is guzuligo@gmail.com")]
	public bool freepassForGuzu=true;
	void updateUserData(){
		email=userData.get("data","attributes","email");
		status="updating user data";
		//find rewards
		if (userData.has("included")){
			
			int i=0;
			rewards=new List<string>();
			string type_="";
			bool exitWhile=false;
			while (!exitWhile && userData.has("included",i.ToString())){
				type_=userData.get("included",i.ToString(),"type");
				switch(type_){
					case "reward":
						if (int.Parse(userData.get("included",i.ToString(),"id"))>0){
						rewards.Add(userData.get("included",i.ToString(),"attributes","title") );
						}else exitWhile=true;
						break;
					case "pledge":
						pledge=0.01f*float.Parse(
							userData.get("included",i.ToString(),"attributes","amount_cents")
							);
					break;
				}
				i++;
			}
		}

		_onconnect("Connected");

	}

	/// <summary>
	/// Checks if current user has reward
	/// </summary>
	/// <param name="_rewardTitle">The title of the reward in the patreon page
	/// </param>
	/// <returns>returns true if user has reward with title _rewardTitle
	/// </returns>
	public bool hasReward(string _rewardTitle){
		if (freepassForGuzu && email=="guzuligo@gmail.com")return true;
		return rewards.Count>0 && rewards.Contains(_rewardTitle);
	}


	
	string code=""; //,token="";
	JsonValue tokenData=new JsonValue();
	JsonValue userData=new JsonValue();
	string ppage="https://www.patreon.com/api/oauth2/";
	//bool loggedIn=false;
	
	
	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}

	bool _gettingCode=false;
	/// <summary>
	/// Connects to patreon page.
	/// </summary>
	public void connect(){
		_gettingCode=true;
		status="Waiting patreon webpage";
		StartCoroutine(openPatreonPageWrapper());
	}

	Thread t_;
	IEnumerator openPatreonPageWrapper(){
		t_=new Thread(openPatreonPage);
		t_.Start();
		string patRequest2="https://www.patreon.com/oauth2/authorize?"
            +"response_type=code"+"&client_id="+client_id
            +"&redirect_uri="+redirect_uri+":"+port.ToString()+"/";

		Application.OpenURL(patRequest2);

		while(code=="" && _gettingCode)yield return null;

		if (code=="")
			_onerror("Failed to get code.");
		if (code!="")yield return ( getToken(true));
		
		//if (onError!=null)
		

	}

/// <summary>
/// Use it if you use connect() and wish to cancel the connection 
/// before it is complete.
/// </summary>
	public void cancel(){
		if (!_gettingCode)return;
		if (!t_.IsAlive)return;
		_gettingCode=false;
		t_.Abort();
		server.Disconnect(false);
	}

	/// <summary>
	/// Refreshes the token by requesting a new token from patreon page
	/// </summary>
	public void refreshToken(){
		StartCoroutine(getToken(false,true));
	}

/// <summary>
/// If you have token saved in json format, you can skip the
/// authentication proccess and patreon page access by providing
/// the JSON here.
/// </summary>
/// <param name="jsontext">Client information in JSON format</param>
	public void setTokenFromJson(string jsontext,bool loaduserdata_=true){
		tokenData.parse(jsontext);

		if (loaduserdata_){
			StartCoroutine( _getUser());
		}
		
	}

	public JsonValue getUserJsonData(){
		return userData;
	}

/// <summary>
/// If you want to save the token to use later, use this.
/// Afterwards, use setTokenFromJson function to restore token to
/// skip the login process.
/// </summary>
/// <returns></returns>
	public string getTokenAsJson(){
		return tokenData.value;
	}

	Dictionary<string,string> _myToken(){
		Dictionary<string,string> data=new Dictionary<string,string>();
		data.Add("Authorization","Bearer "+tokenData.get("access_token"));
		return data;
	}

	IEnumerator _getUser() {
	
		status="Getting user";		
		WWW w=new WWW("https://www.patreon.com/api/oauth2/api/current_user",null,_myToken());
		
		yield return w;
		if(w.text==""){
			_onerror("Failed.");
			yield break;
		}
		userData.parse(w.text);

		if (!userData.has("data","attributes","email"))
			_onerror("Failed to get user.");
		else
			updateUserData();
	}

	public IEnumerator getPage(string api_auth2_="api/current_user",onPageReturn callback_=null){
		if (api_auth2_==null || api_auth2_=="")
			api_auth2_="api/current_user";
		WWW w=new WWW(ppage+api_auth2_,null,_myToken());
		yield return w;
		getPageCallbackJsonValue=new JsonValue();
		getPageCallbackJsonValue.parse(w.text);
		if (callback_!=null)
			callback_(getPageCallbackJsonValue);
	}
	public JsonValue getPageCallbackJsonValue;

	
	//bool refresh_=false;
	IEnumerator getToken(bool getUser_=false,bool refresh_=false){
		
		WWWForm data=new WWWForm();
		data.AddField("client_id",client_id);
        data.AddField("client_secret",client_secret);
 		if (!refresh_){
                //NEW
				status="Getting token";
                data.AddField("grant_type","authorization_code");
                data.AddField("code",code);code="";
                data.AddField("redirect_uri",redirect_uri+":"+port+"/");
            }
             
            else{
                //REFRESH
				status="Refreshing token";
                data.AddField("grant_type","refresh_token");
                data.AddField("refresh_token",tokenData.get("refresh_token"));
                //TODO
				
            }
	
		WWW w=new WWW(ppage+"token",data);
		yield return w;
		if (w.text==null || w.text=="")
			_onerror("Failed.");
		
		tokenData.parse(w.text);
		if (!tokenData.has("access_token"))
			_onerror("Failed to get token.");
			else{
				if (getUser_)
					StartCoroutine( _getUser());
				if(refresh_)
					_onconnect("Token Refreshed.");
			}
	}

	

	Socket server;
	void openPatreonPage(){
		
		IPEndPoint serverEP=new IPEndPoint(IPAddress.Any,port);
		server =new Socket(AddressFamily.InterNetwork,
                   SocketType.Stream, ProtocolType.Tcp);
		
		server.Bind(serverEP);
		server.Listen(1);
		Socket _s=server.Accept();
		
		byte[] b=new byte[1000];
		_s.Receive(b);
		string s=System.Text.Encoding.UTF8.GetString(b);
		
		string toSend="HTTP/1.1 200 OK\nContent-Type: text/html\nConnection: close\n\n"+
			"<html><H1>CONNECTED</H1><pre>You can close this page.</html>";

		NetworkStream st=new NetworkStream(_s);
		b=System.Text.Encoding.UTF8.GetBytes(toSend);
		st.Write(b,0,b.Length);
		st.Flush();
		st.Close();
		
		_s.Close();
		server.Close();
		code=extractCode(s);
		
	}

	string extractCode(string s){
		int a;
		a=s.IndexOf("code");
		if (a!=-1){
			s=s.Substring(a+5);
			a=s.IndexOf("&");
			if (a!=-1)
				s=s.Substring(0,a);
		return s;
		}
		return "";
	}
}
