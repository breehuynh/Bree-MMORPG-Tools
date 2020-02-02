// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutePaymentAPI_Call : MonoBehaviour
{
    public string paymentID;

    public string payerID;

    public string accessToken;

    //[HideInInspector]
    public PayPalExecutePaymentJsonResponse API_SuccessResponse;

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
        API_SuccessResponse = JsonUtility.FromJson<PayPalExecutePaymentJsonResponse>(responseText);
    }

    private void handleErrorResponse(string responseText, string errorText)
    {
        //attempt to parse error response
        API_ErrorResponse = JsonUtility.FromJson<PayPalErrorJsonResponse>(responseText);
    }

    private IEnumerator MakePayAPIcall()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();

        headers.Add("Content-Type", "application/json");
        headers.Add("Authorization", "Bearer " + accessToken);

        PayPalExecutePaymentJsonRequest request = new PayPalExecutePaymentJsonRequest();
        request.payer_id = payerID;

        byte[] formData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(request));

        string baseEndpointURL = StoreProperties.INSTANCE.isUsingSandbox() ?
            "https://api.sandbox.paypal.com/v1/payments/payment/" :
            "https://api.paypal.com/v1/payments/payment/";

        string endpointURL = baseEndpointURL + paymentID + "/execute";
#pragma warning disable
        WWW www = new WWW(endpointURL, formData, headers);
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
