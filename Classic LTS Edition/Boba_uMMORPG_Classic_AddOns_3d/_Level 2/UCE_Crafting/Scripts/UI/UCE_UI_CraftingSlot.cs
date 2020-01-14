// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using UnityEngine;
using UnityEngine.UI;

#if _iMMOCRAFTING

// ===================================================================================
// CRAFTING SLOT
// ===================================================================================
public class UCE_UI_CraftingSlot : MonoBehaviour
{
    public Text nameText;
    public Image professionIcon;
    public Slider expSlider;
    public UIShowToolTip tooltip;

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(UCE_CraftingProfession p)
    {
        float value = 0;

        string lvl = " [L" + p.level.ToString() + "/" + p.maxlevel.ToString() + "]";

        value = p.experiencePercent;

        nameText.text = p.template.name + lvl;
        professionIcon.sprite = p.template.image;
        expSlider.value = value;

        tooltip.enabled = true;
        tooltip.text = ToolTip(p.template);
    }

    // -----------------------------------------------------------------------------------
    // ToolTip
    // -----------------------------------------------------------------------------------
    public string ToolTip(UCE_CraftingProfessionTemplate tpl)
    {
        StringBuilder tip = new StringBuilder();

        tip.Append(tpl.name + "\n");
        tip.Append(tpl.toolTip + "\n");

        return tip.ToString();
    }

    // -----------------------------------------------------------------------------------
}

#endif
