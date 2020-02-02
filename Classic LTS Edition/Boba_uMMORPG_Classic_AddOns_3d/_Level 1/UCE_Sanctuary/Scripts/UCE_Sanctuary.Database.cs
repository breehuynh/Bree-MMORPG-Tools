// =======================================================================================
// Maintained by bobatea#9400 on Discord
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
    // -----------------------------------------------------------------------------------
    // Connect_UCE_Sanctuary
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Sanctuary()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_lastonline (
				`character` VARCHAR(32) NOT NULL,
				lastOnline VARCHAR(64) NOT NULL,
                    PRIMARY KEY(`character`)
                ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_lastonline>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Sanctuary
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Sanctuary(Player player)
    {
#if _MYSQL && _SERVER
		var row = (string)ExecuteScalarMySql("SELECT lastOnline FROM character_lastonline WHERE `character`=@name", new MySqlParameter("@name", player.name));
		if (!string.IsNullOrWhiteSpace(row)) {
			DateTime time 				= DateTime.Parse(row);
			player.UCE_SecondsPassed 	= (DateTime.UtcNow - time).TotalSeconds;
		} else {
			player.UCE_SecondsPassed 	= 0;
		}
#elif _SQLITE && _SERVER
        var results = connection.FindWithQuery<character_lastonline>("SELECT lastOnline FROM character_lastonline WHERE character=?", player.name);
        string row = (results != null) ? results.lastOnline : "";
        if (!string.IsNullOrWhiteSpace(row))
        {
            DateTime time = DateTime.Parse(row);
            player.UCE_SecondsPassed = (DateTime.UtcNow - time).TotalSeconds;
        }
        else
        {
            player.UCE_SecondsPassed = 0;
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Sanctuary
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Sanctuary(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM character_lastonline WHERE `character`=@character", new MySqlParameter("@character", player.name));
        ExecuteNonQueryMySql("INSERT INTO character_lastonline VALUES (@character, @lastOnline)",
				new MySqlParameter("@lastOnline", DateTime.UtcNow.ToString("s")),
				new MySqlParameter("@character", player.name));
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_lastonline WHERE character=?", player.name);
        connection.Insert(new character_lastonline
        {
            character = player.name,
            lastOnline = DateTime.UtcNow.ToString("s")
        });
#endif
    }

    // -----------------------------------------------------------------------------------
}
