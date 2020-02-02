// =======================================================================================
// Maintained by bobatea#9400 on Discord
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
    // Account Unlocked Classes
    // -----------------------------------------------------------------------------------
    class account_unlockedclasses
    {
        public string account { get; set; }
        public string classname { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_UnlockableClasses
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_UnlockableClasses()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS account_unlockedclasses (
 			account VARCHAR(32) NOT NULL,
 			classname VARCHAR(32) NOT NULL
 		)");
#elif _SQLITE && _SERVER
        connection.CreateTable<account_unlockedclasses>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetUnlockedClasses
    // -----------------------------------------------------------------------------------
    public List<string> UCE_GetUnlockedClasses(string accountName)
    {
        List<string> unlockedClasses = new List<string>();
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT classname FROM account_unlockedclasses WHERE `account`=@account", new MySqlParameter("@account", accountName));
        foreach (var row in table)
            unlockedClasses.Add((string)row[0]);
#elif _SQLITE && _SERVER
        var table = connection.Query<account_unlockedclasses>("SELECT classname FROM account_unlockedclasses WHERE account=?", accountName);
        foreach (var row in table)
            unlockedClasses.Add(row.classname);
#endif
        return unlockedClasses;
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_UnlockableClasses
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_UnlockableClasses(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT classname FROM account_unlockedclasses WHERE `account`=@account", new MySqlParameter("@account", player.account));
        foreach (var row in table)
            player.UCE_unlockedClasses.Add((string)row[0]);
#elif _SQLITE && _SERVER
        var table = connection.Query<account_unlockedclasses>("SELECT classname FROM account_unlockedclasses WHERE account=?", player.account);
        foreach (var row in table)
            player.UCE_unlockedClasses.Add(row.classname);
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_UnlockableClasses
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_UnlockableClasses(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM account_unlockedclasses WHERE `account`=@account", new MySqlParameter("@account", player.account));
		for (int i = 0; i < player.UCE_unlockedClasses.Count; ++i) {
			ExecuteNonQueryMySql("INSERT INTO account_unlockedclasses VALUES (@account, @classname)",
 				new MySqlParameter("@account", player.account),
 				new MySqlParameter("@classname", player.UCE_unlockedClasses[i]));
 		}
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM account_unlockedclasses WHERE account=?", player.account);
        for (int i = 0; i < player.UCE_unlockedClasses.Count; ++i)
            connection.Insert(new account_unlockedclasses
            {
                account = player.account,
                classname = player.UCE_unlockedClasses[i]
            });
#endif
    }

    // -----------------------------------------------------------------------------------
}
