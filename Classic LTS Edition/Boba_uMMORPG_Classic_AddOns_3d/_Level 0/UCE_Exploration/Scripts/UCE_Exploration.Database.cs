// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using System;
using System.Collections;

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
    // Character Exploration
    // -----------------------------------------------------------------------------------
    class character_exploration
    {
        public string character { get; set; }
        public string exploredArea { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_Exploration
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Exploration()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_exploration (`character` VARCHAR(32) NOT NULL, exploredArea VARCHAR(32) NOT NULL) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_exploration>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Exploration
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Exploration(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT exploredArea FROM character_exploration WHERE `character`=@character",
						new MySqlParameter("@character", player.name)
						);
		foreach (var row in table) {
			player.UCE_exploredAreas.Add((string)row[0]);
		}
#elif _SQLITE && _SERVER
        var table = connection.Query<character_exploration>("SELECT exploredArea FROM character_exploration WHERE character=?", player.name);
        foreach (var row in table)
        {
            player.UCE_exploredAreas.Add(row.exploredArea);
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Exploration
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Exploration(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM character_exploration WHERE `character`=@character", new MySqlParameter("@character", player.name));
        for (int i = 0; i < player.UCE_exploredAreas.Count; ++i)
        {
            ExecuteNonQueryMySql("INSERT INTO character_exploration VALUES (@character, @exploredArea)",
                 new MySqlParameter("@character", player.name),
                 new MySqlParameter("@exploredArea", player.UCE_exploredAreas[i])
                 );
        }
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_exploration WHERE character=?", player.name);
        for (int i = 0; i < player.UCE_exploredAreas.Count; i++)
        {
            connection.Insert(new character_exploration
            {
                character = player.name,
                exploredArea = player.UCE_exploredAreas[i]
            });
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}
