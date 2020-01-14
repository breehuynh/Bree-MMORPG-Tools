using Mirror;
using UnityEngine;
using System.Text;

[CreateAssetMenu(menuName = "uMMORPG Skill/Target Taunt", order = 999)]
public class TargetTauntSkill : DamageSkill
{
    [Header("~~~~~Boba's Target Taunt Skill~~~~~")]
    public OneTimeTargetSkillEffect effect;

    public LinearFloat tauntChance;

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
        // target exists, alive, not self, ok type?
        return caster.target != null && caster.CanAttack(caster.target);
    }

    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        // target still around?
        if (caster.target != null)
        {
            destination = Utils.ClosestPoint(caster.target, caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

    public override void Apply(Entity caster, int skillLevel)
    {

        // deal damage directly with base damage + skill damage
        caster.combat.DealDamageAt(caster.target,
                                   caster.combat.damage + damage.Get(skillLevel),
                                   stunChance.Get(skillLevel),
                                   stunTime.Get(skillLevel));

        if (UnityEngine.Random.value <= tauntChance.Get(skillLevel)) {
            SpawnEffect(caster, caster.target);
            caster.target.PartialOnAggro(caster);
        }
        
    }

    // tooltip
    public override string ToolTip(int skillLevel, bool showRequirements = false)
    {
        StringBuilder tip = new StringBuilder(base.ToolTip(skillLevel, showRequirements));
        tip.Replace("{TAUNTCHANCE}", Mathf.RoundToInt(tauntChance.Get(skillLevel) * 100).ToString());
        return tip.ToString();
    }
}
