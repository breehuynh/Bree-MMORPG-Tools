// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// DAILY REWARDS UI SLOT
// ===================================================================================
public partial class UCE_UI_Slot_DailyRewards : MonoBehaviour
{
    public Image slotIcon;
    public Text slotText;

    // -----------------------------------------------------------------------------------
    // AddMessage
    // -----------------------------------------------------------------------------------
    public void AddMessage(string msg, Color color, Sprite icon = null)
    {
        slotText.color = color;
        slotText.text = msg;
        slotIcon.sprite = icon;
    }
}
