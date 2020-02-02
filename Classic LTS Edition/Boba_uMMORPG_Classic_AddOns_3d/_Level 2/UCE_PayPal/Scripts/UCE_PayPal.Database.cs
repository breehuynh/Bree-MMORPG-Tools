// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;

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
    // Character Purchases
    // -----------------------------------------------------------------------------------
    class character_purchases
    {
        public string character { get; set; }
        public string product { get; set; }
        public string purchased { get; set; }
        public int counter { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_PayPal
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_PayPal()
    {
#if _MYSQL && _SERVER
		singleton.ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_purchases (
			`character` VARCHAR(16) NOT NULL,
			product VARCHAR(32) NOT NULL,
			purchased VARCHAR(32) NOT NULL,
			counter INTEGER(4) NOT NULL
		)");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_purchases>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_loadCharacterPurchase
    // -----------------------------------------------------------------------------------
    public int UCE_loadCharacterPurchase(string name, string product)
    {
        int counter = 0;

        if (UCE_hasCharacterPurchased(name, product))
        {
#if _MYSQL && _SERVER
			counter = Convert.ToInt32((long)singleton.ExecuteScalarMySql("SELECT counter FROM character_purchases WHERE `character`=@name AND `product`=@product",
							new MySqlParameter("@name", name),
							new MySqlParameter("@product", product)));
#elif _SQLITE && _SERVER
            var results = connection.FindWithQuery<character_purchases>("SELECT counter FROM character_purchases WHERE character=? AND product=?", name, product);
            counter = results.counter;
#endif
        }

        return counter;
    }

    // -----------------------------------------------------------------------------------
    // UCE_loadCharacterPurchase
    // -----------------------------------------------------------------------------------
    public bool UCE_hasCharacterPurchased(string name, string product)
    {
#if _MYSQL && _SERVER
		return ((long)singleton.ExecuteScalarMySql("SELECT Count(*) FROM character_purchases WHERE `character`=@name AND `product`=@product",
							new MySqlParameter("@name", name),
							new MySqlParameter("@product", product)))
							==1;
#elif _SQLITE && _SERVER
        var results = connection.Query<character_purchases>("SELECT Count(*) FROM character_purchases WHERE character=? AND product=?", name, product);
        return results.Count >= 1;
#else
		return false;
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_saveCharacterPurchase
    // -----------------------------------------------------------------------------------
    public void UCE_saveCharacterPurchase(string name, UCE_Tmpl_PayPalProduct product, string purchased)
    {
        int counter = UCE_loadCharacterPurchase(name, product.name);
        counter++;
#if _MYSQL && _SERVER
		singleton.ExecuteNonQueryMySql("DELETE FROM character_purchases WHERE `character`=@name AND `product`=@product",
        				new MySqlParameter("@name", name),
						new MySqlParameter("@product", product.name));

        singleton.ExecuteNonQueryMySql("INSERT INTO character_purchases VALUES (@character, @product, @purchased, @counter)",
                        new MySqlParameter("@character", name),
                        new MySqlParameter("@product", product.name),
                        new MySqlParameter("@purchased", purchased),
                        new MySqlParameter("@counter", counter)
                        );
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_purchases WHERE character=? AND product=?", name, product.name);
        connection.Insert(new character_purchases
        {
            character = name,
            product = product.name,
            purchased = purchased,
            counter = counter
        });
#endif
    }

    // -----------------------------------------------------------------------------------
}
