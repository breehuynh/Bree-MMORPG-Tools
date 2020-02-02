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

public partial class Npc
{
    public AudioClip interactAudio;
    public AudioClip questAudio;

    public void PlayInteractSound()
    {
        AudioSource tempSource = GetComponentInParent<AudioSource>();
        if(tempSource == null) return;

        tempSource.PlayOneShot(interactAudio);
    }

    public void PlayQuestAudio()
    {
        AudioSource tempSource = GetComponentInParent<AudioSource>();
        if(tempSource == null) return;

        tempSource.PlayOneShot(questAudio);
    }

}

public partial class UINpcDialogue
{

    public void interactAudio()
    {
        Player player = Player.localPlayer;
        if(player == null || player.target == null) return;

        if (player.target is Npc)
        {
            Npc npc = (Npc)player.target;
            if (npc.interactAudio == null) return;
            npc.PlayInteractSound();
        }

    }

    public void questAudio()
    {
        Player player = Player.localPlayer;
        if(player == null || player.target == null) return;

        if (player.target is Npc)
        {
            Npc npc = (Npc)player.target;
            if (npc.questAudio == null) return;
            npc.PlayQuestAudio();
        }

    }
}