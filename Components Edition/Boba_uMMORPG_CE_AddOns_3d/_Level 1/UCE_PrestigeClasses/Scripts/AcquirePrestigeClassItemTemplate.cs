// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// ACQUIRE PRESTIGE CLASS TEMPLATE

[CreateAssetMenu(menuName = "uMMORPG Item/UCE Acquire Prestige Class Item", order = 999)]
public class AcquirePrestigeClassItemTemplate : UsableItem
{
    [Header("Usage")]
    public UCE_PrestigeClassTemplate prestigeClass;

    public string successMessage = "You acquired the prestige class: ";

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    // -----------------------------------------------------------------------------------
    // CanUse
    // -----------------------------------------------------------------------------------
    public override bool CanUse(Player player, int inventoryIndex)
    {
        return minLevel < player.level.current;
    }

    // -----------------------------------------------------------------------------------
    // Use
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory.slots[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            // always call base function too
            base.Use(player, inventoryIndex);

            player.UCE_prestigeClass = prestigeClass;
            player.UCE_TargetAddMessage(successMessage + prestigeClass.name);

            // decrease amount
            slot.DecreaseAmount(decreaseAmount);
            player.inventory.slots[inventoryIndex] = slot;
        }
    }

    // -----------------------------------------------------------------------------------
}
