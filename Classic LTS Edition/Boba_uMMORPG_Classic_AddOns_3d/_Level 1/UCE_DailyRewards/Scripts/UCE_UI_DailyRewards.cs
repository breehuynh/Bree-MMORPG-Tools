// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// DAILY REWARDS UI
// ===================================================================================
public partial class UCE_UI_DailyRewards : MonoBehaviour
{
    public GameObject panel;
    public Transform content;
    public ScrollRect scrollRect;
    public UCE_UI_Slot_DailyRewards textPrefab;

    public Sprite currentReward;
    public Sprite nextReward;
    public Sprite resetReward;

    public Sprite goldIcon;
    public Sprite coinsIcon;
    public Sprite expIcon;
    public Sprite skillExpIcon;
#if _iMMOHONORSHOP
    public Sprite honorCurrencyIcon;
#endif

    public Color textColor;
    public Color errorColor;

    public string MSG_REWARD_THIS = "You received a login reward:";
    public string MSG_REWARD_NEXT = "Next login reward: ";
    public string MSG_REWARD_RESET = "<Daily Login Reward Reset>";
    public string MSG_REWARD_GOLD = "Gold: ";
    public string MSG_REWARD_COINS = "Coins: ";
    public string MSG_REWARD_EXP = "Experience: ";
    public string MSG_REWARD_SKILLEXP = "Skill Experience: ";
    public string MSG_REWARD_ITEM = "Item: ";
#if _iMMOHONORSHOP
    public string MSG_REWARD_HONORCURRENCY = "Honor Currency: ";
#endif

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(int dailyRewardCounter)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        for (int i = 0; i < content.childCount; ++i)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        if (player.DailyRewards != null && dailyRewardCounter > -1)
        {
            // ---------- Current Reward
            long rGold = player.DailyRewards.rewards[dailyRewardCounter].rewardGold;
            long rCoins = player.DailyRewards.rewards[dailyRewardCounter].rewardCoins;
            long rExp = player.DailyRewards.rewards[dailyRewardCounter].rewardExperience;
            long rSkillExp = player.DailyRewards.rewards[dailyRewardCounter].rewardSkillExperience;

            AddMessage(MSG_REWARD_THIS, textColor, currentReward);
            if (rGold > 0) AddMessage(MSG_REWARD_GOLD + rGold.ToString(), textColor, goldIcon);
            if (rCoins > 0) AddMessage(MSG_REWARD_COINS + rCoins.ToString(), textColor, coinsIcon);
            if (rExp > 0) AddMessage(MSG_REWARD_EXP + rExp.ToString(), textColor, expIcon);
            if (rSkillExp > 0) AddMessage(MSG_REWARD_SKILLEXP + rSkillExp.ToString(), textColor, skillExpIcon);

            foreach (UCE_ItemRequirement rewardItem in player.DailyRewards.rewards[dailyRewardCounter].rewardItems)
            {
                if (rewardItem.item && rewardItem.amount > 0)
                    AddMessage(MSG_REWARD_ITEM + rewardItem.item.name + " [x" + rewardItem.amount.ToString() + "]", textColor, rewardItem.item.image);
            }

#if _iMMOHONORSHOP
            foreach (UCE_HonorShopCurrencyDrop currency in player.DailyRewards.rewards[dailyRewardCounter].honorCurrencies)
            {
                if (currency.amount > 0 && currency.honorCurrency != null)
                    AddMessage(MSG_REWARD_HONORCURRENCY + currency.honorCurrency.name + " [x" + currency.amount.ToString() + "]", textColor, honorCurrencyIcon);
            }
#endif

            // ---------- Next Reward
            if (dailyRewardCounter + 1 < player.DailyRewards.rewards.Length)
            {
                rGold = player.DailyRewards.rewards[dailyRewardCounter + 1].rewardGold;
                rCoins = player.DailyRewards.rewards[dailyRewardCounter + 1].rewardCoins;
                rExp = player.DailyRewards.rewards[dailyRewardCounter + 1].rewardExperience;

                AddMessage(MSG_REWARD_NEXT, textColor, nextReward);
                if (rGold > 0) AddMessage(MSG_REWARD_GOLD + rGold.ToString(), textColor, goldIcon);
                if (rCoins > 0) AddMessage(MSG_REWARD_COINS + rCoins.ToString(), textColor, coinsIcon);
                if (rExp > 0) AddMessage(MSG_REWARD_EXP + rExp.ToString(), textColor, expIcon);

                foreach (UCE_ItemRequirement rewardItem in player.DailyRewards.rewards[dailyRewardCounter + 1].rewardItems)
                {
                    if (rewardItem.item && rewardItem.amount > 0)
                        AddMessage(MSG_REWARD_ITEM + rewardItem.item.name + " [x" + rewardItem.amount.ToString() + "]", textColor, rewardItem.item.image);
                }

#if _iMMOHONORSHOP
                foreach (UCE_HonorShopCurrencyDrop currency in player.DailyRewards.rewards[dailyRewardCounter + 1].honorCurrencies)
                {
                    if (currency.amount > 0 && currency.honorCurrency != null)
                        AddMessage(MSG_REWARD_HONORCURRENCY + currency.honorCurrency.name + " [x" + currency.amount.ToString() + "]", textColor, honorCurrencyIcon);
                }
#endif
            }
            else
            {
                AddMessage(MSG_REWARD_NEXT, textColor, resetReward);
                AddMessage(MSG_REWARD_RESET, errorColor);
            }

            panel.SetActive(true);
        }
    }

    // -----------------------------------------------------------------------------------
    // AutoScroll
    // -----------------------------------------------------------------------------------
    private void AutoScroll()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    // -----------------------------------------------------------------------------------
    // AddMessage
    // -----------------------------------------------------------------------------------
    public void AddMessage(string msg, Color color, Sprite icon = null)
    {
        UCE_UI_Slot_DailyRewards go = Instantiate(textPrefab);
        go.transform.SetParent(content.transform, false);
        go.AddMessage(msg, color, icon);
        AutoScroll();
    }
}
