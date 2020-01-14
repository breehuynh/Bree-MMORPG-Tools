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

// UCE SCRIPTABLE QUEST TEMPLATE

[CreateAssetMenu(fileName = "New UCE Quest", menuName = "UCE Templates/New UCE Quest", order = 999)]
public partial class UCE_ScriptableQuest : ScriptableObject
{
    [Header("-=-=-=- QUEST -=-=-=-")]
    [Tooltip("Enter hours between quest attempts, set to 0 to disable")]
    public int repeatable;

    [Tooltip("Show the first reward in tooltip (uncheck if you have more)?")]
    public bool showRewards;

    [TextArea(1, 10)] public string toolTip;

    [Header("-=-=-=- QUEST ACCESS REQUIREMENTS -=-=-=-")]
    public UCE_Requirements questRequirements;

    [Header("-=-=-=- QUEST ACCEPTANCE -=-=-=-")]
    [Tooltip("[Optional] Players will receive these items as soon as they accept the quest (removed when quest is cancelled).")]
    public UCE_rewardItem[] acceptItems;

    [Tooltip("[Optional] The items gained when accepting the quest will be removed as soon as the quest is complete.")]
    public bool removeAtCompletion;

    [Header("-=-=-=- QUEST REWARDS -=-=-=-")]
    public UCE_QuestReward[] questRewards;

#if _iMMOFACTIONS
    public UCE_FactionModifier[] factionModifiers;
#endif
#if _iMMOWORLDEVENTS
    public UCE_WorldEventTemplate worldEvent;
    [Range(-99999, 99999)] public int worldEventModifier;
#endif

    [Header("-=-=-=- QUEST FULFILLMENT -=-=-=-")]
#if _iMMOEXPLORATION
    [Tooltip("Add UCE Exploration Areas from Prefabs [Limit 10]")]
    public UCE_Area_Exploration[] exploreTarget;

#endif
#if _iMMOHARVESTING

    [Tooltip("Add UCE Harvest Nodes from Prefabs [Limit 10]")]
    public UCE_harvestTarget[] harvestTarget;

#endif
#if _iMMOCRAFTING

    [Tooltip("Add UCE Crafting Recipes from Resources [Limit 10]")]
    public UCE_craftTarget[] craftTarget;

#endif
#if _iMMOCHEST

    [Tooltip("Add UCE Lootcrate's from Prefabs [Limit 10]")]
    public UCE_lootTarget[] lootTarget;

#endif
#if _iMMOFACTIONS

    [Tooltip("Minimum required faction rating")]
    public UCE_FactionQuest factionRequirement;

#endif

    [Tooltip("Add Npc's from Prefabs [Limit 10]")]
    public Npc[] visitTarget;

#if _iMMOPVP

    [Tooltip("Add PVP Targets [Limit 10]")]
    public UCE_pvpTarget[] pvpTarget;

#endif

    [Tooltip("Add Monsters from Prefabs [Limit 10]")]
    public UCE_killTarget[] killTarget;

    [Tooltip("Add Items from Resources [Limit 10]")]
    public UCE_gatherTarget[] gatherTarget;

    [Tooltip("When checked, the gathered Items wont be removed on quest completion.")]
    public bool DontDestroyGathered;

    [Tooltip("When checked, the Quest will be automatically completed, without the need to return to the Questgiver.")]
    public bool autoCompleteQuest;

    [Header("-=-=-=- DESCRIPTION HEADERS -=-=-=-")]
    public string headerPvpTarget = "* Kill Players ";

    public string headerKillTarget = "* Kill Monsters ";
    public string headerGatherTarget = "* Gather Items ";
    public string headerVisitTarget = "* Visit Npc's ";
    public string headerExploreTarget = "* Explore Areas ";
    public string headerHarvestTarget = "* Harvest Resources ";
    public string headerCraftTarget = "* Craft Items ";
    public string headerLootTarget = "* Loot Crates ";
    public string headerFactionTarget = "* Faction Requirement ";

#if _iMMOPVP

    [Header("-=-=-=- PVP DESCRIPTIONs -=-=-=-")]
    public UCE_pvpDescription pvpDescription;

#endif

    // -----------------------------------------------------------------------------------
    // Caching
    // -----------------------------------------------------------------------------------
    private static Dictionary<int, UCE_ScriptableQuest> _cache;

    public static Dictionary<int, UCE_ScriptableQuest> dict
    {
        get
        {
            if (_cache == null)
            {
                UCE_ScripableObjectEntry entry = UCE_TemplateConfiguration.singleton.GetEntry(typeof(UCE_ScriptableQuest));
                string folderName = entry != null ? entry.folderName : "";
#if _iMMOASSETBUNDLEMANAGER
                if (entry != null && entry.loadFromAssetBundle)
                    _cache = AssetBundleMagic.LoadBundle(entry.bundleName).LoadAllAssets<UCE_ScriptableQuest>().ToDictionary(x => x.name.GetDeterministicHashCode(), x => x);
                else
                    _cache = Resources.LoadAll<UCE_ScriptableQuest>(folderName).ToDictionary(x => x.name.GetDeterministicHashCode(), x => x);
#else
                _cache = Resources.LoadAll<UCE_ScriptableQuest>(UCE_TemplateConfiguration.singleton.GetTemplatePath(typeof(UCE_ScriptableQuest))).ToDictionary(x => x.name.GetDeterministicHashCode(), x => x);
#endif
            }

            return _cache;

        }
    }

    // -----------------------------------------------------------------------------------

}
