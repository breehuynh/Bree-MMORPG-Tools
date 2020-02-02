// =======================================================================================
// Maintained by bobatea#9400 on Discord
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

// PLAYER

public partial class Player
{
    [HideInInspector] public UCE_Area_Exploration UCE_myExploration;

    public SyncListString UCE_exploredAreas = new SyncListString();

    protected double fTimerCache;
    protected double fTimerInterval = 10f;

    // -----------------------------------------------------------------------------------
    // UCE_ExploreArea
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void UCE_ExploreArea()
    {
        if (UCE_myExploration && UCE_myExploration.explorationRequirements.isActive && UCE_myExploration.explorationRequirements.checkRequirements(this))
        {
            // -- explore the area

            if (!UCE_exploredAreas.Contains(UCE_myExploration.name))
            {
                UCE_exploredAreas.Add(UCE_myExploration.name);

                UCE_myExploration.explorationRewards.gainRewards(this);

                var msg = UCE_myExploration.explorePopup.message + UCE_myExploration.name;
                UCE_ShowPopup(msg, UCE_myExploration.explorePopup.iconId, UCE_myExploration.explorePopup.soundId);
                UCE_MinimapSceneText(UCE_myExploration.name);
                fTimerCache = NetworkTime.time + fTimerInterval;

                // -- show notice if already explored
            }
            else if (UCE_myExploration.noticeOnEnter)
            {
                if (NetworkTime.time <= fTimerCache) return;
                var msg = UCE_myExploration.enterPopup.message + UCE_myExploration.name;
                UCE_ShowPopup(msg, UCE_myExploration.enterPopup.iconId, UCE_myExploration.enterPopup.soundId);
                UCE_MinimapSceneText(UCE_myExploration.name);
                fTimerCache = NetworkTime.time + fTimerInterval;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_HasExploredArea
    // -----------------------------------------------------------------------------------
    public bool UCE_HasExploredArea(UCE_Area_Exploration simpleExplorationArea)
    {
        return UCE_exploredAreas.Contains(simpleExplorationArea.name);
    }
}
