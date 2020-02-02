// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class StoreActions : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void openWindow(string url);

    public static StoreActions INSTANCE;

    private void Awake()
    {
        INSTANCE = this;
    }

    public GameObject MainStoreScreen;
    public GameObject PurchaseItemScreen;

    [HideInInspector] public UCE_Tmpl_PayPalProduct product;

    public Image PurchaseItemImageField;
    public Text PurchaseItemNameField;
    public Text PurchaseItemCostField;
    public Text PurchaseItemCurrCodeField;
    public Text PurchaseItemDescField;

    public Text PurchaseStatusField;
    public Text PurchaseActionText;
    public Text PurchasePromptText;
    public Button PurchaseActionButton;

    public GameObject PurchaseItemFields;
    public GameObject PurchaseScreenDividers;
    public GameObject PurchaseScreenSpinner;

    public enum PurchaseStatus
    {
        NO_ITEM_SELECTED,
        CREATING_PURCHASE,
        CHECKOUT_READY,
        WAITING,
        COMPLETE
    };

    public PurchaseStatus currentPurchaseStatus = PurchaseStatus.NO_ITEM_SELECTED;

    // Use this for initialization
    private void Start()
    {
    }

    public void OpenStore()
    {
        MainStoreScreen.SetActive(true);
        changePurchaseStatus(PurchaseStatus.NO_ITEM_SELECTED);
    }

    public void CloseStore()
    {
        gameObject.SetActive(false);
    }

    public void purchaseScreenActionButtonClick()
    {
        if (currentPurchaseStatus == PurchaseStatus.CHECKOUT_READY)
        {
            //Open Checkout page in browser
            string checkoutUrl = StoreProperties.INSTANCE.GetComponent<CreatePaymentAPI_Call>().API_SuccessResponse.links[1].href;

            if (Application.platform.Equals(RuntimePlatform.WebGLPlayer))
            {
                openWindow(checkoutUrl);
            }
            else
            {
                Application.OpenURL(checkoutUrl);
            }

            PaymentListener newPaymentListener = StoreProperties.INSTANCE.gameObject.AddComponent<PaymentListener>();

            CreatePaymentAPI_Call createPaymentAPI_Call = StoreProperties.INSTANCE.GetComponent<CreatePaymentAPI_Call>();
            PayPalCreatePaymentJsonResponse createPaymentResponse = createPaymentAPI_Call.API_SuccessResponse;

            newPaymentListener.accessToken = createPaymentAPI_Call.accessToken;
            newPaymentListener.listeningInterval = 10f;
            newPaymentListener.payID = createPaymentResponse.id;

            InvokeRepeating("checkForPurchaseStatusChange", 1f, 1f);
            changePurchaseStatus(PurchaseStatus.WAITING);
        }
        else if (currentPurchaseStatus == PurchaseStatus.WAITING)
        {
            DialogScreenActions.INSTANCE.setContextConfirmAbortPayment();
            DialogScreenActions.INSTANCE.ShowDialogScreen();
        }
    }

    public void resetCheckoutScreen()
    {
        Destroy(StoreProperties.INSTANCE.gameObject.GetComponent<CreatePaymentAPI_Call>());
        Destroy(StoreProperties.INSTANCE.gameObject.GetComponent<ShowPaymentAPI_Call>());
        Destroy(StoreProperties.INSTANCE.gameObject.GetComponent<ExecutePaymentAPI_Call>());
        Destroy(StoreProperties.INSTANCE.gameObject.GetComponent<PaymentListener>());
        changePurchaseStatus(PurchaseStatus.NO_ITEM_SELECTED);
    }

    public void OpenPurchaseItemScreen(StoreItemContent itemToPurchase)
    {
        //MenuNavigation.INSTANCE.selectPurchaseIcon ();
        MainStoreScreen.SetActive(false);
        PurchaseItemScreen.SetActive(true);

        product = itemToPurchase.product;

        PurchaseItemImageField.sprite = itemToPurchase.itemImage;
        PurchaseItemNameField.text = itemToPurchase.itemName;

        PurchaseItemCostField.text = string.Format("{0:N}", itemToPurchase.itemCost);
        PurchaseItemCostField.text = CurrencyCodeMapper.GetCurrencySymbol(StoreProperties.INSTANCE.currencyCode) + PurchaseItemCostField.text;

        PurchaseItemDescField.text = itemToPurchase.itemDesc;
        PurchaseItemCurrCodeField.text = StoreProperties.INSTANCE.currencyCode;

        changePurchaseStatus(PurchaseStatus.CREATING_PURCHASE);

        CreatePaymentAPI_Call existingCreatePaymentAPIcall = StoreProperties.INSTANCE.gameObject.GetComponent<CreatePaymentAPI_Call>();
        if (existingCreatePaymentAPIcall != null)
        {
            Destroy(existingCreatePaymentAPIcall);
        }

        CreatePaymentAPI_Call createPaymentAPICall = StoreProperties.INSTANCE.gameObject.AddComponent<CreatePaymentAPI_Call>();

        createPaymentAPICall.accessToken = StoreProperties.INSTANCE.GetComponent<GetAccessTokenAPI_Call>().API_SuccessResponse.access_token;
        createPaymentAPICall.transactionDescription = "purchased 1 item from x game";
        createPaymentAPICall.itemCurrency = StoreProperties.INSTANCE.currencyCode;
        createPaymentAPICall.itemDescription = itemToPurchase.itemDesc;
        createPaymentAPICall.itemName = itemToPurchase.itemName;
        createPaymentAPICall.itemPrice = itemToPurchase.itemCost;
        createPaymentAPICall.JSON_CreatePaymentRequest = Resources.Load("Misc/CreatePaymentRequestBody") as TextAsset;

        InvokeRepeating("checkForPurchaseStatusChange", 1f, 1f);
    }

    private void checkForPurchaseStatusChange()
    {
        switch (currentPurchaseStatus)
        {
            case PurchaseStatus.NO_ITEM_SELECTED:
                {
                    //DO NOTHING
                }
                break;

            case PurchaseStatus.CREATING_PURCHASE:
                {
                    if (StoreProperties.INSTANCE.gameObject.GetComponent<CreatePaymentAPI_Call>() != null && StoreProperties.INSTANCE.gameObject.GetComponent<CreatePaymentAPI_Call>().API_SuccessResponse != null)
                    {
                        changePurchaseStatus(PurchaseStatus.CHECKOUT_READY);
                    }
                }
                break;

            case PurchaseStatus.CHECKOUT_READY:
                {
                    //DO NOTHING
                }
                break;

            case PurchaseStatus.WAITING:
                {
                    if (StoreProperties.INSTANCE.gameObject.GetComponent<PaymentListener>().listenerStatus == PaymentListener.ListenerState.SUCCESS)
                    {
                        changePurchaseStatus(PurchaseStatus.COMPLETE);
                    }

                    //DO NOTHING
                }
                break;

            case PurchaseStatus.COMPLETE:
                {
                    //DO NOTHING
                }
                break;
        }
    }

    public void changePurchaseStatus(PurchaseStatus newStatus)
    {
        switch (newStatus)
        {
            case PurchaseStatus.NO_ITEM_SELECTED:
                {
                    CancelInvoke("checkForPurchaseStatusChange");
                    PurchaseItemFields.SetActive(false);
                    PurchaseActionButton.gameObject.SetActive(true);
                    PurchaseScreenSpinner.SetActive(false);
                    PurchaseActionText.text = "Return";
                    PurchaseStatusField.text = "No Item Selected";
                    PurchasePromptText.text = "No item is currently selected.";
                }
                break;

            case PurchaseStatus.CREATING_PURCHASE:
                {
                    PurchaseItemFields.SetActive(true);
                    PurchaseActionButton.gameObject.SetActive(false);
                    PurchaseScreenSpinner.SetActive(true);
                    PurchaseStatusField.text = "Creating Purchase...";
                    PurchasePromptText.text = "Please wait while the purchase is being set up for the following item:";
                }
                break;

            case PurchaseStatus.CHECKOUT_READY:
                {
                    PurchaseItemFields.SetActive(true);
                    PurchaseActionButton.gameObject.SetActive(true);
                    PurchaseScreenSpinner.SetActive(false);
                    PurchaseActionText.text = "Checkout";
                    PurchaseStatusField.text = "Checkout Ready";
                    PurchasePromptText.text = "Please click the 'Checkout' button to complete the purchase for this item with PayPal:";
                    CancelInvoke("checkForPurchaseStatusChange");
                }
                break;

            case PurchaseStatus.WAITING:
                {
                    PurchaseItemFields.SetActive(true);
                    PurchaseActionButton.gameObject.SetActive(true);
                    PurchaseScreenSpinner.SetActive(true);
                    PurchaseActionText.text = "Cancel";
                    PurchaseStatusField.text = "Waiting";
                    PurchasePromptText.text = "A Paypal tab has been opened in your web browser to complete the purchase for the following item:";
                }
                break;

            case PurchaseStatus.COMPLETE:
                {
                    PurchaseItemFields.SetActive(true);
                    PurchaseActionButton.gameObject.SetActive(false);
                    PurchaseScreenSpinner.SetActive(false);
                    PurchaseStatusField.text = "Purchase Completed!";
                    PurchasePromptText.text = "Purchase complete!";
                    CancelInvoke("checkForPurchaseStatusChange");
                    DialogScreenActions.INSTANCE.setContextPurchaseSuccess();
                    DialogScreenActions.INSTANCE.ShowDialogScreen();
                }
                break;
        }

        currentPurchaseStatus = newStatus;
    }
}
