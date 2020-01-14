// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using UnityEngine;

// PLAYER

public partial class Player
{
    [Header("[-=-=- UCE DAILY REWARDS -=-=-]")]
    public UCE_Tmpl_DailyRewards DailyRewards;

    public UCE_Tmpl_ReturnRewards ReturnRewards;

    [HideInInspector] public int dailyRewardCounter = -1;
    [HideInInspector] public double dailyRewardReset = 0;

    protected UCE_UI_DailyRewards _UCE_UI_DailyRewards;
    protected UCE_UI_ReturnRewards _UCE_UI_ReturnRewards;

    // -----------------------------------------------------------------------------------
    // Start_UCE_DailyRewards
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    [DevExtMethods("Start")]
    private void Start_UCE_DailyRewards()
    {
        if (DailyRewards != null)
        {
            bool gainedCurrency = false;
            var rewardHoursPassed = Database.singleton.UCE_DailyRewards_HoursPassed(this);
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSinceEpoch = DateTime.UtcNow - UnixEpoch;

            // ---------- Check for Reset
            if ((dailyRewardCounter == -1 && dailyRewardReset == 0) ||
                (timeSinceEpoch.TotalHours >= (dailyRewardReset + (Mathf.Max(1, DailyRewards.LoginIntervalHours) * Mathf.Max(1, DailyRewards.rewards.Length)))) ||
                (dailyRewardCounter > DailyRewards.rewards.Length))
            {
                dailyRewardReset = timeSinceEpoch.TotalHours;
                dailyRewardCounter = -1;
            }

            // ---------- Check Return Reward
            if (UCE_ValidateReturnRewards() &&
                rewardHoursPassed / 24 >= ReturnRewards.LoginIntervalDays)
            {
                skillExperience += ReturnRewards.reward.rewardSkillExperience;
                experience += ReturnRewards.reward.rewardExperience;
                gold += ReturnRewards.reward.rewardGold;
                coins += ReturnRewards.reward.rewardCoins;

                foreach (UCE_ItemRequirement rewardItem in ReturnRewards.reward.rewardItems)
                {
                    if (rewardItem.item && rewardItem.amount > 0)
                        InventoryAdd(new Item(rewardItem.item), rewardItem.amount);
                }

#if _iMMOHONORSHOP
                foreach (UCE_HonorShopCurrencyDrop currency in ReturnRewards.reward.honorCurrencies)
                {
                    if (currency.amount > 0 && currency.honorCurrency != null)
                    {
                        UCE_AddHonorCurrency(currency.honorCurrency, currency.amount);
                        gainedCurrency = true;
                    }
                }
#endif

                if (0 < ReturnRewards.reward.rewardExperience ||
                    0 < ReturnRewards.reward.rewardSkillExperience ||
                    0 < ReturnRewards.reward.rewardGold ||
                    0 < ReturnRewards.reward.rewardCoins ||
                    0 < ReturnRewards.reward.rewardItems.Length ||
                    gainedCurrency == true
                    )
                {
                    UCE_ReturnRewardsProcess();
                    gainedCurrency = false;
                }

                // ---------- Check Daily Reward
            }
            else if (
              UCE_ValidateDailyRewards() &&
              dailyRewardCounter + 1 < DailyRewards.rewards.Length &&
              rewardHoursPassed >= DailyRewards.LoginIntervalHours)
            {
                dailyRewardCounter++;

                skillExperience += DailyRewards.rewards[dailyRewardCounter].rewardSkillExperience;
                experience += DailyRewards.rewards[dailyRewardCounter].rewardExperience;
                gold += DailyRewards.rewards[dailyRewardCounter].rewardGold;
                coins += DailyRewards.rewards[dailyRewardCounter].rewardCoins;

                foreach (UCE_ItemRequirement rewardItem in DailyRewards.rewards[dailyRewardCounter].rewardItems)
                {
                    if (rewardItem.item && rewardItem.amount > 0)
                        InventoryAdd(new Item(rewardItem.item), rewardItem.amount);
                }

#if _iMMOHONORSHOP
                foreach (UCE_HonorShopCurrencyDrop currency in ReturnRewards.reward.honorCurrencies)
                {
                    if (currency.amount > 0 && currency.honorCurrency != null)
                    {
                        UCE_AddHonorCurrency(currency.honorCurrency, currency.amount);
                        gainedCurrency = true;
                    }
                }
#endif
                if (0 < DailyRewards.rewards[dailyRewardCounter].rewardExperience ||
                    0 < DailyRewards.rewards[dailyRewardCounter].rewardSkillExperience ||
                    0 < DailyRewards.rewards[dailyRewardCounter].rewardGold ||
                    0 < DailyRewards.rewards[dailyRewardCounter].rewardCoins ||
                    0 < DailyRewards.rewards[dailyRewardCounter].rewardItems.Length ||
                    gainedCurrency == true
                    )
                {
                    UCE_DailyRewardsProcess();
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_ValidateDailyRewards
    // -----------------------------------------------------------------------------------
    public bool UCE_ValidateDailyRewards()
    {
        if (DailyRewards == null || !DailyRewards.isActive) return false;

        bool valid = true;

        valid = (DailyRewards.MinLevel == 0 || level >= DailyRewards.MinLevel) ? valid : false;
#if _iMMOQUESTS
        valid = (DailyRewards.requiredQuest == null || UCE_HasCompletedQuest(DailyRewards.requiredQuest.name)) ? valid : false;
#else
		valid = (DailyRewards.requiredQuest == null || HasCompletedQuest(DailyRewards.requiredQuest.name)) ? valid : false;
#endif

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_ValidateReturnRewards
    // -----------------------------------------------------------------------------------
    public bool UCE_ValidateReturnRewards()
    {
        if (ReturnRewards == null || !ReturnRewards.isActive) return false;

        bool valid = true;

        valid = (ReturnRewards.MinLevel == 0 || level >= ReturnRewards.MinLevel) ? valid : false;
#if _iMMOQUESTS
        valid = (ReturnRewards.requiredQuest == null || UCE_HasCompletedQuest(ReturnRewards.requiredQuest.name)) ? valid : false;
#else
		valid = (ReturnRewards.requiredQuest == null || HasCompletedQuest(ReturnRewards.requiredQuest.name)) ? valid : false;
#endif

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_DailyRewardsProcess
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    private void UCE_DailyRewardsProcess()
    {
        Target_UCE_ShowDailyRewardsUI(connectionToClient, dailyRewardCounter);
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_ShowDailyRewardsUI
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    private void Target_UCE_ShowDailyRewardsUI(NetworkConnection target, int dailyRewardCounter)
    {
        if (!_UCE_UI_DailyRewards)
            _UCE_UI_DailyRewards = FindObjectOfType<UCE_UI_DailyRewards>();

        _UCE_UI_DailyRewards.Show(dailyRewardCounter);
    }

    // -----------------------------------------------------------------------------------
    // UCE_ReturnRewardsProcess
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    private void UCE_ReturnRewardsProcess()
    {
        Target_UCE_ShowReturnRewardsUI(connectionToClient);
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_ShowReturnRewardsUI
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    private void Target_UCE_ShowReturnRewardsUI(NetworkConnection target)
    {
        if (!_UCE_UI_DailyRewards)
            _UCE_UI_ReturnRewards = FindObjectOfType<UCE_UI_ReturnRewards>();

        _UCE_UI_ReturnRewards.Show();
    }

    // -----------------------------------------------------------------------------------
}
