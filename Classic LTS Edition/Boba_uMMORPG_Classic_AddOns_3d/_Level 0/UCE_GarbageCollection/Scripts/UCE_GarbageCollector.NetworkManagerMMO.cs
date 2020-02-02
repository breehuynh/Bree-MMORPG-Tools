// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using UnityEngine;
using Mirror;

// NetworkManagerMMO

public partial class NetworkManagerMMO
{
    // -----------------------------------------------------------------------------------
    // OnStartServer_UCE_GarbageCollector
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartServer")]
    private void OnStartServer_UCE_GarbageCollector()
    {
        Invoke("UCE_GarbageCollection", 3);
    }

    // -----------------------------------------------------------------------------------
    // OnServerDisconnect_UCE_GarbageCollector
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerDisconnect")]
    private void OnServerDisconnect_UCE_GarbageCollector(NetworkConnection conn)
    {
        if (Player.onlinePlayers.Count <= 1)
            UCE_GarbageCollection();
    }

    // -----------------------------------------------------------------------------------
    // UCE_GarbageCollection
    // -----------------------------------------------------------------------------------
    protected void UCE_GarbageCollection()
    {
        System.GC.Collect();
        Debug.Log("System Garbage Collection called...");
    }

    // -----------------------------------------------------------------------------------
}
