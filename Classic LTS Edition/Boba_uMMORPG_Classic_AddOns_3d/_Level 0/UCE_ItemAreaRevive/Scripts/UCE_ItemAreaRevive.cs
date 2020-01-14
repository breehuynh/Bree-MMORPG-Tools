// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Text;
using System.Linq;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

// AREA REVIVE ITEM

[CreateAssetMenu(menuName = "uMMORPG Item/UCE Item Area Revive", order = 999)]
public class UCE_ItemAreaRevive : UsableItem
{
    [Header("-=-=-=- UCE Area Revive Item -=-=-=-")]
    [Range(0, 1)] public float successChance;

    public float range;
    public int healsHealth;
    public int healsMana;

    [Header("-=-=-=- Buff on Target -=-=-=-")]
    public BuffSkill applyBuff;

    public int buffLevel;
    [Range(0, 1)] public float buffChance;

    public string successMessage = "You revived: ";
    public string failedMessage = "You failed to revive: ";

    public bool reverseTargeting;

    public bool affectOwnParty;
    public bool affectOwnGuild;
    public bool affectOwnRealm;

    public bool affectPlayers;
    public bool affectEnemies;
    public bool affectPets;

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public bool CheckTarget(Entity caster)
    {
        caster.target = caster;
        return true;
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        destination = caster.transform.position;
        return true;
    }

    // -----------------------------------------------------------------------------------
    // Use
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        List<Entity> targets = new List<Entity>();
        ItemSlot slot = player.inventory[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            // always call base function too
            base.Use(player, inventoryIndex);

            targets = player.UCE_GetCorrectedTargetsInSphere(player.transform, range, true, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectEnemies, affectPets);

            foreach (Entity target in targets)
            {
                if (UnityEngine.Random.value <= successChance)
                {
                    target.health += healsHealth;
                    target.mana += healsMana;
                    target.UCE_ApplyBuff(applyBuff, buffLevel, buffChance);
                    target.UCE_OverrideState("IDLE");
                    player.UCE_TargetAddMessage(successMessage + target.name);
                }
                else
                {
                    player.UCE_TargetAddMessage(failedMessage + target.name);
                }
            }

            // decrease amount
            slot.DecreaseAmount(decreaseAmount);
            player.inventory[inventoryIndex] = slot;
        }

        targets.Clear();
    }

    // -----------------------------------------------------------------------------------
}
