// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System;
using System.Collections;

// UI CHARACTER SELECTION

public partial class UCE_UI_Jukebox_CharacterSelection : MonoBehaviour
{
    [Tooltip("[Required] Assign UI Character Selection panel")]
    public GameObject panel;

    private bool startMusic;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        if (!panel.activeSelf || !NetworkClient.active || Player.localPlayer != null) return;

        if (!startMusic)
        {
            if (UCE_Jukebox.singleton.jukeboxTemplate != null &&
                UCE_Jukebox.singleton.jukeboxTemplate.isActive)
            {
                if (UCE_Jukebox.singleton.jukeboxTemplate.menuMusicClip != null)
                    UCE_Jukebox.singleton.PlayBGM(UCE_Jukebox.singleton.jukeboxTemplate.menuMusicClip, UCE_Jukebox.singleton.jukeboxTemplate.menuFadeInFadeOut, UCE_Jukebox.singleton.jukeboxTemplate.menuAdjustedVol, true);
            }
            else
            {
                Debug.LogWarning("Jukebox: Either inactive or ScriptableObject not found!");
            }

            startMusic = true;
        }
    }

    // -----------------------------------------------------------------------------------
}
