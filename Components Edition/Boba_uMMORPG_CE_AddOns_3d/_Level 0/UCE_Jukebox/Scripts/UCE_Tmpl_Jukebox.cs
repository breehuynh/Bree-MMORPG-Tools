// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UCE JUKEBOX

[CreateAssetMenu(fileName = "UCE Jukebox", menuName = "UCE Templates/New UCE Jukebox", order = 999)]
public class UCE_Tmpl_Jukebox : ScriptableObject
{
    [Header("-=-=-=- UCE JUKEBOX -=-=-=-")]
    public bool isActive;

    [Header("[MENU MUSIC (Only functional on compiled client)]")]
    [Tooltip("This music plays (looped) while the player is not logged in")]
    public AudioClip menuMusicClip;

    public float menuFadeInFadeOut = 1.0f;
    [Range(0, 1)] public float menuAdjustedVol = 1.0f;

    [Header("[DEFAULT GAME MUSIC]")]
    [Tooltip("This music plays (looped) while logged in but not inside any music area")]
    public AudioClip defaultMusicClip;

    public float defaultFadeInFadeOut = 1.0f;
    [Range(0, 1)] public float defaultAdjustedVol = 1.0f;
}
