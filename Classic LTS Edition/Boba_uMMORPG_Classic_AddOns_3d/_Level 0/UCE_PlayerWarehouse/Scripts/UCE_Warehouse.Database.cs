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
using System.IO;
using System.Linq;
using System.Collections.Generic;

#if _MYSQL && _SERVER
using MySql.Data;
using MySql.Data.MySqlClient;
#elif _SQLITE && _SERVER
using SQLite;
#endif

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{

#if _SQLITE && _SERVER
    // -----------------------------------------------------------------------------------
    // UCE Warehouse
    // -----------------------------------------------------------------------------------
    class uce_warehouse
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string character { get; set; }
        public int gold { get; set; }
        public int level { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // UCE Warehouse Items
    // -----------------------------------------------------------------------------------
    class uce_warehouse_items
    {
        public string character { get; set; }
        public int slot { get; set; }
        public string name { get; set; }
        public int amount { get; set; }
        public int summonedHealth { get; set; }
        public int summonedLevel { get; set; }
        public long summonedExperience { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_Warehouse
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Warehouse()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS uce_warehouse (
							`character` VARCHAR(32) NOT NULL PRIMARY KEY,
							gold INTEGER(16) NOT NULL DEFAULT 0,
							level INTEGER(16) NOT NULL DEFAULT 0
							) CHARACTER SET=utf8mb4");

		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS uce_warehouse_items (
                           `character` VARCHAR(32) NOT NULL,
                           slot INTEGER(16) NOT NULL,
                           `name` VARCHAR(32) NOT NULL,
                           amount INTEGER(16) NOT NULL,
                           summonedHealth INTEGER NOT NULL,
                           summonedLevel INTEGER NOT NULL,
                           summonedExperience INTEGER NOT NULL
                           ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<uce_warehouse>();
        connection.CreateTable<uce_warehouse_items>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Warehouse
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Warehouse(Player player)
    {
        player.warehouseActionDone = false;

#if _MYSQL && _SERVER

		var warehouseData = ExecuteReaderMySql("SELECT gold, level FROM uce_warehouse WHERE `character`=@character", new MySqlParameter("@character", player.name));

		if (warehouseData.Count == 1) {
			player.playerWarehouseGold 		= Convert.ToInt32(warehouseData[0][0]);
			player.playerWarehouseLevel 	= Convert.ToInt32(warehouseData[0][1]);
		} else {
			ExecuteNonQueryMySql("INSERT INTO uce_warehouse (`character`, gold, level) VALUES(@character, 0, 0)", new MySqlParameter("@character", player.name));
			player.playerWarehouseGold 		= 0;
			player.playerWarehouseLevel 	= 0;
		}

		for (int i = 0; i < player.playerWarehouseStorageItems; ++i)
			player.UCE_playerWarehouse.Add(new ItemSlot());

		List<List<object>> table = ExecuteReaderMySql("SELECT `name`, slot, amount, summonedHealth, summonedLevel, summonedExperience FROM uce_warehouse_items WHERE `character`=@character", new MySqlParameter("@character", player.name));
		if (table.Count > 0) {
			foreach (List<object> row in table) {
				string itemName 	= (string)row[0];
				int slot 			= Convert.ToInt32(row[1]);
				ScriptableItem template;

				if (slot < player.playerWarehouseStorageItems && ScriptableItem.dict.TryGetValue(itemName.GetStableHashCode(), out template)) {
					Item item 					= new Item(template);
					int amount 					= Convert.ToInt32(row[2]);
					item.summonedHealth 		= Convert.ToInt32(row[3]);
					item.summonedLevel 			= Convert.ToInt32(row[4]);
					item.summonedExperience 	= Convert.ToInt32(row[5]);
					player.UCE_playerWarehouse[slot] = new ItemSlot(item, amount);
				}
			}
		}

#elif _SQLITE && _SERVER

        var warehouseData = connection.FindWithQuery<uce_warehouse>("SELECT gold, level FROM uce_warehouse WHERE character=?", player.name);
        if (warehouseData != null)
        {
            player.playerWarehouseGold = warehouseData.gold;
            player.playerWarehouseLevel = warehouseData.level;
        }
        else
        {
            connection.InsertOrReplace(new uce_warehouse
            {
                character = player.name,
                gold = 0,
                level = 0
            });
            player.playerWarehouseGold = 0;
            player.playerWarehouseLevel = 0;
        }

        for (int i = 0; i < player.playerWarehouseStorageItems; ++i)
            player.UCE_playerWarehouse.Add(new ItemSlot());

        var table = connection.Query<uce_warehouse_items>("SELECT name, slot, amount, summonedHealth, summonedLevel, summonedExperience FROM uce_warehouse_items WHERE character=?", player.name);
        if (table.Count > 0)
        {
            foreach (var row in table)
            {
                string itemName = row.name;
                int slot = row.slot;
                ScriptableItem template;

                if (slot < player.playerWarehouseStorageItems && ScriptableItem.dict.TryGetValue(itemName.GetStableHashCode(), out template))
                {
                    Item item = new Item(template);
                    int amount = row.amount;
                    item.summonedHealth = row.summonedHealth;
                    item.summonedLevel = row.summonedLevel;
                    item.summonedExperience = row.summonedExperience;
                    player.UCE_playerWarehouse[slot] = new ItemSlot(item, amount);
                }
            }
        }

#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Warehouse
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Warehouse(Player player)
    {
#if _MYSQL && _SERVER

		var warehouseGoldEntryExists = ExecuteReaderMySql("SELECT 1 FROM uce_warehouse WHERE `character`=@character", new MySqlParameter("@character", player.name));

		if (warehouseGoldEntryExists.Count == 1) {
			if (player.warehouseActionDone) {
				ExecuteNonQueryMySql("UPDATE uce_warehouse SET gold=@gold, level=@level WHERE `character`=@character",
					new MySqlParameter("@gold", 		player.playerWarehouseGold),
					new MySqlParameter("@level", 		player.playerWarehouseLevel),
					new MySqlParameter("@character", 	player.name));
			}
		}

		if (player.warehouseActionDone) {
			ExecuteNonQueryMySql("DELETE FROM uce_warehouse_items WHERE `character`=@character", new MySqlParameter("@character", player.name));

			for (int i = 0; i < player.UCE_playerWarehouse.Count; ++i) {
				ItemSlot slot = player.UCE_playerWarehouse[i];

				if (slot.amount > 0) {
					ExecuteNonQueryMySql("INSERT INTO uce_warehouse_items VALUES (@character, @slot, @name, @amount, @petHealth, @petLevel, @petExperience)",
									new MySqlParameter("@character", player.name),
									new MySqlParameter("@slot", i),
									new MySqlParameter("@name", slot.item.name),
									new MySqlParameter("@amount", slot.amount),
									new MySqlParameter("@petHealth", slot.item.summonedHealth),
									new MySqlParameter("@petLevel", slot.item.summonedLevel),
									new MySqlParameter("@petExperience", slot.item.summonedExperience));
				}
			}
		}

#elif _SQLITE && _SERVER

        var warehouseGoldEntryExists = connection.FindWithQuery<uce_warehouse>("SELECT 1 FROM uce_warehouse WHERE character=?", player.name);
        if (warehouseGoldEntryExists != null)
        {
            if (player.warehouseActionDone)
            {
                connection.Execute("UPDATE uce_warehouse SET gold=?, level=? WHERE character=?", player.playerWarehouseGold, player.playerWarehouseLevel, player.name);
            }
        }

        if (player.warehouseActionDone)
        {
            connection.Execute("DELETE FROM uce_warehouse_items WHERE character=?", player.name);

            for (int i = 0; i < player.UCE_playerWarehouse.Count; ++i)
            {
                ItemSlot slot = player.UCE_playerWarehouse[i];

                if (slot.amount > 0)
                {
                    connection.Insert(new uce_warehouse_items
                    {
                        character = player.name,
                        slot = i,
                        name = slot.item.name,
                        amount = slot.amount,
                        summonedHealth = slot.item.summonedHealth,
                        summonedLevel = slot.item.summonedLevel,
                        summonedExperience = slot.item.summonedExperience
                    });
                }
            }
        }

#endif
    }

    // -----------------------------------------------------------------------------------
}
