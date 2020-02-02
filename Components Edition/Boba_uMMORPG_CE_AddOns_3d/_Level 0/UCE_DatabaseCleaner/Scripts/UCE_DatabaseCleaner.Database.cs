// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
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
    // Account Last Online
    // -----------------------------------------------------------------------------------
    class account_lastonline
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string account { get; set; }
        public string lastOnline { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_DatabaseCleaner
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_DatabaseCleaner()
    {
#if _MYSQL && _SERVER
 		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS account_lastonline (
 			account VARCHAR(32) NOT NULL,
 			lastOnline VARCHAR(64) NOT NULL,
            PRIMARY KEY(`account`)
 		    )");
#elif _SQLITE && _SERVER
        connection.CreateTable<account_lastonline>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_DatabaseCleanerAccountLastOnline
    // -----------------------------------------------------------------------------------
    public void UCE_DatabaseCleanerAccountLastOnline(string accountName)
    {
        if (string.IsNullOrWhiteSpace(accountName)) return;
#if _MYSQL && _SERVER
 		ExecuteNonQueryMySql("DELETE FROM account_lastonline WHERE account=@name", new MySqlParameter("@name", accountName));
        ExecuteNonQueryMySql("INSERT INTO account_lastonline VALUES (@account, @lastOnline)",
			new MySqlParameter("@lastOnline", DateTime.UtcNow.ToString("s")),
			new MySqlParameter("@account", accountName));
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM account_lastonline WHERE account=?", accountName);
        connection.Insert(new account_lastonline
        {
            account = accountName,
            lastOnline = DateTime.UtcNow.ToString("s")
        });
#endif
    }

    // -----------------------------------------------------------------------------------
}
