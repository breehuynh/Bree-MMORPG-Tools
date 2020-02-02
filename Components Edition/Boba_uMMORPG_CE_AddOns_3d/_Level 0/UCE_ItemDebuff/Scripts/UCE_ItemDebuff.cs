// =======================================================================================
// Maintained by bobatea#9400 on Discord
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

// ITEM DEBUFF

[CreateAssetMenu(menuName = "uMMORPG Item/UCE Item Debuff", order = 999)]
public class UCE_ItemDebuff : UsableItem
{
    [Header("-=-=-=- UCE Item Debuff -=-=-=-")]
    [Tooltip("% Chance to remove the Buff from the target")]
    [Range(0, 1)] public float successChance;

    public BuffSkill[] removeBuffs;
    public float range;
    public string successMessage = "You removed {0} from: {1} ";

    [Header("-=-=-=- New Buff on Target? -=-=-=-")]
    public BuffSkill applyBuff;

    public int buffLevel;
    [Range(0, 1)] public float buffChance;

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public bool CheckTarget(Entity caster)
    {
        return caster.target != null && caster.CanAttack(caster.target);
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPointOnBounds(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= range;
        }
        destination = caster.transform.position;
        return false;
    }

    // -----------------------------------------------------------------------------------
    // Use
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory.slots[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            if (player.target != null && player.target is Entity && player.target.isAlive)
            {
                // always call base function too
                base.Use(player, inventoryIndex);

                foreach (BuffSkill removeBuff in removeBuffs)
                {
                    player.target.UCE_RemoveBuff(removeBuff, successChance);
                    player.UCE_TargetAddMessage(string.Format(successMessage, removeBuff.name, player.target.name));
                }

                player.target.UCE_ApplyBuff(applyBuff, buffLevel, buffChance);

                // decrease amount
                slot.DecreaseAmount(decreaseAmount);
                player.inventory.slots[inventoryIndex] = slot;
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
