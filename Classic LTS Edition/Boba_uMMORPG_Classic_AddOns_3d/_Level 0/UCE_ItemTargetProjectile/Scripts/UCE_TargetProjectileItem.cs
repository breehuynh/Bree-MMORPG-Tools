// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using UnityEngine;

// TARGET PROJECTILE - ITEM

[CreateAssetMenu(menuName = "uMMORPG Item/UCE TargetProjectileItem", order = 999)]
public class UCE_TargetProjectileItem : UsableItem
{
    [Header("-=-=-=- UCE Target Projectile Item -=-=-=-")]
    public TargetProjectileSkill projectile;

    public int level;

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPointOnBounds(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= projectile.castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

    // -----------------------------------------------------------------------------------
    // Use
    // @Server
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        Vector3 destination;

        if (projectile != null &&
            level > 0 &&
            player.target != null &&
            player.CanAttack(player.target) &&
            CheckDistance(player, level, out destination)
            )
        {
            // always call base function too
            base.Use(player, inventoryIndex);

            // launch projectile
            projectile.Apply(player, level);

            // decrease amount
            ItemSlot slot = player.inventory[inventoryIndex];
            slot.DecreaseAmount(1);
            player.inventory[inventoryIndex] = slot;
        }
    }

    // tooltip
    /*public override string ToolTip()
    {
        StringBuilder tip = new StringBuilder(base.ToolTip());
        tip.Replace("{USAGEHEALTH}", usageHealth.ToString());
        tip.Replace("{USAGEMANA}", usageMana.ToString());
        tip.Replace("{USAGEEXPERIENCE}", usageExperience.ToString());
        tip.Replace("{USAGEPETHEALTH}", usagePetHealth.ToString());
        return tip.ToString();
    }*/

    // -----------------------------------------------------------------------------------
}
