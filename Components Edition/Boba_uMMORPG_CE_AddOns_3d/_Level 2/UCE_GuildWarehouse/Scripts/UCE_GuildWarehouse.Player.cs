// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using System.Linq;
using UnityEngine;

// PLAYER

public partial class Player
{
    public SyncListItemSlot UCE_guildWarehouse = new SyncListItemSlot();

    [Header("[-=-=-=- UCE GUILD WAREHOUSE -=-=-=-]")]
    public UCE_Tmpl_GuildWarehouse guildWarehouse;

    [SyncVar] private long _guildWarehouseGold = 0;
    [SyncVar] private int _guildWarehouseLevel = 0;
    [SyncVar, HideInInspector] public bool guildWarehouseLock = false;
    [SyncVar, HideInInspector] public bool guildWarehouseActionDone = false;
    private Npc selectedWarehouseNpc;

    // -----------------------------------------------------------------------------------
    // resetGuildWarehouse
    // -----------------------------------------------------------------------------------
    public void resetGuildWarehouse()
    {
        UCE_guildWarehouse.Clear();
        _guildWarehouseGold = 0;
        _guildWarehouseLevel = 0;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public long guildWarehouseGold
    {
        get { return _guildWarehouseGold; }
        set { _guildWarehouseGold = Math.Max(value, 0); }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public int guildWarehouseLevel
    {
        get { return _guildWarehouseLevel; }
        set { _guildWarehouseLevel = Math.Max(value, 0); }
    }

    // -----------------------------------------------------------------------------------
    // guildWarehouseStorageItems
    // -----------------------------------------------------------------------------------
    public int guildWarehouseStorageItems
    {
        get {
        	if (guildWarehouse)
        	{
        		return guildWarehouse.warehouseStorageItems.Get(guildWarehouseLevel + 1);
        	} else {
        		Debug.LogWarning("You forgot to assign a guild warehouse template to one of your player prefabs");
        		return 0;
         	}
        }
    }

    // -----------------------------------------------------------------------------------
    // guildWarehouseStorageGold
    // -----------------------------------------------------------------------------------
    public long guildWarehouseStorageGold
    {
        get {
        	if (guildWarehouse)
        	{
        		return guildWarehouse.warehouseStorageGold.Get(guildWarehouseLevel + 1);
        	} else {
        		Debug.LogWarning("You forgot to assign a guild warehouse template to one of your player prefabs");
        		return 0;
         	}
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public int GetUCE_warehouseIndexByName(string itemName)
    {
        return UCE_guildWarehouse.FindIndex(slot => slot.amount > 0 && slot.item.name == itemName);
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public int UCE_warehouseSlotsFree()
    {
        return UCE_guildWarehouse.Count(slot => slot.amount == 0);
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public int UCE_warehouseCountAmount(Item item)
    {
        return (from slot in UCE_guildWarehouse
                where slot.amount > 0 && slot.item.Equals(item)
                select slot.amount).Sum();
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public bool UCE_warehouseRemoveAmount(Item item, int amount)
    {
        for (int i = 0; i < UCE_guildWarehouse.Count; ++i)
        {
            ItemSlot slot = UCE_guildWarehouse[i];

            if (slot.amount > 0 && slot.item.Equals(item))
            {
                amount -= slot.DecreaseAmount(amount);
                UCE_guildWarehouse[i] = slot;
                if (amount == 0) return true;
            }
        }
        return false;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public bool UCE_warehouseCanAddAmount(Item item, int amount)
    {
        for (int i = 0; i < UCE_guildWarehouse.Count; ++i)
        {
            if (UCE_guildWarehouse[i].amount == 0)
                amount -= item.maxStack;
            else if (UCE_guildWarehouse[i].item.Equals(item))
                amount -= (UCE_guildWarehouse[i].item.maxStack - UCE_guildWarehouse[i].amount);

            if (amount <= 0) return true;
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public bool UCE_warehouseAddAmount(Item item, int amount)
    {
        if (item.tradable && UCE_warehouseCanAddAmount(item, amount))
        {
            // go through each slot
            for (int i = 0; i < UCE_guildWarehouse.Count; ++i)
            {
                // empty? then fill slot with as many as possible
                if (UCE_guildWarehouse[i].amount == 0)
                {
                    int add = Mathf.Min(amount, item.maxStack);
                    UCE_guildWarehouse[i] = new ItemSlot(item, add);
                    amount -= add;
                }
                // not empty and same type? then add free amount (max-amount)
                else if (UCE_guildWarehouse[i].item.Equals(item))
                {
                    ItemSlot temp = UCE_guildWarehouse[i];
                    amount -= temp.IncreaseAmount(amount);
                    UCE_guildWarehouse[i] = temp;
                }

                // were we able to fit the whole amount already?
                if (amount <= 0) return true;
            }
        }
        return false;
    }

    // ================================== COMMANDS =======================================

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_AccessGuildWarehouse
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_AccessGuildWarehouse(GameObject selectedNpc)
    {
        Npc npc = selectedNpc.GetComponent<Npc>();
        if (npc != null && Database.singleton.UCE_GetGuildWarehouseAccess(this))
        {
            Database.singleton.UCE_SetGuildWarehouseBusy(this);
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_BusyGuildWarehouse
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_BusyGuildWarehouse(GameObject selectedNpc, string guild)
    {
        Npc npc = selectedNpc.GetComponent<Npc>();
        if (npc != null)
        {
            selectedWarehouseNpc = npc;
            selectedWarehouseNpc.guildWarehouseBusy.Add(guild);
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_UnaccessGuildWarehouse
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_UnaccessGuildWarehouse(string guild)
    {
        if (selectedWarehouseNpc != null && selectedWarehouseNpc.guildWarehouseBusy.Contains(guild))
        {
            Cmd_UCE_SaveGuildWarehouse();
            selectedWarehouseNpc.guildWarehouseBusy.Remove(guild);
            selectedWarehouseNpc = null;
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_LoadGuildUCE_warehouse
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_LoadGuildWarehouse()
    {
        if (guild.name == "") return;
        Database.singleton.UCE_LoadGuildWarehouse(this);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_SaveGuildUCE_warehouse
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_SaveGuildWarehouse()
    {
        if (guild.name == "") return;
        Database.singleton.UCE_SaveGuildWarehouse(this);
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdSwapInventoryUCE_warehouse(int fromIndex, int toIndex)
    {
        ItemSlot slot = inventory.slots[fromIndex];

        if ((state == "IDLE" || state == "MOVING" || state == "CASTING") &&
            (guildWarehouse.storeTradable || (!guildWarehouse.storeTradable && !slot.item.tradable)) &&
            (guildWarehouse.storeSellable || (!guildWarehouse.storeSellable && !slot.item.sellable)) &&
            (guildWarehouse.storeDestroyable || (!guildWarehouse.storeDestroyable && !slot.item.destroyable)) &&
            0 <= fromIndex && fromIndex < inventory.slots.Count &&
            0 <= toIndex && toIndex < UCE_guildWarehouse.Count)
        {
            // don't allow player to add items which has zero amount or if it's summoned pet item
            if (slot.amount > 0 && !slot.item.summoned)
            {
                // swap them
                inventory.slots[fromIndex] = UCE_guildWarehouse[toIndex];
                UCE_guildWarehouse[toIndex] = slot;

                guildWarehouseActionDone = true;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdSwapUCE_warehouseInventory(int fromIndex, int toIndex)
    {
        if ((state == "IDLE" || state == "MOVING" || state == "CASTING") &&
            0 <= fromIndex && fromIndex < UCE_guildWarehouse.Count &&
            0 <= toIndex && toIndex < inventory.slots.Count)
        {
            // swap them
            ItemSlot temp = UCE_guildWarehouse[fromIndex];
            UCE_guildWarehouse[fromIndex] = inventory.slots[toIndex];
            inventory.slots[toIndex] = temp;

            guildWarehouseActionDone = true;
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdSwapUCE_warehouseUCE_warehouse(int fromIndex, int toIndex)
    {
        if ((state == "IDLE" || state == "MOVING" || state == "CASTING") &&
            0 <= fromIndex && fromIndex < UCE_guildWarehouse.Count &&
            0 <= toIndex && toIndex < UCE_guildWarehouse.Count &&
            fromIndex != toIndex)
        {
            // swap them
            ItemSlot temp = UCE_guildWarehouse[fromIndex];
            UCE_guildWarehouse[fromIndex] = UCE_guildWarehouse[toIndex];
            UCE_guildWarehouse[toIndex] = temp;

            guildWarehouseActionDone = true;
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdUCE_warehouseSplit(int fromIndex, int toIndex)
    {
        if ((state == "IDLE" || state == "MOVING" || state == "CASTING") &&
            0 <= fromIndex && fromIndex < UCE_guildWarehouse.Count &&
            0 <= toIndex && toIndex < UCE_guildWarehouse.Count &&
            fromIndex != toIndex)
        {
            // slotFrom has to have an entry, slotTo has to be empty
            ItemSlot slotFrom = UCE_guildWarehouse[fromIndex];
            ItemSlot slotTo = UCE_guildWarehouse[toIndex];

            // from entry needs at least amount of 2
            if (slotFrom.amount >= 2 && slotTo.amount == 0)
            {
                // split them serversided (has to work for even and odd)
                slotTo = slotFrom;

                slotTo.amount = slotFrom.amount / 2;
                slotFrom.amount -= slotTo.amount; // works for odd too

                // put back into the list
                UCE_guildWarehouse[fromIndex] = slotFrom;
                UCE_guildWarehouse[toIndex] = slotTo;

                guildWarehouseActionDone = true;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdUCE_warehouseMerge(int fromIndex, int toIndex)
    {
        if ((state == "IDLE" || state == "MOVING" || state == "CASTING") &&
            0 <= fromIndex && fromIndex < UCE_guildWarehouse.Count &&
            0 <= toIndex && toIndex < UCE_guildWarehouse.Count &&
            fromIndex != toIndex)
        {
            // both items have to be valid
            ItemSlot slotFrom = UCE_guildWarehouse[fromIndex];
            ItemSlot slotTo = UCE_guildWarehouse[toIndex];

            if (slotFrom.amount > 0 && slotTo.amount > 0)
            {
                // check if the both items are the same type
                if (slotFrom.item.Equals(slotTo.item))
                {
                    int put = slotTo.IncreaseAmount(slotFrom.amount);
                    slotFrom.DecreaseAmount(put);

                    // put back into the list
                    UCE_guildWarehouse[fromIndex] = slotFrom;
                    UCE_guildWarehouse[toIndex] = slotTo;

                    guildWarehouseActionDone = true;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_DepositGold(int amount)
    {
        UCE_warehouseAddGold(amount);
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_WithdrawGold(int amount)
    {
        UCE_warehouseRemoveGold(amount);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_UpgradeGuildWarehouse
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_UpgradeGuildWarehouse()
    {
        UCE_UpgradeGuildWarehouse();
    }

    // -----------------------------------------------------------------------------------
    // UCE_UpgradePlayerWarehouse
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_UpgradeGuildWarehouse()
    {
        if (UCE_CanUpgradeGuildWarehouse())
        {
            guildWarehouse.warehouseUpgradeCost[guildWarehouseLevel].payCost(this);

            int oldSize = guildWarehouseStorageItems;
            guildWarehouseLevel++;

            int sizeDifference = guildWarehouseStorageItems - oldSize;

            for (int i = 0; i < sizeDifference; ++i)
            {
                UCE_guildWarehouse.Add(new ItemSlot());
            }

            guildWarehouseActionDone = true;

            UCE_ShowPopup(guildWarehouse.warehouseUpgradeLabel);
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_ToggleGuildWarehouseLock
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void Cmd_UCE_ToggleGuildWarehouseLock()
    {
        if (guild.InGuild() && guild.guild.CanNotify(name) &&
            NetworkTime.time >= nextRiskyActionTime
            )
        {
            guildWarehouseLock = (guildWarehouseLock == true) ? false : true;
            nextRiskyActionTime = NetworkTime.time + GuildSystem.NoticeWaitSeconds;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_HasEnoughGoldOnInventory
    // -----------------------------------------------------------------------------------
    [Server]
    public bool UCE_HasEnoughGoldOnInventory(long amount)
    {
        return amount > 0 && gold >= amount;
    }

    // -----------------------------------------------------------------------------------
    // HasEnoughGoldOnUCE_warehouse
    // -----------------------------------------------------------------------------------
    [Server]
    public bool HasEnoughGoldOnUCE_warehouse(long amount)
    {
        return amount > 0 && guildWarehouseGold >= amount;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HasEnoughGoldSpace
    // -----------------------------------------------------------------------------------
    public bool UCE_HasEnoughGoldSpace(long amount = 1)
    {
        return amount > 0 && guildWarehouseStorageGold >= guildWarehouseGold + amount;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanUpgradeGuildWarehouse
    // @Client OR @Server
    // -----------------------------------------------------------------------------------
    public bool UCE_CanUpgradeGuildWarehouse()
    {
        return (guildWarehouse.warehouseUpgradeCost.Length > 0 && guildWarehouse.warehouseUpgradeCost.Length > guildWarehouseLevel && guildWarehouse.warehouseUpgradeCost[guildWarehouseLevel].checkCost(this));
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanAccessGuildWarehouse
    // @Client OR @Server
    // -----------------------------------------------------------------------------------
    public bool UCE_CanAccessGuildWarehouse()
    {
        return (isAlive &&
                (!guildWarehouseLock ||
                (guildWarehouseLock && guild.InGuild() && guild.guild.CanNotify(name)))
                );
    }

    // -----------------------------------------------------------------------------------
    // UCE_warehouseAddGold
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_warehouseAddGold(long amount)
    {
        if (UCE_HasEnoughGoldOnInventory(amount) && UCE_HasEnoughGoldSpace(amount))
        {
            gold -= amount;
            guildWarehouseGold += amount;
            guildWarehouseActionDone = true;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_warehouseRemoveGold
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_warehouseRemoveGold(long amount)
    {
        if (HasEnoughGoldOnUCE_warehouse(amount))
        {
            guildWarehouseGold -= amount;
            gold += amount;
            guildWarehouseActionDone = true;
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_UCE_warehouseSlot_UCE_warehouseSlot(int[] slotIndices)
    {
        // merge? (just check the name, rest is done server sided)
        if (UCE_guildWarehouse[slotIndices[0]].amount > 0 && UCE_guildWarehouse[slotIndices[1]].amount > 0 &&
            UCE_guildWarehouse[slotIndices[0]].item.Equals(UCE_guildWarehouse[slotIndices[1]].item))
        {
            CmdUCE_warehouseMerge(slotIndices[0], slotIndices[1]);
            // split?
        }
        else if (Utils.AnyKeyPressed(inventory.splitKeys))
        {
            CmdUCE_warehouseSplit(slotIndices[0], slotIndices[1]);
            // swap?
        }
        else
        {
            CmdSwapUCE_warehouseUCE_warehouse(slotIndices[0], slotIndices[1]);
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_UCE_warehouseSlot_InventorySlot(int[] slotIndices)
    {
        if (UCE_guildWarehouse[slotIndices[0]].amount > 0)
        {
            CmdSwapUCE_warehouseInventory(slotIndices[0], slotIndices[1]);
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_InventorySlot_UCE_warehouseSlot(int[] slotIndices)
    {
        if (inventory.slots[slotIndices[0]].amount > 0)
        {
            CmdSwapInventoryUCE_warehouse(slotIndices[0], slotIndices[1]);
        }
    }
    
    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDestroy")]
    private void OnDestroy_UCE_GuildWarehouse()
    {
        if (NetworkServer.active)
        {
            if (guild.InGuild())
                Cmd_UCE_UnaccessGuildWarehouse(guild.name);
        }
    }

    // -----------------------------------------------------------------------------------
}
