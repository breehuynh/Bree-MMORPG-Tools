// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// EQUIPMENT ITEM

public partial class EquipmentItem
{

    // -----------------------------------------------------------------------------------
    // CanUnequip (Swapping)
    // -----------------------------------------------------------------------------------
    public bool CanUnequip(Player player, int inventoryIndex, int equipmentIndex)
    {
        MutableWrapper<bool> bValid = new MutableWrapper<bool>(true);
        //this.InvokeInstanceDevExtMethods("CanUnequip", player, inventoryIndex, equipmentIndex, bValid);
        Utils.InvokeMany(typeof(EquipmentItem), this, "CanUnequip_", player, inventoryIndex, equipmentIndex, bValid);
        return bValid.Value;
    }

    // -----------------------------------------------------------------------------------
    // CanUnequipClick (Clicking)
    // -----------------------------------------------------------------------------------
    public bool CanUnequipClick(Player player, EquipmentItem item)
    {
        MutableWrapper<bool> bValid = new MutableWrapper<bool>(true);
        //this.InvokeInstanceDevExtMethods("CanUnequipClick", player, item, bValid);
        Utils.InvokeMany(typeof(EquipmentItem), this, "CanUnequipClick_", player, item, bValid);
        return bValid.Value;
    }

    // -----------------------------------------------------------------------------------
}
