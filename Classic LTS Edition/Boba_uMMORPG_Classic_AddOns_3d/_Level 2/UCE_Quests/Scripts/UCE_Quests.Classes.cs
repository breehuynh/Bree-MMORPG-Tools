// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// QUEST REWARD - CLASS

[System.Serializable]
public partial struct UCE_QuestReward
{
    [Header("-=-=-=- Requirements -=-=-=-")]
    [Tooltip("Ignore if the quest has only one reward. When there are more, one is chosen randomly. Does not work with class based rewards.")]
    [Range(0, 1)] public float rewardChance;

    [Tooltip("Reward only available to the added classes, or all if left empty. Overrides random reward Chance! [Assign a player prefab here]")]
    public GameObject[] availableToClass;

    [Header("-=-=-=- Rewards -=-=-=-")]
    public long rewardGold;

    public long rewardCoins;
    public long rewardExperience;
    public UCE_rewardItem[] rewardItem;
#if _iMMOTRAVEL
    public UCE_Unlockroute[] rewardUnlockroutes;
#endif
#if _iMMOHONORSHOP
    public UCE_HonorShopCurrencyCost[] honorCurrency;
#endif
#if _iMMOPVP
    public UCE_Tmpl_Realm changeRealm;
    public UCE_Tmpl_Realm changeAlliedRealm;
#endif
}

//

[System.Serializable]
public partial class UCE_pvpDescription
{
    public string pvpKill = "Kill ";
    public string pvpPlayer = " Player(s) ";
    public string pvpLevel = " level ";
    public string pvpParty = "Party.";
    public string pvpGuild = "Guild.";
    public string pvpRealm = "Realm.";
    public string pvpNot = " Not ";
    public string pvpMy = "in my ";
}

// PVP TARGET - CLASS

[System.Serializable]
public partial class UCE_pvpTarget
{
    public int levelRange;
    public int amount;

    [Tooltip("AnyGuild = Target must be in a guild!\nAnyParty = Target must be in a party!\nMyParty/OtherParty = Both must be in a party!\nMyGuild/OtherGuild = Both must be in a Guild!")]
    public pvpType type;

    public enum pvpType { Any, AnyGuild, MyGuild, OtherGuild, AnyParty, MyParty, OtherParty, MyRealm, OtherRealm }
}

// KILL TARGET - CLASS

[System.Serializable]
public partial class UCE_killTarget
{
    public Monster target;
    public int amount;
}

// GATHER TARGET - CLASS

[System.Serializable]
public partial class UCE_gatherTarget
{
    public ScriptableItem target;
    public int amount;
}

// HARVEST TARGET - CLASS

#if _iMMOHARVESTING

[System.Serializable]
public partial class UCE_harvestTarget
{
    public UCE_HarvestingProfessionTemplate target;
    public int amount;
}

#endif

// CRAFT TARGET - CLASS

#if _iMMOCRAFTING

[System.Serializable]
public partial class UCE_craftTarget
{
    public UCE_Tmpl_Recipe target;
    public int amount;
}

#endif

// LOOT TARGET - CLASS

#if _iMMOCHEST

[System.Serializable]
public partial class UCE_lootTarget
{
    public UCE_Lootcrate target;
    public int amount;
}

#endif

// REWARD ITEM - CLASS

[System.Serializable]
public partial class UCE_rewardItem
{
    public ScriptableItem item;
    public int amount;
}
