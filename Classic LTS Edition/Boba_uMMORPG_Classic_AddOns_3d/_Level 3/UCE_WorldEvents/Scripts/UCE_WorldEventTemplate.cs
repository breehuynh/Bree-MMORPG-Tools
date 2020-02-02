// =======================================================================================
// Maintained by bobatea#9400 on Discord
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

// UCE WORLD EVENT

[CreateAssetMenu(fileName = "New UCE WorldEvent", menuName = "UCE Templates/New UCE WorldEvent", order = 999)]
public partial class UCE_WorldEventTemplate : ScriptableObject
{
    [Header("[EVENT THRESHOLDS (checked top to bottom)]")]
    public UCE_WorldEventData[] thresholdData;

    // -----------------------------------------------------------------------------------
    // Caching
    // -----------------------------------------------------------------------------------
    private static Dictionary<int, UCE_WorldEventTemplate> _cache;

    public static Dictionary<int, UCE_WorldEventTemplate> dict
    {
        get
        {
            if (_cache == null)
            {
                UCE_ScripableObjectEntry entry = UCE_TemplateConfiguration.singleton.GetEntry(typeof(UCE_WorldEventTemplate));
                string folderName = entry != null ? entry.folderName : "";
#if _iMMOASSETBUNDLEMANAGER
                if (entry != null && entry.loadFromAssetBundle)
                    _cache = AssetBundleMagic.LoadBundle(entry.bundleName).LoadAllAssets<UCE_WorldEventTemplate>().ToDictionary(x => x.name.GetStableHashCode(), x => x);
                else
                    _cache = Resources.LoadAll<UCE_WorldEventTemplate>(folderName).ToDictionary(x => x.name.GetStableHashCode(), x => x);
#else
                _cache = Resources.LoadAll<UCE_WorldEventTemplate>(UCE_TemplateConfiguration.singleton.GetTemplatePath(typeof(UCE_WorldEventTemplate))).ToDictionary(x => x.name.GetStableHashCode(), x => x);
#endif
            }

            return _cache;

        }
    }

    // -----------------------------------------------------------------------------------
}
