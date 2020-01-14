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

public partial class Database
{

#if _SQLITE && _SERVER
	// -----------------------------------------------------------------------------------
    // Character Prestige Classes
    // -----------------------------------------------------------------------------------
    class character_prestigeclasses
    {
        public string character { get; set; }
        public string class1 { get; set; }
        public string class2 { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_PrestigeClasses
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_PrestigeClasses()
    {
#if _MYSQL && _SERVER
		ExecuteReaderMySql(@"CREATE TABLE IF NOT EXISTS character_prestigeclasses (
			`character` VARCHAR(32) NOT NULL,
			class1 VARCHAR(32) NOT NULL,
			class2 VARCHAR(32) NOT NULL
		) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_prestigeclasses>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_PrestigeClasses
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_PrestigeClasses(Player player)
    {
#if _SERVER
#if _MYSQL
		var table = ExecuteReaderMySql("SELECT class1, class2 FROM character_prestigeclasses WHERE `character`=@name", new MySqlParameter("@name", player.name));
#elif _SQLITE
        var table = connection.Query<character_prestigeclasses>("SELECT class1, class2 FROM character_prestigeclasses WHERE character=?", player.name);
#endif
        if (table.Count == 1)
        {
            var row = table[0];
#if _MYSQL
            string class1 = (string)row[0];
            string class2 = (string)row[1];
#elif _SQLITE
            string class1 = row.class1;
            string class2 = row.class2;
#endif
            UCE_PrestigeClassTemplate prestigeClass1 = null;
            if (UCE_PrestigeClassTemplate.dict.TryGetValue(class1.GetDeterministicHashCode(), out prestigeClass1))
                player.UCE_prestigeClass = prestigeClass1;
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_PrestigeClasses
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_PrestigeClasses(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM character_prestigeclasses WHERE `character`=@character", new MySqlParameter("@character", player.name));
        ExecuteNonQueryMySql("INSERT INTO character_prestigeclasses VALUES (@character, @class1, @class2)",
				new MySqlParameter("@character", 	player.name),
				new MySqlParameter("@class1", 		(player.UCE_prestigeClass != null) ? player.UCE_prestigeClass.name : ""),
				new MySqlParameter("@class2", 		""));
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_prestigeclasses WHERE character=?", player.name);
        connection.Insert(new character_prestigeclasses
        {
            character = player.name,
            class1 = (player.UCE_prestigeClass != null) ? player.UCE_prestigeClass.name : "",
            class2 = ""
        });
#endif
    }

    // -----------------------------------------------------------------------------------
}
