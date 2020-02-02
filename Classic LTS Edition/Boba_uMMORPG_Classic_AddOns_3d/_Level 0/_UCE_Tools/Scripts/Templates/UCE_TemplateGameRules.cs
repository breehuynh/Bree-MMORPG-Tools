// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using UnityEngine;
using System.Linq;

// TemplateGameRules

[CreateAssetMenu(menuName = "UCE Other/UCE GameRules", fileName = "UCE_GameRules", order = 999)]
public partial class UCE_TemplateGameRules : ScriptableObject
{

#if _iMMOATTRIBUTES
    [Header("Damage Formula")]
    [Tooltip("[Optional] All damage dealt can vary randomly (0.25 = +/- 25%) (0 to disable)")]
    [Range(0, 1)] public float randomDamageDeviation = 0.25f;

    [Tooltip("[Optional] Check to use new relational damage formula, uncheck to use old (attack-defense=damage) formula.")]
    public bool relationalDamage = true;
#endif

    static UCE_TemplateGameRules _instance;

    // -----------------------------------------------------------------------------------
    // singleton
    // -----------------------------------------------------------------------------------
    public static UCE_TemplateGameRules singleton
    {
        get
        {
            if (!_instance)
                _instance = Resources.FindObjectsOfTypeAll<UCE_TemplateGameRules>().FirstOrDefault();
            return _instance;
        }
    }

    // -----------------------------------------------------------------------------------
}
