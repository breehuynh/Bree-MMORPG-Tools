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
using System;

// PLAYER

public partial class Player
{
    [Header("-=-=-=- UCE LEVEL UP NOTICE -=-=-=-")]
    public UCE_PopupClass levelUpNotice;

    // -----------------------------------------------------------------------------------
    // OnLevelUp_UCE_LevelUpNotice
    // -----------------------------------------------------------------------------------
    [Server]
    [DevExtMethods("OnLevelUp")]
    private void OnLevelUp_UCE_LevelUpNotice()
    {
        Target_UCE_ShowPopup(connectionToClient, levelUpNotice.message + level.ToString(), levelUpNotice.iconId, levelUpNotice.soundId);
    }
}
