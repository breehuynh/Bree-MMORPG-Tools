// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

// PLAYER

public partial class Player
{
    public SyncListUCE_WorldEvent UCE_WorldEvents = new SyncListUCE_WorldEvent();

    // -----------------------------------------------------------------------------------
    // UCE_CheckWorldEvent
    // -----------------------------------------------------------------------------------
    public bool UCE_CheckWorldEvent(UCE_WorldEventTemplate ev, int minCount, int maxCount)
    {
        if (ev == null || (minCount == 0 && maxCount == 0)) return true;

        int count = UCE_GetWorldEventCount(ev);

        if (count == 0) return false;

        return (
            (minCount == 0 || count >= minCount) &&
            (maxCount == 0 || count <= maxCount)
            );
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetWorldEventCount
    // -----------------------------------------------------------------------------------
    public int UCE_GetWorldEventCount(UCE_WorldEventTemplate ev)
    {
        return UCE_WorldEvents.FirstOrDefault(x => x.template == ev).count;
    }

    // -----------------------------------------------------------------------------------
    // UCE_ModifyWorldEventCount
    // -----------------------------------------------------------------------------------
    public void UCE_ModifyWorldEventCount(UCE_WorldEventTemplate ev, int value)
    {
        // -- update the players event list

        int id = UCE_WorldEvents.FindIndex(x => x.template == ev);

        if (id != -1)
        {
            UCE_WorldEvent e = UCE_WorldEvents[id];
            e.Modify(value, false);
            UCE_WorldEvents[id] = e;
        }

        // -- update the global event list as well

        NetworkManagerMMO.UCE_ModifyWorldEventCount(ev, value);
    }

    // -----------------------------------------------------------------------------------
}
