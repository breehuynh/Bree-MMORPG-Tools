// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;

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
    // Account Unlockables
    // -----------------------------------------------------------------------------------
    class account_unlockables
    {
        public string account { get; set; }
        public string unlockable { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_AccountUnlockables
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_AccountUnlockables()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS account_unlockables (
 			account VARCHAR(32) NOT NULL,
 			unlockable VARCHAR(32) NOT NULL
 		)");
#elif _SQLITE && _SERVER
        connection.CreateTable<account_unlockables>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetAccountUnlockables
    // -----------------------------------------------------------------------------------
    public List<string> UCE_GetAccountUnlockables(string accountName)
    {
        List<string> unlockables = new List<string>();

#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT unlockable FROM account_unlockables WHERE `account`=@account", new MySqlParameter("@account", accountName));
        foreach (var row in table)
            unlockables.Add((string)row[0]);
#elif _SQLITE && _SERVER
        var table = connection.Query<account_unlockables>("SELECT unlockable FROM account_unlockables WHERE account=?", accountName);
        foreach (var row in table)
            unlockables.Add(row.unlockable);
#endif
        return unlockables;
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_AccountUnlockables
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_AccountUnlockables(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT unlockable FROM account_unlockables WHERE `account`=@account", new MySqlParameter("@account", player.account));
        foreach (var row in table)
            player.UCE_accountUnlockables.Add((string)row[0]);
#elif _SQLITE && _SERVER
        var table = connection.Query<account_unlockables>("SELECT unlockable FROM account_unlockables WHERE account=?", player.account);
        foreach (var row in table)
            player.UCE_accountUnlockables.Add(row.unlockable);
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_AccountUnlockables
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_AccountUnlockables(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM account_unlockables WHERE `account`=@account", new MySqlParameter("@account", player.account));
		for (int i = 0; i < player.UCE_accountUnlockables.Count; ++i) {
			ExecuteNonQueryMySql("INSERT INTO account_unlockables VALUES (@account, @unlockable)",
 				new MySqlParameter("@account", player.account),
 				new MySqlParameter("@unlockable", player.UCE_accountUnlockables[i]));
 		}
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM account_unlockables WHERE account=?", player.account);
        for (int i = 0; i < player.UCE_accountUnlockables.Count; ++i)
            connection.Insert(new account_unlockables
            {
                account = player.account,
                unlockable = player.UCE_accountUnlockables[i]
            });
#endif
    }

    // -----------------------------------------------------------------------------------
}
