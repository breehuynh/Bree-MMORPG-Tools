using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Example2 : MonoBehaviour {

	public Text textArea;
	Patreon patreon;
	// Use this for initialization
	void Start () {
		patreon=GetComponent<Patreon>();
		patreon.onConnect=setTextValueOnConnect;
		patreon.onError=setTextValue;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)){
			textArea.text="";
			patreon.connect();
			
		}
		
	}
	void setTextValue(string text){
		textArea.text=text;
	}
	void setTextValueOnConnect(string text){
		textArea.text="Connected as "+patreon.email
			+"\nwith "+patreon.rewards.Count+" rewards.";
	}
}
