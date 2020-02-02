// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
#if _MYSQL && _SERVER
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
#elif _SQLITE && _SERVER
using SQLite;
#endif

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{

#if _SQLITE && _SERVER
	// -----------------------------------------------------------------------------------
    // UCE Guild Upgrades
    // -----------------------------------------------------------------------------------
    class UCE_guild_upgrades
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string guild { get; set; }
        public int level { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_GuildUpgrades
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_GuildUpgrades()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"
            CREATE TABLE IF NOT EXISTS UCE_guild_upgrades(
			        guild VARCHAR(32) NOT NULL,
					level INTEGER(16) NOT NULL DEFAULT 0,
                    PRIMARY KEY(guild)
                  ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<UCE_guild_upgrades>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_GuildUpgrades
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_GuildUpgrades(Player player)
    {
#if _SERVER
        UCE_SaveGuildUpgrades(player);
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_LoadGuildUpgrades
    // -----------------------------------------------------------------------------------
    public void UCE_LoadGuildUpgrades(Player player)
    {
        if (!player.InGuild()) return;

#if _MYSQL && _SERVER

		var guildLevel = ExecuteScalarMySql("SELECT level FROM UCE_guild_upgrades WHERE guild=@guild", new MySqlParameter("@guild", player.guild.name));

        // -- exists already? load to player

        if (guildLevel != null)
        {
            player.guildLevel = Convert.ToInt32((long)guildLevel);
            ExecuteNonQueryMySql("UPDATE UCE_guild_upgrades SET busy=1 WHERE guild=@guild", new MySqlParameter("@guild", 	player.guild.name));
        }
        else
        {
        	// -- does not exist? create new

            ExecuteNonQueryMySql("INSERT INTO UCE_guild_upgrades (guild, level) VALUES(@guild, 0)", new MySqlParameter("@guild", player.guild.name));

            player.guildLevel 	= 0;
        }

#elif _SQLITE && _SERVER

        var results = connection.FindWithQuery<UCE_guild_upgrades>("SELECT level FROM UCE_guild_upgrades WHERE guild=?", player.guild.name);
        int guildLevel = results.level;

        // -- exists already? load to player

        if (results != null)
        {
            player.guildLevel = guildLevel;
            connection.Execute("UPDATE UCE_guild_upgrades SET busy=1 WHERE guild=?", player.guild.name);
        }
        else
        {
            // -- does not exist? create new
            connection.Insert(new UCE_guild_upgrades
            {
                guild = player.guild.name,
                level = 0
            });
            player.guildLevel = 0;
        }

#endif

        player.guildWarehouseActionDone = false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SaveGuildUpgrades
    // -----------------------------------------------------------------------------------
    public void UCE_SaveGuildUpgrades(Player player)
    {
        if (!player.InGuild()) return;

#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM UCE_guild_upgrades WHERE guild=@guild", new MySqlParameter("@guild", player.guild.name));
		ExecuteNonQueryMySql("INSERT INTO UCE_guild_upgrades (guild, level) VALUES(@guild, @level)",
			new MySqlParameter("@level", 	player.guildLevel),
			new MySqlParameter("@guild", 	player.guild.name));

#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM UCE_guild_upgrades WHERE guild=?", player.guild.name);
        connection.Insert(new UCE_guild_upgrades
        {
            guild = player.guild.name,
            level = player.guildLevel
        });
#endif
    }

    // -----------------------------------------------------------------------------------
}
