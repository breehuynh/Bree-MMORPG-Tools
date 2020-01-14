// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System;
using System.Collections;

#if _MYSQL
using MySql.Data;								// From MySql.Data.dll in Plugins folder
using MySql.Data.MySqlClient;                   // From MySql.Data.dll in Plugins folder
#elif _SQLITE

using SQLite; 						// copied from Unity/Mono/lib/mono/2.0 to Plugins

#endif

// NETWORK MANAGER MMO

public partial class NetworkManagerMMO
{
    public UCE_Tmpl_DatabaseCleaner DatabaseCleaner;

#if _SQLITE

    private class accounts
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string name { get; set; }

        public string password { get; set; }

        // created & lastlogin for statistics like CCU/MAU/registrations/...
        public DateTime created { get; set; }

        public DateTime lastlogin { get; set; }
        public bool banned { get; set; }
    }

    private class account_lastonline
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string account { get; set; }

        public string lastOnline { get; set; }
    }

    private class sqlite_master
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string table { get; set; }

        public long count { get; set; }
    }

#endif

    // -----------------------------------------------------------------------------------
    // OnStartServer_UCE_DatabaseCleaner
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartServer")]
    public void OnStartServer_UCE_DatabaseCleaner()
    {
#if _SERVER
        if (DatabaseCleaner && DatabaseCleaner.isActive)
        {
            var i = 0;

            // ---------- Prune outdated accounts
            if (DatabaseCleaner.PruneInactiveAfterDays > 0 || DatabaseCleaner.PruneBannedAfterDays > 0)
            {
#if _MYSQL
				var table = Database.singleton.ExecuteReaderMySql("SELECT account, lastOnline FROM account_lastonline");
#elif _SQLITE
                var table = Database.singleton.UCE_connection.Query<account_lastonline>("SELECT account, lastOnline FROM account_lastonline");
#endif

                foreach (var row in table)
                {
#if _MYSQL
						var accountName = (string)row[0];
                        var lastOnline = (string)row[1];
#elif _SQLITE
                    var accountName = row.account;
                    var lastOnline = row.lastOnline;
#endif

                    if (!string.IsNullOrWhiteSpace(accountName))
                    {
                        DateTime time = DateTime.Parse(lastOnline);
                        var HoursPassed = (DateTime.UtcNow - time).TotalDays;

                        // ---------- Prune outdated accounts
                        if (DatabaseCleaner.PruneInactiveAfterDays > 0 && HoursPassed > DatabaseCleaner.PruneInactiveAfterDays)
                        {
                            UCE_DatabaseCleanup(accountName);
                            i++;
                        }

                        // ---------- Prune banned accounts
                        if (DatabaseCleaner.PruneBannedAfterDays > 0 && HoursPassed > DatabaseCleaner.PruneBannedAfterDays)
                        {
#if _MYSQL
								bool banned = (bool)Database.singleton.ExecuteScalarMySql("SELECT banned FROM accounts WHERE name=@name", new MySqlParameter("@name", accountName));
								if (banned) {
									UCE_DatabaseCleanup(accountName);
									i++;
								}
#elif _SQLITE
                            var checkAccount = Database.singleton.UCE_connection.FindWithQuery<accounts>("SELECT banned FROM accounts WHERE name=?", accountName);
                            if (checkAccount.banned)
                            {
                                UCE_DatabaseCleanup(accountName);
                                i++;
                            }
#endif
                        }
                    }
                }
            }

            // ---------- Prune empty accounts (no characters)
            if (DatabaseCleaner.PruneEmptyAccounts)
            {
#if _MYSQL
				var table2 = Database.singleton.ExecuteReaderMySql("SELECT name FROM accounts");
#elif _SQLITE
                var table2 = Database.singleton.UCE_connection.Query<accounts>("SELECT name FROM accounts");
#endif

                foreach (var row in table2)
                {
#if _MYSQL
						var accountChars = Database.singleton.CharactersForAccount((string)row[0]);
						if (accountChars.Count < 1) {
							UCE_DatabaseCleanup((string)row[0]);
							i++;
#elif _SQLITE
                    var accountChars = Database.singleton.CharactersForAccount(row.name);
                    if (accountChars.Count < 1)
                    {
                        UCE_DatabaseCleanup(row.name);
                        i++;
#endif
                    }
                }
            }

            Debug.Log("DatabaseCleaner checking accounts ...pruned [" + i + "] account(s)");
        }
        else
        {
            Debug.LogWarning("DatabaseCleaner: Either inactive or ScriptableObject not found!");
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // OnServerDisconnect_UCE_DatabaseCleaner
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerDisconnect")]
    private void OnServerDisconnect_UCE_DatabaseCleaner(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var accountName = conn.identity.gameObject.GetComponent<Player>().account;
            Database.singleton.UCE_DatabaseCleanerAccountLastOnline(accountName);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_DatabaseCleanup
    // -----------------------------------------------------------------------------------
    public void UCE_DatabaseCleanup(string accountName)
    {
        var accountChars = Database.singleton.CharactersForAccount(accountName);

#if _MYSQL

		foreach (string accountChar in accountChars) {
			if (0 < (long)Database.singleton.ExecuteScalarMySql("SELECT count(*) FROM character_buffs WHERE `character`=@name", new MySqlParameter("@name", accountChar)))
				Database.singleton.ExecuteNonQueryMySql("DELETE FROM character_buffs WHERE `character`=@name", new MySqlParameter("@name", accountChar));

			if (0 < (long)Database.singleton.ExecuteScalarMySql("SELECT count(*) FROM character_inventory WHERE `character`=@name", new MySqlParameter("@name", accountChar)))
				Database.singleton.ExecuteNonQueryMySql("DELETE FROM character_inventory WHERE `character`=@name", new MySqlParameter("@name", accountChar));

			if (0 < (long)Database.singleton.ExecuteScalarMySql("SELECT count(*) FROM character_equipment WHERE `character`=@name", new MySqlParameter("@name", accountChar)))
				Database.singleton.ExecuteNonQueryMySql("DELETE FROM character_equipment WHERE `character`=@name", new MySqlParameter("@name", accountChar));

			if (0 < (long)Database.singleton.ExecuteScalarMySql("SELECT count(*) FROM character_skills WHERE `character`=@name", new MySqlParameter("@name", accountChar)))
				Database.singleton.ExecuteNonQueryMySql("DELETE FROM character_skills WHERE `character`=@name", new MySqlParameter("@name", accountChar));

			if (0 < (long)Database.singleton.ExecuteScalarMySql("SELECT count(*) FROM character_quests WHERE `character`=@name", new MySqlParameter("@name", accountChar)))
				Database.singleton.ExecuteNonQueryMySql("DELETE FROM character_quests WHERE `character`=@name", new MySqlParameter("@name", accountChar));

			if (0 < (long)Database.singleton.ExecuteScalarMySql("SELECT count(*) FROM character_orders WHERE `character`=@name", new MySqlParameter("@name", accountChar)))
				Database.singleton.ExecuteNonQueryMySql("DELETE FROM character_orders WHERE `character`=@name", new MySqlParameter("@name", accountChar));

			foreach (string charTable in DatabaseCleaner.characterTables) {
				if (charTable != "") {
					if (0 < (long)Database.singleton.ExecuteScalarMySql("SELECT count(*) FROM "+charTable))
						Database.singleton.ExecuteNonQueryMySql("DELETE FROM "+charTable+" WHERE `character`=@name", new MySqlParameter("@name", accountChar));
				}
			}

			foreach (string accountTable in DatabaseCleaner.accountTables) {
				if (accountTable != "") {
					if (0 < (long)Database.singleton.ExecuteScalarMySql("SELECT count(*) FROM "+accountTable))
						Database.singleton.ExecuteNonQueryMySql("DELETE FROM "+accountTable+" WHERE account=@name", new MySqlParameter("@name", accountName));
				}
			}
		}

		if (0 < (long)Database.singleton.ExecuteScalarMySql("SELECT count(*) FROM characters"))
			Database.singleton.ExecuteNonQueryMySql("DELETE FROM characters WHERE account=@name", new MySqlParameter("@name", accountName));

		if (0 < (long)Database.singleton.ExecuteScalarMySql("SELECT count(*) FROM accounts"))
			Database.singleton.ExecuteNonQueryMySql("DELETE FROM accounts WHERE name=@name", new MySqlParameter("@name", accountName));

#elif _SQLITE

        foreach (string accountChar in accountChars)
        {
            Database.singleton.UCE_connection.Execute("DELETE FROM character_buffs WHERE character=?", accountChar);
            Database.singleton.UCE_connection.Execute("DELETE FROM character_inventory WHERE character=?", accountChar);
            Database.singleton.UCE_connection.Execute("DELETE FROM character_equipment WHERE character=?", accountChar);
            Database.singleton.UCE_connection.Execute("DELETE FROM character_skills WHERE character=?", accountChar);
            Database.singleton.UCE_connection.Execute("DELETE FROM character_quests WHERE character=?", accountChar);
            Database.singleton.UCE_connection.Execute("DELETE FROM character_orders WHERE character=?", accountChar);

            foreach (string charTable in DatabaseCleaner.characterTables)
            {
                if (charTable != "")
                {
                    var compare = Database.singleton.UCE_connection.FindWithQuery<sqlite_master>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name=?", charTable);
                    if (0 < compare.count)
                        Database.singleton.UCE_connection.Execute("DELETE FROM " + charTable + " WHERE character=?", accountChar);
                }
            }

            foreach (string accountTable in DatabaseCleaner.accountTables)
            {
                if (accountTable != "")
                {
                    var compare = Database.singleton.UCE_connection.FindWithQuery<sqlite_master>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name=?", accountTable);
                    if (0 < compare.count)
                        Database.singleton.UCE_connection.Execute("DELETE FROM " + accountTable + " WHERE account=?", accountName);
                }
            }
        }

        Database.singleton.UCE_connection.Execute("DELETE FROM characters WHERE account=?", accountName);
        Database.singleton.UCE_connection.Execute("DELETE FROM accounts WHERE name=?", accountName);
#endif

        Debug.Log("DatabaseCleaner deleted characters of account [" + accountName + "] and all associated tables.");
    }

    // -----------------------------------------------------------------------------------
}
