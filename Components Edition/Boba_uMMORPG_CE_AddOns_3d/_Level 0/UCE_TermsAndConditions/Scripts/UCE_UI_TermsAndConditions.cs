// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE UI TERMS AND CONDITIONS

public partial class UCE_UI_TermsAndConditions : MonoBehaviour
{
    public GameObject panel;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Start()
    {
        if (PlayerPrefs.GetInt("UCETermsAndConditions", 0) == 1)
            Inactivate();
        else
            panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // OnClickAccept
    // -----------------------------------------------------------------------------------
    public void OnClickAccept()
    {
        PlayerPrefs.SetInt("UCETermsAndConditions", 1);
        PlayerPrefs.Save();
        Inactivate();
    }

    // -----------------------------------------------------------------------------------
    // OnClickDecline
    // -----------------------------------------------------------------------------------
    public void OnClickDecline()
    {
        Application.Quit();
    }

    // -----------------------------------------------------------------------------------
    // Inactivate
    // -----------------------------------------------------------------------------------
    private void Inactivate()
    {
        panel.SetActive(false);
        Destroy(this.gameObject);
    }

    // -----------------------------------------------------------------------------------
}
