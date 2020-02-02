// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if _iMMOASSETBUNDLEMANAGER
using Jacovone.AssetBundleMagic;
#endif

// UCE TRAIT TEMPLATE

[CreateAssetMenu(fileName = "New UCE Trait", menuName = "UCE Templates/New UCE Trait", order = 999)]
public class UCE_TraitTemplate : ScriptableObject
{
    [Header("[-=-=- UCE TRAIT -=-=-]")]
    public Sprite image;

    [TextArea(1, 30)]
    public string toolTip;

    public int traitCost;

    [Tooltip("A player can feature only one trait of each unique group id (0 = none)")]
    public int uniqueGroup;

    public GameObject[] allowedClasses;

    [Header("-=-=- UCE SKILL MODIFIERS (must be in prefabs skill list!) -=-=-")]
    public UCE_SkillRequirement[] startingSkills;

    [Header("-=-=- UCE STARTING ITEMS -=-=-")]
    public UCE_ItemModifier[] startingItems;

#if _iMMOPRESTIGECLASSES

    [Header("-=-=- UCE STARTING PRESTIGE CLASS -=-=-")]
    public UCE_PrestigeClassTemplate startingPrestigeClass;

#endif
#if _iMMOHONORSHOP

    [Header("-=-=- UCE STARING HONOR CURRENCY -=-=-")]
    public UCE_HonorShopCurrencyCost[] startingHonorCurrency;

#endif
#if _iMMOFACTIONS

    [Header("-=-=- UCE STARTING FACTION MODIFIER -=-=-")]
    public UCE_FactionRating[] startingFactions;

#endif
#if _iMMOCRAFTING

    [Header("-=-=- UCE CRAFT PROFESSION -=-=-")]
    public UCE_CraftingProfessionRequirement[] startingCraftingProfession;

#endif
#if _iMMOHARVESTING

    [Header("-=-=- UCE HARVEST PROFESSION -=-=-")]
    public UCE_HarvestingProfessionRequirement[] startingHarvestingProfession;

#endif
#if _iMMOPVP

    [Header("-=-=- UCE PVP REALM CHANGE -=-=-")]
    public UCE_Tmpl_Realm changeRealm;

    public UCE_Tmpl_Realm changeAlliedRealm;
#endif

    public UCE_StatModifier statModifiers;

    // -----------------------------------------------------------------------------------
    // Caching
    // -----------------------------------------------------------------------------------
    private static Dictionary<int, UCE_TraitTemplate> _cache;

    public static Dictionary<int, UCE_TraitTemplate> dict
    {
        get
        {
            if (_cache == null)
            {
                UCE_ScripableObjectEntry entry = UCE_TemplateConfiguration.singleton.GetEntry(typeof(UCE_TraitTemplate));
                string folderName = entry != null ? entry.folderName : "";
#if _iMMOASSETBUNDLEMANAGER
                if (entry != null && entry.loadFromAssetBundle)
                    _cache = AssetBundleMagic.LoadBundle(entry.bundleName).LoadAllAssets<UCE_TraitTemplate>().ToDictionary(x => x.name.GetStableHashCode(), x => x);
                else
                    _cache = Resources.LoadAll<UCE_TraitTemplate>(folderName).ToDictionary(x => x.name.GetStableHashCode(), x => x);
#else
                _cache = Resources.LoadAll<UCE_TraitTemplate>(UCE_TemplateConfiguration.singleton.GetTemplatePath(typeof(UCE_TraitTemplate))).ToDictionary(x => x.name.GetStableHashCode(), x => x);
#endif
            }

            return _cache;

        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public string ToolTip()
    {
        // we use a StringBuilder so that addons can modify tooltips later too
        // ('string' itself can't be passed as a mutable object)
        StringBuilder tip = new StringBuilder(toolTip);
        tip.Replace("{NAME}", name);

        /*tip.Replace("{PERCENTHEALTH}", percentHealth.ToString("0.0"));
        tip.Replace("{FLATHEALTH}", flatHealth.ToString());
        tip.Replace("{PERCENTMANA}", percentMana.ToString("0.0"));
        tip.Replace("{FLATMANA}", flatMana.ToString());
        tip.Replace("{PERCENTDAMAGE}", percentDamage.ToString("0.0"));
        tip.Replace("{FLATDAMAGE}", flatDamage.ToString());
        tip.Replace("{PERCENTDEFENSE}", percentDefense.ToString("0.0"));
        tip.Replace("{FLATDEFENSE}", flatDefense.ToString());
        tip.Replace("{PERCENTBLOCK}", percentBlock.ToString("0.0"));
        tip.Replace("{PERCENTCRITICAL}", percentCritical.ToString("0.0"));*/

        return tip.ToString();
    }

    // -----------------------------------------------------------------------------------
}
