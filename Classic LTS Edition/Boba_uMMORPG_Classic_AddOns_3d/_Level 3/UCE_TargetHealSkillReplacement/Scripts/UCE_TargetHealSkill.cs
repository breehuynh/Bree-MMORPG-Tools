// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// TARGET HEAL SKILL

[CreateAssetMenu(menuName = "UCE Skills/UCE Target Heal Skill", order = 999)]
public partial class UCE_TargetHealSkill : HealSkill
{
    [Header("[-=-=-=- UCE Target Heal Skill -=-=-=-]")]

    [Header("[Effects]")]
    public LinearInt unstunSeconds;

    public LinearFloat unstunChance;
    public LinearInt modifyCooldown;
    public LinearFloat modifyCooldownChance;
    public LinearInt modifyBuffDuration;
    public LinearFloat modifyBuffDurationChance;
    public BuffType buffType;

    [Header("[Experience]")]
    [Range(0, 9999)] public float expPerHealth;

    [Range(0, 9999)] public float skillExpPerHealth;

    [Header("[Targets]")]
    public bool canHealSelf = true;

    public bool canHealOthers = false;

    // -----------------------------------------------------------------------------------
    // CorrectedTarget
    // -----------------------------------------------------------------------------------
    private Entity CorrectedTarget(Entity caster)
    {
        if (caster.target == null)
            return canHealSelf ? caster : null;

        if (caster.target == caster)
            return canHealSelf ? caster : null;

        if (caster.target.GetType() == caster.GetType())
        {
            if (canHealOthers)
                return caster.target;
            else if (canHealSelf)
                return caster;
            else
                return null;
        }

        return canHealSelf ? caster : null;
    }

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        caster.target = CorrectedTarget(caster);
        return caster.target != null && caster.target.isAlive;
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPoint(caster.transform.position);
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
        if (caster.target != null && caster.target.isAlive)
        {
            int oldHealth = caster.target.health;

            caster.target.health += healsHealth.Get(skillLevel);
            caster.target.mana += healsMana.Get(skillLevel);

            // -- check for unstun
            if (unstunChance.Get(skillLevel) > 0 && caster.target.UCE_Stunned() && UnityEngine.Random.value <= unstunChance.Get(skillLevel))
            {
                caster.target.UCE_SetStun(unstunSeconds.Get(skillLevel));
            }

            // -- check for skill cooldown
            if (modifyCooldownChance.Get(skillLevel) > 0)
            {
                for (int i = 0; i < caster.target.skills.Count; ++i)
                {
                    Skill skill = caster.target.skills[i];
                    if (skill.IsOnCooldown() && UnityEngine.Random.value <= modifyCooldownChance.Get(skillLevel))
                    {
                        skill.cooldownEnd += modifyCooldown.Get(skillLevel);
                        caster.target.skills[i] = skill;
                    }
                }
            }

            // -- check for buff duration
            if (modifyBuffDurationChance.Get(skillLevel) > 0)
            {
                for (int i = 0; i < caster.target.buffs.Count; ++i)
                {
                    Buff buff = caster.target.buffs[i];
                    if (buff.CheckBuffType(buffType) && buff.BuffTimeRemaining() > 0 && UnityEngine.Random.value <= modifyBuffDurationChance.Get(skillLevel))
                    {
                        buff.buffTimeEnd += modifyBuffDuration.Get(skillLevel);
                        caster.target.buffs[i] = buff;
                    }
                }
            }

            // -- check for experience gain on heal
            if (caster is Player)
            {
                if (expPerHealth > 0)
                    ((Player)caster).experience += (int)Mathf.Round((caster.target.health - oldHealth) * expPerHealth);

                if (skillExpPerHealth > 0)
                    ((Player)caster).skillExperience += (int)Mathf.Round((caster.target.health - oldHealth) * skillExpPerHealth);
            }

            SpawnEffect(caster, caster.target);

            //this.InvokeInstanceDevExtMethods("OnApply", caster, skillLevel);
            Utils.InvokeMany(typeof(ScriptableSkill), this, "OnApply_", caster, skillLevel);
        }
    }

    // -----------------------------------------------------------------------------------
}
