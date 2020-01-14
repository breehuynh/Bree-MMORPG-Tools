// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using System.Linq;
using UnityEngine;

// PLAYER

public partial class Player
{
    [Header("[UCE GUILD UPGRADES]")]
    public UCE_Tmpl_GuildUpgrades guildUpgrades;

    [SyncVar] private int _guildUpgradeLevel = 0;

    // -----------------------------------------------------------------------------------
    // guildLevel
    // -----------------------------------------------------------------------------------
    public int guildLevel
    {
        get { return _guildUpgradeLevel; }
        set { _guildUpgradeLevel = value; }
    }

    // -----------------------------------------------------------------------------------
    // guildCapacity
    // -----------------------------------------------------------------------------------
    public int guildCapacity
    {
        get { return guildUpgrades.guildCapacity.Get(guildLevel); }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanInvite
    // -----------------------------------------------------------------------------------
    public bool UCE_CanInvite(string requesterName, string targetName)
    {
        return guild.members.Length < guildCapacity &&
               guild.CanInvite(requesterName, targetName);
    }

    // ================================== COMMANDS =======================================

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_LoadGuildUCE_warehouse
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_LoadGuildUpgrades()
    {
        if (guild.name == "") return;
        Database.singleton.UCE_LoadGuildWarehouse(this);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_SaveGuildUCE_warehouse
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_SaveGuildUpgrades()
    {
        if (guild.name == "") return;
        Database.singleton.UCE_SaveGuildWarehouse(this);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_UpgradeGuildWarehouse
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_UpgradeGuild()
    {
        UCE_UpgradeGuild();
    }

    // -----------------------------------------------------------------------------------
    // UCE_UpgradePlayerWarehouse
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_UpgradeGuild()
    {
        if (UCE_CanUpgradeGuild())
        {
            guildUpgrades.guildUpgradeCost[guildLevel].payCost(this);

            if (guildUpgrades.rewardItem.Length >= guildLevel)
            {
                InventoryAdd(new Item(guildUpgrades.rewardItem[guildLevel].item), guildUpgrades.rewardItem[guildLevel].amount);
            }

            guildLevel++;

            UCE_BroadCastPopupToOnlineGuildMembers(guildUpgrades.guildUpgradeLabel);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanUpgradeGuildWarehouse
    // @Client OR @Server
    // -----------------------------------------------------------------------------------
    public bool UCE_CanUpgradeGuild()
    {
        return (
                guildUpgrades.guildUpgradeCost.Length > 0 &&
                guildUpgrades.guildUpgradeCost.Length > guildLevel &&
                guildUpgrades.guildUpgradeCost[guildLevel].checkCost(this) &&
                UCE_CanAddRewardItem()
                );
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanAddRewardItem
    // @Client OR @Server
    // -----------------------------------------------------------------------------------
    public bool UCE_CanAddRewardItem()
    {
        return
                (
                guildUpgrades.rewardItem.Length == 0 ||
                guildUpgrades.rewardItem.Length < guildLevel ||
                (
                guildUpgrades.rewardItem.Length >= guildLevel &&
                InventoryCanAdd(new Item(guildUpgrades.rewardItem[guildLevel].item), guildUpgrades.rewardItem[guildLevel].amount)
                )
                );
    }

    // -----------------------------------------------------------------------------------
    // UCE_BroadCastPopupToOnlineGuildMembers
    // @Server
    // -----------------------------------------------------------------------------------
    public void UCE_BroadCastPopupToOnlineGuildMembers(string message)
    {
        foreach (Player player in Player.onlinePlayers.Values)
        {
            if (InGuild() && player.InGuild() && guild.name == player.guild.name)
            {
                player.UCE_ShowPopup(message);
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
