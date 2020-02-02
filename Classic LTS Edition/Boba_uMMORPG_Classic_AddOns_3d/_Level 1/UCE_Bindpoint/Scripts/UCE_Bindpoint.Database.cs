// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

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
    // Character Bindpoint
    // -----------------------------------------------------------------------------------
    class character_bindpoint
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string character { get; set; }
        public string name { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public string sceneName { get; set; }
    }
#endif
	
    // -----------------------------------------------------------------------------------
    // Connect_UCE_Bindpoint
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Bindpoint()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_bindpoint (
					 `character` VARCHAR(32) NOT NULL,
					 `name` VARCHAR(32) NOT NULL,
					x FLOAT NOT NULL,
            		y FLOAT NOT NULL,
            		z FLOAT NOT NULL,
            		sceneName VARCHAR(64) NOT NULL,
                    PRIMARY KEY(`character`)
                ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_bindpoint>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Bindpoint
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Bindpoint(Player player)
    {
        if (!player.UCE_myBindpoint.Valid) return;

#if _MYSQL && _SERVER
        ExecuteNonQueryMySql("DELETE FROM character_bindpoint WHERE `character`=@character", new MySqlParameter("@character", player.name));
        ExecuteNonQueryMySql("INSERT INTO character_bindpoint VALUES (@character, @name, @x, @y, @z, @sceneName)",
				new MySqlParameter("@character", 	player.name),
				new MySqlParameter("@name", 		player.UCE_myBindpoint.name),
				new MySqlParameter("@x", 			player.UCE_myBindpoint.position.x),
				new MySqlParameter("@y", 			player.UCE_myBindpoint.position.y),
				new MySqlParameter("@z", 			player.UCE_myBindpoint.position.z),
				new MySqlParameter("@sceneName", 	player.UCE_myBindpoint.SceneName)
				);
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_bindpoint WHERE character=?", player.name);
        connection.Insert(new character_bindpoint
        {
            character = player.name,
            name = player.UCE_myBindpoint.name,
            x = player.UCE_myBindpoint.position.x,
            y = player.UCE_myBindpoint.position.y,
            z = player.UCE_myBindpoint.position.z,
            sceneName = player.UCE_myBindpoint.SceneName
        });
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Bindpoint
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Bindpoint(Player player)
    {
#if _SERVER
        player.UCE_myBindpoint = new UCE_BindPoint();
		
#if _MYSQL
		var table = ExecuteReaderMySql("SELECT name, x, y, z, sceneName FROM character_bindpoint WHERE `character`=@name", new MySqlParameter("@name", player.name));
#elif _SQLITE
        var table = connection.Query<character_bindpoint>("SELECT name, x, y, z, sceneName FROM character_bindpoint WHERE character=?", player.name);
#endif

        if (table.Count == 1)
        {
            var row = table[0];

#if _MYSQL
            Vector3 p = new Vector3((float)row[1], (float)row[2], (float)row[3]);
            string sceneName = (string)row[4];
#elif _SQLITE
            Vector3 p = new Vector3(row.x, row.y, row.z);
            string sceneName = row.sceneName;
#endif
            if (p != Vector3.zero && !string.IsNullOrEmpty(sceneName))
            {
#if _MYSQL
                player.UCE_myBindpoint.name = (string)row[0];
#elif _SQLITE
                player.UCE_myBindpoint.name = row.name;
#endif
                player.UCE_myBindpoint.position = p;
                player.UCE_myBindpoint.SceneName = sceneName;
            }
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}
