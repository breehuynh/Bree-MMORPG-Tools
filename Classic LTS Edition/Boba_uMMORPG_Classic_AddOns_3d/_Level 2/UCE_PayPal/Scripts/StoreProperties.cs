// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

public class StoreProperties : MonoBehaviour
{
    public static StoreProperties INSTANCE;

    private void Awake()
    {
        INSTANCE = this;
    }

    public string currencyCode;
    /*
        public enum StoreTheme {
            BASIC,
            AQUA_PAPER,
            DARK_STONE,
            DIAMOND,
            BUBBLES,
            MARBLE,
            METAL,
            MOSS,
            PINSTRIPE,
            WEATHERED,
            WOOD
        }

        public StoreTheme storeTheme = StoreTheme.BASIC;
    */

    public enum PayPalEndpoint
    {
        SANDBOX,
        LIVE
    }

    public PayPalEndpoint payPalEndpoint;

    public GameObject[] storeScreens;

    public Text[] textBoxes;

    private void OnEnable()
    {
        //if basic is selected then don't change background
        /*if (storeTheme != StoreTheme.BASIC) {
            for (int i=0; i<storeScreens.Length; i++) {
                GameObject nextStoreScreen = storeScreens [i];
                nextStoreScreen.GetComponent<Image> ().sprite = Resources.Load <Sprite> ("StoreThemes/" + storeTheme.ToString ());
                nextStoreScreen.GetComponent<Image> ().color = Color.white;
            }
        }*/

        //setTextColours ();

        GetComponent<GetAccessTokenAPI_Call>().enabled = true;

        InvokeRepeating("checkForValidPayPalCreds", 1f, 1f);

        DialogScreenActions.INSTANCE.setContextStoreOpen();
        DialogScreenActions.INSTANCE.ShowDialogScreen();
    }

    private void OnDisable()
    {
        foreach (GameObject go in storeScreens)
        {
            go.SetActive(false);
        }
    }

    private void checkForValidPayPalCreds()
    {
        bool validCreds = GetComponent<GetAccessTokenAPI_Call>().API_SuccessResponse.access_token != "";
        bool invalidCreds = GetComponent<GetAccessTokenAPI_Call>().API_ErrorResponse.message != "";

        if (validCreds)
        {
            StoreActions.INSTANCE.OpenStore();
            DialogScreenActions.INSTANCE.HideDialogScreen();
            CancelInvoke("checkForValidPayPalCreds");
        }
        else if (invalidCreds)
        {
            CancelInvoke("checkForValidPayPalCreds");
            DialogScreenActions.INSTANCE.setContextStoreLoadFailure();
        }
        else
        {
            //keep waiting for one of above conditions to happen
        }
    }

    public bool isUsingSandbox()
    {
        return payPalEndpoint == PayPalEndpoint.SANDBOX;
    }

    private void setTextColours()
    {
        Color textColorToUse = Color.black;
        /*
                switch (storeTheme) {
                case StoreTheme.METAL :
                case StoreTheme.DARK_STONE :
                case StoreTheme.BASIC :
                case StoreTheme.WEATHERED : {
                        textColorToUse = Color.white;
                    } break;
                }
        */
        foreach (Text t in textBoxes)
        {
            t.color = textColorToUse;
        }
    }
}
