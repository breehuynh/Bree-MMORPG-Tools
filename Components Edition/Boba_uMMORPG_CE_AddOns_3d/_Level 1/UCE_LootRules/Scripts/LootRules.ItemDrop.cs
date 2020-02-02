// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

#if _iMMOITEMDROP && _iMMOLOOTRULES

// ===================================================================================
// TAGGED LOOTING
// ===================================================================================
public partial class UCE_ItemDrop
{
    [HideInInspector, SyncVar] public float LiftRulesAfter;
    [HideInInspector, SyncVar] public bool LootVictorParty;
    [HideInInspector, SyncVar] public bool LootVictorGuild;
#if _iMMOPVP
    [HideInInspector, SyncVar] public bool LootVictorRealm;
#endif
    [HideInInspector, SyncVar] public bool LootEverybody;

    [HideInInspector, SyncVar] public GameObject _lastAggressor;
    [HideInInspector, SyncVar] public GameObject _owner;
    [HideInInspector, SyncVar] public int hashRealm;
    [HideInInspector, SyncVar] public int hashAlly;

    // -----------------------------------------------------------------------------------
    // UCE_ValidateTaggedLooting
    // -----------------------------------------------------------------------------------
    public bool UCE_ValidateTaggedLooting(Player player)
    {
        if (LootEverybody ||
        lastAggressor == null ||
        lastAggressor == player ||
        !(lastAggressor is Player) ||
        owner == player ||
        (LiftRulesAfter > 0 && NetworkTime.time > (decayTimeEnd - decayTime) + LiftRulesAfter)
        ) return true;
        if (LootVictorParty && UCE_ValidateTaggedLootingParty(player)) return true;
        if (LootVictorGuild && UCE_ValidateTaggedLootingGuild(player)) return true;
#if _iMMOPVP
        if (LootVictorRealm && UCE_ValidateTaggedLootingRealm(player)) return true;
#endif
        return false;
    }

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
        if (hashRealm == 0 && player.hashRealm == 0 || hashAlly == 0 && player.hashAlly == 0)
            return true;

        if (hashRealm == player.hashRealm || hashAlly == player.hashAlly || hashRealm == player.hashAlly || hashAlly == player.hashRealm)
            return true;

        return false;
    }

#endif

    // -----------------------------------------------------------------------------------
    // lastAggressor
    // -----------------------------------------------------------------------------------
    public Entity lastAggressor
    {
        get { return _lastAggressor != null ? _lastAggressor.GetComponent<Entity>() : null; }
        set { _lastAggressor = value != null ? value.gameObject : null; }
    }

    // -----------------------------------------------------------------------------------
    // owner
    // -----------------------------------------------------------------------------------
    public Entity owner
    {
        get { return _owner != null ? _owner.GetComponent<Entity>() : null; }
        set { _owner = value != null ? value.gameObject : null; }
    }

    // -----------------------------------------------------------------------------------
}

#endif
