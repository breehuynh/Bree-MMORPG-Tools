// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// ===================================================================================
// LIMITED TELEPORT AREA (SPHERE)
// ===================================================================================
[RequireComponent(typeof(SphereCollider))]
public class UCE_AreaSphere_LimitedTeleport : MonoBehaviour
{
    public UCE_Area_LimitedTeleport[] connectedTeleporters;

    // -----------------------------------------------------------------------------------
    // OnTriggerExit
    // -----------------------------------------------------------------------------------
    private void OnTriggerExit(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();

        if (player != null && connectedTeleporters != null && connectedTeleporters.Length > 0)
        {
            foreach (UCE_Area_LimitedTeleport teleporter in connectedTeleporters)
            {
                if (teleporter.enterLimit > 0)
                {
                    if (teleporter.groupType == UCE_Area_LimitedTeleport.GroupType.None)
                    {
                        teleporter.enterCount--;
                    }
                    else if (teleporter.groupType == UCE_Area_LimitedTeleport.GroupType.Party && player.party.InParty())
                    {
                        if (teleporter.enterParty == player.party.party.members[0])
                            teleporter.enterCount--;
                    }
                    else if (teleporter.groupType == UCE_Area_LimitedTeleport.GroupType.Guild && player.guild.InGuild())
                    {
                        if (teleporter.enterGuild == player.guild.name)
                            teleporter.enterCount--;
                    }
                    else if (teleporter.groupType == UCE_Area_LimitedTeleport.GroupType.Realm)
                    {
#if _iMMOPVP
                        if (player.UCE_getAlliedRealms(teleporter.interactionRequirements.requiredRealm, teleporter.interactionRequirements.requiredAlly))
                            teleporter.enterCount--;
#endif
                    }

                    if (teleporter.enterCount <= 0)
                        teleporter.ResetLimits();
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
