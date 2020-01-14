// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// =======================================================================================
using System.Text;
using UnityEngine;
using Mirror;

// =======================================================================================
// TARGET PROJECTILE SKILL
// =======================================================================================
[CreateAssetMenu(menuName= "UCE Skills/UCE Target Projectile Skill", order=999)]
public class UCE_TargetProjectileSkill : UCE_BaseDamageSkill
{
    [Header("Projectile")]
    public UCE_ProjectileSkillEffect projectile; // Arrows, Bullets, Fireballs, ...

    bool HasRequiredWeaponAndAmmo(Entity caster)
    {
        int weaponIndex = caster.GetEquippedWeaponIndex();
        if (weaponIndex != -1)
        {
            // no ammo required, or has that ammo equipped?
            WeaponItem itemData = (WeaponItem)caster.equipment[weaponIndex].item.data;
            return itemData.requiredAmmo == null ||
                   caster.GetEquipmentIndexByName(itemData.requiredAmmo.name) != -1;
        }
        return true;
    }

    void ConsumeRequiredWeaponsAmmo(Entity caster)
    {
        int weaponIndex = caster.GetEquippedWeaponIndex();
        if (weaponIndex != -1)
        {
            // no ammo required, or has that ammo equipped?
            WeaponItem itemData = (WeaponItem)caster.equipment[weaponIndex].item.data;
            if (itemData.requiredAmmo != null)
            {
                int ammoIndex = caster.GetEquipmentIndexByName(itemData.requiredAmmo.name);
                if (ammoIndex != 0)
                {
                    // reduce it
                    ItemSlot slot = caster.equipment[ammoIndex];
                    --slot.amount;
                    caster.equipment[ammoIndex] = slot;
                }
            }
        }
    }

    public override bool CheckSelf(Entity caster, int skillLevel)
    {
        // check base and ammo
        return base.CheckSelf(caster, skillLevel) &&
               HasRequiredWeaponAndAmmo(caster);
    }

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
            destination = caster.target.collider.ClosestPointOnBounds(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

    public override void Apply(Entity caster, int skillLevel)
    {
        // consume ammo if needed
        ConsumeRequiredWeaponsAmmo(caster);

        // spawn the skill effect. this can be used for anything ranging from
        // blood splatter to arrows to chain lightning.
        // -> we need to call an RPC anyway, it doesn't make much of a diff-
        //    erence if we use NetworkServer.Spawn for everything.
        // -> we try to spawn it at the weapon's projectile mount
        if (projectile != null)
        {
            GameObject go = Instantiate(projectile.gameObject, caster.effectMount.position, caster.effectMount.rotation);
            UCE_ProjectileSkillEffect effect = go.GetComponent<UCE_ProjectileSkillEffect>();
            effect.target = caster.target;
            effect.caster = caster;
            effect.skillLevel = skillLevel;
            effect.parentSkill = this;
            NetworkServer.Spawn(go);
        }
        else Debug.LogWarning(name + ": missing projectile");
    }
}
