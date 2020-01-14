// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

// UCE NPC EMOTES

public class UCE_NpcEmotes : NetworkBehaviour
{
    [Header("-=-=- UCE EMOTES & ANIMATION -=-=-")]
    public int secondsBetweenEmotes = 2;

    [Header("[EMOTES]")]
    public UCE_Emotes_Emote[] emotes;

    /*
	[Header("[ANIMATIONS]")]
	public UCE_Emotes_Animation[] animations;
	*/
    private bool emoteUp = false;
    //int currentAnimation = -1;

    // -----------------------------------------------------------------------------------
    // emoteWait
    // -----------------------------------------------------------------------------------
    private IEnumerator emoteWait(float fWaitTime)
    {
        yield return new WaitForSeconds(fWaitTime);

        /*
        if (currentAnimation > -1) {
        	ShowAnimation(currentAnimation, false);
        	currentAnimation = -1;
        }
        */

        emoteUp = false;
    }

    // -----------------------------------------------------------------------------------
    // ShowEmote
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public void ShowEmote(int nIndex)
    {
        if (emoteUp) return;

        Npc npc = GetComponent<Npc>();
        if (!npc) return;

        if (emotes.Length >= nIndex && emotes[nIndex].emote != null)
        {
            if (emotes[nIndex].soundEffect != null && npc.audioSource != null)
                npc.audioSource.PlayOneShot(emotes[nIndex].soundEffect);

            GameObject emoteObject = null;

            emoteObject = Instantiate(emotes[nIndex].emote, transform.position + emotes[nIndex].distanceAboveHead, Quaternion.identity);

            if (emoteObject)
            {
                emoteObject.transform.parent = npc.gameObject.transform;
                //NetworkServer.Spawn(emoteObject);
            }
        }

        emoteUp = true;
        IEnumerator coroutine = emoteWait(secondsBetweenEmotes);
        StartCoroutine(coroutine);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_ShowEmote
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_ShowEmote(int nIndex)
    {
        Npc npc = GetComponent<Npc>();
        if (!npc) return;
        Debug.Log("ShowEmote222");
        if (emotes.Length >= nIndex && emotes[nIndex].emote != null)
        {
            if (emotes[nIndex].soundEffect != null && npc.audioSource != null)
                npc.audioSource.PlayOneShot(emotes[nIndex].soundEffect);

            GameObject emoteObject = null;

            emoteObject = Instantiate(emotes[nIndex].emote, transform.position + emotes[nIndex].distanceAboveHead, Quaternion.identity);

            if (emoteObject)
            {
                emoteObject.transform.parent = npc.gameObject.transform;
                NetworkServer.Spawn(emoteObject);
            }
        }

        emoteUp = true;
        IEnumerator coroutine = emoteWait(secondsBetweenEmotes);
        StartCoroutine(coroutine);
    }

    // -----------------------------------------------------------------------------------
    // ShowAnimation
    // @Client
    // -----------------------------------------------------------------------------------
    /*
        [ClientCallback]
        public void ShowAnimation(int nIndex, bool bStart = true) {
            if (emoteUp) return;

            Npc npc = GetComponent<Npc>();
            if (!npc) return;

            if (animations.Length >= nIndex && string.IsNullOrWhiteSpace(animations[nIndex].animationName)) {
                if (bStart)
                    npc.StartAnimation(animations[nIndex].animationName, animations[nIndex].soundEffect);
                else
                    npc.StopAnimation(animations[nIndex].animationName);
            }

            currentAnimation = nIndex;
            emoteUp = true;

            float fWaitTime = secondsBetweenEmotes;

            if (animations[nIndex].secondsBetweenEmotes > 0)
                fWaitTime = animations[nIndex].secondsBetweenEmotes;

            IEnumerator coroutine = emoteWait(fWaitTime);
            StartCoroutine(coroutine);
        }
     */
    // -----------------------------------------------------------------------------------
}
