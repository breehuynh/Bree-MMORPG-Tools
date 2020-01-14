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

// ===================================================================================
// JUKEBOX AREA
// ===================================================================================
[RequireComponent(typeof(SphereCollider))]
public class UCE_Area_Jukebox : MonoBehaviour
{
    [Header("[-=-=- UCE JUKEBOX AREA -=-=-]")]
    [Tooltip("The audio clip that is played while the player is inside this area")]
    public AudioClip areaMusicClip;

    [Tooltip("The duration it takes to fade the music in/out when entering/leaving the area")]
    public float fadeInFadeOut = 2.0f;

    [Range(0, 1)] public float adjustedVolume;

    [Tooltip("Set to true if you want the music to loop while the player stays inside the area")]
    public bool loop = true;

    // -------------------------------------------------------------------------------
    // OnTriggerEnter
    // -------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider co)
    {
        if (!NetworkClient.active || areaMusicClip == null) return;

        Player player = Player.localPlayer;
        if (!player) return;

        Player entity = co.GetComponentInParent<Player>();

        if (entity != null && entity == player)
            UCE_Jukebox.singleton.PlayBGM(areaMusicClip, fadeInFadeOut, adjustedVolume, loop);
    }

    // -------------------------------------------------------------------------------
    // OnTriggerExit
    // -------------------------------------------------------------------------------
    private void OnTriggerExit(Collider co)
    {
        if (!NetworkClient.active || areaMusicClip == null) return;

        Player player = Player.localPlayer;
        if (!player) return;

        Player entity = co.GetComponentInParent<Player>();

        if (entity != null && entity == player)
            UCE_Jukebox.singleton.revertBGM(areaMusicClip, fadeInFadeOut, adjustedVolume);
    }

    // -------------------------------------------------------------------------------
}
