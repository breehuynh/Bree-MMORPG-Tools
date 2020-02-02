// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using UnityEngine;

// UCE TARGET MELEE SKILL

[CreateAssetMenu(menuName = "UCE Skills/UCE Target Melee Skill", order = 999)]
public partial class UCE_TargetMeleeSkill : UCE_DamageSkill
{
    protected List<Entity> targets = new List<Entity>();
    protected Entity _target = null;

    // -----------------------------------------------------------------------------------
    // Apply
    // @Server
    // -----------------------------------------------------------------------------------
    public override void Apply(Entity caster, int skillLevel)
    {
        base.Apply(caster, skillLevel);
        OnSkillImpact(caster, caster.target, skillLevel);
    }

    // -----------------------------------------------------------------------------------
    // OnSkillImpact
    // @Server
    // -----------------------------------------------------------------------------------
    protected void OnSkillImpact(Entity caster, Entity _target, int skillLevel)
    {
        targets.Clear();

        // ------ spawn visual effect if any
        if (visualEffectOnMainTargetOnly || impactRadius.Get(skillLevel) <= 0)
            SpawnEffect(caster, _target);

        // ------ get all valid targets
        if (impactRadius.Get(skillLevel) > 0)
        {
            if (caster is Player)
                targets = ((Player)caster).UCE_GetCorrectedTargetsInSphere(_target.transform, impactRadius.Get(skillLevel), false, notAffectSelf, notAffectOwnParty, notAffectOwnGuild, notAffectOwnRealm, reverseTargeting, notAffectPlayers, notAffectMonsters, notAffectPets);
            else
                targets = caster.UCE_GetCorrectedTargetsInSphere(_target.transform, impactRadius.Get(skillLevel), false, notAffectSelf, notAffectOwnParty, notAffectOwnGuild, notAffectOwnRealm, reverseTargeting, notAffectPlayers, notAffectMonsters, notAffectPets);
        }
        else
        {
            targets.Add(_target);
        }

        // ----- apply effects to targets
        foreach (Entity target in targets)
        {
            // ------ Deal Damage

            int dmg = damage.Get(skillLevel);
            if (addCasterDamage) dmg += caster.damage;

            float stunChnce = stunChance.Get(skillLevel);
#if _iMMOATTRIBUTES
            if (stunAddAccuracy) stunChnce = target.UCE_HarmonizeChance(stunChnce, caster.accuracy);
#endif

            caster.DealDamageAt(target, dmg, stunChnce, UnityEngine.Random.Range(minStunTime.Get(skillLevel), maxStunTime.Get(skillLevel)));

            // ------ Remove random Buff
            if (removeRandomBuff.Get(skillLevel) > 0 && caster.target.buffs.Count > 0)
            {
                float removeChnce = 0;
#if _iMMOATTRIBUTES
                if (removeAddAccuracy) removeChnce = target.UCE_HarmonizeChance(removeChnce, caster.accuracy);
#endif
                caster.target.UCE_CleanupStatusBuffs(removeChance.Get(skillLevel), removeChnce, removeRandomBuff.Get(skillLevel));
            }

            // ------ Cooldown Target
            if (cooldownChance.Get(skillLevel) > 0)
            {
                float cldwnChnce = cooldownChance.Get(skillLevel);
#if _iMMOATTRIBUTES
                if (cooldownAddAccuracy) cldwnChnce = target.UCE_HarmonizeChance(cldwnChnce, caster.accuracy);
#endif
                for (int i = 0; i < target.skills.Count; ++i)
                {
                    Skill skill = target.skills[i];
                    if (skill.IsOnCooldown() && UnityEngine.Random.value <= cldwnChnce)
                    {
                        skill.cooldownEnd += cooldownDuration.Get(skillLevel);
                        target.skills[i] = skill;
                    }
                }
            }

            // ------ Recoil Target
            if (recoilChance.Get(skillLevel) > 0 && minRecoilTarget.Get(skillLevel) > -100f && maxRecoilTarget.Get(skillLevel) > -100f)
            {
                float recoilChnce = recoilChance.Get(skillLevel);
#if _iMMOATTRIBUTES
                if (recoilAddAccuracy) recoilChnce = target.UCE_HarmonizeChance(recoilChnce, caster.accuracy);
#endif
                if (UnityEngine.Random.value <= recoilChnce)
                    target.UCE_Recoil(caster, UnityEngine.Random.Range(minRecoilTarget.Get(skillLevel), maxRecoilTarget.Get(skillLevel)));
            }

            // ------ Apply Buff  (if any)
            if (applyBuff.Length > 0 && applyBuff.Length >= skillLevel && applyBuff[skillLevel - 1] != null)
            {
                float buffModifier = 0;
#if _iMMOATTRIBUTES
                if (buffAddAccuracy) buffModifier = target.UCE_HarmonizeChance(buffModifier, caster.accuracy);
#endif
                target.UCE_ApplyBuff(applyBuff[skillLevel - 1], buffLevel.Get(skillLevel), buffChance.Get(skillLevel), buffModifier);
            }

            // ------ Spawn visual effect (if any)
            if (!visualEffectOnMainTargetOnly && impactRadius.Get(skillLevel) > 0)
                SpawnEffect(caster, target);

            // ------ Check for Aggro Trigger
            target.UCE_OnAggro(caster, triggerAggroChance.Get(skillLevel));
        }

        // ------ create object at impact loaction
        if (createOnTarget.Length > 0 && createOnTarget.Length >= skillLevel - 1 && createOnTarget[skillLevel - 1] != null && UnityEngine.Random.value <= createChance.Get(skillLevel))
        {
            GameObject go = Instantiate(createOnTarget[skillLevel - 1], caster.target.transform.position, caster.target.transform.rotation);
            NetworkServer.Spawn(go);
        }
    }

    // -----------------------------------------------------------------------------------
}
