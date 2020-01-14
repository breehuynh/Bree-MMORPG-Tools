// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAccessTokenAPI_Call : MonoBehaviour
{
    public string clientID;
    public string secret;

    //[HideInInspector]
    public PayPalGetAccessTokenJsonResponse API_SuccessResponse;

    //[HideInInspector]
    public PayPalErrorJsonResponse API_ErrorResponse;

    // Use this for initialization
    private void Start()
    {
        StartCoroutine(MakePayAPIcall());
    }

    private void handleSuccessResponse(string responseText)
    {
        //attempt to parse reponse text
        API_SuccessResponse = JsonUtility.FromJson<PayPalGetAccessTokenJsonResponse>(responseText);
    }

    private void handleErrorResponse(string responseText, string errorText)
    {
        //attempt to parse error response
        API_ErrorResponse = JsonUtility.FromJson<PayPalErrorJsonResponse>(responseText);

        //if no responseText and only error text
        if (API_ErrorResponse == null)
        {
            API_ErrorResponse = new PayPalErrorJsonResponse();
            API_ErrorResponse.message = errorText;
        }
    }

    private IEnumerator MakePayAPIcall()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();

        headers.Add("Accept", "application/json");
        headers.Add("Accept-Language", "en_US");
        headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(clientID + ":" + secret)));

        WWWForm postData = new WWWForm();

        postData.AddField("grant_type", "client_credentials");

        string endpointURL = StoreProperties.INSTANCE.isUsingSandbox() ?
            "https://api.sandbox.paypal.com/v1/oauth2/token" :
            "https://api.paypal.com/v1/oauth2/token";
#pragma warning disable
        WWW www = new WWW(endpointURL, postData.data, headers);
#pragma warning restore

        yield return www;

        //if ok response
        if (www.error == null)
        {
            handleSuccessResponse(www.text);
        }
        else
        {
            handleErrorResponse(www.text, www.error);
        }
    }
}
