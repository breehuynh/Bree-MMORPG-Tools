// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

// ===================================================================================
// UCE QUEST
// ===================================================================================
[Serializable]
public partial struct UCE_Quest
{
    public int hash;
    public bool completed;
    public bool completedAgain;
    public string lastCompleted;
    public int[] pvpedTarget;
    public int[] killedTarget;
    public int[] gatheredTarget;
    public int[] harvestedTarget;
    public int[] visitedTarget;
    public int[] craftedTarget;
    public int[] lootedTarget;
    public int visitedCount;
    public int counter;

    public string _tooltipCache;
    public float _tooltipTimer;

    // -------------------------------------------------------------------------------
    // UCE_Quest (Constructor)
    // -------------------------------------------------------------------------------
    public UCE_Quest(UCE_ScriptableQuest data)
    {
        hash = data.name.GetStableHashCode();
        completed = false;
        completedAgain = false;
        pvpedTarget = new int[10];
        killedTarget = new int[10];
        gatheredTarget = new int[10];
        harvestedTarget = new int[10];
        visitedTarget = new int[10];
        craftedTarget = new int[10];
        lootedTarget = new int[10];
        visitedCount = 0;
        lastCompleted = "";
        counter = 0;
        _tooltipCache = "";
        _tooltipTimer = 0;
    }

    // ============================== GETTER WRAPPERS ====================================

    public bool checkRequirements(Player player)
    {
        return data.questRequirements.checkRequirements(player);
    }

    public UCE_Requirements questRequirements { get { return data.questRequirements; } }
    public UCE_ScriptableQuest data { get { return UCE_ScriptableQuest.dict[hash]; } }
    public string name { get { return data.name; } }
    public bool showRewards { get { return data.showRewards; } }
    public UCE_rewardItem[] acceptItems { get { return data.acceptItems; } }
    public bool removeAtCompletion { get { return data.removeAtCompletion; } }
    public UCE_QuestReward[] questRewards { get { return data.questRewards; } }
#if _iMMOWORLDEVENTS
    public UCE_WorldEventTemplate worldEvent { get { return data.worldEvent; } }
    public int worldEventModifier { get { return data.worldEventModifier; } }
#endif
    public Npc[] visitTarget { get { return data.visitTarget; } }
#if _iMMOPVP
    public UCE_pvpTarget[] pvpTarget { get { return data.pvpTarget; } }
#endif
    public UCE_killTarget[] killTarget { get { return data.killTarget; } }
    public UCE_gatherTarget[] gatherTarget { get { return data.gatherTarget; } }
    public bool DontDestroyGathered { get { return data.DontDestroyGathered; } }
    public bool autoCompleteQuest { get { return data.autoCompleteQuest; } }
    public int repeatable { get { return data.repeatable; } }

    public string headerPvpTarget { get { return data.headerPvpTarget; } }
    public string headerKillTarget { get { return data.headerKillTarget; } }
    public string headerGatherTarget { get { return data.headerGatherTarget; } }
    public string headerVisitTarget { get { return data.headerVisitTarget; } }
    public string headerExploreTarget { get { return data.headerExploreTarget; } }
    public string headerHarvestTarget { get { return data.headerHarvestTarget; } }
    public string headerCraftTarget { get { return data.headerCraftTarget; } }
    public string headerLootTarget { get { return data.headerLootTarget; } }
    public string headerFactionTarget { get { return data.headerFactionTarget; } }

#if _iMMOPVP
    public string pvpKill { get { return data.pvpDescription.pvpKill; } }
    public string pvpPlayer { get { return data.pvpDescription.pvpPlayer; } }
    public string pvpLevel { get { return data.pvpDescription.pvpLevel; } }
    public string pvpParty { get { return data.pvpDescription.pvpParty; } }
    public string pvpGuild { get { return data.pvpDescription.pvpGuild; } }
    public string pvpRealm { get { return data.pvpDescription.pvpRealm; } }
    public string pvpNot { get { return data.pvpDescription.pvpNot; } }
    public string pvpMy { get { return data.pvpDescription.pvpMy; } }
#endif

#if _iMMOFACTIONS
    public UCE_FactionModifier[] factionModifiers { get { return data.factionModifiers; } }
#endif
#if _iMMOEXPLORATION
    public UCE_Area_Exploration[] exploreTarget { get { return data.exploreTarget; } }
#endif
#if _iMMOHARVESTING
    public UCE_harvestTarget[] harvestTarget { get { return data.harvestTarget; } }
#endif
#if _iMMOCRAFTING
    public UCE_craftTarget[] craftTarget { get { return data.craftTarget; } }
#endif
#if _iMMOFACTIONS
    public UCE_FactionQuest factionRequirement { get { return data.factionRequirement; } }
#endif
#if _iMMOCHEST
    public UCE_lootTarget[] lootTarget { get { return data.lootTarget; } }
#endif

