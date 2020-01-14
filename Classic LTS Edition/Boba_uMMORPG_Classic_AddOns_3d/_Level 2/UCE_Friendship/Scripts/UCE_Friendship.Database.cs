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
    // Character Friends
    // -----------------------------------------------------------------------------------
    class character_friends
    {
        public string character { get; set; }
        public string friendName { get; set; }
        public string lastGifted { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_Friendlist
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Friendlist()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_friends (
                        `character` VARCHAR(32) NOT NULL,
                        friendName VARCHAR(32) NOT NULL,
                        lastGifted TEXT,
                        PRIMARY KEY(`character`, friendName)
                        ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_friends>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // ´CharacterLoad_UCE_Friendlist
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Friendlist(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT `character`, friendName, lastGifted FROM character_friends WHERE `character`=@character", new MySqlParameter("character", player.name));
        if (table.Count > 0) {
            for (int i = 0; i < table.Count; i++) {
                var row = table[i];
                UCE_Friend frnd = new UCE_Friend((string)row[1], (string)row[2]);
                player.UCE_Friends.Add(frnd);
            }
        }
#elif _SQLITE && _SERVER
        var table = connection.Query<character_friends>("SELECT character, friendName, lastGifted FROM character_friends WHERE character=?", player.name);
        if (table.Count > 0)
        {
            for (int i = 0; i < table.Count; i++)
            {
                var row = table[i];
                UCE_Friend frnd = new UCE_Friend(row.friendName, row.lastGifted);
                player.UCE_Friends.Add(frnd);
            }
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Friendlist
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Friendlist(Player player)
    {
#if _MYSQL && _SERVER
		var query2 = @"
            INSERT INTO character_friends
            SET
            `character`=@character,
            friendName = @friendName,
            lastGifted = @lastGifted

            ON DUPLICATE KEY UPDATE
            friendName = @friendName,
            lastGifted = @lastGifted
            ";

        for (int i = 0; i < player.UCE_Friends.Count; i++)
        {
            UCE_Friend frnd = player.UCE_Friends[i];
            ExecuteNonQueryMySql(query2,
                                 new MySqlParameter("@character", player.name),
                                 new MySqlParameter("@friendName", frnd.name),
                                 new MySqlParameter("@lastGifted", frnd.lastGifted)
            );
        }
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_friends WHERE character=?", player.name);

        for (int i = 0; i < player.UCE_Friends.Count; i++)
        {
            UCE_Friend frnd = player.UCE_Friends[i];
            connection.Insert(new character_friends
            {
                character = player.name,
                friendName = frnd.name,
                lastGifted = frnd.lastGifted
            });
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}
