// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE_PVPZONE_Settings

[System.Serializable]
public partial class UCE_PVPZONE_Settings
{
    [Header("[REQUIREMENTS]")]
    public UCE_Requirements requirements;

    [Header("[PVP SETTINGS]")]
    [Tooltip("OpenPvP - players can attack all other players (overrides all other settings)")]
    public bool OpenPvp = false;

    [Tooltip("RealmPvP - players can attack players of other realms, their allied realms or neutral realms - but not their own realm")]
    public bool RealmPvp = false;

    [Tooltip("GuildPvP - players can attack players of other guilds, but not their own guild (only checked if the player is in a guild)")]
    public bool GuildPvp = false;

    [Tooltip("PartyPvP - players can attack players in other parties, but not their own party members (only check if in a party)")]
    public bool PartyPvp = false;

    [Header("[MESSAGES (go in InfoBox)]")]
    public string labelZoneEnter = "You just entered a battle area!";

    public string labelZoneLeave = "You just left the battle area";
    public string labelOpenPvp = "Open PvP";
    public string labelGuildPvp = "Guild PvP";
    public string labelPartyPvp = "Party PvP";
    public string labelRealmPvp = "Realm PvP";
}