    // ============================ TOOLTIP FUNCTIONS ====================================

    // -----------------------------------------------------------------------------------
    // ToolTip
    // -----------------------------------------------------------------------------------
    public string ToolTip(int[] gathered, int explored = 0, bool factionReqMet = false, int level = 0)
    {
        // -- check cache
        if (Time.time < _tooltipTimer)
            return _tooltipCache;

        // -- update gather targets
        gatheredTarget = gathered;

        string tmpString = "";
        StringBuilder tip = new StringBuilder(data.toolTip);

        tip.Replace("{NAME}", name);
        tip.Replace("{PVPTARGET}", TooltipSnippet_PvpTarget(level));
        tip.Replace("{KILLTARGET}", TooltipSnippet_KillTarget());
        tip.Replace("{GATHERTARGET}", TooltipSnippet_GatherTarget());
        tip.Replace("{VISITTARGET}", TooltipSnippet_VisitTarget());
        tip.Replace("{EXPLORETARGET}", TooltipSnippet_ExploreTarget(explored));
        tip.Replace("{HARVESTTARGET}", TooltipSnippet_HarvestTarget());
        tip.Replace("{CRAFTTARGET}", TooltipSnippet_CraftTarget());
        tip.Replace("{LOOTTARGET}", TooltipSnippet_LootTarget());
        tip.Replace("{FACTIONTARGET}", TooltipSnippet_FactionTarget());

        if (showRewards && questRewards != null && questRewards.Length > 0)
        {
            tmpString = "";
            foreach (UCE_rewardItem reward in questRewards[0].rewardItem)
            {
                tmpString += "* " + reward.item.name + " x" + reward.amount.ToString() + "\n";
            }
            tip.Replace("{REWARDITEMS}", tmpString);
            tip.Replace("{REWARDGOLD}", questRewards[0].rewardGold.ToString());
            tip.Replace("{REWARDCOINS}", questRewards[0].rewardCoins.ToString());
            tip.Replace("{REWARDEXPERIENCE}", questRewards[0].rewardExperience.ToString());
        }

        tmpString = "";
        if (repeatable > 0)
        {
            int hours = Mathf.Max(0, Convert.ToInt32(getLastCompleted() - repeatable));
            tmpString = "Completed: " + hours.ToString() + "/" + repeatable.ToString() + " hours ago\n";
        }
        tip.Replace("{REPEATABLE}", tmpString);

        // -- update cache
        _tooltipCache = tip.ToString();
        _tooltipTimer = Time.time + 1.0f;

        return _tooltipCache;
    }

