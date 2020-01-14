// =======================================================================================
// Created and maintained by Boba
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
    // Character UCE Traits
    // -----------------------------------------------------------------------------------
    class character_UCE_traits
    {
        public string character { get; set; }
        public string name { get; set; }
    }
#endif
    
    // -----------------------------------------------------------------------------------
    // Connect_UCE_Traits
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Traits()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_UCE_traits (`character` VARCHAR(32) NOT NULL, name VARCHAR(32) NOT NULL)");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_UCE_traits>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Traits
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Traits(Player player)
    {
#if _SERVER
#if _MYSQL
		var table = ExecuteReaderMySql("SELECT name FROM character_UCE_traits WHERE `character`=@character", new MySqlParameter("@character", player.name));
#elif _SQLITE
        var table = connection.Query<character_UCE_traits>("SELECT name FROM character_UCE_traits WHERE character=?", player.name);
#endif
        foreach (var row in table)
        {
#if _MYSQL
            UCE_TraitTemplate tmpl = UCE_TraitTemplate.dict[((string)row[0]).GetDeterministicHashCode()];
            player.UCE_Traits.Add(new UCE_Trait(tmpl));
#elif _SQLITE
            UCE_TraitTemplate tmpl = UCE_TraitTemplate.dict[row.name.GetDeterministicHashCode()];
            player.UCE_Traits.Add(new UCE_Trait(tmpl));
#endif
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Traits
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Traits(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM character_UCE_traits WHERE `character`=@character", new MySqlParameter("@character", player.name));
        for (int i = 0; i < player.UCE_Traits.Count; ++i)
        {
            UCE_Trait trait = player.UCE_Traits[i];
            ExecuteNonQueryMySql("INSERT INTO character_UCE_traits VALUES (@character, @name)",
                    new MySqlParameter("@character", player.name),
                    new MySqlParameter("@name", trait.name)
                    );
        }
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_UCE_traits WHERE character=?", player.name);
        for (int i = 0; i < player.UCE_Traits.Count; ++i)
        {
            UCE_Trait trait = player.UCE_Traits[i];
            connection.Insert(new character_UCE_traits
            {
                character = player.name,
                name = trait.name
            });
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}
