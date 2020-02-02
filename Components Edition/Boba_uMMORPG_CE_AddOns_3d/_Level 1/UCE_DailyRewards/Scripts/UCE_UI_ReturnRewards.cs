// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// RETURN REWARDS UI
// ===================================================================================
public partial class UCE_UI_ReturnRewards : MonoBehaviour
{
    public GameObject panel;
    public Transform content;
    public ScrollRect scrollRect;
    public UCE_UI_Slot_DailyRewards textPrefab;

    public Sprite currentReward;
    public Sprite goldIcon;
    public Sprite coinsIcon;
    public Sprite expIcon;
    public Sprite skillExpIcon;
#if _iMMOHONORSHOP
    public Sprite honorCurrencyIcon;
#endif

    public Color textColor;
    public Color errorColor;

    public string MSG_REWARD_THIS = "You received a return reward:";
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
    public void Show()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        for (int i = 0; i < content.childCount; ++i)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        if (player.ReturnRewards != null)
        {
            // ---------- Return Reward
            long rGold = player.ReturnRewards.reward.rewardGold;
            long rCoins = player.ReturnRewards.reward.rewardCoins;
            long rExp = player.ReturnRewards.reward.rewardExperience;
            long rSkillExp = player.ReturnRewards.reward.rewardSkillExperience;

            if (rGold > 0 || rCoins > 0 || rExp > 0 || rSkillExp > 0)
            {
                AddMessage(MSG_REWARD_THIS, textColor, currentReward);
                if (rGold > 0) AddMessage(MSG_REWARD_GOLD + rGold.ToString(), textColor, goldIcon);
                if (rCoins > 0) AddMessage(MSG_REWARD_COINS + rCoins.ToString(), textColor, coinsIcon);
                if (rExp > 0) AddMessage(MSG_REWARD_EXP + rExp.ToString(), textColor, expIcon);
                if (rSkillExp > 0) AddMessage(MSG_REWARD_SKILLEXP + rSkillExp.ToString(), textColor, skillExpIcon);
                if (rExp > 0) AddMessage(MSG_REWARD_EXP + rExp.ToString(), textColor, skillExpIcon);
            }

            foreach (UCE_ItemRequirement rewardItem in player.ReturnRewards.reward.rewardItems)
            {
                if (rewardItem.item && rewardItem.amount > 0)
                    AddMessage(MSG_REWARD_ITEM + rewardItem.item.name + " [x" + rewardItem.amount.ToString() + "]", textColor, rewardItem.item.image);
            }

#if _iMMOHONORSHOP
            foreach (UCE_HonorShopCurrencyDrop currency in player.ReturnRewards.reward.honorCurrencies)
            {
                if (currency.amount > 0 && currency.honorCurrency != null)
                    AddMessage(MSG_REWARD_HONORCURRENCY + currency.honorCurrency.name + " [x" + currency.amount.ToString() + "]", textColor, honorCurrencyIcon);
            }
#endif

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
