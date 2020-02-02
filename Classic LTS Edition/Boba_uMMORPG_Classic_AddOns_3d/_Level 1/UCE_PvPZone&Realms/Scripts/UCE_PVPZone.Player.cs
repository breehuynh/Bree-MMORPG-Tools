// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// PLAYER

public partial class Player
{
    // Bit of overhead but this aproach has the advantage that is not hackable because
    // of being a SyncVar. Otherwise, if the client would change that it would result
    // in a catastrophe!
    [SyncVar, HideInInspector] public bool inAreaOpenPvp = false;

    [SyncVar, HideInInspector] public bool inAreaRealmPvp = false;
    [SyncVar, HideInInspector] public bool inAreaPartyPvp = false;
    [SyncVar, HideInInspector] public bool inAreaGuildPvp = false;

    public const string UCE_PVPREGION_ENTER = "You just entered a battle area!";
    public const string UCE_PVPREGION_LEAVE = "You just left the battle area";

    public const string UCE_PVPREGION_OPEN = "Open PvP";
    public const string UCE_PVPREGION_GUILD = "Guild PvP";
    public const string UCE_PVPREGION_PARTY = "Party PvP";
    public const string UCE_PVPREGION_REALM = "Realm PvP";

    [Header("-=-=-=- UCE AUTOMATIC RETALIATION -=-=-=-")]
    public bool autoRetaliation;

    public int skillIndex;

    protected UCE_PVPZONE_Settings pvpSettings;

    // -----------------------------------------------------------------------------------
    // OnDamageDealt_UCE_PVPZone
    // Custom Hook
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    [DevExtMethods("OnDamageDealt")]
    private void OnDamageDealt_UCE_PVPZone()
    {
        if (autoRetaliation && (target == null || target == lastAggressor) && lastAggressor != null)
        {
            if (CanAttack(lastAggressor))
            {
                if (skills.Count > 0 && skills[skillIndex].IsReady())
                {
                    CmdUseSkill(skillIndex);
                    target = lastAggressor;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_getInPvpRegion
    // -----------------------------------------------------------------------------------
    public override bool UCE_getInPvpRegion()
    {
        return inAreaOpenPvp || inAreaRealmPvp || inAreaPartyPvp || inAreaGuildPvp;
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkPvPSettingsMet
    // -----------------------------------------------------------------------------------
    public bool UCE_checkPvPSettingsMet()
    {
        return (pvpSettings != null && pvpSettings.requirements.checkRequirements(this));
    }

    // -----------------------------------------------------------------------------------
    // UCE_getAttackAllowance
    // @Server / @Client
    // -----------------------------------------------------------------------------------
    public override bool UCE_getAttackAllowance(Entity target)
    {
        if (_cannotCast) return false;

        // ---------- Basic PVP Check
        if (target is Player)
        {
            // ---------- Check for Requirements
            if (
                (!UCE_checkPvPSettingsMet() || !((Player)target).UCE_checkPvPSettingsMet())
                )
                return false;

            // ---------- Open PvP - You can attack every player
            if (inAreaOpenPvp && ((Player)target).inAreaOpenPvp)
            {
                return true;
            }

            // ---------- Check Realm PVP
            // You can attack other realm, but not own realm or allied realm
            if (inAreaRealmPvp && ((Player)target).inAreaRealmPvp)
            {
                if (!UCE_getAlliedRealms(target))
                    return true;
            }

            // ---------- Party PVP - You can attack other party members
            if (inAreaPartyPvp && ((Player)target).inAreaPartyPvp)
            {
                if (InParty() && ((Player)target).InParty() && party.Contains(target.name))
                {
                    return true;
                }
            }

            // ---------- You can NEVER attack your own party members
            if (InParty() && ((Player)target).InParty() && party.Contains(target.name))
            {
                return false;
            }

            // ---------- Guild PVP - You can attack other guild members
            if (inAreaGuildPvp && ((Player)target).inAreaGuildPvp)
            {
                if (InGuild() && ((Player)target).InGuild() && guild.name != ((Player)target).guild.name)
                {
                    return true;
                }
            }

            // ---------- You can NEVER attack your own guild members
            if (InGuild() && ((Player)target).InGuild() && guild.name == ((Player)target).guild.name)
            {
                return false;
            }

            return false;
        }

        // ---------- Basic PVE Check

        // ---------- Check Realm PVE
        // You can attack other realm/neutral, but not own realm or allied realm
        return UCE_getHostileRealms(target);
    }

    // -----------------------------------------------------------------------------------
    // UCE_PvpRegionEnter
    // @Server
    // -----------------------------------------------------------------------------------
    //[ServerCallback]
    public void UCE_PvpRegionEnter(UCE_PVPZONE_Settings settings)
    {
        pvpSettings = settings;

        if (
            pvpSettings.requirements.checkRequirements(this) &&
            (pvpSettings.OpenPvp || pvpSettings.RealmPvp || pvpSettings.GuildPvp || pvpSettings.PartyPvp)
            )
        {
            string msg = "";

            if (inAreaPartyPvp != pvpSettings.PartyPvp) msg += pvpSettings.labelPartyPvp + "/";
            if (inAreaGuildPvp != pvpSettings.GuildPvp) msg += pvpSettings.labelGuildPvp + "/";
            if (inAreaRealmPvp != pvpSettings.RealmPvp) msg += pvpSettings.labelRealmPvp;
            if (inAreaOpenPvp != pvpSettings.OpenPvp) msg = pvpSettings.labelOpenPvp;

            UCE_TargetAddMessage(pvpSettings.labelZoneEnter + " (" + msg + ")");
        }

        inAreaOpenPvp = pvpSettings.OpenPvp;
        inAreaRealmPvp = pvpSettings.RealmPvp;
        inAreaPartyPvp = pvpSettings.PartyPvp;
        inAreaGuildPvp = pvpSettings.GuildPvp;
    }

    // -----------------------------------------------------------------------------------
    // UCE_PvpRegionLeave
    // @Server
    // -----------------------------------------------------------------------------------
    //[ServerCallback]
    public void UCE_PvpRegionLeave()
    {
        if (pvpSettings.requirements.checkRequirements(this))
        {
            string msg = "";

            if (inAreaPartyPvp) msg += pvpSettings.labelPartyPvp + "/";
            if (inAreaGuildPvp) msg += pvpSettings.labelGuildPvp + "/";
            if (inAreaRealmPvp) msg += pvpSettings.labelRealmPvp;
            if (inAreaOpenPvp) msg = pvpSettings.labelOpenPvp;

            UCE_TargetAddMessage(pvpSettings.labelZoneLeave + " (" + msg + ")");
        }

        inAreaOpenPvp = false;
        inAreaRealmPvp = false;
        inAreaPartyPvp = false;
        inAreaGuildPvp = false;

        pvpSettings = null;
    }

    // -----------------------------------------------------------------------------------
}
