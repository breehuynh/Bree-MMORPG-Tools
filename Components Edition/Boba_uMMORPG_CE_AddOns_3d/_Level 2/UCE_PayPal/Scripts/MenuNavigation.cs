// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

public class MenuNavigation : MonoBehaviour
{
    public static MenuNavigation INSTANCE;

    public void Awake()
    {
        INSTANCE = this;
    }

    public GameObject MainStoreScreen;
    public GameObject PurchaseScreen;

    public void selectStoreIcon()
    {
        if (StoreActions.INSTANCE.currentPurchaseStatus == StoreActions.PurchaseStatus.WAITING)
        {
            DialogScreenActions.INSTANCE.setContextConfirmAbortPayment();
            DialogScreenActions.INSTANCE.ShowDialogScreen();
            return;
        }

        MainStoreScreen.SetActive(true);
        PurchaseScreen.SetActive(false);
    }

    public void selectPurchaseIcon()
    {
        MainStoreScreen.SetActive(false);
        PurchaseScreen.SetActive(true);
    }
}
