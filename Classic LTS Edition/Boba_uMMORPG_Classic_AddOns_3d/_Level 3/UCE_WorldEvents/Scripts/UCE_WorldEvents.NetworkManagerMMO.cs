// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mirror;

// NetworkManagerMMO

public partial class NetworkManagerMMO
{
    public static List<UCE_WorldEvent> UCE_WorldEvents = new List<UCE_WorldEvent>();
    public static bool UCE_worldEventsChanged = true;

    // -----------------------------------------------------------------------------------
    // OnStartServer_UCE_WorldEvents
    // @Server
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartServer")]
    private void OnStartServer_UCE_WorldEvents()
    {
        UCE_WorldEvents.Clear();
        foreach (UCE_WorldEventTemplate template in UCE_WorldEventTemplate.dict.Values)
        {
            UCE_WorldEvent e = new UCE_WorldEvent();
            e.name = template.name;
            e.count = 0;
            UCE_WorldEvents.Add(e);
        }

        Database.singleton.UCE_Load_WorldEvents();

        UCE_worldEventsChanged = true;
    }

    // -----------------------------------------------------------------------------------
    // OnStopServer_UCE_WorldEvents
    // @Server
    // we save all world events when the server stops
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStopServer")]
    private void OnStopServer_UCE_WorldEvents()
    {
        UCE_SaveWorldEvents();
    }

    // -----------------------------------------------------------------------------------
    // OnServerDisconnect_UCE_WorldEvents
    // @Server
    // we also save all world events when a client disconnects (thats not too often but
    // frequently enough
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerDisconnect")]
    private void OnServerDisconnect_UCE_WorldEvents(NetworkConnection conn)
    {
        UCE_SaveWorldEvents();
    }

    // -----------------------------------------------------------------------------------
    // UCE_SaveWorldEvents
    // we only save the world events when its required
    // @Server
    // -----------------------------------------------------------------------------------
    public static void UCE_SaveWorldEvents()
    {
        if (UCE_worldEventsChanged)
        {
            Database.singleton.UCE_Save_WorldEvents();
            UCE_worldEventsChanged = false;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckWorldEvent
    // @Server
    // -----------------------------------------------------------------------------------
    public static bool UCE_CheckWorldEvent(UCE_WorldEventTemplate ev, int minCount, int maxCount)
    {
        if (ev == null) return false;
        if (minCount == 0 && maxCount == 0) return true;

        int count = UCE_GetWorldEventCount(ev);

        return (
            (minCount <= 0 || count >= minCount) &&
            (maxCount <= 0 || count <= maxCount)
            );
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetWorldEventCount
    // @Server
    // -----------------------------------------------------------------------------------
    public static int UCE_GetWorldEventCount(UCE_WorldEventTemplate ev)
    {
        return UCE_WorldEvents.FirstOrDefault(x => x.template == ev).count;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SetWorldEventCount
    // @Server
    // -----------------------------------------------------------------------------------
    public static void UCE_SetWorldEventCount(string name, int value)
    {
        int id = UCE_WorldEvents.FindIndex(x => x.name == name);

        if (id != -1)
        {
            UCE_WorldEvent e = UCE_WorldEvents[id];
            e.count = value;
            UCE_WorldEvents[id] = e;
            UCE_worldEventsChanged = true;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_ModifyWorldEventCount
    // @Server
    // -----------------------------------------------------------------------------------
    public static void UCE_ModifyWorldEventCount(UCE_WorldEventTemplate ev, int value)
    {
        int id = UCE_WorldEvents.FindIndex(x => x.template == ev);

        if (id != -1)
        {
            UCE_WorldEvent e = UCE_WorldEvents[id];
            e.Modify(value);
            UCE_WorldEvents[id] = e;
            UCE_worldEventsChanged = true;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_BroadCastPopupToOnlinePlayers
    // @Server
    // -----------------------------------------------------------------------------------
    public static void UCE_BroadCastPopupToOnlinePlayers(UCE_WorldEventTemplate ev, bool participatedOnly, string message)
    {
        foreach (Player player in Player.onlinePlayers.Values)
        {
            if (!participatedOnly || (participatedOnly && player.UCE_WorldEvents.FirstOrDefault(x => x.template == ev).participated))
            {
                player.UCE_ShowPopup(message);
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
