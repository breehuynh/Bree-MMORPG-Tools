// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// =======================================================================================
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// =======================================================================================
// 
// =======================================================================================
public abstract partial class UCE_BaseDamageSkill : DamageSkill
{


    [Header("Visuals")]
    public OneTimeTargetSkillEffect effect;


    [Header("[Radius]")]
    [Tooltip("[Required] The radius around the caster/target, all legal targets therein are affected.")]
    public LinearFloat castRadius;

    [Header("[Debuff]")]
    [Tooltip("[Optional] Remove only buffs, nerfs or both status effect types from each target?")]
    public BuffType removeBuffType;
    [Tooltip("[Optional] % Chance to successfully remove status effects from the target (each)")]
    public LinearFloat debuffChance;
    [Tooltip("[Optional] How many status effects are removed from the target at maximum (0 for unlimited)?")]
    public LinearInt debuffAmount;

    [Tooltip("[Optional] Add the casters Accuracy to the debuff chance?")]
    public bool debuffAddAccuracy;

    [Tooltip("[Optional] Remove specific buffs from the target in addition to the type set above (not limited by count)?")]
    public BuffSkill[] removeBuffs;

    [Header("[Buff]")]
    [Tooltip("[Optional] One or more status effects applied to each target randomly.")]
    public BuffSkill[] applyBuffs;
    [Tooltip("[Optional] Level of the buffs applied (capped at buffs max level).")]
    public LinearInt buffLevel;
    [Tooltip("[Optional] % Chance to to apply each buff on each target.")]
    public LinearFloat buffChance;
    [Tooltip("[Optional] How many status effects are added to the target at maximum (0 for unlimited)?")]
    public LinearInt buffAmount;

    [Tooltip("[Optional] Add the casters Accuracy to the buff chance?")]
    public bool buffAddAccuracy;

    [Tooltip("[Optional] Add the casters damage to the skill damage?")]
    public bool addCasterDamage;

    [Header("[Effects]")]
    [Tooltip("[Optional] ")]
    public LinearInt unstunSeconds;
    [Tooltip("[Optional] ")]
    public LinearFloat unstunChance;
    [Tooltip("[Optional] ")]
    public LinearInt modifyCooldown;
    [Tooltip("[Optional] ")]
    public LinearFloat modifyCooldownChance;
    [Tooltip("[Optional] ")]
    public LinearInt modifyBuffDuration;
    [Tooltip("[Optional] ")]
    public LinearFloat modifyBuffDurationChance;
    [Tooltip("[Optional] ")]
    public BuffType buffType;

    [Header("[Targets]")]
    [Tooltip("[Optional] Changes 'affect' affect into 'not affect' and vice-versa")]
    public bool reverseTargeting;
    [Tooltip("[Optional] Does affect the caster")]
    public bool affectSelf;
    [Tooltip("[Optional] Does affect other players (overwrites settings below)")]
    public bool affectPlayers;
    [Tooltip("[Optional] Does affect monsters")]
    public bool affectEnemies;
    [Tooltip("[Optional] Does affect members of the own party")]
    public bool affectOwnParty;
    [Tooltip("[Optional] Does affect members of the own guild")]
    public bool affectOwnGuild;
    [Tooltip("[Optional] Does affect members of the own realm (requires UCE PVP ZONE AddOn")]
    public bool affectOwnRealm;

    [Header("[Visuals]")]
    [Tooltip("[Optional] Visual effect spawn on main target only or on all targets?")]
    public bool SpawnEffectOnMainTargetOnly;

    /*[Header("[Messages]")]
	public string labelCasterHeal = "You healed {0} for {1} health.";
	public string labelTargetHeal = "{0} healed you for {1} health.";*/

    // -----------------------------------------------------------------------------------
    // SpawnEffect
    // -----------------------------------------------------------------------------------
    public void SpawnEffect(Entity caster, Entity spawnTarget)
    {
        if (effect != null)
        {
            GameObject go = Instantiate(effect.gameObject, spawnTarget.transform.position, Quaternion.identity);
            go.GetComponent<OneTimeTargetSkillEffect>().caster = caster;
            go.GetComponent<OneTimeTargetSkillEffect>().target = spawnTarget;
            NetworkServer.Spawn(go);
        }
    }


