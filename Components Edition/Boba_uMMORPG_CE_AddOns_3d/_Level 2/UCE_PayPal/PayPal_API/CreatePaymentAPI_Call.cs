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

public class CreatePaymentAPI_Call : MonoBehaviour
{
    public string accessToken;

    public string transactionDescription;

    public string itemName;

    public string itemDescription;

    public string itemPrice;

    public string itemCurrency;

    public TextAsset JSON_CreatePaymentRequest;

    //[HideInInspector]
    public PayPalCreatePaymentJsonResponse API_SuccessResponse;

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
        API_SuccessResponse = JsonUtility.FromJson<PayPalCreatePaymentJsonResponse>(responseText);
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

        byte[] formData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(createRequest()));

        string endpointURL = StoreProperties.INSTANCE.isUsingSandbox() ?
            "https://api.sandbox.paypal.com/v1/payments/payment" :
            "https://api.paypal.com/v1/payments/payment";

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

    private PayPalCreatePaymentJsonRequest createRequest()
    {
        //create skeleton request object
        PayPalCreatePaymentJsonRequest request = JsonUtility.FromJson<PayPalCreatePaymentJsonRequest>(JSON_CreatePaymentRequest.text);

        //map provided values into skeleton object
        request.transactions[0].amount.total = itemPrice;
        request.transactions[0].amount.currency = itemCurrency;
        request.transactions[0].description = transactionDescription;
        request.transactions[0].invoice_number = System.Guid.NewGuid().ToString();
        request.transactions[0].item_list.items[0].name = itemName;
        request.transactions[0].item_list.items[0].description = itemDescription;
        request.transactions[0].item_list.items[0].price = itemPrice;
        request.transactions[0].item_list.items[0].currency = itemCurrency;

        return request;
    }
}