    // -----------------------------------------------------------------------------------
    // ToolTip
    // -----------------------------------------------------------------------------------
    public string TrackerTip(int[] gathered, int explored = 0, bool factionReqMet = false, int level = 0)
    {
        // -- update gather targets
        gatheredTarget = gathered;

        string tmpString = "";

        tmpString += TooltipSnippet_PvpTarget(level);
        tmpString += TooltipSnippet_KillTarget();
        tmpString += TooltipSnippet_GatherTarget();
        tmpString += TooltipSnippet_VisitTarget();
        tmpString += TooltipSnippet_ExploreTarget(explored);
        tmpString += TooltipSnippet_HarvestTarget();
        tmpString += TooltipSnippet_CraftTarget();
        tmpString += TooltipSnippet_LootTarget();
        tmpString += TooltipSnippet_FactionTarget();

        return tmpString;
    }

    // -------------------------------------------------------------------------------
    // TooltipSnippet_PvpTarget
    // -------------------------------------------------------------------------------
    [DevExtMethods("TooltipSnippet")]
    private string TooltipSnippet_PvpTarget(int level)
    {
        string tmpString = "";
#if _iMMOPVP
        if (pvpTarget.Length > 0)
        {
            tmpString = headerPvpTarget + getPvped().ToString() + "/" + getTotalPvped().ToString() + "\n";

            for (int j = 0; j < pvpTarget.Length; ++j)
            {
                int minLevel = Mathf.Max(1, level - pvpTarget[j].levelRange);
                int maxLevel = level + pvpTarget[j].levelRange;

                tmpString += " - " + pvpKill + pvpTarget[j].amount + pvpPlayer + " ";
                tmpString += "   " + pvpLevel + minLevel.ToString() + "-" + maxLevel.ToString() + " ";

                if (pvpTarget[j].type == UCE_pvpTarget.pvpType.OtherGuild ||
                    pvpTarget[j].type == UCE_pvpTarget.pvpType.OtherParty ||
                    pvpTarget[j].type == UCE_pvpTarget.pvpType.OtherRealm)
                {
                    tmpString += "   " + pvpNot;
                }

                if (pvpTarget[j].type == UCE_pvpTarget.pvpType.MyParty ||
                    pvpTarget[j].type == UCE_pvpTarget.pvpType.MyGuild ||
                    pvpTarget[j].type == UCE_pvpTarget.pvpType.MyRealm)
                {
                    tmpString += "   " + pvpMy;
                }

                if (pvpTarget[j].type == UCE_pvpTarget.pvpType.MyParty ||
                    pvpTarget[j].type == UCE_pvpTarget.pvpType.OtherParty)
                {
                    tmpString += pvpParty;
                }

                if (pvpTarget[j].type == UCE_pvpTarget.pvpType.MyGuild ||
                    pvpTarget[j].type == UCE_pvpTarget.pvpType.OtherGuild)
                {
                    tmpString += pvpGuild;
                }

                if (pvpTarget[j].type == UCE_pvpTarget.pvpType.MyRealm ||
                    pvpTarget[j].type == UCE_pvpTarget.pvpType.OtherRealm)
                {
                    tmpString += pvpRealm;
                }

                tmpString += "\n";
            }
        }
#endif
        return tmpString;
    }

    // -------------------------------------------------------------------------------
    // TooltipSnippet_KillTarget
    // -------------------------------------------------------------------------------
    [DevExtMethods("TooltipSnippet")]
    private string TooltipSnippet_KillTarget()
    {
        string tmpString = "";
        if (killTarget.Length > 0)
        {
            tmpString = headerKillTarget + getKilled().ToString() + "/" + getTotalKilled().ToString() + "\n";
            for (int j = 0; j < killTarget.Length; ++j)
            {
                tmpString += " - " + killTarget[j].target.name + " " + killedTarget[j] + "/" + killTarget[j].amount.ToString() + "\n";
            }
        }
        return tmpString;
    }

    // -------------------------------------------------------------------------------
    // TooltipSnippet_GatherTarget
    // -------------------------------------------------------------------------------
    [DevExtMethods("TooltipSnippet")]
    private string TooltipSnippet_GatherTarget()
    {
        string tmpString = "";
        if (gatherTarget.Length > 0)
        {
            tmpString = headerGatherTarget + getGathered().ToString() + "/" + getTotalGathered().ToString() + "\n";
            for (int j = 0; j < gatherTarget.Length; ++j)
            {
                tmpString += " - " + gatherTarget[j].target.name + " " + gatheredTarget[j] + "/" + gatherTarget[j].amount.ToString() + "\n";
            }
        }
        return tmpString;
    }

