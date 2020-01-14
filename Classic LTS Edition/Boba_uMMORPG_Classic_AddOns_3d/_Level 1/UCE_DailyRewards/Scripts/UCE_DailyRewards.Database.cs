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
    // Character Last Online
    // -----------------------------------------------------------------------------------
    class character_lastonline
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string character { get; set; }
        public string lastOnline { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // Character Daily Rewards
    // -----------------------------------------------------------------------------------
    class character_dailyrewards
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string character { get; set; }
        public int counter { get; set; }
        public double resetTime { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_DailyRewards
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_DailyRewards()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_dailyrewards (
					`character` VARCHAR(32) NOT NULL,
					counter INTEGER NOT NULL,
					resetTime INTEGER NOT NULL,
                        PRIMARY KEY (`character`)
			) CHARACTER SET=utf8mb4");

        ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_lastonline (
				    `character` VARCHAR(32) NOT NULL,
				    lastOnline VARCHAR(64) NOT NULL,
                        PRIMARY KEY(`character`)
              ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_lastonline>();
        connection.CreateTable<character_dailyrewards>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_DailyRewards
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_DailyRewards(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT counter, resetTime FROM character_dailyrewards WHERE `character`=@name", new MySqlParameter("@name", player.name));
		if (table.Count == 1) {
            var row = table[0];
			player.dailyRewardCounter 	= (int)row[0];
			player.dailyRewardReset		= (int)row[1];
		}
#elif _SQLITE && _SERVER
        var table = connection.Query<character_dailyrewards>("SELECT counter, resetTime FROM character_dailyrewards WHERE character=?", player.name);
        if (table.Count == 1)
        {
            var row = table[0];
            player.dailyRewardCounter = row.counter;
            player.dailyRewardReset = row.resetTime;
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_DailyRewards
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_DailyRewards(Player player)
    {
        DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan timeSinceEpoch = DateTime.UtcNow - UnixEpoch;
#if _MYSQL && _SERVER
		var query2 = @"
            INSERT INTO character_dailyrewards
            SET
            `character`=@character,
            counter = @counter,
            resetTime = @resetTime

            ON DUPLICATE KEY UPDATE
            counter = @counter,
            resetTime = @resetTime
            ";
        ExecuteNonQueryMySql(query2,
                    new MySqlParameter("@character", player.name),
                    new MySqlParameter("@counter", player.dailyRewardCounter),
                    new MySqlParameter("@resetTime", timeSinceEpoch.TotalHours));

        var query = @"
            INSERT INTO character_lastonline
            SET
            `character`=@character,
            lastOnline=@lastOnline

            ON DUPLICATE KEY UPDATE
            lastOnline=@lastOnline
            ";
        ExecuteNonQueryMySql(query,
                    new MySqlParameter("@lastOnline", DateTime.UtcNow.ToString("s")),
                    new MySqlParameter("@character", player.name));
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_lastonline WHERE character=?", player.name);
        connection.Insert(new character_lastonline
        {
            character = player.name,
            lastOnline = DateTime.UtcNow.ToString("s")
        });

        connection.Execute("DELETE FROM character_dailyrewards WHERE character=?", player.name);
        connection.Insert(new character_dailyrewards
        {
            character = player.name,
            counter = player.dailyRewardCounter,
            resetTime = timeSinceEpoch.TotalHours
        });
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_DailyRewards_HoursPassed
    // -----------------------------------------------------------------------------------
    public double UCE_DailyRewards_HoursPassed(Player player)
    {
#if _MYSQL && _SERVER
		var row = (string)ExecuteScalarMySql("SELECT lastOnline FROM character_lastonline WHERE  `character`=@name", new MySqlParameter("@name", player.name));
		if (!string.IsNullOrWhiteSpace(row)) {
			DateTime time 			= DateTime.Parse(row);
			return (DateTime.UtcNow - time).TotalSeconds/3600;
		}
#elif _SQLITE && _SERVER
        var results = connection.FindWithQuery<character_lastonline>("SELECT lastOnline FROM character_lastonline WHERE character=?", player.name);
        string row = (results != null) ? results.lastOnline : "";
        if (!string.IsNullOrWhiteSpace(row))
        {
            DateTime time = DateTime.Parse(row);
            return (DateTime.UtcNow - time).TotalSeconds / 3600;
        }
#endif
		return 0;
    }

    // -----------------------------------------------------------------------------------
}
