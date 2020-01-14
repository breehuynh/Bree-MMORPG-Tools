// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

// UCE SHARED INSTANCE MANAGER

public class UCE_LimitedZonesManager : MonoBehaviour
{
    public UCE_LimitedZonesEntry[] sharedInstances;

    // -----------------------------------------------------------------------------------
    // getAvailableSharedInstances
    // Retrieve a list of shared instances the player is allowed to see
    // -----------------------------------------------------------------------------------
    public List<UCE_LimitedZonesEntry> getAvailableSharedInstances(Player player, int instanceCategory)
    {
        return sharedInstances.Where(x => x.instanceCategory == instanceCategory && x.canPlayerSee(player)).ToList();
    }

    // -----------------------------------------------------------------------------------
}