// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// LIMITED TELEPORTER AREA

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NetworkIdentity))]
public class UCE_Area_LimitedTeleport : UCE_AreaBox_InteractableTeleport
{
    [Header("-=-=-=- UCE Limited Teleport -=-=-=-")]
    [Tooltip("[Required] The maximum amount of players that can enter the zone through this teleporter")]
    public int enterLimit;

    [Tooltip("[Optional] The type of player group that can enter the zone through this teleporter")]
    public GroupType groupType;

    [Header("-=-=-=- Configureable Labels -=-=-=-")]
    public string labelZoneCapacity = "- Zone Capacity: ";

    public string labelZoneParty = "- Current Party: ";
    public string labelZoneGuild = "- Current Guild: ";
    public string labelZoneRealm = "- Current Realm: ";
    public string labelGroup = "- Limited to: ";
    public string labelNone = "[None]";

    [SyncVar, HideInInspector] public int enterCount;
    [SyncVar, HideInInspector] public string enterParty;
    [SyncVar, HideInInspector] public string enterGuild;
    [SyncVar, HideInInspector] public int enterRealm;
    [SyncVar, HideInInspector] public int enterAlliedRealm;

    [HideInInspector] public enum GroupType { None, Party, Guild, Realm }

    // -----------------------------------------------------------------------------------
    // ShowAccessRequirementsUI
    // @Client
    // Overwritten for custom functionality
    // -----------------------------------------------------------------------------------
    protected override void ShowAccessRequirementsUI()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (instance == null)
            instance = FindObjectOfType<UCE_UI_InteractableAccessRequirement>();

        instance.Show(this);

        instance.AddMessage(labelZoneCapacity + enterCount.ToString() + "/" + enterLimit.ToString(), enterCount < enterLimit ? instance.textColor : instance.errorColor);

        instance.AddMessage(labelGroup + groupType.ToString(), instance.textColor);

        if (groupType == GroupType.Party)
        {
            if (enterParty == "")
            {
                instance.AddMessage(labelZoneParty + labelNone, instance.textColor);
            }
            else
            {
                instance.AddMessage(labelZoneParty + enterParty, checkParty(player) ? instance.textColor : instance.errorColor);
            }
        }

        if (groupType == GroupType.Guild)
        {
            if (enterGuild == "")
            {
                instance.AddMessage(labelZoneGuild + labelNone, instance.textColor);
            }
            else
            {
                instance.AddMessage(labelZoneGuild + enterGuild, checkGuild(player) ? instance.textColor : instance.errorColor);
            }
        }

        if (groupType == GroupType.Realm)
        {
            if (enterRealm <= 0)
            {
                instance.AddMessage(labelZoneRealm + labelNone, instance.textColor);
            }
            else
            {
                instance.AddMessage(labelZoneRealm + enterRealm, checkRealm(player) ? instance.textColor : instance.errorColor);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // OnInteractServer
    // @Server
    // Overwritten for custom functionality
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public override void OnInteractServer(Player player)
    {
        if (teleportationTarget.Valid)
        {
            UpdateLimitedZoneTeleporter(player);
            teleportationTarget.OnTeleport(player);
        }
    }

    // -----------------------------------------------------------------------------------
    // ReseLimits
    // -----------------------------------------------------------------------------------
    public void ResetLimits()
    {
        enterCount = 0;
        enterParty = "";
        enterGuild = "";
    }

    // -----------------------------------------------------------------------------------
    // ValidateLimitedZoneTeleporter
    // -----------------------------------------------------------------------------------
    protected bool ValidateLimitedZoneTeleporter(Player player)
    {
        if (enterLimit <= 0 || enterCount <= 0) return true;

        if (enterCount >= enterLimit) return false;

        if (groupType == GroupType.None)
            return true;

        // -- check party bound
        if (checkParty(player))
        {
            return true;
        }

        // -- check guild bound
        if (checkGuild(player))
        {
            return true;
        }
#if _iMMOPVP
        // -- check realm bound
        if (checkRealm(player))
        {
            return true;
        }
#endif

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UpdateLimitedZoneTeleporter
    // -----------------------------------------------------------------------------------
    protected void UpdateLimitedZoneTeleporter(Player player)
    {
        bool valid = false;

        // -- check party bound
        if (checkParty(player))
        {
            enterParty = player.party.members[0];
            valid = true;
        }

        // -- check guild bound
        if (checkGuild(player))
        {
            enterGuild = player.guild.name;
            valid = true;
        }
#if _iMMOPVP
        // -- check realm bound
        if (checkRealm(player))
        {
            enterRealm = player.hashRealm;
            enterAlliedRealm = player.hashAlly;
            valid = true;
        }
#endif
        if (valid || groupType == GroupType.None)
            enterCount++;
    }

    // -----------------------------------------------------------------------------------
    // checkParty
    // -----------------------------------------------------------------------------------
    protected bool checkParty(Player player)
    {
        return (groupType == GroupType.Party && player.InParty() && (enterParty == "" || enterParty == player.party.members[0]));
    }

    // -----------------------------------------------------------------------------------
    // checkGuild
    // -----------------------------------------------------------------------------------
    protected bool checkGuild(Player player)
    {
        return (groupType == GroupType.Guild && player.InGuild() && (enterGuild == "" || enterGuild == player.guild.name));
    }

    // -----------------------------------------------------------------------------------
    // checkRealm
    // -----------------------------------------------------------------------------------
    protected bool checkRealm(Player player)
    {
#if _iMMOPVP
        return (groupType == GroupType.Realm && ((enterRealm == 0 && enterAlliedRealm == 0) || player.UCE_getAlliedRealms(interactionRequirements.requiredRealm, interactionRequirements.requiredAlly)));
#else
		return true;
#endif
    }

    // -----------------------------------------------------------------------------------
}
