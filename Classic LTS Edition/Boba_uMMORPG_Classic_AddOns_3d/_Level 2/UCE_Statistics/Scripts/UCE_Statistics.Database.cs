// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
#if _MYSQL && _SERVER
using MySql.Data;
using MySql.Data.MySqlClient;
#elif _SQLITE && _SERVER
using SQLite;
#endif

// DATABASE (SQLite / mySQL Hybrid)

#if _iMMOSTATISTICS

public partial class Database
{

#if _SQLITE && _SERVER
	// -----------------------------------------------------------------------------------
    // Character Statistics
    // -----------------------------------------------------------------------------------
    class character_statistics
    {
        public string character { get; set; }
        public string statistic { get; set; }
        public long amount { get; set; }
        public long total { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_Statistics
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Statistics()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_statistics (
			`character` VARCHAR(32) NOT NULL,
			statistic VARCHAR(32) NOT NULL,
			amount INTEGER(16) NOT NULL,
			total INTEGER(16) NOT NULL
		    )CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        UCE_connection.CreateTable<character_statistics>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Statistics
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Statistics(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT statistic, amount, total FROM character_statistics WHERE `character`=@name", new MySqlParameter("@name", player.name));
        foreach (var row in table)
            player.GetComponent<UCE_PlayerStatistics>().AddStatistic((string)row[0], (int)row[1], (int)row[2]);
#elif _SQLITE && _SERVER
        var table = UCE_connection.Query<character_statistics>("SELECT statistic, amount, total FROM character_statistics WHERE character=?", player.name);
        foreach (var row in table)
            player.GetComponent<UCE_PlayerStatistics>().AddStatistic(row.statistic, row.amount, row.total);
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Statistics
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Statistics(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM character_statistics WHERE `character`=@character", new MySqlParameter("@character", player.name));
        foreach (UCE_Statistic statistic in player.GetComponent<UCE_PlayerStatistics>().UCE_statistics)
        {
            ExecuteNonQueryMySql("INSERT INTO character_statistics VALUES (@character, @statistic, @amount, @total)",
                 new MySqlParameter("@character",   player.name),
                 new MySqlParameter("@currency",    statistic.name),
                 new MySqlParameter("@amount",      statistic.amount),
                 new MySqlParameter("@total",       statistic.total)
                 );
        }
#elif _SQLITE && _SERVER
        UCE_connection.Execute("DELETE FROM character_statistics WHERE character=?", player.name);
        foreach (UCE_Statistic statistic in player.GetComponent<UCE_PlayerStatistics>().UCE_statistics)
            UCE_connection.InsertOrReplace(new character_statistics
            {
                character   = player.name,
                statistic   = statistic.name,
                amount      = statistic.amount,
                total       = statistic.total
            });
#endif
    }

    // -----------------------------------------------------------------------------------
}

#endif