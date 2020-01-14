// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if _iMMOASSETBUNDLEMANAGER
using Jacovone.AssetBundleMagic;
#endif

// PRESTIGE CLASS - TEMPLATE

[CreateAssetMenu(fileName = "UCE_Tmpl_Faction", menuName = "UCE Templates/New UCE Faction", order = 998)]
public class UCE_Tmpl_Faction : ScriptableObject
{
    [Header("-=-=-=- UCE FACTION -=-=-=-")]
    public Sprite image;

    [TextArea(1, 10)] public string description;

    public UCE_FactionRank[] ranks;
#if _iMMOPVP

    [Tooltip("Monsters set to 'aggressive' will only attack a player when their faction ranking falls below this threshold.")]
    public float aggressiveThreshold;

#endif

    // -----------------------------------------------------------------------------------
    // getRank
    // -----------------------------------------------------------------------------------
    public string getRank(int rating)
    {
        foreach (UCE_FactionRank rank in ranks)
            if (rank.min <= rating && rank.max >= rating) return rank.name;

        return "???";
    }

    // -----------------------------------------------------------------------------------
    // getRank
    // -----------------------------------------------------------------------------------
    public string getRank(int min, int max)
    {
        foreach (UCE_FactionRank rank in ranks)
            if (min >= rank.min && max <= rank.max) return rank.name;

        return "???";
    }

    // -----------------------------------------------------------------------------------
    // checkAggressive
    // -----------------------------------------------------------------------------------
    public bool checkAggressive(int rating)
    {
#if _iMMOPVP
        return (rating <= aggressiveThreshold);
#else
		return false;
#endif
    }

    // -----------------------------------------------------------------------------------
    // Caching
    // -----------------------------------------------------------------------------------
    private static Dictionary<int, UCE_Tmpl_Faction> _cache;

    public static Dictionary<int, UCE_Tmpl_Faction> dict
    {
        get
        {
            if (_cache == null)
            {
                UCE_ScripableObjectEntry entry = UCE_TemplateConfiguration.singleton.GetEntry(typeof(UCE_Tmpl_Faction));
                string folderName = entry != null ? entry.folderName : "";
#if _iMMOASSETBUNDLEMANAGER
                if (entry != null && entry.loadFromAssetBundle)
                    _cache = AssetBundleMagic.LoadBundle(entry.bundleName).LoadAllAssets<UCE_Tmpl_Faction>().ToDictionary(x => x.name.GetDeterministicHashCode(), x => x);
                else
                    _cache = Resources.LoadAll<UCE_Tmpl_Faction>(folderName).ToDictionary(x => x.name.GetDeterministicHashCode(), x => x);
#else
                _cache = Resources.LoadAll<UCE_Tmpl_Faction>(UCE_TemplateConfiguration.singleton.GetTemplatePath(typeof(UCE_Tmpl_Faction))).ToDictionary(x => x.name.GetDeterministicHashCode(), x => x);
#endif
            }

            return _cache;

        }
    }

    // -----------------------------------------------------------------------------------

}
