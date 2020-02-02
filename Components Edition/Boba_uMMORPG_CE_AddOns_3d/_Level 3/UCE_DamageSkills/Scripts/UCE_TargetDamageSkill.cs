// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// =======================================================================================
using System.Collections.Generic;
using UnityEngine;

// =======================================================================================
// 
// =======================================================================================
[CreateAssetMenu(menuName = "UCE Skills/UCE Target Damage Skill", order = 999)]
public class UCE_TargetDamageSkill : UCE_BaseDamageSkill
{
	
	// -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        return caster.target != null || affectSelf;
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        // target still around?
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPointOnBounds(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

    // -----------------------------------------------------------------------------------
    // Apply
    // -----------------------------------------------------------------------------------
    public override void Apply(Entity caster, int skillLevel)
    {
        List<Entity> targets = new List<Entity>();

        if (SpawnEffectOnMainTargetOnly)
            SpawnEffect(caster, caster.target);
		
		if (caster.target is Player && caster is Player && ((Player)caster).UCE_SameCheck((Player)caster.target, affectSelf, affectPlayers, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting) ||
			(caster.target is Monster && affectEnemies ) ||
			(caster is Monster && caster.target is Monster && affectEnemies) ||
			(caster is Monster && caster.target is Player && affectPlayers)
		)
        	if (caster.target.isAlive)
                targets.Add(caster.target);
		
		if (castRadius.Get(skillLevel) > 0) {
		
        	if (caster is Player)
        	    targets.AddRange( ((Player)caster).UCE_GetCorrectedTargetsInSphere(caster.target.transform, castRadius.Get(skillLevel), false, affectSelf, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectEnemies) );
        	else
            	targets.AddRange( caster.UCE_GetCorrectedTargetsInSphere(caster.target.transform, castRadius.Get(skillLevel), false, affectSelf, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectEnemies) );
		
		}
		
        ApplyToTargets(targets, caster, skillLevel);

        targets.Clear();
    }
	
	// -----------------------------------------------------------------------------------
	
}

// =======================================================================================