    // -------------------------------------------------------------------------------
    // TooltipSnippet_VisitTarget
    // -------------------------------------------------------------------------------
    [DevExtMethods("TooltipSnippet")]
    private string TooltipSnippet_VisitTarget()
    {
        string tmpString = "";
        if (visitTarget.Length > 0)
        {
            tmpString = headerVisitTarget + visitedCount.ToString() + "/" + visitTarget.Length + "\n";
            foreach (Npc npc in visitTarget)
            {
                string done = visitedTarget.Any(x => x == npc.name.GetStableHashCode()) ? " [ok]" : " [x]";
                tmpString += " - " + npc.name + done + "\n";
            }
        }
        return tmpString;
    }

    // -------------------------------------------------------------------------------
    // TooltipSnippet_ExploreTarget
    // -------------------------------------------------------------------------------
    [DevExtMethods("TooltipSnippet")]
    private string TooltipSnippet_ExploreTarget(int explored = 0)
    {
        string tmpString = "";
#if _iMMOEXPLORATION
        if (exploreTarget.Length > 0)
        {
            explored = Mathf.Min(explored, exploreTarget.Length);
            tmpString = headerExploreTarget + explored.ToString() + "/" + exploreTarget.Length + "\n";
            foreach (UCE_Area_Exploration area in exploreTarget)
            {
                tmpString += " - " + area.name + "\n";
            }
        }
#endif
        return tmpString;
    }

    // -------------------------------------------------------------------------------
    // TooltipSnippet_HarvestTarget
    // -------------------------------------------------------------------------------
    [DevExtMethods("TooltipSnippet")]
    private string TooltipSnippet_HarvestTarget()
    {
        string tmpString = "";
#if _iMMOHARVESTING
        if (harvestTarget.Length > 0)
        {
            tmpString = headerHarvestTarget + getHarvested().ToString() + "/" + getTotalHarvested().ToString() + "\n";
            for (int j = 0; j < harvestTarget.Length; ++j)
            {
                tmpString += " - " + harvestTarget[j].target.name + " " + harvestedTarget[j] + "/" + harvestTarget[j].amount.ToString() + "\n";
            }
        }
#endif
        return tmpString;
    }

    // -------------------------------------------------------------------------------
    // TooltipSnippet_CraftTarget
    // -------------------------------------------------------------------------------
    [DevExtMethods("TooltipSnippet")]
    private string TooltipSnippet_CraftTarget()
    {
        string tmpString = "";
#if _iMMOCRAFTING
        if (craftTarget.Length > 0)
        {
            tmpString = headerCraftTarget + getCrafted().ToString() + "/" + getTotalCrafted().ToString() + "\n";
            for (int j = 0; j < craftTarget.Length; ++j)
            {
                tmpString += " - " + craftTarget[j].target.name + " " + craftedTarget[j] + "/" + craftTarget[j].amount.ToString() + "\n";
            }
        }
#endif
        return tmpString;
    }

    // -------------------------------------------------------------------------------
    // TooltipSnippet_LootTarget
    // -------------------------------------------------------------------------------
    [DevExtMethods("TooltipSnippet")]
    private string TooltipSnippet_LootTarget()
    {
        string tmpString = "";
#if _iMMOCHEST
        if (lootTarget.Length > 0)
        {
            tmpString = headerLootTarget + getLooted().ToString() + "/" + getTotalLooted().ToString() + "\n";
            for (int j = 0; j < lootTarget.Length; ++j)
            {
                tmpString += " - " + lootTarget[j].target.name + " " + lootedTarget[j] + "/" + lootTarget[j].amount.ToString() + "\n";
            }
        }
#endif
        return tmpString;
    }

