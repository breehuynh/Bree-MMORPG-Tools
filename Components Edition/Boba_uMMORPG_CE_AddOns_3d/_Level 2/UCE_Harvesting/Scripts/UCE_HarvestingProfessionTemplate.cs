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

#if _iMMOHARVESTING

// HARVESTING PROFESSION TEMPLATE

[CreateAssetMenu(fileName = "New UCE Harvesting Profession", menuName = "UCE Templates/New UCE Harvesting Profession", order = 999)]
public class UCE_HarvestingProfessionTemplate : ScriptableObject
{
    [Header("Harvesting Profession")]
    public int[] levels;

    public Sprite image;

    [Tooltip("[Required] Animator State used to play animation while harvesting (has to be present on Player Animation Controller)")]
    public string animatorState;

    [Tooltip("[Optional] Sound effect that is played, when the player starts harvesting.")]
    public AudioClip startPlayerSound;

    [Tooltip("[Optional] Sound effect that is played, when the player finishes harvesting.")]
    public AudioClip stopPlayerSound;

    [Tooltip("[Required] Base Harvesting Success Chance (0=0%, 0.5=50%, 1.0=100%")]
    [Range(0, 1)] public float baseHarvestChance = 1.0f;

    [Tooltip("[Required] Critical Harvesting Success Chance (0=0%, 0.5=50%, 1.0=100%")]
    [Range(0, 1)] public float criticalHarvestChance = 0.05f;

    [Tooltip("[Required] Bonus Harvesting Success Chance per level that exceeds the Resource Nodes level(0=0%, 0.5=50%, 1.0=100%")]
    [Range(0, 1)] public float bonusHarvestChance = 0.05f;

    [Header("[-=-=-=- Harvesting Requirements -=-=-=-]")]
    public UCE_Requirements harvestingRequirements;

    [Header("-=-=-=- Required Tools [See Tooltips] -=-=-=-")]
    [Tooltip("[Required] Are all Tools required or just any one of them?")]
    public bool requiresAllTools;

    [Tooltip("[Optional] Tool item required, must be equipped? Modifies Success Probability? Modifies Duration?")]
    public UCE_HarvestingTool[] tools;

    [Header("-=-=-=- Optional Tools [See Tooltips] -=-=-=-")]
    [Tooltip("[Optional] Optional tool item, must be equipped? Modifies Success Probability? Modifies Duration?")]
    public UCE_HarvestingTool[] optionalTools;

    [Header("-=-=-=- Modifiers [See Tooltips] -=-=-=-")]
    [Tooltip("[Optional] +/- to basic Success chance per harvesting profession level")]
    public float probabilityPerSkillLevel;

    [Tooltip("[Optional] +/- to craft duration per harvesting profession level")]
    public float durationPerSkillLevel;

    [Tooltip("[Optional] +/- to probability of generating a Critical Result per harvesting profession level")]
    public float criticalProbabilityPerSkillLevel;

    [Header("-=-=-=- Tooltip -=-=-=-")]
    [TextArea(1, 30)] public string toolTip;

    // -----------------------------------------------------------------------------------
    // ToolTip
    // -----------------------------------------------------------------------------------
    public string ToolTip()
    {
        StringBuilder tip = new StringBuilder();

        tip.Append(name + "\n");
        tip.Append(toolTip + "\n");
        tip.Append("\n");
        tip.Append("Basic Harvest Chance: " + (baseHarvestChance * 100).ToString() + "%\n");

        if (harvestingRequirements.requiredEquipment.Length > 0)
        {
            tip.Append("Required Equipment:\n");
            foreach (ScriptableItem equip in harvestingRequirements.requiredEquipment)
                tip.Append("- " + equip.name + "\n");
        }

        if (harvestingRequirements.requiredItems.Length > 0)
        {
            tip.Append("Required Item:\n");
            foreach (UCE_ItemRequirement item in harvestingRequirements.requiredItems)
                tip.Append("- " + item.amount.ToString() + "x " + item.item.name + "\n");
        }

        return tip.ToString();
    }

    // -----------------------------------------------------------------------------------
    // Caching
    // -----------------------------------------------------------------------------------
    private static Dictionary<int, UCE_HarvestingProfessionTemplate> _cache;

    public static Dictionary<int, UCE_HarvestingProfessionTemplate> dict
    {
        get
        {
            if (_cache == null)
            {
                UCE_ScripableObjectEntry entry = UCE_TemplateConfiguration.singleton.GetEntry(typeof(UCE_HarvestingProfessionTemplate));
                string folderName = entry != null ? entry.folderName : "";
#if _iMMOASSETBUNDLEMANAGER
                if (entry != null && entry.loadFromAssetBundle)
                    _cache = AssetBundleMagic.LoadBundle(entry.bundleName).LoadAllAssets<UCE_HarvestingProfessionTemplate>().ToDictionary(x => x.name.GetStableHashCode(), x => x);
                else
                    _cache = Resources.LoadAll<UCE_HarvestingProfessionTemplate>(folderName).ToDictionary(x => x.name.GetStableHashCode(), x => x);
#else
                _cache = Resources.LoadAll<UCE_HarvestingProfessionTemplate>(UCE_TemplateConfiguration.singleton.GetTemplatePath(typeof(UCE_HarvestingProfessionTemplate))).ToDictionary(x => x.name.GetStableHashCode(), x => x);
#endif
            }

            return _cache;

        }
    }

    // -----------------------------------------------------------------------------------

}

#endif
