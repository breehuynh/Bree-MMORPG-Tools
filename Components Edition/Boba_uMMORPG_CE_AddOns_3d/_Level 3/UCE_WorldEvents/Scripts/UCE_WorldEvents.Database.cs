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
    // UCE World Events
    // -----------------------------------------------------------------------------------
    class uce_worldevents
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string name { get; set; }
        public int count { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_WorldEvents
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_WorldEvents()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS uce_worldevents (`name` VARCHAR(64) NOT NULL, `count` INTEGER NOT NULL) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<uce_worldevents>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_Load_WorldEvents
    // -----------------------------------------------------------------------------------
    public void UCE_Load_WorldEvents()
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT `name`, `count` FROM uce_worldevents");
		foreach (var row in table) {
			string name = (string)row[0];
			int count 	= (int)row[1];

			if (!string.IsNullOrWhiteSpace(name) && count != 0)
			{
				NetworkManagerMMO.UCE_SetWorldEventCount(name, count);
			}
		}
#elif _SQLITE && _SERVER
        var table = connection.Query<uce_worldevents>("SELECT `name`, `count` FROM uce_worldevents");
        foreach (var row in table)
        {
            string name = row.name;
            int count = row.count;

            if (!string.IsNullOrWhiteSpace(name) && count != 0)
            {
                NetworkManagerMMO.UCE_SetWorldEventCount(name, count);
            }
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_Save_WorldEvents
    // -----------------------------------------------------------------------------------
    public void UCE_Save_WorldEvents()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM uce_worldevents");
        foreach (UCE_WorldEvent ev in NetworkManagerMMO.UCE_WorldEvents)
        {
            ExecuteNonQueryMySql("INSERT INTO uce_worldevents VALUES (@name, @count)",
                 new MySqlParameter("@name", ev.name),
                 new MySqlParameter("@count", ev.count)
                 );
        }
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM uce_worldevents");
        foreach (UCE_WorldEvent ev in NetworkManagerMMO.UCE_WorldEvents)
            connection.Insert(new uce_worldevents
            {
                name = ev.name,
                count = ev.count
            });
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_WorldEvents
    // refresh the world event list once a character is loaded to populate it with data
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_WorldEvents(Player player)
    {
#if _SERVER
        player.UCE_WorldEvents.Clear();
        foreach (UCE_WorldEvent ev in NetworkManagerMMO.UCE_WorldEvents)
        {
            UCE_WorldEvent e = new UCE_WorldEvent();
            e.name = ev.name;
            e.count = ev.count;
            player.UCE_WorldEvents.Add(e);
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_WorldEvents
    // refresh the world event list every time a character is saved to keep it in sync
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_WorldEvents(Player player)
    {
#if _SERVER
        foreach (UCE_WorldEvent ev in NetworkManagerMMO.UCE_WorldEvents)
        {
            int id = player.UCE_WorldEvents.FindIndex(x => x.template == ev.template);

            if (id != -1)
            {
                UCE_WorldEvent e = player.UCE_WorldEvents[id];
                e.count = ev.count;
                player.UCE_WorldEvents[id] = e;
            }
        }

        // -- we save the world events as well here, but only if they changed and only once (not for every player)
        NetworkManagerMMO.UCE_SaveWorldEvents();
#endif
    }

    // -----------------------------------------------------------------------------------
}
