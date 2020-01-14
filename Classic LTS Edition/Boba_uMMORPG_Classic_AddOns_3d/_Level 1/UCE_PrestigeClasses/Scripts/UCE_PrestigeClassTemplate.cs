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

// PRESTIGE CLASS TEMPLATE

[CreateAssetMenu(fileName = "New UCE Prestige Class", menuName = "UCE Templates/New UCE Prestige Class", order = 999)]
public class UCE_PrestigeClassTemplate : ScriptableObject
{

    // -----------------------------------------------------------------------------------
    // Caching
    // -----------------------------------------------------------------------------------
    private static Dictionary<int, UCE_PrestigeClassTemplate> _cache;

    public static Dictionary<int, UCE_PrestigeClassTemplate> dict
    {
        get
        {
            if (_cache == null)
            {
                UCE_ScripableObjectEntry entry = UCE_TemplateConfiguration.singleton.GetEntry(typeof(UCE_PrestigeClassTemplate));
                string folderName = entry != null ? entry.folderName : "";
#if _iMMOASSETBUNDLEMANAGER
                if (entry != null && entry.loadFromAssetBundle)
                    _cache = AssetBundleMagic.LoadBundle(entry.bundleName).LoadAllAssets<UCE_PrestigeClassTemplate>().ToDictionary(x => x.name.GetDeterministicHashCode(), x => x);
                else
                    _cache = Resources.LoadAll<UCE_PrestigeClassTemplate>(folderName).ToDictionary(x => x.name.GetDeterministicHashCode(), x => x);
#else
                _cache = Resources.LoadAll<UCE_PrestigeClassTemplate>(UCE_TemplateConfiguration.singleton.GetTemplatePath(typeof(UCE_PrestigeClassTemplate))).ToDictionary(x => x.name.GetDeterministicHashCode(), x => x);
#endif
            }

            return _cache;

        }
    }

    // -----------------------------------------------------------------------------------

}
