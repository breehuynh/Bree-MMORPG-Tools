// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// TAUNT ITEM

[CreateAssetMenu(menuName = "uMMORPG Item/UCE Item Taunt", order = 999)]
public class UCE_ItemTaunt : UsableItem
{
    [Header("-=-=-=- UCE Taunt Item -=-=-=-")]
    [Range(0, 1)] public float successChance;

    public float range;
    public string tauntMessage = "You taunted: ";
    public string failedMessage = "You failed to taunt: ";

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
        ItemSlot slot = player.inventory[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            if (player.target != null && player.target is Monster)
            {
                // always call base function too
                base.Use(player, inventoryIndex);

                Monster monster = player.target.GetComponent<Monster>();

                if (UnityEngine.Random.value <= successChance)
                {
                    monster.UCE_OnAggro(player, 1);
                    player.UCE_TargetAddMessage(tauntMessage + monster.name);
                }
                else
                {
                    player.UCE_TargetAddMessage(failedMessage + monster.name);
                }

                // decrease amount
                slot.DecreaseAmount(decreaseAmount);
                player.inventory[inventoryIndex] = slot;
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
