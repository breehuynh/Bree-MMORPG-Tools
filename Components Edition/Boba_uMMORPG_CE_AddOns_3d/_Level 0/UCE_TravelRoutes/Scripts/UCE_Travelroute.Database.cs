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
#elif _SQLITE && _SERVER
using SQLite;
#endif

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{

#if _SQLITE && _SERVER
	// -----------------------------------------------------------------------------------
    // Character Travel Routes
    // -----------------------------------------------------------------------------------
    class character_travelroutes
    {
        public string character { get; set; }
        public string travelroute { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_Travelroutes
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Travelroutes()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_travelroutes (
				`character` VARCHAR(32) NOT NULL,
				travelroute VARCHAR(32) NOT NULL
				) CHARACTER SET=utf8mb4 ");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_travelroutes>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Travelroutes
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Travelroutes(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT travelroute FROM character_travelroutes WHERE `character`=@name", new MySqlParameter("@name", player.name));
		foreach (var row in table) {
			UCE_TravelrouteClass tRoute = new UCE_TravelrouteClass((string)row[0]);
			player.UCE_travelroutes.Add(tRoute);
		}
#elif _SQLITE && _SERVER
        var table = connection.Query<character_travelroutes>("SELECT travelroute FROM character_travelroutes WHERE character=?", player.name);
        foreach (var row in table)
        {
            UCE_TravelrouteClass tRoute = new UCE_TravelrouteClass(row.travelroute);
            player.UCE_travelroutes.Add(tRoute);
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Travelroutes
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Travelroutes(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM character_travelroutes WHERE `character`=@character", new MySqlParameter("@character", player.name));
		for (int i = 0; i < player.UCE_travelroutes.Count; ++i) {
            ExecuteNonQueryMySql("INSERT INTO character_travelroutes VALUES (@character, @travelroute)",
 				new MySqlParameter("@character", player.name),
 				new MySqlParameter("@travelroute", player.UCE_travelroutes[i].name));
 		}
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_travelroutes WHERE character=?", player.name);
        for (int i = 0; i < player.UCE_travelroutes.Count; ++i)
        {
            connection.Insert(new character_travelroutes
            {
                character = player.name,
                travelroute = player.UCE_travelroutes[i].name
            });
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}
