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
    // Character Pvp Zones
    // -----------------------------------------------------------------------------------
    class character_pvpzones
    {
        public string character { get; set; }
        public string realm { get; set; }
        public string alliedrealm { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_PVPZone
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_PVPZone()
    {
#if _MYSQL && _SERVER
		ExecuteReaderMySql(@"CREATE TABLE IF NOT EXISTS character_pvpzones (
			`character` VARCHAR(32) NOT NULL,
			realm VARCHAR(32) NOT NULL,
			alliedrealm VARCHAR(32) NOT NULL
		) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_pvpzones>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_PVPZone
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_PVPZone(Player player)
    {
#if _SERVER
#if _MYSQL
		var table = ExecuteReaderMySql("SELECT realm, alliedrealm FROM character_pvpzones WHERE `character`=@name", new MySqlParameter("@name", player.name));
#elif _SQLITE
        var table = connection.Query<character_pvpzones>("SELECT realm, alliedrealm FROM character_pvpzones WHERE character=?", player.name);
#endif
        if (table.Count == 1)
        {
            var row = table[0];
#if _MYSQL
            string realm = (string)row[0];
            string ally = (string)row[1];
            player.UCE_setRealm(realm.GetStableHashCode(), ally.GetStableHashCode());
#elif _SQLITE
            string realm = row.realm;
            string ally = row.alliedrealm;
            player.UCE_setRealm(realm.GetStableHashCode(), ally.GetStableHashCode());
#endif 
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_PVPZone
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_PVPZone(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM character_pvpzones WHERE `character`=@character", new MySqlParameter("@character", player.name));
        ExecuteNonQueryMySql("INSERT INTO character_pvpzones VALUES (@character, @realm, @alliedrealm)",
				new MySqlParameter("@character", 	player.name),
				new MySqlParameter("@realm", 		(player.Realm != null) ? player.Realm.name : ""),
				new MySqlParameter("@alliedrealm", 	(player.Ally != null) ? player.Ally.name : ""));
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_pvpzones WHERE character=?", player.name);
        connection.Insert(new character_pvpzones
        {
            character = player.name,
            realm = (player.Realm != null) ? player.Realm.name : "",
            alliedrealm = (player.Ally != null) ? player.Ally.name : ""
        });
#endif
    }

    // -----------------------------------------------------------------------------------
}
