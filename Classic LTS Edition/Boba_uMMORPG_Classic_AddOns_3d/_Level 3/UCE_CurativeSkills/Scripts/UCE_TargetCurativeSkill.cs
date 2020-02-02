// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using UnityEngine;

// TARGET CURATIVE SKILL

[CreateAssetMenu(menuName = "UCE Skills/UCE Target Curative Skill", order = 999)]
public class UCE_TargetCurativeSkill : UCE_CurativeSkill
{
   
    // -----------------------------------------------------------------------------------
    // Apply
    // -----------------------------------------------------------------------------------
    public override void Apply(Entity caster, int skillLevel)
    {
        List<Entity> targets = new List<Entity>();

        if (SpawnEffectOnMainTargetOnly)
            SpawnEffect(caster, caster.target);

        if (
            (caster is Player && caster.target is Player && ((Player)caster).UCE_SameCheck((Player)caster.target, affectSelf, affectPlayers, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting)) ||
            (caster.target is Monster && affectEnemies) ||
            (caster.target == caster && affectSelf)
            )
        {
            if (reviveChance.Get(skillLevel) > 0 && !caster.target.isAlive)
                targets.Add(caster.target);
            else if (reviveChance.Get(skillLevel) <= 0 && caster.target.isAlive)
                targets.Add(caster.target);
        }

        if (castRadius.Get(skillLevel) > 0)
        {
            if (caster is Player)
                targets.AddRange(((Player)caster).UCE_GetCorrectedTargetsInSphere(caster.target.transform, castRadius.Get(skillLevel), reviveChance.Get(skillLevel) > 0, affectSelf, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectEnemies));
            else
                targets.AddRange(caster.UCE_GetCorrectedTargetsInSphere(caster.target.transform, castRadius.Get(skillLevel), reviveChance.Get(skillLevel) > 0, affectSelf, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectEnemies));
        }

        ApplyToTargets(targets, caster, skillLevel);

        targets.Clear();
    }

    // -----------------------------------------------------------------------------------
}
