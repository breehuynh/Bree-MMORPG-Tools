// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using UnityEngine;

// UCE_Sanctuary_HonorCurrency

#if _iMMOHONORSHOP

[Serializable]
public partial struct UCE_Sanctuary_HonorCurrency
{
    public UCE_Tmpl_HonorCurrency honorCurrency;

    [Tooltip("[Optional] Seconds spent offline to gain 1 Honor Currency unit")]
    public int SecondsPerUnit;

    [Tooltip("[Optional] Max. Honor Currency cap offline per session (set 0 to disable)")]
    public int MaxUnits;
}

#endif
