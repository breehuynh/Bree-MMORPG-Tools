// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// GUILD WAREHOUSE - NPC

public partial class Npc
{
    [Header("[-=-=-=- UCE GUILD WAREHOUSE -=-=-=-]")]
    public bool offersGuildWarehouse = false;

    public bool accessViceOrMasterOnly;
    [HideInInspector] public SyncListString guildWarehouseBusy = new SyncListString();

    // -----------------------------------------------------------------------------------
    // checkWarehouseAccess
    // -----------------------------------------------------------------------------------
    public bool checkWarehouseAccess(Player player)
    {
        if (!offersGuildWarehouse || !player.guild.InGuild()) return false;
        if (!accessViceOrMasterOnly) return true;

        return player.guild.guild.CanNotify(player.name);
    }

    // -----------------------------------------------------------------------------------
}
