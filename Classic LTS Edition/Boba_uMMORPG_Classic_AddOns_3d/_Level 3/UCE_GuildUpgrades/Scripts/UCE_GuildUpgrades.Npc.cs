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
    [Header("[-=-=-=- UCE GUILD UPGRADES -=-=-=-]")]
    public bool offersGuildUpgrade = false;

    // -----------------------------------------------------------------------------------
    // checkGuildUpgradeAccess
    // -----------------------------------------------------------------------------------
    public bool checkGuildUpgradeAccess(Player player)
    {
        if (!offersGuildUpgrade || !player.InGuild()) return false;

        return player.guild.CanNotify(player.name);
    }

    // -----------------------------------------------------------------------------------
}
