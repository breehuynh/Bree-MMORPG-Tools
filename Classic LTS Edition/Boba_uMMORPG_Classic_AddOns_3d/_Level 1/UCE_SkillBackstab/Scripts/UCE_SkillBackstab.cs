// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "UCE Skills/UCE Backstab", order = 999)]
public class UCE_SkillBackstab : DamageSkill
{
    public float teleportDistanceBehind = 1f;

    public override bool CheckTarget(Entity caster)
    {
        // target exists, alive, not self, oktype?
        return caster.target != null && caster.CanAttack(caster.target);
    }

    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        // target still around?
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPoint(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

    public override void Apply(Entity caster, int skillLevel)
    {
        Vector3 teleport = caster.target.transform.position - caster.target.transform.forward * teleportDistanceBehind;
        caster.agent.Warp(teleport);
        caster.RpcBackstabStartTeleport(teleport, caster.target.transform.position);

        // deal damage directly with base damage + skill damage
        caster.DealDamageAt(caster.target,
                            caster.damage + damage.Get(skillLevel),
                            stunChance.Get(skillLevel),
                            stunTime.Get(skillLevel));
    }
}