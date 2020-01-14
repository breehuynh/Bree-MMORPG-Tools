// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;

#if _MYSQL && _SERVER
using MySql.Data;
using MySql.Data.MySqlClient;
using SqlParameter = MySql.Data.MySqlClient.MySqlParameter;
#elif _SQLITE && _SERVER
using SQLite;
#endif

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{

#if _SQLITE && _SERVER
	// -----------------------------------------------------------------------------------
    // Character Scene
    // -----------------------------------------------------------------------------------
    class character_scene
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string character { get; set; }
        public string scene { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // Zones Online
    // -----------------------------------------------------------------------------------
    class zones_online
    {
        public string online { get; set; }
    }
#endif
    
    // -----------------------------------------------------------------------------------
    // Connect_UCE_NetworkZone
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_NetworkZone()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"
        CREATE TABLE IF NOT EXISTS character_scene (
            `character` VARCHAR(32) NOT NULL,
            scene VARCHAR(64) NOT NULL,
            PRIMARY KEY(`character`)
            ) CHARACTER SET=utf8mb4");

        ExecuteNonQueryMySql(@"
        CREATE TABLE IF NOT EXISTS zones_online (
            id INT NOT NULL AUTO_INCREMENT,
            PRIMARY KEY(id),
            online TIMESTAMP NOT NULL
        ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_scene>();
        connection.CreateTable<zones_online>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // IsCharacterOnlineAnywhere
    // a character is online on any of the servers if the online string is not
    // empty and if the time difference is less than the save interval * 2
    // (* 2 to have some tolerance)
    // -----------------------------------------------------------------------------------
    public bool IsCharacterOnlineAnywhere(string characterName)
    {
        float saveInterval = ((NetworkManagerMMO)NetworkManager.singleton).saveInterval;

#if _MYSQL && _SERVER
		var obj = ExecuteScalarMySql("SELECT online FROM characters WHERE name=@name", new SqlParameter("@name", characterName));
		if (obj != null)
        {
            var time = (DateTime)obj;
            double elapsedSeconds = (DateTime.UtcNow - time).TotalSeconds;
            return elapsedSeconds < saveInterval * 2;
        }
#elif _SQLITE && _SERVER
        object obj = connection.FindWithQuery<characters>("SELECT online FROM characters WHERE name=?", characterName);
        if (obj != null)
        {
            string online = (string)obj;
            if (online != "")
            {
                DateTime time = DateTime.Parse(online);
                double elapsedSeconds = (DateTime.UtcNow - time).TotalSeconds;

                return elapsedSeconds < saveInterval * 2;
            }
        }
#endif
        return false;
    }

    // -----------------------------------------------------------------------------------
    // AnyAccountCharacterOnline
    // -----------------------------------------------------------------------------------
    public bool AnyAccountCharacterOnline(string account)
    {
#if _SERVER
        List<string> characters = CharactersForAccount(account);
        return characters.Any(IsCharacterOnlineAnywhere);
#endif
		return false;
    }

    // -----------------------------------------------------------------------------------
    // GetCharacterScene
    // -----------------------------------------------------------------------------------
    public string GetCharacterScene(string characterName)
    {
#if _MYSQL && _SERVER
        object obj = ExecuteScalarMySql("SELECT scene FROM character_scene WHERE `character`=@character", new SqlParameter("@character", characterName));
        return obj != null ? (string)obj : "";
#elif _SQLITE && _SERVER
        character_scene characterScene = connection.FindWithQuery<character_scene>("SELECT scene FROM character_scene WHERE character=?", characterName);
        if (characterScene != null)
            return characterScene.scene;
#endif
        return "";
    }

    // -----------------------------------------------------------------------------------
    // SaveCharacterScene
    // -----------------------------------------------------------------------------------
    public void SaveCharacterScene(string characterName, string sceneName)
    {
#if _MYSQL && _SERVER
		var query = @"
            INSERT INTO character_scene
            SET
                `character`=@character,
                scene=@scene
            ON DUPLICATE KEY UPDATE
                scene=@scene";

        ExecuteNonQueryMySql(query,
                             new SqlParameter("@character", characterName),
                             new SqlParameter("@scene", sceneName));
#elif _SQLITE && _SERVER
        connection.InsertOrReplace(new character_scene
        {
            character = characterName,
            scene = sceneName
        });
#endif
    }

    // -----------------------------------------------------------------------------------
    // TimeElapsedSinceMainZoneOnline
    // a zone is online if the online string is not empty and if the time
    // difference is less than the write interval * multiplier
    // (* multiplier to have some tolerance)
    // -----------------------------------------------------------------------------------
    public double TimeElapsedSinceMainZoneOnline()
    {
#if _MYSQL && _SERVER
		var obj = ExecuteScalarMySql("SELECT online FROM zones_online");
        if (obj != null)
        {
            var time = (DateTime)obj;
            return (DateTime.Now - time).TotalSeconds;
        }
#elif _SQLITE && _SERVER
        object obj = connection.FindWithQuery<zones_online>("SELECT online FROM zones_online");
        if (obj != null)
        {
            string online = (string)obj;
            if (online != "")
            {
                DateTime time = DateTime.Parse(online);
                return (DateTime.UtcNow - time).TotalSeconds;
            }
        }
#endif
        return Mathf.Infinity;
    }

    // -----------------------------------------------------------------------------------
    // SaveMainZoneOnlineTime
    // Note: should only be called by main zone
    // online status:
    //   '' if offline (if just logging out etc.)
    //   current time otherwise
    // -> it uses the ISO 8601 standard format
    // -----------------------------------------------------------------------------------
    public void SaveMainZoneOnlineTime()
    {
#if _MYSQL && _SERVER
		var query = @"
            INSERT INTO zones_online
            SET
                id=@id,
                online=@online
            ON DUPLICATE KEY UPDATE
                online=@online";

        ExecuteNonQueryMySql(query,
                             new SqlParameter("@id", 1),
                             new SqlParameter("@online", DateTime.Now));
#elif _SQLITE && _SERVER
        string onlineString = DateTime.UtcNow.ToString("s");
        connection.Execute("DELETE FROM zones_online");
        connection.InsertOrReplace(new zones_online
        {
            online = onlineString
        });
#endif
    }

    // -----------------------------------------------------------------------------------
}
