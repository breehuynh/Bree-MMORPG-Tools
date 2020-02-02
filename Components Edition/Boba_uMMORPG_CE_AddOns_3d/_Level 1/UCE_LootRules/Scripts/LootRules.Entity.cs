// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// ===================================================================================
// LOOT RULES - ENTITY
// ===================================================================================
public partial class Entity
{
    // -----------------------------------------------------------------------------------
    // UCE_ValidateTaggedLootingParty
    // -----------------------------------------------------------------------------------
    public bool UCE_ValidateTaggedLootingParty(Player player)
    {
        if (lastAggressor == null) return true;

        Player plyr;

        if (lastAggressor is Player)
            plyr = (Player)lastAggressor;
        else
            return true;

        bool valid = (
                (
                (plyr.party.InParty() && plyr.party.party.Contains(player.name)) ||
                (player.party.InParty() && player.party.party.Contains(plyr.name))
                )
                );

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_ValidateTaggedLootingGuild
    // -----------------------------------------------------------------------------------
    public bool UCE_ValidateTaggedLootingGuild(Player player)
    {
        if (lastAggressor == null) return true;

        Player plyr;

        if (lastAggressor is Player)
            plyr = (Player)lastAggressor;
        else
            return true;

        return (
                plyr.guild.InGuild() &&
                player.guild.InGuild() &&
                 plyr.guild.name != "" &&
                player.guild.name != "" &&
                plyr.guild.name == player.guild.name);
    }

    // -----------------------------------------------------------------------------------
    // UCE_ValidateTaggedLootingRealm
    // -----------------------------------------------------------------------------------
#if _iMMOPVP

    public bool UCE_ValidateTaggedLootingRealm(Player player)
    {
        return UCE_getAlliedRealms(player);
    }

#endif

    // -----------------------------------------------------------------------------------
}
