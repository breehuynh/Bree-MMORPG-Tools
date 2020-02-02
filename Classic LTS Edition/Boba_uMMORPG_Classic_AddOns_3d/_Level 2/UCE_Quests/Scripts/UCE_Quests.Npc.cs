// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// NPC

public partial class Npc
{
    [Header("[-=-=-=- UCE Quests -=-=-=-]")]
    public UCE_ScriptableQuest[] UCE_quests;

    // =============================== CORE SCRIPT REWRITES ==============================

    // -----------------------------------------------------------------------------------
    // UpdateOverlays
    // -----------------------------------------------------------------------------------
    protected override void UpdateOverlays()
    {
        base.UpdateOverlays();
    }

    // -----------------------------------------------------------------------------------
    // UpdateClient_UCE_quests
    // -----------------------------------------------------------------------------------
    [Client]
    [DevExtMethods("UpdateClient")]
    protected void UpdateClient_UCE_quests()
    {
        if (questOverlay != null)
        {
            Player player = Player.localPlayer;

            if (player != null)
            {
                if (UCE_quests.Any(q => player.UCE_CanCompleteQuest(q.name)))
                    questOverlay.text = "!";
                else if (UCE_quests.Any(player.UCE_CanAcceptQuest))
                    questOverlay.text = "?";
                else
                    questOverlay.text = "";
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_questsVisibleFor
    // -----------------------------------------------------------------------------------
    public List<UCE_ScriptableQuest> UCE_QuestsVisibleFor(Player player)
    {
        return UCE_quests.Where(q =>
                                        player.UCE_CanAcceptQuest(q) ||
                                        player.UCE_CanCompleteQuest(q.name) ||
                                        player.UCE_HasActiveQuest(q.name)
                                ).ToList();
    }

    // -----------------------------------------------------------------------------------
}
