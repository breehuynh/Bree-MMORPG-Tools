// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE UI HONOR SHOP BUTTON

public partial class UCE_UI_HonorShopButton : MonoBehaviour
{
    public GameObject honorShopCurrencyPanel;

    // -----------------------------------------------------------------------------------
    // ShowHonorShopCurrencyPanel
    // -----------------------------------------------------------------------------------
    public void ShowHonorShopCurrencyPanel()
    {
        if (honorShopCurrencyPanel.activeInHierarchy)
            honorShopCurrencyPanel.SetActive(false);
        else
            honorShopCurrencyPanel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
}
