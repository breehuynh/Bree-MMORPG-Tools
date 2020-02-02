// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE_NpcExtendedText

[System.Serializable]
public class UCE_NpcExtendedText
{
    [Header("[-=-=-=- UCE Extended Text -=-=-=-]")]
    [TextArea(1, 30)] public string text;

    public bool displayRandomly;
#if _iMMOEMOTES

    [Header("[EMOTES & ANIMATION]")]
    [Tooltip("[Optional] Entry number of emote to show (0 is first one, -1 to disable)")]
    public int emotesId = -1;

    /*
	[Tooltip("[Optional] Entry number of animation to play (0 is first one, -1 to disable - works only when emote is disabled)")]
	public int animationId = -1;
	*/
#endif

    public UCE_InteractionRequirements displayRequirements;
}
