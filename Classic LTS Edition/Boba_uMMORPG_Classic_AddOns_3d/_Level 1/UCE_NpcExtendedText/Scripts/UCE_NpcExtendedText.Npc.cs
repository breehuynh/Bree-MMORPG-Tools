// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using UnityEngine;

// ===================================================================================
// NPC EXTENDED TEXT
// ===================================================================================
public partial class Npc
{
    [Header("[-=-=-=- Npc Extended Text -=-=-=-]")]
    public UCE_NpcExtendedText[] extendedTexts;

    protected string _welcome;

    // -----------------------------------------------------------------------------------
    // UCE_UpdateNpcExtendedText
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public string UCE_UpdateNpcExtendedText(Player player)
    {
        if (_welcome == "")
            _welcome = welcome;

        List<UCE_NpcExtendedText> welcomeTexts = new List<UCE_NpcExtendedText>();

        foreach (UCE_NpcExtendedText extendedText in extendedTexts)
        {
            bool valid = extendedText.displayRequirements.checkRequirements(player);

            if (valid)
            {
                if (extendedText.displayRandomly)
                {
                    welcomeTexts.Add(extendedText);
                }
                else
                {
#if _iMMOEMOTES
                    UCE_NpcEmotesAndAnimations(extendedText);
#endif
                    return extendedText.text;
                }
            }
        }

        if (welcomeTexts.Count > 1)
        {
            System.Random rnd = new System.Random();
            int r = rnd.Next(welcomeTexts.Count);
#if _iMMOEMOTES
            UCE_NpcEmotesAndAnimations(welcomeTexts[r]);
#endif
            return welcomeTexts[r].text;
        }
        else if (welcomeTexts.Count == 1)
        {
#if _iMMOEMOTES
            UCE_NpcEmotesAndAnimations(welcomeTexts[0]);
#endif
            return welcomeTexts[0].text;
        }

        return _welcome;
    }

#if _iMMOEMOTES

    // -----------------------------------------------------------------------------------
    // UCE_NpcEmotesAndAnimations
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public void UCE_NpcEmotesAndAnimations(UCE_NpcExtendedText extendedText)
    {
        UCE_NpcEmotes e = GetComponent<UCE_NpcEmotes>();

        if (e == null) return;

        if (extendedText.emotesId > -1)
            e.ShowEmote(extendedText.emotesId);
        /*
		else if (extendedText.animationId > -1)
			e.ShowAnimation(extendedText.animationId);
		*/
    }

#endif

    // -----------------------------------------------------------------------------------
}
