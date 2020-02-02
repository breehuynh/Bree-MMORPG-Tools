// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// PORTABLE TELEPORT - ITEM

[CreateAssetMenu(menuName = "uMMORPG Item/UCE Bindpoint Teleport", order = 998)]
public class UCE_ItemBindpointTeleport : UsableItem
{
    [Header("-=-=-=- Teleport to current Bindpoint -=-=-=-")]
    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    // -----------------------------------------------------------------------------------
    // Use
    // @Server
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory.slots[inventoryIndex];

        // -- Only activate if enough charges left + a bindpoint has been set
        if (
            player.UCE_myBindpoint.Valid &&
            (decreaseAmount == 0 || slot.amount >= decreaseAmount)
            )
        {
            // always call base function too
            base.Use(player, inventoryIndex);

            // -- Decrease Amount
            if (decreaseAmount != 0)
            {
                slot.DecreaseAmount(decreaseAmount);
                player.inventory.slots[inventoryIndex] = slot;
            }

            // -- Activate Teleport
            player.movement.Warp(player.UCE_myBindpoint.position);
        }
    }

    // -----------------------------------------------------------------------------------
}
