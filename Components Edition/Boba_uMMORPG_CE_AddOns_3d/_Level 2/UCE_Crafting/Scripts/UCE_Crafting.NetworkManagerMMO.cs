// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;

#if _iMMOCRAFTING

// NETWORK MANAGER MMO

public partial class NetworkManagerMMO
{
    // -----------------------------------------------------------------------------------
    // OnServerCharacterCreate_UCE_Crafting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerCharacterCreate")]
    private void OnServerCharacterCreate_UCE_Crafting(CharacterCreateMsg message, Player player)
    {

        // -- check starting craft professions
        foreach (UCE_DefaultCraftingProfession craft in player.startingCrafts)
        {
            if (!player.UCE_HasCraftingProfession(craft.craftProfession))
            {
                UCE_CraftingProfession tmpProf = new UCE_CraftingProfession(craft.craftProfession.name);
                tmpProf.experience = craft.startingExp;
                player.UCE_Crafts.Add(tmpProf);
            }
        }

        // -- check starting recipes
        foreach (UCE_Tmpl_Recipe recipe in player.startingRecipes)
        {
            if (!player.UCE_recipes.Any(r => r == recipe.name))
                player.UCE_recipes.Add(recipe.name);
        }
    }

    // -----------------------------------------------------------------------------------
}

#endif
