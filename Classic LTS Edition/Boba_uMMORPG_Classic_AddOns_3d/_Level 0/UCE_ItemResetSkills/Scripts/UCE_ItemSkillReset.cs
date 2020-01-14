// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Text;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

// Reset ITEM

[CreateAssetMenu(menuName = "uMMORPG Item/UCE Item Reset Skill", order = 999)]
public class UCE_ItemSkillReset : UsableItem
{
    [Header("-=-=-=- UCE Reset Skill Item -=-=-=-")]
    public string resetMessage = "Your skill points have been reset!";

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    // -----------------------------------------------------------------------------------
    // Use
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            // always call base function too
            base.Use(player, inventoryIndex);

            long mySkillPoints = 0;

            for (int skillIndex = 0; skillIndex < player.skills.Count; skillIndex++)
            {
                Skill skill = player.skills[skillIndex];

                if (skill.level > 0)
                {
                    for (int j = skill.level; j > 0; j--)
                    {
                        mySkillPoints += skill.data.requiredSkillExperience.Get(j);
                    }
                    skill.level = 0;
                    player.skills[skillIndex] = skill;
                }
            }

            if (mySkillPoints > 0)
            {
                player.skillExperience += mySkillPoints;
            }

            player.UCE_TargetAddMessage(resetMessage);

            // decrease amount
            slot.DecreaseAmount(decreaseAmount);
            player.inventory[inventoryIndex] = slot;
        }
    }

    // -----------------------------------------------------------------------------------
}
