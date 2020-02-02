// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using UnityEngine;

// BASE CURATIVE SKILL

public abstract class UCE_CurativeSkill : HealSkill
{
    [Header("[Radius]")]
    [Tooltip("[Required] The radius around the caster/target, all legal targets therein are affected.")]
    public LinearFloat castRadius;

    [Header("[Revive]")]
    [Tooltip("[Optional] % Chance to revive each target (from dead to alive with 1 health)")]
    public LinearFloat reviveChance;

    [Header("[Debuff]")]
    [Tooltip("[Optional] Remove only buffs, nerfs or both status effect types from each target?")]
    public BuffType removeBuffType;

    [Tooltip("[Optional] % Chance to successfully remove status effects from the target (each)")]
    public LinearFloat debuffChance;

    [Tooltip("[Optional] How many status effects are removed from the target at maximum (0 for unlimited)?")]
    public LinearInt debuffAmount;

#if _iMMOATTRIBUTES

    [Tooltip("[Optional] Add the casters Accuracy to the debuff chance?")]
    public bool debuffAddAccuracy;

#endif

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

#if _iMMOATTRIBUTES

    [Tooltip("[Optional] Add the casters Accuracy to the buff chance?")]
    public bool buffAddAccuracy;

#endif

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

    [Header("[Healing Experience]")]
    [Range(0, 9999)] public float expPerHealth;

    [Range(0, 9999)] public float skillExpPerHealth;

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

    [Header("[Messages]")]
    public string labelCasterHeal = "You healed {0} for {1} health.";

    public string labelTargetHeal = "{0} healed you for {1} health.";

    public string labelCasterRevive = "You revived {0} from the dead.";
    public string labelTargetRevive = "{0} revived you from the dead.";

    // -----------------------------------------------------------------------------------
    // CorrectedTarget
    // -----------------------------------------------------------------------------------
    private Entity CorrectedTarget(Entity caster)
    {
        // targeting nothing? then try to cast on self
        if (caster.target == null)
            return affectSelf ? caster : null;

        // targeting self?
        if (caster.target == caster)
            return affectSelf ? caster : null;

        // targeting player but can target self?
        if (caster is Monster && caster.target is Player)
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
        destination = caster.transform.position;

        if (caster.target == caster && affectSelf) return true;

        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPoint(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    // ApplyToTargets
    // -----------------------------------------------------------------------------------
    public void ApplyToTargets(List<Entity> targets, Entity caster, int skillLevel)
    {

        // ---------------------------------------------------------- Apply to all targets

        foreach (Entity target in targets)
        {
            int oldHealth = target.health.current;
            int hldHealth = 0;
            float buffModifier = 0;

            // ---------------------------------------------------------------- Revive Dead
            if (!target.isAlive && reviveChance.Get(skillLevel) > 0 && UnityEngine.Random.value <= reviveChance.Get(skillLevel))
            {
                target.health.current += 1;
                target.UCE_OverrideState("IDLE");

                caster.UCE_TargetAddMessage(string.Format(labelCasterRevive, target.name));
                if (target != caster)
                    target.UCE_TargetAddMessage(string.Format(labelTargetRevive, caster.name));
            }

            // ---------------------------------------------------------------- Heal
            if (target.isAlive && healsHealth.Get(skillLevel) > 0 || healsMana.Get(skillLevel) > 0)
            {
                target.health.current += healsHealth.Get(skillLevel);
                target.mana.current += healsMana.Get(skillLevel);

                hldHealth = target.health.current - oldHealth;

                if (hldHealth != 0)
                {
                
                    target.RpcOnHealingReceived(hldHealth);
                    
                    if (caster is Player) { 
                        caster.UCE_TargetAddMessage( string.Format(labelCasterHeal, target.name, hldHealth.ToString()) );
                		if (target != caster)
                			target.UCE_TargetAddMessage( string.Format(labelTargetHeal, caster.name, hldHealth.ToString()) );
                    }
                    
                }
            }

            // ---------------------------------------------------------------- Debuff
            if (removeBuffType != BuffType.None || removeBuffs.Length > 0)
            {
                foreach (BuffSkill removeBuff in removeBuffs)
                    target.UCE_RemoveBuff(removeBuff, debuffChance.Get(skillLevel), buffModifier);

#if _iMMOATTRIBUTES
                buffModifier = (debuffAddAccuracy) ? target.UCE_HarmonizeChance(0, caster.accuracy) : 0f;
#endif
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
#if _iMMOATTRIBUTES
                buffModifier = (buffAddAccuracy) ? target.UCE_HarmonizeChance(0, caster.accuracy) : 0f;
#endif
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

            // ---------------------------------------------------------------- Experience
            if (caster is Player && hldHealth > 0)
            {
                if (expPerHealth > 0)
                    ((Player)caster).experience.current += (int)Mathf.Round(hldHealth * expPerHealth);

                if (skillExpPerHealth > 0)
                    ((PlayerSkills)((Player)caster).skills).skillExperience += (int)Mathf.Round(hldHealth * skillExpPerHealth);
            }

            // ---------------------------------------------------------------- Spawn Effect
            SpawnEffect(caster, target);
        }

        if (caster is Monster)
            caster.target = null;

        targets.Clear();
    }

    // -----------------------------------------------------------------------------------
}
