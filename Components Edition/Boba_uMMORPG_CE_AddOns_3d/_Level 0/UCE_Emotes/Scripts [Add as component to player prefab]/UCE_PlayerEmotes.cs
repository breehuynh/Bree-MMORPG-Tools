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

// UCE PLAYER EMOTES

public class UCE_PlayerEmotes : NetworkBehaviour
{
    [Header("-=-=- UCE EMOTES & ANIMATION -=-=-")]
    public int secondsBetweenEmotes = 2;

    [Header("[EMOTES]")]
    public KeyCode emotesHotKey = KeyCode.LeftShift;

    public UCE_Emotes_Emote[] emotes;
    /*
	[Header("[ANIMATIONS]")]
	public KeyCode animationsHotKey = KeyCode.LeftControl;
	public UCE_Emotes_Animation[] animations;
	*/
    private bool emoteUp = false;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (emoteUp == false &&
            player.isAlive &&
            !UIUtils.AnyInputActive() &&
            (player.state == "IDLE" || player.state == "MOVING")
            )
        {
            checkEmotes();
        }
    }

    // -----------------------------------------------------------------------------------
    // emoteWait
    // -----------------------------------------------------------------------------------
    private IEnumerator emoteWait(float fWaitTime)
    {
        yield return new WaitForSeconds(fWaitTime);
        emoteUp = false;
    }

    // -----------------------------------------------------------------------------------
    // ShowEmote
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    private void ShowEmote(int index)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        Cmd_ShowEmote(index);
        emoteUp = true;

        if (emotes[index].soundEffect != null && player.audioSource != null)
            player.audioSource.PlayOneShot(emotes[index].soundEffect);

        IEnumerator coroutine = emoteWait(secondsBetweenEmotes);
        StartCoroutine(coroutine);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_ShowEmote
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    private void Cmd_ShowEmote(int nIndex)
    {
        Player player = GetComponent<Player>();
        if (!player) return;
        Rpc_ShowEmote(nIndex);
    }

    [ClientRpc]
    private void Rpc_ShowEmote(int nIndex)
    {
        Player player = GetComponent<Player>();
        if (emotes.Length >= nIndex && emotes[nIndex].emote != null)
        {
            GameObject emoteObject = null;
            emoteObject = Instantiate(emotes[nIndex].emote, transform.position + emotes[nIndex].distanceAboveHead, Quaternion.identity) as GameObject;

            if (emoteObject)
            {
                emoteObject.transform.parent = player.gameObject.transform;
            }
        }
    }

    /*
        // -----------------------------------------------------------------------------------
        // ShowAnimation
        // @Client
        // -----------------------------------------------------------------------------------
        [Client]
        private void ShowAnimation(int nIndex, bool bStart) {
            Player player = Player.localPlayer;
            if (!player) return;

            Cmd_ShowAnimation(nIndex, bStart);
            emoteUp = true;

            float fWaitTime = secondsBetweenEmotes;

            if (animations[nIndex].secondsBetweenEmotes > 0)
                fWaitTime = animations[nIndex].secondsBetweenEmotes;

            if (bStart && animations[nIndex].soundEffect != null && player.audioSource != null)
                player.audioSource.PlayOneShot(animations[nIndex].soundEffect);

            IEnumerator coroutine = emoteWait(fWaitTime);
            StartCoroutine(coroutine);
        }
    */

    // -----------------------------------------------------------------------------------
    // Cmd_ShowAnimation
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    /*
        [Command]
        private void Cmd_ShowAnimation(int nIndex, bool start) {
            Player player = GetComponent<Player>();
            if (!player) return;

            if (animations.Length >= nIndex && string.IsNullOrWhiteSpace(animations[nIndex].animationName)) {
                if (start)
                    player.StartAnimation(animations[nIndex].animationName);
                else
                    player.StopAnimation(animations[nIndex].animationName);
            }
        }
    */

    // -----------------------------------------------------------------------------------
    // checkEmotes
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    private void checkEmotes()
    {
        for (int i = 0; i < emotes.Length; ++i)
        {
            if (emotes[i].emote == null) return;

            if (Input.GetKey(emotesHotKey) && Input.GetKeyDown(emotes[i].hotKey))
                ShowEmote(i);

            /*
    		if (Input.GetKey(emotesHotKey) && !Input.GetKey(animationsHotKey) && Input.GetKeyDown(emotes[i].hotKey))
    			ShowEmote(i);
    		*/
        }
        /*
    	for (int i = 0; i < animations.Length; ++i) {
    		if (string.IsNullOrWhiteSpace(animations[i].animationName)) return;

    		if (Input.GetKey(animationsHotKey) && !Input.GetKey(emotesHotKey) && Input.GetKeyDown(animations[i].hotKey))
    			ShowAnimation(i, true);

    		if (Input.GetKey(animationsHotKey) && !Input.GetKey(emotesHotKey) && Input.GetKeyUp(animations[i].hotKey))
    			ShowAnimation(i, false);
    	}
    	*/
    }

    // -----------------------------------------------------------------------------------
}
