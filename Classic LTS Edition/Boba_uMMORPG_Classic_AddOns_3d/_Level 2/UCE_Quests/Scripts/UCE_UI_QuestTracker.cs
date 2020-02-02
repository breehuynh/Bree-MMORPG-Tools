// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// UCE UI QUEST TRACKER

public partial class UCE_UI_QuestTracker : MonoBehaviour
{
    public GameObject panel;
    public Transform content;
    public UCE_UI_QuestSlot slotPrefab;

    public string expandPrefix = "[+] ";
    public string hidePrefix = "[-] ";

    public Color fulfilledQuestColor;
    public Color inprogressQuestColor;

    public int maxActiveQuestsToShow = 5;
    public float cacheInterval = 2.0f;

    protected string[] cacheTooltip;
    protected float[] cacheTimer;

    // -----------------------------------------------------------------------------------
    // OnEnable
    // -----------------------------------------------------------------------------------
    private void OnEnable()
    {
        cacheTooltip = new string[maxActiveQuestsToShow];
        cacheTimer = new float[maxActiveQuestsToShow];
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (panel.activeSelf)
        {
            List<UCE_Quest> activeQuests = player.UCE_quests.Where(q => !q.completed || (q.repeatable > 0 && !q.completedAgain)).ToList();

            int maxQuests = Mathf.Min(activeQuests.Count, maxActiveQuestsToShow);

            UIUtils.BalancePrefabs(slotPrefab.gameObject, maxQuests, content);

            // -- refresh all
            for (int i = 0; i < maxQuests; ++i)
            {
                int index = i;
                UCE_UI_QuestSlot slot = content.GetChild(index).GetComponent<UCE_UI_QuestSlot>();
                UCE_Quest quest = activeQuests[index];

                // -- check cache
                if (Time.time > cacheTimer[index])
                {
                    // =======================================================================

                    // -- check gathered items
                    int[] gathered = player.checkGatheredItems(quest);

                    // -- check explored areas
                    int explored = 0;
#if _iMMOEXPLORATION
                    foreach (UCE_Area_Exploration area in quest.exploreTarget)
                    {
                        if (player.UCE_HasExploredArea(area))
                            explored++;
                    }
#endif

                    // -- check faction requirement
                    bool factionRequirementsMet = true;
#if _iMMOFACTIONS
                    factionRequirementsMet = player.UCE_CheckFactionRating(quest.factionRequirement);
#endif

                    // =======================================================================

                    // name button
                    GameObject descriptionPanel = slot.descriptionText.gameObject;
                    string prefix = descriptionPanel.activeSelf ? hidePrefix : expandPrefix;
                    slot.nameButton.GetComponentInChildren<Text>().text = prefix + quest.name;
                    slot.nameButton.onClick.SetListener(() =>
                    {
                        descriptionPanel.SetActive(!descriptionPanel.activeSelf);
                    });

                    if (quest.IsFulfilled(gathered, explored, factionRequirementsMet))
                    {
                        slot.nameButton.GetComponent<Image>().color = fulfilledQuestColor;
                    }
                    else
                    {
                        slot.nameButton.GetComponent<Image>().color = inprogressQuestColor;
                    }

                    // -- cancel button
                    slot.cancelButton.gameObject.SetActive(false);

                    // -- update cache
                    cacheTooltip[index] = quest.TrackerTip(gathered, explored, factionRequirementsMet, player.level);
                    cacheTimer[index] = Time.time + cacheInterval;

                    // -- update description
                    slot.descriptionText.text = cacheTooltip[index];
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
