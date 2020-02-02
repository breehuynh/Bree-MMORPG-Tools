// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
#if _UMA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if _MYSQL
using MySql.Data;								// From MySql.Data.dll in Plugins folder
using MySql.Data.MySqlClient;                   // From MySql.Data.dll in Plugins folder
#elif _SQLITE

using SQLite;

#endif

public partial class Database
{
    private class uce_uma
    {
        public string character { get; set; }
        public string dna { get; set; }
    }

    private void Connect_UmaIntegration()
    {
        connection.CreateTable<uce_uma>();
#if _MYSQL
         ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS uce_uma (`character` VARCHAR(32) NOT NULL, dna VARCHAR(1000) NOT NULL) CHARACTER SET=utf8mb4");
#endif
    }

    private void CharacterLoad_UmaIntegration(Player player)
    { LoadUma(player); }

    private void LoadUma(Player player)
    {
        string loadedDna = connection.ExecuteScalar<string>(
         "SELECT dna FROM uce_uma WHERE character=?", player.name);

        player.umaDna = loadedDna;

#if _MYSQL
        var table = ExecuteReaderMySql("SELECT dna FROM uce_uma WHERE `character`=@character", new MySqlParameter("@character", player.name));
        if (table.Count == 1)
        {
            List<object> mainrow = table[0];
            string tempdna = (string)mainrow[0];
            player.umaDna = tempdna;
        }
#endif
    }

    private void CharacterSave_UmaIntegration(Player player)
    { saveUMA(player); }

    private void saveUMA(Player player)
    {
        connection.Execute("DELETE FROM uce_uma WHERE character =?", player.name);
        connection.InsertOrReplace(new uce_uma
        {
            character = player.name,
            dna = player.umaDna
        });

#if _MYSQL
        ExecuteNonQueryMySql("DELETE FROM uce_uma WHERE `character`=@character", new MySqlParameter("@character", player.name));
        ExecuteNonQueryMySql("INSERT INTO uce_uma VALUES (@character, @dna)",
         	new MySqlParameter("@character", player.name),
         	new MySqlParameter("@dna", player.umaDna));
#endif
    }
}

#endif