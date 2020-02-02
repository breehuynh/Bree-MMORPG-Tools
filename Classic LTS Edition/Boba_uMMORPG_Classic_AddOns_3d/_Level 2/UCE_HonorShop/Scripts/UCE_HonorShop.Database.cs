// =======================================================================================
// Maintained by bobatea#9400 on Discord
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
    // Character Currencies
    // -----------------------------------------------------------------------------------
    class character_currencies
    {
        public string character { get; set; }
        public string currency { get; set; }
        public long amount { get; set; }
        public long total { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_HonorShop
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_HonorShop()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_currencies (
			`character` VARCHAR(32) NOT NULL,
			currency VARCHAR(32) NOT NULL,
			amount INTEGER(16) NOT NULL,
			total INTEGER(16) NOT NULL
		    )CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_currencies>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_HonorShop
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_HonorShop(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT currency, amount, total FROM character_currencies WHERE `character`=@name", new MySqlParameter("@name", player.name));
        foreach (var row in table)
        {
            string tmplName = (string)row[0];
            UCE_Tmpl_HonorCurrency tmplCurrency;

            if (UCE_Tmpl_HonorCurrency.dict.TryGetValue(tmplName.GetStableHashCode(), out tmplCurrency))
            {
                UCE_HonorShopCurrency hsc = new UCE_HonorShopCurrency();
                hsc.honorCurrency = tmplCurrency;
                hsc.amount = (int)row[1];
                hsc.total = (int)row[2];
                player.UCE_currencies.Add(hsc);
            }
        }
#elif _SQLITE && _SERVER
        var table = connection.Query<character_currencies>("SELECT currency, amount, total FROM character_currencies WHERE character=?", player.name);
        foreach (var row in table)
        {
            string tmplName = row.currency;
            UCE_Tmpl_HonorCurrency tmplCurrency;

            if (UCE_Tmpl_HonorCurrency.dict.TryGetValue(tmplName.GetStableHashCode(), out tmplCurrency))
            {
                UCE_HonorShopCurrency hsc = new UCE_HonorShopCurrency();
                hsc.honorCurrency = tmplCurrency;
                hsc.amount = row.amount;
                hsc.total = row.total;
                player.UCE_currencies.Add(hsc);
            }
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_HonorShop
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_HonorShop(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM character_currencies WHERE `character`=@character", new MySqlParameter("@character", player.name));
        for (int i = 0; i < player.UCE_currencies.Count; ++i)
        {
            ExecuteNonQueryMySql("INSERT INTO character_currencies VALUES (@character, @currency, @amount, @total)",
                 new MySqlParameter("@character", player.name),
                 new MySqlParameter("@currency", player.UCE_currencies[i].honorCurrency.name),
                 new MySqlParameter("@amount", player.UCE_currencies[i].amount),
                 new MySqlParameter("@total", player.UCE_currencies[i].total)
                 );
        }
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_currencies WHERE character=?", player.name);
        for (int i = 0; i < player.UCE_currencies.Count; ++i)
            connection.InsertOrReplace(new character_currencies
            {
                character = player.name,
                currency = player.UCE_currencies[i].honorCurrency.name,
                amount = player.UCE_currencies[i].amount,
                total = player.UCE_currencies[i].total
            });
#endif
    }

    // -----------------------------------------------------------------------------------
}