    // -----------------------------------------------------------------------------------
    // CorrectedTarget
    // -----------------------------------------------------------------------------------
    Entity CorrectedTarget(Entity caster)
    {
        // targeting nothing? then try to cast on self
        if (caster.target == null)
            return affectSelf ? caster : null;

        // targeting self?
        if (caster.target == caster)
            return affectSelf ? caster : null;

        // no valid target? try to cast on self or don't cast at all
        return caster.target;
    }

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        caster.target = CorrectedTarget(caster);
        return caster.target != null;
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPointOnBounds(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

    // -----------------------------------------------------------------------------------
    // ApplyToTargets
    // -----------------------------------------------------------------------------------
    public void ApplyToTargets(List<Entity> targets, Entity caster, int skillLevel)
    {

        //DamageType damageType = DamageType.Normal;

        // ---------------------------------------------------------- Apply to all targets

        foreach (Entity target in targets)
        {

            float buffModifier = 0;
            int dmg = 0;

            // ---------------------------------------------------------------- Debuff
            if (removeBuffType != BuffType.None || removeBuffs.Length > 0)
            {

                foreach (BuffSkill removeBuff in removeBuffs)
                    target.UCE_RemoveBuff(removeBuff, debuffChance.Get(skillLevel), buffModifier);

                buffModifier = (debuffAddAccuracy) ? target.UCE_HarmonizeChance(0, caster.accuracy) : 0f;

                if (removeBuffType == BuffType.Buff)
                {
                    target.UCE_CleanupStatusBuffs(debuffChance.Get(skillLevel), buffModifier, debuffAmount.Get(skillLevel));
                }
                else if (removeBuffType == BuffType.Nerf)
                {
                    target.UCE_CleanupStatusNerfs(debuffChance.Get(skillLevel), buffModifier, debuffAmount.Get(skillLevel));
                }
                else if (removeBuffType == BuffType.Both)
                {

                    target.UCE_CleanupStatusAny(debuffChance.Get(skillLevel), buffModifier, debuffAmount.Get(skillLevel));
                }

            }

            // ---------------------------------------------------------------- Buff
            if (target.isAlive && applyBuffs.Length > 0)
            {
                buffModifier = (buffAddAccuracy) ? target.UCE_HarmonizeChance(0, caster.accuracy) : 0f;
                target.UCE_ApplyBuffs(applyBuffs, buffLevel.Get(skillLevel), buffChance.Get(skillLevel), buffModifier, buffAmount.Get(skillLevel));
            }

            // ---------------------------------------------------------------- Effects

            // -- check for unstun
            if (unstunChance.Get(skillLevel) > 0 && target.UCE_Stunned() && UnityEngine.Random.value <= unstunChance.Get(skillLevel))
            {
                target.UCE_SetStun(unstunSeconds.Get(skillLevel));
            }

            // -- check for skill cooldown
            if (modifyCooldownChance.Get(skillLevel) > 0)
            {
                for (int i = 0; i < target.skills.skills.Count; ++i)
                {
                    Skill skill = target.skills.skills[i];
                    if (skill.IsOnCooldown() && UnityEngine.Random.value <= modifyCooldownChance.Get(skillLevel))
                    {
                        skill.cooldownEnd += modifyCooldown.Get(skillLevel);
                        target.skills.skills[i] = skill;
                    }
                }
            }

            // -- check for buff duration
            if (modifyBuffDurationChance.Get(skillLevel) > 0)
            {
                for (int i = 0; i < target.skills.buffs.Count; ++i)
                {
                    Buff buff = target.skills.buffs[i];
                    if (buff.CheckBuffType(buffType) && buff.BuffTimeRemaining() > 0 && UnityEngine.Random.value <= modifyBuffDurationChance.Get(skillLevel))
                    {
                        buff.buffTimeEnd += modifyBuffDuration.Get(skillLevel);
                        target.skills.buffs[i] = buff;
                    }
                }
            }

            // ---------------------------------------------------------------- Deal Damage

            dmg = damage.Get(skillLevel);

            if (addCasterDamage) dmg += caster.combat.damage;

            caster.DealDamageAt(target, dmg, stunChance.Get(skillLevel), stunTime.Get(skillLevel));

            // ---------------------------------------------------------------- Spawn Effect
            SpawnEffect(caster, target);

        }

        targets.Clear();

    }

    // -----------------------------------------------------------------------------------

}

// =======================================================================================