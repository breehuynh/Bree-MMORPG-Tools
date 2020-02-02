// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// UCE_UI_AssetBundleDownloader
// ===================================================================================
public class UCE_UI_AssetBundleDownloader : MonoBehaviour
{

    public static UCE_UI_AssetBundleDownloader singleton;

    public GameObject panel;
    public Text messageText;
    public Slider progressSlider;
    public Text progressText;
    public Button cancelButton;

    // -------------------------------------------------------------------------------
    // UCE_UI_AssetBundleDownloader
    // -------------------------------------------------------------------------------
    public UCE_UI_AssetBundleDownloader()
    {
        if (singleton == null) singleton = this;
    }

    // -------------------------------------------------------------------------------
    // Show
    // -------------------------------------------------------------------------------
    public void Show()
    {
        cancelButton.onClick.SetListener(() => {
            NetworkManagerMMO.Quit();
        });
        panel.SetActive(true);
    }

    // -------------------------------------------------------------------------------
    // Hide
    // -------------------------------------------------------------------------------
    public void Hide()
    {
        panel.SetActive(false);
    }

    // -------------------------------------------------------------------------------
    // UpdateUI
    // -------------------------------------------------------------------------------
    public void UpdateUI(string sMessage, float fAmount=0f, string sText="")
    {

        if (!string.IsNullOrWhiteSpace(sMessage))
            messageText.text        = sMessage;

        if (fAmount <= 0 && string.IsNullOrWhiteSpace(sText) && string.IsNullOrWhiteSpace(sMessage))
        {
            progressSlider.gameObject.SetActive(false);
        }
        else
        {
            progressSlider.value = fAmount;

            if (!string.IsNullOrWhiteSpace(sText))
                progressText.text = sText;

            progressSlider.gameObject.SetActive(true);
        }

        if (!panel.activeInHierarchy)
            Show();

    }

    // -------------------------------------------------------------------------------

}

// ===================================================================================
