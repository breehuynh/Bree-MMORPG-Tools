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

#if _iMMOHARVESTING

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{

#if _SQLITE && _SERVER
	// -----------------------------------------------------------------------------------
    // Character Professions
    // -----------------------------------------------------------------------------------
    class character_professions
    {
        public string character { get; set; }
        public string profession { get; set; }
        public long experience { get; set; }
    }
#endif
    
    // -----------------------------------------------------------------------------------
    // Connect_UCE_Harvesting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Harvesting()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS character_professions (
                    `character` VARCHAR(32) NOT NULL,
                    profession VARCHAR(32) NOT NULL,
                    experience BIGINT,
                     PRIMARY KEY(`character`, profession)
                    ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_professions>();
        connection.CreateIndex(nameof(character_professions), new[] { "character", "profession" });
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Harvesting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Harvesting(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT profession, experience FROM character_professions WHERE `character`=@character", new MySqlParameter("@character", player.name));
        foreach (var row in table)
        {
            UCE_HarvestingProfession profession = new UCE_HarvestingProfession((string)row[0]);
            profession.experience = (long)row[1];
            player.UCE_Professions.Add(profession);
        }
#elif _SQLITE && _SERVER
        var table = connection.Query<character_professions>("SELECT profession, experience FROM character_professions WHERE character=?", player.name);
        foreach (var row in table)
        {
            UCE_HarvestingProfession profession = new UCE_HarvestingProfession(row.profession);
            profession.experience = row.experience;
            player.UCE_Professions.Add(profession);
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Harvesting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Harvesting(Player player)
    {
#if _MYSQL && _SERVER
		var query2 = @"
            INSERT INTO character_professions
            SET
            `character`=@character,
            profession=@profession,
            experience = @experience

            ON DUPLICATE KEY UPDATE
            `character`=@character,
            profession=@profession,
            experience = @experience
            ";

        foreach (var profession in player.UCE_Professions)
            ExecuteNonQueryMySql(query2,
           new MySqlParameter("@character", player.name),
           new MySqlParameter("@profession", profession.templateName),
           new MySqlParameter("@experience", profession.experience));
#elif _SQLITE && _SERVER
        connection.Execute("DELETE FROM character_professions WHERE character=?", player.name);
        foreach (var profession in player.UCE_Professions)
            connection.InsertOrReplace(new character_professions
            {
                character = player.name,
                profession = profession.templateName,
                experience = profession.experience
            });
#endif
    }

    // -----------------------------------------------------------------------------------
}

#endif
