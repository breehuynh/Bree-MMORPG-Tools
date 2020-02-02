// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

// PLAYER

public partial class Player
{
    public SyncListItemSlot UCE_playerWarehouse = new SyncListItemSlot();

    [Header("[-=-=- UCE PLAYER WAREHOUSE -=-=-]")]
    public UCE_Tmpl_PlayerWarehouse playerWarehouse;

    [SyncVar] private long _playerWarehouseGold = 0;
    [SyncVar] private int _playerWarehouseLevel = 0;
    [SyncVar, HideInInspector] public bool warehouseActionDone = false;

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public long playerWarehouseGold
    {
        get { return _playerWarehouseGold; }
        set { _playerWarehouseGold = Math.Max(value, 0); }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public int playerWarehouseLevel
    {
        get { return _playerWarehouseLevel; }
        set { _playerWarehouseLevel = Math.Max(value, 0); }
    }

    // -----------------------------------------------------------------------------------
    // playerWarehouseStorageItems
    // -----------------------------------------------------------------------------------
    public int playerWarehouseStorageItems
    {
        get {
        	if (playerWarehouse)
        	{
        		return playerWarehouse.warehouseStorageItems.Get(_playerWarehouseLevel + 1);
        	} else{ 
        		Debug.LogWarning("You forgot to assign a warehouse template to one of your player prefabs");
        		return 0;
        	}
         }
    }

    // -----------------------------------------------------------------------------------
    // playerWarehouseStorageGold
    // -----------------------------------------------------------------------------------
    public long playerWarehouseStorageGold
    {
        get {
        	if (playerWarehouse)
        	{
        		return playerWarehouse.warehouseStorageGold.Get(playerWarehouseLevel + 1);
        	} else {
        		Debug.LogWarning("You forgot to assign a warehouse template to one of your player prefabs");
        		return 0;
         	}
         }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public int GetWarehouseIndexByName(string itemName)
    {
        return UCE_playerWarehouse.FindIndex(slot => slot.amount > 0 && slot.item.name == itemName);
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public int WarehouseSlotsFree()
    {
        return UCE_playerWarehouse.Count(slot => slot.amount == 0);
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public int WarehouseCountAmount(Item item)
    {
        return (from slot in UCE_playerWarehouse
                where slot.amount > 0 && slot.item.Equals(item)
                select slot.amount).Sum();
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public bool WarehouseRemoveAmount(Item item, int amount)
    {
        for (int i = 0; i < UCE_playerWarehouse.Count; ++i)
        {
            ItemSlot slot = UCE_playerWarehouse[i];
            if (slot.amount > 0 && slot.item.Equals(item))
            {
                amount -= slot.DecreaseAmount(amount);
                UCE_playerWarehouse[i] = slot;
                if (amount == 0) return true;
            }
        }
        return false;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public bool WarehouseCanAddAmount(Item item, int amount)
    {
        for (int i = 0; i < UCE_playerWarehouse.Count; ++i)
        {
            if (UCE_playerWarehouse[i].amount == 0)
                amount -= item.maxStack;
            else if (UCE_playerWarehouse[i].item.Equals(item))
                amount -= (UCE_playerWarehouse[i].item.maxStack - UCE_playerWarehouse[i].amount);
            if (amount <= 0) return true;
        }
        return false;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public bool WarehouseAddAmount(Item item, int amount)
    {
        // we only want to add them if there is enough space for all of them, so
        // let's double check
        if (item.tradable && WarehouseCanAddAmount(item, amount))
        {
            // go through each slot
            for (int i = 0; i < UCE_playerWarehouse.Count; ++i)
            {
                // empty? then fill slot with as many as possible
                if (UCE_playerWarehouse[i].amount == 0)
                {
                    int add = Mathf.Min(amount, item.maxStack);
                    UCE_playerWarehouse[i] = new ItemSlot(item, add);
                    amount -= add;
                }
                // not empty and same type? then add free amount (max-amount)
                else if (UCE_playerWarehouse[i].item.Equals(item))
                {
                    ItemSlot temp = UCE_playerWarehouse[i];
                    amount -= temp.IncreaseAmount(amount);
                    UCE_playerWarehouse[i] = temp;
                }

                // were we able to fit the whole amount already?
                if (amount <= 0) return true;
            }
            // we should have been able to add all of them
            if (amount != 0) Debug.LogError("warehouse add failed: " + item.name + " " + amount);
        }
        return false;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdSwapInventoryWarehouse(int fromIndex, int toIndex)
    {
        ItemSlot slot = inventory[fromIndex];

        if ((state == "IDLE" || state == "MOVING" || state == "CASTING") &&
            (playerWarehouse.storeTradable || (!playerWarehouse.storeTradable && !slot.item.tradable)) &&
            (playerWarehouse.storeSellable || (!playerWarehouse.storeSellable && !slot.item.sellable)) &&
            (playerWarehouse.storeDestroyable || (!playerWarehouse.storeDestroyable && !slot.item.destroyable)) &&
            WarehouseSlotsFree() >= 1 &&
            0 <= fromIndex && fromIndex < inventory.Count &&
            0 <= toIndex && toIndex < UCE_playerWarehouse.Count)
        {
            // don't allow player to add items which has zero amount or if it's summoned pet item
            if (slot.amount > 0 && !slot.item.summoned)
            {
                // swap them
                inventory[fromIndex] = UCE_playerWarehouse[toIndex];
                UCE_playerWarehouse[toIndex] = slot;

                warehouseActionDone = true;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdSwapWarehouseInventory(int fromIndex, int toIndex)
    {
        if ((state == "IDLE" || state == "MOVING" || state == "CASTING") &&
            0 <= fromIndex && fromIndex < UCE_playerWarehouse.Count &&
            0 <= toIndex && toIndex < inventory.Count)
        {
            // swap them
            ItemSlot temp = UCE_playerWarehouse[fromIndex];
            UCE_playerWarehouse[fromIndex] = inventory[toIndex];
            inventory[toIndex] = temp;

            warehouseActionDone = true;
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdSwapWarehouseWarehouse(int fromIndex, int toIndex)
    {
        if ((state == "IDLE" || state == "MOVING" || state == "CASTING") &&
            0 <= fromIndex && fromIndex < UCE_playerWarehouse.Count &&
            0 <= toIndex && toIndex < UCE_playerWarehouse.Count &&
            fromIndex != toIndex)
        {
            // swap them
            ItemSlot temp = UCE_playerWarehouse[fromIndex];
            UCE_playerWarehouse[fromIndex] = UCE_playerWarehouse[toIndex];
            UCE_playerWarehouse[toIndex] = temp;

            warehouseActionDone = true;
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdWarehouseSplit(int fromIndex, int toIndex)
    {
        if ((state == "IDLE" || state == "MOVING" || state == "CASTING") &&
            0 <= fromIndex && fromIndex < UCE_playerWarehouse.Count &&
            0 <= toIndex && toIndex < UCE_playerWarehouse.Count &&
            fromIndex != toIndex)
        {
            // slotFrom has to have an entry, slotTo has to be empty
            ItemSlot slotFrom = UCE_playerWarehouse[fromIndex];
            ItemSlot slotTo = UCE_playerWarehouse[toIndex];

            // from entry needs at least amount of 2
            if (slotFrom.amount >= 2 && slotTo.amount == 0)
            {
                // split them serversided (has to work for even and odd)
                slotTo = slotFrom;

                slotTo.amount = slotFrom.amount / 2;
                slotFrom.amount -= slotTo.amount; // works for odd too

                // put back into the list
                UCE_playerWarehouse[fromIndex] = slotFrom;
                UCE_playerWarehouse[toIndex] = slotTo;

                warehouseActionDone = true;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdWarehouseMerge(int fromIndex, int toIndex)
    {
        if ((state == "IDLE" || state == "MOVING" || state == "CASTING") &&
            0 <= fromIndex && fromIndex < UCE_playerWarehouse.Count &&
            0 <= toIndex && toIndex < UCE_playerWarehouse.Count &&
            fromIndex != toIndex)
        {
            // both items have to be valid
            ItemSlot slotFrom = UCE_playerWarehouse[fromIndex];
            ItemSlot slotTo = UCE_playerWarehouse[toIndex];

            if (slotFrom.amount > 0 && slotTo.amount > 0)
            {
                // check if the both items are the same type
                if (slotFrom.item.Equals(slotTo.item))
                {
                    int put = slotTo.IncreaseAmount(slotFrom.amount);
                    slotFrom.DecreaseAmount(put);

                    // put back into the list
                    UCE_playerWarehouse[fromIndex] = slotFrom;
                    UCE_playerWarehouse[toIndex] = slotTo;

                    warehouseActionDone = true;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdDepositGold(int amount)
    {
        WarehouseAddGold(amount);
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdWithdrawGold(int amount)
    {
        WarehouseRemoveGold(amount);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_UpgradePlayerWarehouse
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_UpgradePlayerWarehouse()
    {
        UCE_UpgradePlayerWarehouse();
    }

    // -----------------------------------------------------------------------------------
    // UCE_UpgradePlayerWarehouse
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_UpgradePlayerWarehouse()
    {
        if (UCE_CanUpgradePlayerWarehouse())
        {
            playerWarehouse.warehouseUpgradeCost[playerWarehouseLevel].payCost(this);

            int oldSize = playerWarehouseStorageItems;
            playerWarehouseLevel++;

            int sizeDifference = playerWarehouseStorageItems - oldSize;

            for (int i = 0; i < sizeDifference; ++i)
            {
                UCE_playerWarehouse.Add(new ItemSlot());
            }

            warehouseActionDone = true;

            UCE_ShowPopup(playerWarehouse.upgradeLabel);
        }
    }

    // -----------------------------------------------------------------------------------
    // HasEnoughGoldOnInventory
    // -----------------------------------------------------------------------------------
    [Server]
    public bool HasEnoughGoldOnInventory(long amount)
    {
        return amount > 0 && gold >= amount;
    }

    // -----------------------------------------------------------------------------------
    // HasEnoughGoldOnWarehouse
    // -----------------------------------------------------------------------------------
    [Server]
    public bool HasEnoughGoldOnWarehouse(long amount)
    {
        return amount > 0 && playerWarehouseGold >= amount;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HasEnoughGoldSpace
    // -----------------------------------------------------------------------------------
    public bool UCE_HasEnoughPlayerWarehouseGoldSpace(long amount = 1)
    {
        return amount > 0 && playerWarehouseStorageGold >= playerWarehouseGold + amount;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanUpgradePlayerWarehouse
    // @Client OR @Server
    // -----------------------------------------------------------------------------------
    public bool UCE_CanUpgradePlayerWarehouse()
    {
        return (playerWarehouse.warehouseUpgradeCost.Length > 0 && playerWarehouse.warehouseUpgradeCost.Length > playerWarehouseLevel && playerWarehouse.warehouseUpgradeCost[playerWarehouseLevel].checkCost(this));
    }

    // -----------------------------------------------------------------------------------
    // WarehouseAddGold
    // -----------------------------------------------------------------------------------
    [Server]
    public void WarehouseAddGold(long amount)
    {
        if (HasEnoughGoldOnInventory(amount) && UCE_HasEnoughPlayerWarehouseGoldSpace(amount))
        {
            gold -= amount;
            playerWarehouseGold += amount;
            warehouseActionDone = true;
        }
    }

    // -----------------------------------------------------------------------------------
    // WarehouseRemoveGold
    // -----------------------------------------------------------------------------------
    [Server]
    public void WarehouseRemoveGold(long amount)
    {
        if (HasEnoughGoldOnWarehouse(amount))
        {
            playerWarehouseGold -= amount;
            gold += amount;
            warehouseActionDone = true;
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_WarehouseSlot_WarehouseSlot(int[] slotIndices)
    {
        // merge? (just check the name, rest is done server sided)
        if (UCE_playerWarehouse[slotIndices[0]].amount > 0 && UCE_playerWarehouse[slotIndices[1]].amount > 0 &&
            UCE_playerWarehouse[slotIndices[0]].item.Equals(UCE_playerWarehouse[slotIndices[1]].item))
        {
            CmdWarehouseMerge(slotIndices[0], slotIndices[1]);
            // split?
        }
        else if (Utils.AnyKeyPressed(inventorySplitKeys))
        {
            CmdWarehouseSplit(slotIndices[0], slotIndices[1]);
            // swap?
        }
        else
        {
            CmdSwapWarehouseWarehouse(slotIndices[0], slotIndices[1]);
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_WarehouseSlot_InventorySlot(int[] slotIndices)
    {
        if (UCE_playerWarehouse[slotIndices[0]].amount > 0)
        {
            CmdSwapWarehouseInventory(slotIndices[0], slotIndices[1]);
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_InventorySlot_WarehouseSlot(int[] slotIndices)
    {
        if (inventory[slotIndices[0]].amount > 0)
        {
            CmdSwapInventoryWarehouse(slotIndices[0], slotIndices[1]);
        }
    }

    // -----------------------------------------------------------------------------------
}
