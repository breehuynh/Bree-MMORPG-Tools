// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using UnityEngine;

// SKILL AREA REVIVE

[CreateAssetMenu(menuName = "uMMORPG Skill/UCE Skill Area Revive", order = 999)]
public class UCE_SkillAreaRevive : HealSkill
{
    [Header("-=-=-=- Buff on Target -=-=-=-")]
    public BuffSkill applyBuff;

    public LinearInt buffLevel;
    public LinearFloat buffChance;

    public bool reverseTargeting;

    public bool affectOwnParty;
    public bool affectOwnGuild;
    public bool affectOwnRealm;

    public bool affectPlayers;
    public bool affectEnemies;
    public bool affectPets;

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        // no target necessary, but still set to self so that LookAt(target)
        // doesn't cause the player to look at a target that doesn't even matter
        caster.target = caster;
        return true;
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
        List<Entity> targets = new List<Entity>();

        if (caster is Player)
            targets = ((Player)caster).UCE_GetCorrectedTargetsInSphere(caster.transform, castRange.Get(skillLevel), true, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectEnemies, affectPets);
        else
            targets = caster.UCE_GetCorrectedTargetsInSphere(caster.transform, castRange.Get(skillLevel), true, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectEnemies, affectPets);

        foreach (Entity target in targets)
        {
            target.health += healsHealth.Get(skillLevel);
            target.mana += healsMana.Get(skillLevel);
            target.UCE_ApplyBuff(applyBuff, buffLevel.Get(skillLevel), buffChance.Get(skillLevel));
            target.UCE_OverrideState("IDLE");
            SpawnEffect(caster, target);
        }

        targets.Clear();
    }
}
