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

#if _iMMOCRAFTING

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{

#if _SQLITE && _SERVER
	// -----------------------------------------------------------------------------------
    // Character Crafts
    // -----------------------------------------------------------------------------------
    class character_crafts
    {
        public string character { get; set; }
        public string profession { get; set; }
        public long experience { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // Character Recipes
    // -----------------------------------------------------------------------------------
    class character_recipes
    {
        public string character { get; set; }
        public string recipe { get; set; }
    }
#endif

    // -----------------------------------------------------------------------------------
    // Connect_UCE_Crafting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Crafting()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_crafts (
			`character` VARCHAR(32) NOT NULL,
			profession VARCHAR(32) NOT NULL,
			experience BIGINT
            ) CHARACTER SET=utf8mb4");

        ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_recipes (
			`character` VARCHAR(32) NOT NULL,
			recipe VARCHAR(32) NOT NULL
             ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_crafts>();
        connection.CreateIndex(nameof(character_crafts), new[] { "character", "profession" });
        connection.CreateTable<character_recipes>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Crafting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Crafting(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT profession, experience FROM character_crafts WHERE `character`=@character",
                    new MySqlParameter("@character", player.name));

        foreach (var row in table)
        {
            UCE_CraftingProfession profession = new UCE_CraftingProfession((string)row[0]);
            profession.experience = (long)row[1];
            player.UCE_Crafts.Add(profession);
        }

        var table2 = ExecuteReaderMySql("SELECT recipe FROM character_recipes WHERE `character`=@name", new MySqlParameter("@name", player.name));
        foreach (var row in table2)
        {
            player.UCE_recipes.Add((string)row[0]);
        }
#elif _SQLITE && _SERVER
        var table = connection.Query<character_crafts>("SELECT profession, experience FROM character_crafts WHERE character=?", player.name);
        foreach (var row in table)
        {
            UCE_CraftingProfession profession = new UCE_CraftingProfession(row.profession);
            profession.experience = row.experience;
            player.UCE_Crafts.Add(profession);
        }

        var table2 = connection.Query<character_recipes>("SELECT recipe FROM character_recipes WHERE character=?", player.name);
        foreach (var row in table2)
            player.UCE_recipes.Add(row.recipe);
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Crafting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Crafting(Player player)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("DELETE FROM character_crafts WHERE `character`=@character", new MySqlParameter("@character", player.name));
        foreach (var profession in player.UCE_Crafts)
            ExecuteNonQueryMySql("INSERT INTO character_crafts VALUES (@character, @profession, @experience)",
                            new MySqlParameter("@character", player.name),
                            new MySqlParameter("@profession", profession.templateName),
                            new MySqlParameter("@experience", profession.experience));

        ExecuteNonQueryMySql("DELETE FROM character_recipes WHERE `character`=@character", new MySqlParameter("@character", player.name));
        for (int i = 0; i < player.UCE_recipes.Count; ++i)
        {
            ExecuteNonQueryMySql("INSERT INTO character_recipes VALUES (@character, @recipe)",
                 new MySqlParameter("@character", player.name),
                 new MySqlParameter("@recipe", player.UCE_recipes[i]));
        }
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_crafts WHERE character=?", player.name);
        foreach (var profession in player.UCE_Crafts)
            connection.InsertOrReplace(new character_crafts
            {
                character = player.name,
                profession = profession.templateName,
                experience = profession.experience
            });

        connection.Execute("DELETE FROM character_recipes WHERE character=?", player.name);
        for (int i = 0; i < player.UCE_recipes.Count; ++i)
        {
            connection.InsertOrReplace(new character_recipes
            {
                character = player.name,
                recipe = player.UCE_recipes[i]
            });
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}

#endif
