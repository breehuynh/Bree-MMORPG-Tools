// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using UnityEngine;
using Mirror;

// SKILL EXPERIENCE - ITEM

[CreateAssetMenu(menuName = "uMMORPG Item/UCE Item Skill Experience", order = 998)]
public class UCE_Tmpl_ItemSkillExperience : UsableItem
{
    [Header("-=-=-=- Skill Experience Item -=-=-=-")]
    [Tooltip("The amount of Skill Experience gained when used")]
    public int skillExperienceBonus;

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    // -----------------------------------------------------------------------------------
    // Use
    // @Server
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            // always call base function too
            base.Use(player, inventoryIndex);

            // -- Decrease Amount
            if (decreaseAmount != 0)
            {
                slot.DecreaseAmount(skillExperienceBonus);
                player.inventory[inventoryIndex] = slot;
            }

            // -- Activate Teleport
            player.skillExperience += skillExperienceBonus;
        }
    }

    // -----------------------------------------------------------------------------------
    // Tooltip
    // @Client
    // -----------------------------------------------------------------------------------
    public override string ToolTip()
    {
        StringBuilder tip = new StringBuilder(base.ToolTip());
        tip.Replace("{SKILLEXPERIENCE}", skillExperienceBonus.ToString());
        return tip.ToString();
    }

    // -----------------------------------------------------------------------------------
}
