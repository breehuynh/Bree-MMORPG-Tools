// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// ===================================================================================
// SIMPLE DAILY REWARD
// ===================================================================================
[System.Serializable]
public class UCE_DailyReward
{
    [Header("-=-=- UCE Daily Reward -=-=-")]
    public long rewardGold;

    public long rewardCoins;
    public long rewardExperience;
    public long rewardSkillExperience;

    public UCE_ItemRequirement[] rewardItems;

#if _iMMOHONORSHOP
    public UCE_HonorShopCurrencyDrop[] honorCurrencies;
#endif
}

// ===================================================================================
