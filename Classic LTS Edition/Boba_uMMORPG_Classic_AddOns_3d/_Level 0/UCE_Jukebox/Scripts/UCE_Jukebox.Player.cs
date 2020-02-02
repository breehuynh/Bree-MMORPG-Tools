// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System;

// PLAYER

public partial class Player
{
    // -----------------------------------------------------------------------------------
    // Start_UCE_Jukebox
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    [DevExtMethods("Start")]
    private void Start_UCE_Jukebox()
    {
    	if (UCE_Jukebox.singleton == null)
    	{
    		Debug.LogWarning("No Jukebox has been added to the scene!");
    		return;
    	}
    	
        if (UCE_Jukebox.singleton.jukeboxTemplate != null &&
            UCE_Jukebox.singleton.jukeboxTemplate.isActive &&
            UCE_Jukebox.singleton.jukeboxTemplate.defaultMusicClip != null)
            UCE_Jukebox.singleton.PlayBGM(UCE_Jukebox.singleton.jukeboxTemplate.defaultMusicClip, UCE_Jukebox.singleton.jukeboxTemplate.defaultFadeInFadeOut, UCE_Jukebox.singleton.jukeboxTemplate.defaultAdjustedVol, true);
        else
            Debug.LogWarning("No Jukebox assigned or missing audio clip!");
    }

    // -----------------------------------------------------------------------------------
    // OnDeath_UCE_Jukebox
    // @Server
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDeath")]
    private void OnDeath_UCE_Jukebox()
    {
        Target_UCE_JukeboxRevert(this.connectionToClient);
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_JukeboxRevert
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void Target_UCE_JukeboxRevert(NetworkConnection target)
    {
        if (UCE_Jukebox.singleton != null && UCE_Jukebox.singleton.jukeboxTemplate != null)
            UCE_Jukebox.singleton.revertBGM(UCE_Jukebox.singleton.jukeboxTemplate.defaultMusicClip, UCE_Jukebox.singleton.jukeboxTemplate.defaultFadeInFadeOut, UCE_Jukebox.singleton.jukeboxTemplate.defaultAdjustedVol);
        else Debug.Log("Jukebox is missing!");
    }

    // -----------------------------------------------------------------------------------
}
