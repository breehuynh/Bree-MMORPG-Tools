// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// NETWORK MANAGER MMO

public partial class NetworkManagerMMO
{
    // -----------------------------------------------------------------------------------
    // OnServerCharacterCreate_UCE_Factions
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerCharacterCreate")]
    private void OnServerCharacterCreate_UCE_Factions(CharacterCreateMsg message, Player player)
    {
        foreach (UCE_FactionRating faction in player.startingFactions)
            player.UCE_AddFactionRating(faction.faction, faction.startRating);
    }

    // -----------------------------------------------------------------------------------
}
