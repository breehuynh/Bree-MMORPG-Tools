// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE SKILL PARTY LEADER BUFF

[CreateAssetMenu(menuName = "UCE Item/UCE Item Party Teleport", order = 999)]
public class UCE_ItemPartyTeleport : UsableItem
{
    [Header("[-=-=-=- Party Teleport -=-=-=-]")]
    [Tooltip("[Optional] Members must be within distance to caster in order to teleport (0 for unlimited distance)")]
    public float maxDistanceToCaster;

    [Tooltip("[Required] GameObject prefab with coordinates OR off scene coordinates (requires UCE Network Zones AddOn)")]
    public UCE_TeleportationTarget teleportationTarget;

    [Tooltip("This will ignore the teleport Location and choose the nearest spawn point instead")]
    public bool teleportToClosestSpawnpoint;

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    // -----------------------------------------------------------------------------------
    // Apply
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            if (player.InParty())
            {
                // always call base function too
                base.Use(player, inventoryIndex);

                foreach (string member in (player.party.members))
                {
                    Player plyr = UCE_Tools.FindOnlinePlayerByName(member);

                    // -- Teleport everybody but not the caster
                    if (plyr != null && plyr != player)
                    {
                        // -- Check Distance

                        if (maxDistanceToCaster <= 0 || Utils.ClosestDistance(player, plyr) <= maxDistanceToCaster || member == player.party.master)
                        {
                            // -- Determine Teleportation Target
                            if (teleportToClosestSpawnpoint)
                            {
                                Transform target = NetworkManagerMMO.GetNearestStartPosition(plyr.transform.position);
                                plyr.UCE_Warp(target.position);
                            }
                            else
                            {
                                teleportationTarget.OnTeleport(plyr);
                            }
                        }

                        plyr = null;
                    }
                }

                // decrease amount
                slot.DecreaseAmount(decreaseAmount);
                player.inventory[inventoryIndex] = slot;

                // -- Teleport the caster now
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
    }

    // -----------------------------------------------------------------------------------
}
