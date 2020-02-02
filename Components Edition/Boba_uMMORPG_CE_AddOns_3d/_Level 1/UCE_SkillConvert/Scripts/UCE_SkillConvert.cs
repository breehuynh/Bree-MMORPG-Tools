// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// CONVERT SKILL

[CreateAssetMenu(menuName = "uMMORPG Skill/UCE Skill Convert", order = 999)]
public class UCE_SkillConvert : ScriptableSkill
{
    [Header("-=-=-=- UCE Convert Skill -=-=-=-")]
    public LinearFloat successChance;

    [Tooltip("[Optional] Maximum level of the target (0 to disable)")]
    public LinearInt maxTargetLevel;

    [Tooltip("[Optional] Uses player level as a base to calculate max Level of monster")]
    public bool basePlayerLevel;

    public string convertMessage = "You converted: ";
    public string failedMessage = "You failed to convert: ";
    public string errorMessage = "You cannot convert that target!";

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        return caster.target != null && caster.CanAttack(caster.target);
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        if (caster.target != null && caster.target is Monster)
        {
            int maxLevel = maxTargetLevel.Get(skillLevel);

            if (basePlayerLevel)
                maxLevel += caster.level.current;

            Monster monster = caster.target.GetComponent<Monster>();

            if (monster.level.current <= maxLevel)
            {
                destination = caster.target.collider.ClosestPointOnBounds(caster.transform.position);
                return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
            }
        }

        ((Player)caster).UCE_TargetAddMessage(errorMessage);
        destination = caster.transform.position;
        return false;
    }

    // -----------------------------------------------------------------------------------
    // Apply
    // -----------------------------------------------------------------------------------
    public override void Apply(Entity caster, int skillLevel)
    {
        Player player = (Player)caster;

        if (player.target != null && player.target is Monster)
        {
            int maxLevel = maxTargetLevel.Get(skillLevel);

            if (basePlayerLevel)
                maxLevel += player.level.current;

            Monster monster = player.target.GetComponent<Monster>();

            if (monster.level.current <= maxLevel && UnityEngine.Random.value <= successChance.Get(skillLevel))
            {
                //monster.UCE_setRealm(player.hashRealm, player.hashAlly);
                player.UCE_TargetAddMessage(convertMessage + monster.name);
            }
            else
            {
                player.UCE_TargetAddMessage(failedMessage + monster.name);
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
