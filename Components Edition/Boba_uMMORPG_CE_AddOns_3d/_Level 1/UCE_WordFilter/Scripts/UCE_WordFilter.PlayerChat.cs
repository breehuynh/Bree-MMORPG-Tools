// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

//

public partial class PlayerChat
{
    [Header("-=-=- UCE WORD FILTER -=-=-")]
    public UCE_Tmpl_WordFilter wordFilter;

    // -----------------------------------------------------------------------------------
    // UCE_IsAllowedChatText
    // -----------------------------------------------------------------------------------
    public bool UCE_IsAllowedChatText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        if (!wordFilter || wordFilter.badwords.Length == 0) return true;

        return UCE_WordFilter(text.ToLower());
    }

    // -----------------------------------------------------------------------------------
    // UCE_WordFilter
    // -----------------------------------------------------------------------------------
    public bool UCE_WordFilter(string text)
    {
        foreach (string badword in wordFilter.badwords)
        {
            if (text.Contains(badword.ToLower()))
                return false;
        }

        return true;
    }

    // -----------------------------------------------------------------------------------
}
