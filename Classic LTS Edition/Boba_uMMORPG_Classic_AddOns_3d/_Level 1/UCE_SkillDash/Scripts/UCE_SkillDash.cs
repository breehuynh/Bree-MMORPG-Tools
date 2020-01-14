// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UCE Skills/UCE Dash", order = 999)]
public class UCE_SkillDash : ScriptableSkill
{
    [Header("Stuffs")]
    public LinearFloat distance;
    public LinearFloat timeDash;

    public override void Apply(Entity caster, int skillLevel)
    {
        caster.StartDash(caster.transform.position + (caster.LookDirection * distance.Get(skillLevel)), timeDash.Get(skillLevel));
    }

    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        destination = caster.transform.position;
        return true;
    }

    public override bool CheckTarget(Entity caster)
    {
        return true;
    }
}