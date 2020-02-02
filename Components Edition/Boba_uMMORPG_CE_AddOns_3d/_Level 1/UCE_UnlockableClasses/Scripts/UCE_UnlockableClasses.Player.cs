// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Linq;
using UnityEngine;

// PLAYER

public partial class Player
{
    [HideInInspector] public SyncListString UCE_unlockedClasses = new SyncListString();

    // -----------------------------------------------------------------------------------
    // UCE_UnlockClass
    // -----------------------------------------------------------------------------------
    public bool UCE_UnlockClass(Player player, string message, byte iconId, byte soundId)
    {
        if (!UCE_HasUnlockedClass(player))
        {
            UCE_unlockedClasses.Add(player.name);
            UCE_ShowPopup(message + player.name, iconId, soundId);
            return true;
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HasUnlockedClass
    // -----------------------------------------------------------------------------------
    public bool UCE_HasUnlockedClass(Player player)
    {
        if (player == null || player.name == "") return false;

        return (UCE_unlockedClasses.Any(s => s == player.name));
    }

    // -----------------------------------------------------------------------------------
}
