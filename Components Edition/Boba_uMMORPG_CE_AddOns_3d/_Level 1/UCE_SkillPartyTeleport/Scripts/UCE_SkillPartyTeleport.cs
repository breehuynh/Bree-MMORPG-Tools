// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE SKILL PARTY LEADER BUFF

[CreateAssetMenu(menuName = "UCE Skills/UCE Skill Party Teleport", order = 999)]
public class UCE_SkillPartyTeleport : ScriptableSkill
{
    [Header("[-=-=-=- Party Teleport -=-=-=-]")]
    public bool CasterMustBeLeader;

    [Tooltip("[Optional] Members must be within distance to leader in order to teleport (0 for unlimited distance)")]
    public float maxDistanceToCaster;

    [Tooltip("[Required] GameObject prefab with coordinates OR off scene coordinates (requires UCE Network Zones AddOn)")]
    public UCE_TeleportationTarget teleportationTarget;

    [Tooltip("This will ignore the teleport Location and choose the nearest spawn point instead")]
    public bool teleportToClosestSpawnpoint;

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        if (((Player)caster).party.InParty() &&
            (
            (!CasterMustBeLeader ||
            (CasterMustBeLeader &&
            ((Player)caster).party.party.master == caster.name))
            ))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        // can cast anywhere
        destination = caster.transform.position;
        return true;
    }

    // -----------------------------------------------------------------------------------
    // Apply
    // -----------------------------------------------------------------------------------
    public override void Apply(Entity caster, int skillLevel)
    {
        foreach (string member in ((Player)caster).party.party.members)
        {
            Player player = UCE_Tools.FindOnlinePlayerByName(member);

            // -- Teleport everybody but not the caster
            if (player != null && player != (Player)caster)
            {
                // -- Check Distance
                if (maxDistanceToCaster <= 0 || Utils.ClosestDistance(player, caster) <= maxDistanceToCaster || member == ((Player)caster).party.party.master)
                {
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

                player = null;
            }
        }

        // -- Teleport the caster now
        if (teleportToClosestSpawnpoint)
        {
            Transform target = NetworkManagerMMO.GetNearestStartPosition(((Player)caster).transform.position);
            ((Player)caster).UCE_Warp(target.position);
        }
        else
        {
            teleportationTarget.OnTeleport(((Player)caster));
        }
    }

    // -----------------------------------------------------------------------------------
}
