// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE SKILL PARTY LEADER BUFF

[CreateAssetMenu(menuName = "uMMORPG Skill/UCE Party Leader Buff", order = 999)]
public class UCE_SkillPartyLeaderBuff : BuffSkill
{
    [Header("-=-=-=- Leader Buff on Target -=-=-=-")]
    public BuffSkill applyBuff;

    public bool CasterMustBeLeader;

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        if (((Player)caster).InParty() &&
            (
            (!CasterMustBeLeader ||
            (CasterMustBeLeader &&
            ((Player)caster).party.members[0] == caster.name))
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
        foreach (string member in ((Player)caster).party.members)
        {
            if (Player.onlinePlayers.ContainsKey(member))
            {
                Player player = Player.onlinePlayers[member];
                if (player.isAlive)
                {
                    player.UCE_ApplyBuff(applyBuff, skillLevel, 1);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
