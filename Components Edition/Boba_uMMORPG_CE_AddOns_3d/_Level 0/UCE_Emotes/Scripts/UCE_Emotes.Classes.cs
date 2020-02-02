// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

// UCE EMOTES

// ---------------------------------------------------------------------------------------
// UCE_Emotes_Emote
// ---------------------------------------------------------------------------------------
[System.Serializable]
public class UCE_Emotes_Emote
{
    public GameObject emote;
    public AudioClip soundEffect;
    public KeyCode hotKey = KeyCode.C;
    public Vector3 distanceAboveHead = new Vector3(0f, 2.5f, 0f);
}

// ---------------------------------------------------------------------------------------
// UCE_Emotes_Animation
// ---------------------------------------------------------------------------------------
[System.Serializable]
public class UCE_Emotes_Animation
{
    public string animationName;
    public AudioClip soundEffect;
    public KeyCode hotKey = KeyCode.C;
    public float secondsBetweenEmotes;
}