    // -------------------------------------------------------------------------------
    // TooltipSnippet_FactionTarget
    // -------------------------------------------------------------------------------
    [DevExtMethods("TooltipSnippet")]
    private string TooltipSnippet_FactionTarget()
    {
        string tmpString = "";
#if _iMMOFACTIONS
        if (factionRequirement.faction != null)
        {
            tmpString = headerFactionTarget + "\n";
            tmpString += " - " + factionRequirement.faction.name + " [" + factionRequirement.faction.getRank(factionRequirement.min) + "]\n";
        }
#endif
        return tmpString;
    }

    // =========================== FULFILLMENT CHECKS ====================================

    // -------------------------------------------------------------------------------
    // IsFulfilled
    // -------------------------------------------------------------------------------
    public bool IsFulfilled(int[] gathered, int explored, bool factionReqMet)
    {
        gatheredTarget = new int[10];
        gathered.CopyTo(gatheredTarget, 0);

        return
        factionReqMet &&
        IsPvpedFulfilled() &&
        IsKilledFulfilled() &&
        IsHarvestedFulfilled() &&
        IsCraftedFulfilled() &&
        IsLootedFulfilled() &&
        IsGatheredFulfilled() &&
        IsExploredFulfilled(explored) &&
        visitedCount >= visitTarget.Length;
    }

    // -------------------------------------------------------------------------------
    // IsExploredFulfilled
    // -------------------------------------------------------------------------------
    private bool IsExploredFulfilled(int explored)
    {
        bool valid = true;
#if _iMMOEXPLORATION
        int exploredCount = exploreTarget.Length;
        valid = explored >= exploredCount;
#endif
        return valid;
    }

    // -------------------------------------------------------------------------------
    // IsPvpedFulfilled
    // -------------------------------------------------------------------------------
    private bool IsPvpedFulfilled()
    {
        bool valid = true;
#if _iMMOPVP
        for (int j = 0; j < pvpTarget.Length; ++j)
        {
            if (pvpedTarget[j] < pvpTarget[j].amount)
            {
                valid = false;
                break;
            }
        }
#endif
        return valid;
    }

    // -------------------------------------------------------------------------------
    // IsKilledFulfilled
    // -------------------------------------------------------------------------------
    private bool IsKilledFulfilled()
    {
        bool valid = true;
        for (int j = 0; j < killTarget.Length; ++j)
        {
            if (killedTarget[j] < killTarget[j].amount)
            {
                valid = false;
                break;
            }
        }
        return valid;
    }

    // -------------------------------------------------------------------------------
    // IsHarvestedFulfilled
    // -------------------------------------------------------------------------------
    private bool IsHarvestedFulfilled()
    {
        bool valid = true;
#if _iMMOHARVESTING
        for (int j = 0; j < harvestTarget.Length; ++j)
        {
            if (harvestedTarget[j] < harvestTarget[j].amount)
            {
                valid = false;
                break;
            }
        }
#endif
        return valid;
    }

    // -------------------------------------------------------------------------------
    // IsCraftedFulfilled
    // -------------------------------------------------------------------------------
    private bool IsCraftedFulfilled()
    {
        bool valid = true;
#if _iMMOCRAFTING
        for (int j = 0; j < craftTarget.Length; ++j)
        {
            if (craftedTarget[j] < craftTarget[j].amount)
            {
                valid = false;
                break;
            }
        }
#endif
        return valid;
    }

    // -------------------------------------------------------------------------------
    // IsLootedFulfilled
    // -------------------------------------------------------------------------------
    private bool IsLootedFulfilled()
    {
        bool valid = true;
#if _iMMOCHEST
        for (int j = 0; j < lootTarget.Length; ++j)
        {
            if (lootedTarget[j] < lootTarget[j].amount)
            {
                valid = false;
                break;
            }
        }
#endif
        return valid;
    }

