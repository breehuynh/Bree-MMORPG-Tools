// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UCE UI PROMPT

public class UCE_UI_Prompt : MonoBehaviour
{
    public GameObject panel;
    public Text messageText;
    public bool forceUseChat;

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(string message)
    {
        messageText.text = message;
        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
}
