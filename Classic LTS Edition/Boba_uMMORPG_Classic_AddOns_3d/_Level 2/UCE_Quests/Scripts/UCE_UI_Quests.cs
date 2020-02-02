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

// UCE UI QUESTS

public partial class UCE_UI_Quests : MonoBehaviour
{
    public static UCE_UI_Quests singleton;

    public KeyCode hotKey = KeyCode.Q;
    public GameObject panel;
    public Transform content;
    public UCE_UI_QuestSlot slotPrefab;

    public Button activeQuestsButton;
    public Button completedQuestsButton;
    public Button trackerButton;

    public UCE_UI_CancelQuest cancelQuestPanel;
    public GameObject trackerPanel;

    public string expandPrefix = "[+] ";
    public string hidePrefix = "[-] ";

    public Color fulfilledQuestColor;
    public Color inprogressQuestColor;

    public float cacheInterval = 2.0f;

    protected bool showActiveQuests = true;

    protected string[] cacheTooltip;
    protected float[] cacheTimer;

    // -----------------------------------------------------------------------------------
    // Awake
    // -----------------------------------------------------------------------------------
    void Awake()
    {
        if (singleton == null) singleton = this;
    }

    // -----------------------------------------------------------------------------------
    // OnEnable
    // -----------------------------------------------------------------------------------
    private void OnEnable()
    {
        activeQuestsButton.onClick.SetListener(() =>
        {
            cacheTimer = null;
            showActiveQuests = true;
        });

        completedQuestsButton.onClick.SetListener(() =>
        {
            cacheTimer = null;
            showActiveQuests = false;
        });

        trackerButton.onClick.SetListener(() =>
        {
            if (trackerPanel.activeInHierarchy)
            {
                trackerPanel.SetActive(false);
            }
            else
            {
                trackerPanel.SetActive(true);
            }
        });
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        // hotkey (not while typing in chat, etc.)
        if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
            panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf)
        {
            List<UCE_Quest> activeQuests = new List<UCE_Quest>();

            if (showActiveQuests)
            {
                activeQuests = player.UCE_quests.Where(q => !q.completed || (q.repeatable > 0 && !q.completedAgain)).ToList();
            }
            else
            {
                activeQuests = player.UCE_quests.Where(q => q.completed).ToList();
            }

            if (cacheTimer == null || cacheTimer.Length != activeQuests.Count)
            {
                cacheTooltip = new string[player.UCE_quests.Count];
                cacheTimer = new float[player.UCE_quests.Count];
            }

            UIUtils.BalancePrefabs(slotPrefab.gameObject, activeQuests.Count, content);

            // -- refresh all
            for (int i = 0; i < activeQuests.Count; ++i)
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

                    if (showActiveQuests)
                    {
                        if (quest.IsFulfilled(gathered, explored, factionRequirementsMet))
                        {
                            slot.nameButton.GetComponent<Image>().color = fulfilledQuestColor;
                        }
                        else
                        {
                            slot.nameButton.GetComponent<Image>().color = inprogressQuestColor;
                        }
                    }
                    else
                    {
                        slot.nameButton.GetComponent<Image>().color = fulfilledQuestColor;
                    }

                    slot.nameButton.onClick.SetListener(() =>
                    {
                        descriptionPanel.SetActive(!descriptionPanel.activeSelf);
                    });

                    // -- share button
                    if (showActiveQuests && player.InParty())
                    {
                        slot.shareButton.gameObject.SetActive(true);
                        slot.shareButton.onClick.SetListener(() =>
                        {
                            player.Cmd_UCE_ShareQuest(quest.name);
                            panel.gameObject.SetActive(false);
                        });
                    }
                    else
                    {
                        slot.shareButton.gameObject.SetActive(false);
                    }

                    // -- cancel button
                    if (showActiveQuests)
                    {
                        slot.cancelButton.gameObject.SetActive(true);
                        slot.cancelButton.onClick.SetListener(() =>
                        {
                            cancelQuestPanel.Show(quest.name);
                        });
                    }
                    else
                    {
                        slot.cancelButton.gameObject.SetActive(false);
                    }

                    // -- update cache
                    cacheTooltip[index] = quest.ToolTip(gathered, explored, factionRequirementsMet);
                    cacheTimer[index] = Time.time + cacheInterval;

                    // -- update description
                    slot.descriptionText.text = cacheTooltip[index];
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
