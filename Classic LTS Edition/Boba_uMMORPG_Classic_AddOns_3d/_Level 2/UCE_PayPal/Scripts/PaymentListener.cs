// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

public class PaymentListener : MonoBehaviour
{
    public string payID;

    public string accessToken;

    public float listeningInterval;

    public enum ListenerState { NOT_STARTED, LISTENING, VERIFIED, SUCCESS, FAILURE };

    public ListenerState listenerStatus;

    private void Start()
    {
        StartListening();
    }

    private void Update()
    {
        if (listenerStatus == ListenerState.LISTENING)
        {
            checkAPI_Response();
        }

        if (listenerStatus == ListenerState.VERIFIED)
        {
            checkExecutePaymentAPI_Response();
        }
    }

    // Use this for initialization
    public void StartListening()
    {
        listenerStatus = ListenerState.LISTENING;
        InvokeRepeating("pollPayPalShowPaymentAPI", 0f, listeningInterval);
    }

    private void pollPayPalShowPaymentAPI()
    {
        ShowPaymentAPI_Call lastAPIcall = this.GetComponent<ShowPaymentAPI_Call>();

        if (lastAPIcall != null)
        {
            Destroy(lastAPIcall);
        }

        ShowPaymentAPI_Call apiCall = this.gameObject.AddComponent<ShowPaymentAPI_Call>();

        apiCall.accessToken = accessToken;
        apiCall.payID = payID;
    }

    private void pollPayPalExecutePaymentAPI()
    {
        ExecutePaymentAPI_Call lastAPIcall = this.GetComponent<ExecutePaymentAPI_Call>();

        if (lastAPIcall != null)
        {
            Destroy(lastAPIcall);
        }

        ExecutePaymentAPI_Call apiCall = this.gameObject.AddComponent<ExecutePaymentAPI_Call>();

        apiCall.accessToken = accessToken;
        apiCall.paymentID = payID;
        apiCall.payerID = GetComponent<ShowPaymentAPI_Call>().API_SuccessResponse.payer.payer_info.payer_id;
    }

    private void checkAPI_Response()
    {
        ShowPaymentAPI_Call lastAPIcall = GetComponent<ShowPaymentAPI_Call>();

        if (lastAPIcall != null && lastAPIcall.API_SuccessResponse != null && lastAPIcall.API_SuccessResponse.payer.status == "VERIFIED")
        {
            CancelInvoke("pollPayPalShowPaymentAPI");
            transitionToVerified();
        }
    }

    private void checkExecutePaymentAPI_Response()
    {
        ExecutePaymentAPI_Call executePaymentAPI_Call = this.gameObject.GetComponent<ExecutePaymentAPI_Call>();

        if (executePaymentAPI_Call != null && executePaymentAPI_Call.API_SuccessResponse != null && executePaymentAPI_Call.API_SuccessResponse.state == "approved")
        {
            transitionToSuccess();
        }

        if (executePaymentAPI_Call != null && executePaymentAPI_Call.API_SuccessResponse != null && executePaymentAPI_Call.API_SuccessResponse.state == "failed")
        {
            transitionToFailure();
        }
    }

    private void transitionToVerified()
    {
        listenerStatus = ListenerState.VERIFIED;

        InvokeRepeating("pollPayPalExecutePaymentAPI", 0f, listeningInterval);
    }

    private void transitionToSuccess()
    {
        listenerStatus = ListenerState.SUCCESS;
        CancelInvoke("pollPayPalExecutePaymentAPI");
    }

    private void transitionToFailure()
    {
        listenerStatus = ListenerState.FAILURE;
        CancelInvoke("pollPayPalExecutePaymentAPI");
    }
}
