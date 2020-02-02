// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// EQUIPMENT ITEM

public partial class EquipmentItem
{
    [Header("[-=-=- UCE EQUIPABLE BAG -=-=-]")]
    public int extraInventorySize;

    public string bagCannotUnequipMsg = "You carry too many items - bag cannot be unequipped!";

    // -----------------------------------------------------------------------------------
    // UCE_canUnequipBag
    // -----------------------------------------------------------------------------------
    public bool UCE_canUnequipBag(Player player)
    {
        if (extraInventorySize == 0) return true;

        bool bValid = player.UCE_inventorySlotCount(player) < player.inventorySize - extraInventorySize; // has to be less, because we have to take the unequipped item into account as well

        if (bValid == false)
            player.UCE_TargetAddMessage(bagCannotUnequipMsg);

        return bValid;
    }

    // -----------------------------------------------------------------------------------
    // CanUnequip (Swapping)
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CanUnequip")]
    private void CanUnequip_UCE_EquipableBag(Player player, int inventoryIndex, int equipmentIndex, MutableWrapper<bool> bValid)
    {
        if (!bValid.Value) return; //when not valid, we dont have to check at all

        bValid.Value = player.UCE_inventorySlotCount(player) < player.inventorySize - extraInventorySize; // has to be less, because we have to take the unequipped item into account as well

        if (bValid.Value == false)
            player.UCE_TargetAddMessage(bagCannotUnequipMsg);
    }

    // -----------------------------------------------------------------------------------
    // CanUnequipClick (Clicking)
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CanUnequipClick")]
    private void CanUnequipClick_UCE_EquipableBag(Player player, EquipmentItem item, MutableWrapper<bool> bValid)
    {
        if (!bValid.Value) return; //when not valid, we dont have to check at all

        bValid.Value = player.UCE_inventorySlotCount(player) < player.inventorySize - extraInventorySize; // has to be less, because we have to take the unequipped item into account as well

        if (bValid.Value == false)
            player.UCE_TargetAddMessage(bagCannotUnequipMsg);
    }

    // -----------------------------------------------------------------------------------
}
