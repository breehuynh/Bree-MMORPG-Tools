// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// MOUNT - ITEM

[CreateAssetMenu(menuName = "uMMORPG Item/UCE MountItem", order = 999)]
public partial class UCE_Tmpl_MountItem : SummonableItem
{
    [Header("[-=-=-=- UCE MOUNT -=-=-=-]")]
    [Tooltip("[Optional] Immediately ride the mount when its summoned?")]
    public bool autoRide;

    [Tooltip("[Optional] Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    // -----------------------------------------------------------------------------------
    // Use
    // @Server
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory[inventoryIndex];

        // -- Only activate if enough charges left
        if ((player.state == "IDLE" || player.state == "MOVING") && (decreaseAmount == 0 || slot.amount >= decreaseAmount))
        {
            // always call base function too
            base.Use(player, inventoryIndex);

            // -- Summon Mount
            player.UCE_ToggleMount(inventoryIndex);

            // -- Decrease Amount
            if (decreaseAmount != 0)
            {
                slot.DecreaseAmount(decreaseAmount);
                player.inventory[inventoryIndex] = slot;
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
