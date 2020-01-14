// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using UnityEngine;

// SKILL AREA CLEANSE

[CreateAssetMenu(menuName = "uMMORPG Skill/UCE Skill Area Cleanse", order = 999)]
public class UCE_SkillAreaCleanse : HealSkill
{
    [Header("-=-=-=- UCE Area Cleanse Skill -=-=-=-")]
    [Tooltip("% Chance to remove each Nerf on the target (Nerf = Buff set to 'disadvantegous')")]
    public LinearFloat successChance;

    [Header("-=-=-=- Buff on Target -=-=-=-")]
    public BuffSkill applyBuff;

    public LinearInt buffLevel;
    public LinearFloat buffChance;

    [Header("-=-=-=- Targeting -=-=-=-")]
    [Tooltip("[Optional] Changes 'affect' affect into 'not affect' and vice-versa")]
    public bool reverseTargeting;

    [Tooltip("[Optional] Does affect the caster")]
    public bool affectSelf;

    [Tooltip("[Optional] Does affect members of the own party")]
    public bool affectOwnParty;

    [Tooltip("[Optional] Does affect members of the own guild")]
    public bool affectOwnGuild;

    [Tooltip("[Optional] Does affect members of the own realm (requires UCE PVP ZONE AddOn")]
    public bool affectOwnRealm;

    public bool affectPlayers;
    public bool affectMonsters;
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
            targets = ((Player)caster).UCE_GetCorrectedTargetsInSphere(caster.transform, castRange.Get(skillLevel), false, affectSelf, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectMonsters, affectPets);
        else
            targets = caster.UCE_GetCorrectedTargetsInSphere(caster.transform, castRange.Get(skillLevel), false, affectSelf, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectMonsters, affectPets);

        foreach (Entity target in targets)
        {
            target.UCE_CleanupStatusNerfs(successChance.Get(skillLevel));
            target.health += healsHealth.Get(skillLevel);
            target.mana += healsMana.Get(skillLevel);
            target.UCE_ApplyBuff(applyBuff, buffLevel.Get(skillLevel), buffChance.Get(skillLevel));

            SpawnEffect(caster, target);
        }

        targets.Clear();
    }
}