    // -------------------------------------------------------------------------------
    // IsGatheredFulfilled
    // -------------------------------------------------------------------------------
    private bool IsGatheredFulfilled()
    {
        bool valid = true;

        for (int j = 0; j < gatherTarget.Length; ++j)
        {
            if (gatheredTarget[j] < gatherTarget[j].amount)
            {
                valid = false;
                break;
            }
        }

        return valid;
    }

    // -------------------------------------------------------------------------------
    // getPvped
    // -------------------------------------------------------------------------------
    private int getPvped()
    {
        return pvpedTarget.Sum();
    }

    // -------------------------------------------------------------------------------
    // getTotalPvped
    // -------------------------------------------------------------------------------
    private int getTotalPvped()
    {
#if _iMMOPVP
        return pvpTarget.Sum(x => x.amount);
#else
		return 0;
#endif
    }

    // -------------------------------------------------------------------------------
    // getKilled
    // -------------------------------------------------------------------------------
    private int getKilled()
    {
        return killedTarget.Sum();
    }

    // -------------------------------------------------------------------------------
    // getTotalKilled
    // -------------------------------------------------------------------------------
    private int getTotalKilled()
    {
        return killTarget.Sum(x => x.amount);
    }

    // -------------------------------------------------------------------------------
    // getGathered
    // -------------------------------------------------------------------------------
    private int getGathered()
    {
        return gatheredTarget.Sum();
    }

    // -------------------------------------------------------------------------------
    // getHarvested
    // -------------------------------------------------------------------------------
    private int getHarvested()
    {
        return harvestedTarget.Sum();
    }

    // -------------------------------------------------------------------------------
    // getCrafted
    // -------------------------------------------------------------------------------
    private int getCrafted()
    {
        return craftedTarget.Sum();
    }

    // -------------------------------------------------------------------------------
    // getLooted
    // -------------------------------------------------------------------------------
    private int getLooted()
    {
        return lootedTarget.Sum();
    }

    // -------------------------------------------------------------------------------
    // getTotalHarvested
    // -------------------------------------------------------------------------------
    private int getTotalHarvested()
    {
#if _iMMOHARVESTING
        return harvestTarget.Sum(x => x.amount);
#else
		return 0;
#endif
    }

    // -------------------------------------------------------------------------------
    // getTotalCrafted
    // -------------------------------------------------------------------------------
    private int getTotalCrafted()
    {
#if _iMMOCRAFTING
        return craftTarget.Sum(x => x.amount);
#else
		return 0;
#endif
    }

    // -------------------------------------------------------------------------------
    // getTotalLooted
    // -------------------------------------------------------------------------------
    private int getTotalLooted()
    {
#if _iMMOCHEST
        return lootTarget.Sum(x => x.amount);
#else
		return 0;
#endif
    }

    // -------------------------------------------------------------------------------
    // getTotalGathered
    // -------------------------------------------------------------------------------
    private int getTotalGathered()
    {
        return gatherTarget.Sum(x => x.amount);
    }

    // -------------------------------------------------------------------------------
    // getLastCompleted
    // -------------------------------------------------------------------------------
    public double getLastCompleted()
    {
        if (!string.IsNullOrWhiteSpace(lastCompleted))
        {
            DateTime time = DateTime.Parse(lastCompleted);
            return (DateTime.UtcNow - time).TotalSeconds / 3600;
        }
        return 0;
    }

    // -------------------------------------------------------------------------------
    // resetQuest
    // -------------------------------------------------------------------------------
    public void resetQuest()
    {
        pvpedTarget = new int[10];
        killedTarget = new int[10];
        gatheredTarget = new int[10];
        harvestedTarget = new int[10];
        visitedTarget = new int[10];
        craftedTarget = new int[10];
        lootedTarget = new int[10];
        visitedCount = 0;
    }

    // -------------------------------------------------------------------------------
}

public class SyncListUCE_Quest : SyncList<UCE_Quest> { }
