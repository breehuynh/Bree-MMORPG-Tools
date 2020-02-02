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

// PORTABLE TELEPORT - ITEM

[CreateAssetMenu(menuName = "uMMORPG Item/UCE PortableTeleport", order = 998)]
public class UCE_Tmpl_PortableTeleport : UsableItem
{
    [Header("[-=-=-=- Portable Teleport -=-=-=-]")]
    [Tooltip("[Required] GameObject prefab with coordinates OR off scene coordinates (requires UCE Network Zones AddOn)")]
    public UCE_TeleportationTarget teleportationTarget;

    [Tooltip("This will ignore the teleport Location and choose the nearest spawn point instead")]
    public bool teleportToClosestSpawnpoint;

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
                slot.DecreaseAmount(decreaseAmount);
                player.inventory[inventoryIndex] = slot;
            }

            // -- Determine Teleportation Target
            if (teleportToClosestSpawnpoint)
            {
                Transform target = NetworkManagerMMO.GetNearestStartPosition(player.transform.position);
                player.UCE_Warp(target.position);
            }
            else
            {
                teleportationTarget.OnTeleport(player);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Tooltip
    // @Client
    // -----------------------------------------------------------------------------------
    public override string ToolTip()
    {
        StringBuilder tip = new StringBuilder(base.ToolTip());
        tip.Replace("{DESTINATION}", teleportationTarget.name);
        return tip.ToString();
    }

    // -----------------------------------------------------------------------------------
}
