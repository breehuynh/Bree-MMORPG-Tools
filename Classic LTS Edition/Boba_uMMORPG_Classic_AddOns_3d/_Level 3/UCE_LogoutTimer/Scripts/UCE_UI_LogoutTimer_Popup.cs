// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

public class UCE_UI_LogoutTimer_Popup : MonoBehaviour
{
    public GameObject panel;

    public static UCE_UI_LogoutTimer_Popup singleton;

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    private void Start()
    {
        if (singleton == null) singleton = this;
    }

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show()
    {
        if (panel.activeSelf) return;
        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // Hide
    // -----------------------------------------------------------------------------------
    public void Hide()
    {
        if (!panel.activeSelf) return;
        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
}
