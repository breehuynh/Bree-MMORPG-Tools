// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UCE UI NPC QUESTS

public partial class UCE_UI_NpcQuests : MonoBehaviour
{
    public GameObject panel;
    public UCE_UI_NpcQuestSlot slotPrefab;
    public Transform content;

    public string expandPrefix = "[+] ";
    public string hidePrefix = "[-] ";
    public string notEnoughSpace = "Not enough inventory space!";
    public string acceptButton = "Accept";
    public string completeButton = "Complete";

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (player.target != null && player.target is Npc &&
            Utils.ClosestDistance(player, player.target) <= player.interactionRange)
        {
            Npc npc = (Npc)player.target;

            List<UCE_ScriptableQuest> questsAvailable = npc.UCE_QuestsVisibleFor(player);

            UIUtils.BalancePrefabs(slotPrefab.gameObject, questsAvailable.Count, content);

            // refresh all
            for (int i = 0; i < questsAvailable.Count; ++i)
            {
                UCE_UI_NpcQuestSlot slot = content.GetChild(i).GetComponent<UCE_UI_NpcQuestSlot>();

                // find quest index in original npc quest list (unfiltered)
                int npcIndex = Array.FindIndex(npc.UCE_quests, q => q.name == questsAvailable[i].name);

                // find quest index in player quest list
                int questIndex = player.UCE_GetQuestIndexByName(npc.UCE_quests[npcIndex].name);

                if (questIndex != -1)
                {
                    UCE_Quest quest = player.UCE_quests[questIndex];

                    // -- quest must be acceptable or complete-able to show
                    if (player.UCE_CanRestartQuest(quest.data) || player.UCE_CanAcceptQuest(quest.data) || player.UCE_CanCompleteQuest(quest.name))
                    {
                        ScriptableItem reward = null;
                        int amount = 0;
                        if (npc.UCE_quests[npcIndex].questRewards.Length > 0 &&
                            npc.UCE_quests[npcIndex].questRewards[0].rewardItem.Length > 0)
                        {
                            reward = npc.UCE_quests[npcIndex].questRewards[0].rewardItem[0].item;
                            amount = npc.UCE_quests[npcIndex].questRewards[0].rewardItem[0].amount;
                        }

                        int gathered = 0;
                        foreach (UCE_gatherTarget gatherTarget in npc.UCE_quests[npcIndex].gatherTarget)
                        {
                            gathered += player.InventoryCount(new Item(gatherTarget.target));
                        }

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
                        // -- check has space
                        bool hasSpace = player.getHasEnoughSpace(quest);

                        // -- set gameobject active
                        slot.gameObject.SetActive(true);

                        // -- name button
                        GameObject descriptionText = slot.descriptionText.gameObject;
                        string prefix = descriptionText.activeSelf ? hidePrefix : expandPrefix;

                        slot.nameButton.GetComponentInChildren<Text>().text = prefix + quest.name;
                        slot.nameButton.onClick.SetListener(() =>
                        {
                            descriptionText.SetActive(!descriptionText.activeSelf);
                        });

                        // description + not enough space warning (if needed)
                        slot.descriptionText.text = quest.ToolTip(player.checkGatheredItems(quest), explored, factionRequirementsMet);
                        if (!hasSpace)
                            slot.descriptionText.text += "\n<color=red>" + notEnoughSpace + "</color>";

                        if (player.UCE_CanAcceptQuest(quest.data))
                        {
                            // repeatable quest
                            slot.actionButton.interactable = true;
                            slot.actionButton.GetComponentInChildren<Text>().text = acceptButton;
                            slot.actionButton.onClick.SetListener(() =>
                            {
                                player.Cmd_UCE_AcceptQuest(npcIndex);
                            });
                        }
                        else
                        {
                            slot.actionButton.interactable = player.UCE_CanCompleteQuest(quest.name) && hasSpace;
                            slot.actionButton.GetComponentInChildren<Text>().text = completeButton;
                            slot.actionButton.onClick.SetListener(() =>
                            {
                                player.Cmd_UCE_CompleteQuest(npcIndex);
                                panel.SetActive(false);
                            });
                        }
                    }
                    else
                    {
                        // -- deactivate slot
                        slot.gameObject.SetActive(false);
                    }
                }
                else
                {
                    UCE_Quest quest = new UCE_Quest(npc.UCE_quests[npcIndex]);

                    // -- set gameobject active
                    slot.gameObject.SetActive(true);

                    // -- name button
                    GameObject descriptionText = slot.descriptionText.gameObject;
                    string prefix = descriptionText.activeSelf ? hidePrefix : expandPrefix;
                    slot.nameButton.GetComponentInChildren<Text>().text = prefix + quest.name;
                    slot.nameButton.onClick.SetListener(() =>
                    {
                        descriptionText.SetActive(!descriptionText.activeSelf);
                    });

                    // -- new quest
                    slot.descriptionText.text = quest.ToolTip(player.checkGatheredItems(quest));
                    slot.actionButton.interactable = true;
                    slot.actionButton.GetComponentInChildren<Text>().text = acceptButton;
                    slot.actionButton.onClick.SetListener(() =>
                    {
                        player.Cmd_UCE_AcceptQuest(npcIndex);
                    });
                }
            }
        }
        else panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
}
