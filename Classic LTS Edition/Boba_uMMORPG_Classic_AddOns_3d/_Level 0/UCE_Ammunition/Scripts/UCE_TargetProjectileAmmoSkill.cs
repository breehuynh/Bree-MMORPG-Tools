// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using UnityEngine;
using Mirror;

// TARGET PROJECTILE AMMO SKILL

[CreateAssetMenu(menuName = "uMMORPG Skill/UCE Ammo based Target Projectile", order = 998)]
public class UCE_TargetProjectileAmmoSkill : TargetProjectileSkill
{
    [Header("-=-=-=- Ammunition -=-=-=-")]
    [Tooltip("[Required] Having ANY of these weapons equipped is enough for the skill to be useable")]
    public ScriptableItem[] equippedWeapons;

    [Tooltip("[Required] The ammunition must be in the players inventory")]
    public ScriptableItem requiredAmmo;

    [Tooltip("[Required] How much ammo is deducted per skill use?")]
    public int ammoAmount;

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        bool valid = true;

        if (caster.target == null && !caster.CanAttack(caster.target))
            return false;

        Player player = caster.GetComponent<Player>();

        if (player != null)
        {
            if (requiredAmmo && player.InventoryCount(new Item(requiredAmmo)) < ammoAmount)
                return false;

            valid = false;

            foreach (ScriptableItem equippedWeapon in equippedWeapons)
            {
                if (equippedWeapon && player.UCE_Ammunition_checkHasEquipment(equippedWeapon))
                {
                    valid = true;
                    break;
                }
            }
        }

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPointOnBounds(caster.transform.position);
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
        if (projectile != null)
        {
            // ---- Deduct Ammunition
            Player player = caster.GetComponent<Player>();
            if (player)
            {
                if (player.InventoryCount(new Item(requiredAmmo)) >= ammoAmount)
                    player.InventoryRemove(new Item(requiredAmmo), ammoAmount);
            }

            GameObject go = Instantiate(projectile.gameObject, caster.effectMount.position, caster.effectMount.rotation);
            ProjectileSkillEffect effect = go.GetComponent<ProjectileSkillEffect>();
            effect.target = caster.target;
            effect.caster = caster;
            effect.damage = damage.Get(skillLevel);
            NetworkServer.Spawn(go);
        }
        else Debug.LogWarning(name + ": missing projectile");
    }

    // -----------------------------------------------------------------------------------
    // ToolTip
    // -----------------------------------------------------------------------------------
    public override string ToolTip(int skillLevel, bool showRequirements = false)
    {
        StringBuilder tip = new StringBuilder(base.ToolTip(skillLevel, showRequirements));
        tip.Replace("{DAMAGE}", damage.Get(skillLevel).ToString());

        string s = "";
        if (equippedWeapons.Length > 0)
        {
            s += "Allowed Weapon(s): \n";
            foreach (ScriptableItem equippedWeapon in equippedWeapons)
            {
                if (equippedWeapon)
                    s += "* " + equippedWeapon.name + "\n";
            }
        }
        tip.Replace("{EQUIPPEDWEAPON}", s);

        if (requiredAmmo)
        {
            tip.Replace("{REQUIREDAMMO}", "Required Ammunition: \n" + requiredAmmo.name + "[x" + ammoAmount + "]\n");
        }
        else
        {
            tip.Replace("{REQUIREDAMMO}", "");
        }

        return tip.ToString();
    }

    // -----------------------------------------------------------------------------------
}
