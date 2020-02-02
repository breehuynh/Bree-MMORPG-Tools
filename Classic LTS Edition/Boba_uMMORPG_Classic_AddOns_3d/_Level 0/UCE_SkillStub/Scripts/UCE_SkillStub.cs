// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// SKILL STUB

[CreateAssetMenu(menuName = "UCE Skills/UCE Skill Stub", order = 999)]
public class UCE_SkillStub : ScriptableSkill
{
    public override bool CheckTarget(Entity entity)
    {
        return true;
    }

    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        destination = Vector3.zero;
        return true;
    }

    public override void Apply(Entity caster, int skillLevel)
    {
    }
}
