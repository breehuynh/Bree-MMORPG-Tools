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
    // Character Timegates
    // -----------------------------------------------------------------------------------
    class character_timegates
    {
        public string character { get; set; }
        public string timegateName { get; set; }
        public int timegateCount { get; set; }
        public string timegateHours { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_SimpleTimegate
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_SimpleTimegate()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_timegates (
			`character` VARCHAR(32) NOT NULL,
			timegateName TEXT NOT NULL,
			timegateCount INTEGER NOT NULL,
			timegateHours TEXT NOT NULL
              ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_timegates>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_SimpleTimegate
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_SimpleTimegate(Player player)
    {
        player.UCE_timegates.Clear();

#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT timegateName, timegateCount, timegateHours FROM character_timegates WHERE `character`=@name", new MySqlParameter("@name", player.name));
		foreach (var row in table) {
			UCE_Timegate timegate = new UCE_Timegate();
			timegate.name = (string)row[0];
			timegate.count = Convert.ToInt32((int)row[1]);
			timegate.hours = (string)row[2];
			timegate.valid = true;
			player.UCE_timegates.Add(timegate);
		}
#elif _SQLITE && _SERVER
        var table = connection.Query<character_timegates>("SELECT timegateName, timegateCount, timegateHours FROM character_timegates WHERE character=?", player.name);
        foreach (var row in table)
        {
            UCE_Timegate timegate = new UCE_Timegate();
            timegate.name = row.timegateName;
            timegate.count = row.timegateCount;
            timegate.hours = row.timegateHours;
            timegate.valid = true;
            player.UCE_timegates.Add(timegate);
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_SimpleTimegate
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_SimpleTimegate(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM character_timegates WHERE `character`=@character", new MySqlParameter("@character", player.name));
		for (int i = 0; i < player.UCE_timegates.Count; ++i) {
            ExecuteNonQueryMySql("INSERT INTO character_timegates VALUES (@character, @timegateName, @timegateCount, @timegateHours)",
 				new MySqlParameter("@character", player.name),
 				new MySqlParameter("@timegateName", player.UCE_timegates[i].name),
 				new MySqlParameter("@timegateCount", player.UCE_timegates[i].count),
 				new MySqlParameter("@timegateHours", player.UCE_timegates[i].hours));
 		}
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_timegates WHERE character=?", player.name);
        for (int i = 0; i < player.UCE_timegates.Count; ++i)
        {
            connection.Insert(new character_timegates
            {
                character = player.name,
                timegateName = player.UCE_timegates[i].name,
                timegateCount = player.UCE_timegates[i].count,
                timegateHours = player.UCE_timegates[i].hours
            });
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}
