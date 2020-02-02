// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// DAILY REWARDS - TEMPLATE

[CreateAssetMenu(fileName = "UCE_Tmpl_DailyRewards", menuName = "UCE Templates/New UCE DailyRewards", order = 999)]
public class UCE_Tmpl_DailyRewards : ScriptableObject
{
    [Header("-=-=-=- Daily Rewards -=-=-=-")]
    [Tooltip("One click deactivation")]
    public bool isActive = true;

    [Tooltip("Min player level to enable the reward")]
    public int MinLevel = 1;

    [Tooltip("This quest must be completed first")]
#if _iMMOQUESTS
    public UCE_ScriptableQuest requiredQuest;

#else
	public ScriptableQuest requiredQuest;
#endif

    [Tooltip("Hours between login to advance reward counter")]
    public int LoginIntervalHours = 23;

    [Tooltip("Define your rewards by adding and editing entries")]
    public UCE_DailyReward[] rewards;
}
