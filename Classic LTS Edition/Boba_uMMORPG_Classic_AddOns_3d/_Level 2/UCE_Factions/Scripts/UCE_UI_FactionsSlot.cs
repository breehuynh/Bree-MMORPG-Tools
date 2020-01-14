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
// HARVESTING SLOT
// ===================================================================================
public class UCE_UI_FactionsSlot : MonoBehaviour
{
    public Text nameText;
    public Image factionIcon;
    public Slider ratingSlider;
    public UIShowToolTip tooltip;

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(UCE_Faction faction)
    {
        UCE_Tmpl_Faction data = faction.data;

        nameText.text = data.name + " [" + data.getRank(faction.rating) + "]";
        factionIcon.sprite = data.image;
        ratingSlider.value = faction.rating;

        tooltip.enabled = true;
        tooltip.text = data.name + " [" + faction.rating.ToString() + " " + data.getRank(faction.rating) + "]\n" + data.description;
    }

    // -----------------------------------------------------------------------------------
}
