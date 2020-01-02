using Mirror;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[CreateAssetMenu(menuName = "uMMORPG Skill/Target AoE", order = 999)]
public class TargetAreaOfEffectSkill : DamageSkill
{
    [Header("~~~~~Boba's Target AoE Skill~~~~~")]
    public OneTimeTargetSkillEffect effect;

    public LinearFloat castRadius;
    public LinearFloat tauntChance;

    // OverlapSphereNonAlloc array to avoid allocations.
    // -> static so we don't create one per skill
    // -> this is worth it because skills are casted a lot!
    // -> should be big enough to work in just about all cases
    static Collider[] hitsBuffer = new Collider[10000];

    // helper function to spawn the skill effect on someone
    // (used by all the buff implementations and to load them after saving)
    public void SpawnEffect(Entity caster, Entity spawnTarget)
    {
        if (effect != null)
        {
            GameObject go = Instantiate(effect.gameObject, spawnTarget.transform.position, Quaternion.identity);
            OneTimeTargetSkillEffect effectComponent = go.GetComponent<OneTimeTargetSkillEffect>();
            effectComponent.caster = caster;
            effectComponent.target = spawnTarget;
            NetworkServer.Spawn(go);
        }
    }

    public override bool CheckTarget(Entity caster)
    {
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
        // candidates hashset to be 100% sure that we don't apply an area skill
        // to a candidate twice. this could happen if the candidate has more
        // than one collider (which it often has).
        HashSet<Entity> candidates = new HashSet<Entity>();

        // find all entities of same type in castRange around the caster
        int hits = Physics.OverlapSphereNonAlloc(caster.transform.position, castRadius.Get(skillLevel), hitsBuffer);
        for (int i = 0; i < hits; ++i)
        {
            Collider co = hitsBuffer[i];
            Entity candidate = co.GetComponentInParent<Entity>();
            if (candidate != null &&
                candidate.health.current > 0 && // can't attack dead enemies
                candidate.GetType() != caster.GetType()) // only on different type
            {
                candidates.Add(candidate);
            }
        }

        // apply to all candidates
        foreach (Entity candidate in candidates)
        {   
            // deal damage directly with base damage + skill damage
            caster.combat.DealDamageAt(candidate,
                                       caster.combat.damage + damage.Get(skillLevel),
                                       stunChance.Get(skillLevel),
                                       stunTime.Get(skillLevel));
            // show effect on candidate
            SpawnEffect(caster, candidate);
            if (UnityEngine.Random.value <= tauntChance.Get(skillLevel)) 
            {
                candidate.PartialOnAggro(caster);
            }
        }
    }

    // tooltip
    public override string ToolTip(int skillLevel, bool showRequirements = false)
    {
        StringBuilder tip = new StringBuilder(base.ToolTip(skillLevel, showRequirements));
        tip.Replace("{AOEDAMAGE}", damage.Get(skillLevel).ToString());
        tip.Replace("{TAUNTCHANCE}", Mathf.RoundToInt(tauntChance.Get(skillLevel) * 100).ToString());
        return tip.ToString();
    }
}
