// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using SQLite;
using UnityEngine;

// DATABASE CLASSES

public partial class Database
{
    public enum DatabaseType { SQLite, mySQL }

    // -----------------------------------------------------------------------------------
    // UCE_connection
    // @ workaround because uMMORPGs default database connection is private.
    // -----------------------------------------------------------------------------------
    public SQLiteConnection UCE_connection {
		get {
#if _SQLITE && _SERVER
        return connection;
#else
        return null;
#endif
		}
    }

    [Header("Database Type")]
    public DatabaseType databaseType = DatabaseType.SQLite;

    // uses Suriyun Editor tools to toggle visiblity of the following fields
    // those fields are only visible when mySQL is selected

    [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
    public string dbHost = "localhost";
    [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
    public string dbName = "dbName";
    [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
    public string dbUser = "dbUser";
    [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
    public string dbPassword = "dbPassword";
    [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
    public uint dbPort = 3306;
    [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
    public string dbCharacterSet = "utf8mb4";

    protected const string DB_SQLITE = "_SQLITE";
    protected const string DB_MYSQL = "_MYSQL";

    // -----------------------------------------------------------------------------------
    // OnValidate
    // -----------------------------------------------------------------------------------
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (databaseType == Database.DatabaseType.SQLite)
        {
            UCE_EditorTools.RemoveScriptingDefine(DB_MYSQL);
            UCE_EditorTools.AddScriptingDefine(DB_SQLITE);
        }
        else if (databaseType == Database.DatabaseType.mySQL)
        {
            UCE_EditorTools.RemoveScriptingDefine(DB_SQLITE);
            UCE_EditorTools.AddScriptingDefine(DB_MYSQL);
        }
#endif
    }

}
