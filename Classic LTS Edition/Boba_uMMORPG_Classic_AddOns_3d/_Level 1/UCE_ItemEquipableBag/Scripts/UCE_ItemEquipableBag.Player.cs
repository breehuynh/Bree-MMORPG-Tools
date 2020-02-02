// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// PLAYER

public partial class Player
{
    [Header("[-=-=- UCE INVENTORY SIZE -=-=-]")]
    public int _inventorySize = 30;

    protected int UCE_inventoryExtraSize;

    // -----------------------------------------------------------------------------------
    // inventorySize
    // -----------------------------------------------------------------------------------
    public int inventorySize
    {
        get
        {
            UCE_calculateInventoryExtraSlots();
            return _inventorySize + UCE_inventoryExtraSize;
        }
    }

    // -----------------------------------------------------------------------------------
    // OnStartLocalPlayer_UCE_EquipableBag
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartLocalPlayer")]
    private void OnStartLocalPlayer_UCE_EquipableBag()
    {
        OnEquipmentChanged_UCE_EquipableBag();
    }

    // -----------------------------------------------------------------------------------
    // OnEquipmentChanged_UCE_EquipableBag
    // @Client
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnEquipmentChanged")]
    private void OnEquipmentChanged_UCE_EquipableBag()
    {
        UCE_calculateInventoryExtraSlots();

        if (inventorySize != inventory.Count)
            Cmd_UCE_calculcateInvenvtory();
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_calculcateInvenvtory
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    private void Cmd_UCE_calculcateInvenvtory()
    {
        UCE_calculateInventoryExtraSlots();

        if (inventorySize > inventory.Count)
        {
            for (int i = inventory.Count; i < inventorySize; i++)
                inventory.Add(new ItemSlot());
        }
        else if (inventorySize < inventory.Count)
        {
            for (int i = inventory.Count - 1; inventory.Count > inventorySize && i >= 0; i--)
            {
                if (inventory[i].amount == 0)
                    inventory.RemoveAt(i);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_getInventoryExtraSlots
    // -----------------------------------------------------------------------------------
    public void UCE_calculateInventoryExtraSlots()
    {
        UCE_inventoryExtraSize = 0;

        for (int i = 0; i < equipment.Count; ++i)
        {
            if (equipment[i].amount > 0)
                UCE_inventoryExtraSize += ((EquipmentItem)equipment[i].item.data).extraInventorySize;
        }
    }

    // -----------------------------------------------------------------------------------
}
