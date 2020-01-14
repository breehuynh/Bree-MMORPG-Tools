// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;

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
    // UCE Guild Warehouse
    // -----------------------------------------------------------------------------------
    class UCE_guild_warehouse
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string guild { get; set; }
        public int gold { get; set; }
        public int level { get; set; }
        public int locked { get; set; }
        public int busy { get; set; }
    }
    
    // -----------------------------------------------------------------------------------
    // UCE Guild Warehouse Items
    // -----------------------------------------------------------------------------------
    class UCE_guild_warehouse_items
    {
        public string guild { get; set; }
        public int slot { get; set; }
        public string name { get; set; }
        public int amount { get; set; }
        public int summonedHealth { get; set; }
        public int summonedLevel { get; set; }
        public long summonedExperience { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_GuildWareHouse
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_GuildWareHouse()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"
            CREATE TABLE IF NOT EXISTS UCE_guild_warehouse(
			        guild VARCHAR(32) NOT NULL,
					gold INTEGER(16) NOT NULL DEFAULT 0,
					level INTEGER(16) NOT NULL DEFAULT 0,
					locked INTEGER(1) NOT NULL DEFAULT 0,
					busy INTEGER(1) NOT NULL DEFAULT 0,
                    PRIMARY KEY(guild)
                  ) CHARACTER SET=utf8mb4");

        ExecuteNonQueryMySql(@"
           CREATE TABLE IF NOT EXISTS UCE_guild_warehouse_items(
                    guild VARCHAR(32) NOT NULL,
                    slot INTEGER(16) NOT NULL,
                    name TEXT(16) NOT NULL,
                    amount INTEGER(16) NOT NULL,
                    summonedHealth INTEGER NOT NULL,
                    summonedLevel INTEGER NOT NULL,
                    summonedExperience INTEGER NOT NULL
                  ) CHARACTER SET=utf8mb4");

#elif _SQLITE && _SERVER
        connection.CreateTable<UCE_guild_warehouse>();
        connection.CreateTable<UCE_guild_warehouse_items>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_GuildWarehouse
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_GuildWarehouse(Player player)
    {
#if _SERVER
        UCE_SaveGuildWarehouse(player);
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_LoadGuildWarehouse
    // -----------------------------------------------------------------------------------
    public void UCE_LoadGuildWarehouse(Player player)
    {
        player.resetGuildWarehouse();
        if (!player.InGuild()) return;

#if _MYSQL && _SERVER

		var UCE_warehouseData = ExecuteReaderMySql("SELECT gold, level, locked FROM UCE_guild_warehouse WHERE guild=@guild", new MySqlParameter("@guild", player.guild.name));

        // -- exists already? load to player and set busy

        if (UCE_warehouseData.Count == 1)
        {
            player.guildWarehouseGold 	= Convert.ToInt32(UCE_warehouseData[0][0]);
            player.guildWarehouseLevel	= Convert.ToInt32(UCE_warehouseData[0][1]);
            player.guildWarehouseLock	= (Convert.ToInt32(UCE_warehouseData[0][2]) == 1) ? true : false;
            ExecuteNonQueryMySql("UPDATE UCE_guild_warehouse SET busy=1 WHERE guild=@guild", new MySqlParameter("@guild", 	player.guild.name));
        }
        else
        {
        	// -- does not exist? create new and set busy

            ExecuteNonQueryMySql("INSERT INTO UCE_guild_warehouse (guild, gold, level, locked, busy) VALUES(@guild, 0, 0, 0, 1)", new MySqlParameter("@guild", player.guild.name));
            player.guildWarehouseGold 	= 0;
            player.guildWarehouseLevel 	= 0;
            player.guildWarehouseLock	= false;
        }

        for (int i = 0; i < player.guildWarehouseStorageItems; ++i) {
			player.UCE_guildWarehouse.Add(new ItemSlot());
		}

		var table = ExecuteReaderMySql("SELECT name, slot, amount, summonedHealth, summonedLevel, summonedExperience FROM UCE_guild_warehouse_items WHERE guild=@guild", new MySqlParameter("@guild", player.guild.name));
        if (table.Count > 0)
        {
            foreach (var row in table)
            {
                string itemName 	= (string)row[0];
                int slot 			= Convert.ToInt32(row[1]);
                ScriptableItem template;
                if (slot < player.guildWarehouseStorageItems && ScriptableItem.dict.TryGetValue(itemName.GetDeterministicHashCode(), out template))
                {
                    Item item 					= new Item(template);
                    int amount 					= Convert.ToInt32(row[2]);
                    item.summonedHealth 		= Convert.ToInt32(row[3]);
                    item.summonedLevel 			= Convert.ToInt32(row[4]);
                    item.summonedExperience 	= Convert.ToInt32(row[5]);
                    player.UCE_guildWarehouse[slot] = new ItemSlot(item, amount);
                }
            }
        }

#elif _SQLITE && _SERVER

        var UCE_warehouseData = connection.FindWithQuery<UCE_guild_warehouse>("SELECT gold, level, locked FROM UCE_guild_warehouse WHERE guild=?", player.guild.name);

        // -- exists already? load to player and set busy

        if (UCE_warehouseData != null)
        {
            player.guildWarehouseGold = UCE_warehouseData.gold;
            player.guildWarehouseLevel = UCE_warehouseData.level;
            player.guildWarehouseLock = (UCE_warehouseData.locked == 1) ? true : false;
            connection.Execute("UPDATE UCE_guild_warehouse SET busy=1 WHERE guild=?", player.guild.name);
        }
        else
        {
            // -- does not exist? create new and set busy

            connection.InsertOrReplace(new UCE_guild_warehouse
            {
                guild = player.guild.name,
                gold = 0,
                level = 0,
                locked = 0,
                busy = 1
            });
            player.guildWarehouseGold = 0;
            player.guildWarehouseLevel = 0;
            player.guildWarehouseLock = false;
        }

        for (int i = 0; i < player.guildWarehouseStorageItems; ++i)
        {
            player.UCE_guildWarehouse.Add(new ItemSlot());
        }

        var table = connection.Query<UCE_guild_warehouse_items>("SELECT name, slot, amount, summonedHealth, summonedLevel, summonedExperience FROM UCE_guild_warehouse_items WHERE guild=?", player.guild.name);

        if (table.Count > 0)
        {
            foreach (var row in table)
            {
                string itemName = row.name;
                int slot = row.slot;
                ScriptableItem template;

                if (slot < player.guildWarehouseStorageItems && ScriptableItem.dict.TryGetValue(itemName.GetDeterministicHashCode(), out template))
                {
                    Item item = new Item(template);
                    int amount = row.amount;
                    item.summonedHealth = row.summonedHealth;
                    item.summonedLevel = row.summonedLevel;
                    item.summonedExperience = row.summonedExperience;
                    player.UCE_guildWarehouse[slot] = new ItemSlot(item, amount);
                }
            }
        }

#endif

        player.guildWarehouseActionDone = false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SaveGuildWarehouse
    // -----------------------------------------------------------------------------------
    public void UCE_SaveGuildWarehouse(Player player)
    {
        if (!player.InGuild()) player.resetGuildWarehouse();
        if (!player.InGuild() || !player.guildWarehouseActionDone) return;

#if _MYSQL && _SERVER

		long EntryExistsOrBusy = Convert.ToInt32(ExecuteScalarMySql("SELECT busy FROM UCE_guild_warehouse WHERE guild=@guild", new MySqlParameter("@guild", player.guild.name)));

		// -- check if exists, only delete entries when it does and is not busy

		if (EntryExistsOrBusy != 1)
            ExecuteNonQueryMySql("DELETE FROM UCE_guild_warehouse_items WHERE guild=@guild", new MySqlParameter("@guild", player.guild.name));

		for (int i = 0; i < player.UCE_guildWarehouse.Count; ++i) {
			ItemSlot slot = player.UCE_guildWarehouse[i];

			if (slot.amount > 0) {
				ExecuteNonQueryMySql("INSERT INTO UCE_guild_warehouse_items VALUES (@guild, @slot, @name, @amount, @summonedHealth, @summonedLevel, @summonedExperience)",
								new MySqlParameter("@guild", 				player.guild.name),
								new MySqlParameter("@slot", 				i),
								new MySqlParameter("@name", 				slot.item.name),
								new MySqlParameter("@amount", 				slot.amount),
								new MySqlParameter("@summonedHealth", 		slot.item.summonedHealth),
								new MySqlParameter("@summonedLevel", 		slot.item.summonedLevel),
								new MySqlParameter("@summonedExperience", 	slot.item.summonedExperience));
			}
		}

		ExecuteNonQueryMySql("UPDATE UCE_guild_warehouse SET gold=@gold, level=@level, locked=@locked, busy=0 WHERE guild=@guild",
			new MySqlParameter("@gold", 	player.guildWarehouseGold),
			new MySqlParameter("@level", 	player.guildWarehouseLevel),
			new MySqlParameter("@locked", 	(player.guildWarehouseLock) ? 1 : 0),
			new MySqlParameter("@guild", 	player.guild.name));

#elif _SQLITE && _SERVER

        var results = connection.FindWithQuery<UCE_guild_warehouse>("SELECT busy FROM UCE_guild_warehouse WHERE guild=?", player.guild.name);
        long EntryExistsOrBusy = results.busy;

        // -- check if exists, only delete entries when it does and is not busy

        if (EntryExistsOrBusy != 1)
            connection.Execute("DELETE FROM UCE_guild_warehouse_items WHERE guild=?", player.guild.name);

        for (int i = 0; i < player.UCE_guildWarehouse.Count; ++i)
        {
            ItemSlot slot = player.UCE_guildWarehouse[i];

            if (slot.amount > 0)
            {
                connection.Insert(new UCE_guild_warehouse_items
                {
                    guild = player.guild.name,
                    slot = i,
                    name = slot.item.name,
                    amount = slot.amount,
                    summonedHealth = slot.item.summonedHealth,
                    summonedLevel = slot.item.summonedLevel,
                    summonedExperience = slot.item.summonedExperience
                });
            }
        }

        connection.Execute("UPDATE UCE_guild_warehouse SET gold=?, level=?, locked=?, busy=0 WHERE guild=?", player.guildWarehouseGold, player.guildWarehouseLevel, player.guildWarehouseLock ? 1 : 0, player.guild.name);
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_SetGuildWarehouseBusy
    // -----------------------------------------------------------------------------------
    public void UCE_SetGuildWarehouseBusy(Player player, int isbusy = 1)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("UPDATE UCE_guild_warehouse SET busy=@busy WHERE guild=@guild", new MySqlParameter("@guild", player.guild.name), new MySqlParameter("@busy", 	isbusy));
#elif _SQLITE && _SERVER
        connection.Execute("UPDATE UCE_guild_warehouse SET busy=? WHERE guild=?", isbusy, player.guild.name);
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetGuildWarehouseAccess
    // -----------------------------------------------------------------------------------
    public bool UCE_GetGuildWarehouseAccess(Player player)
    {
#if _MYSQL && _SERVER
		return Convert.ToInt32(ExecuteScalarMySql("SELECT busy FROM UCE_guild_warehouse WHERE guild=@guild", new MySqlParameter("@guild", player.guild.name))) != 1;
#elif _SQLITE && _SERVER
        return Convert.ToInt32(connection.FindWithQuery<UCE_guild_warehouse>("SELECT busy FROM UCE_guild_warehouse WHERE guild=?", player.guild.name)) != 1;
#else
		return false;
#endif
    }

    // -----------------------------------------------------------------------------------
}
