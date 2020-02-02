using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName="UCE Item/Potion", order=999)]
public class UCE_PotionItem : UsableItem
{
    [Header("Potion")]
    public int usageHealth;
    public int usageMana;
    public int usageStamina;
    public int usageExperience;
    public int usagePetHealth; // to heal pet

    // usage
    public override void Use(Player player, int inventoryIndex)
    {
        // always call base function too
        base.Use(player, inventoryIndex);

        // increase health/mana/etc.
        player.health.current 		+= usageHealth;
        player.mana.current += usageMana;
        player.stamina 		+= usageStamina;
        player.experience.current += usageExperience;
        if (player.petControl.activePet != null) player.petControl.activePet.health.current += usagePetHealth;

        // decrease amount
        ItemSlot slot = player.inventory.slots[inventoryIndex];
        slot.DecreaseAmount(1);
        player.inventory.slots[inventoryIndex] = slot;
    }

    // tooltip
    public override string ToolTip()
    {
        StringBuilder tip = new StringBuilder(base.ToolTip());
        tip.Replace("{USAGEHEALTH}", usageHealth.ToString());
        tip.Replace("{USAGEMANA}", usageMana.ToString());
        tip.Replace("{USAGESTAMINA}", usageStamina.ToString());
        tip.Replace("{USAGEEXPERIENCE}", usageExperience.ToString());
        tip.Replace("{USAGEPETHEALTH}", usagePetHealth.ToString());
        return tip.ToString();
    }
}
