// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

// PLAYER

public partial class Player
{
    [Header("[-=-=-=- UCE QUESTS -=-=-=-]")]
    [Tooltip("[Required] Contains active and completed quests (=all)")]
    public int UCE_activeQuestLimit = 100;

    public string shareQuestMessage = "Quest shared: ";

    public UCE_PopupClass questCompletePopup;

    public SyncListUCE_Quest UCE_quests = new SyncListUCE_Quest();

    // =============================== CORE SCRIPT REWRITES ==============================

    // -----------------------------------------------------------------------------------
    // QuestsOnKilled
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void QuestsOnKilled(Entity victim)
    {
#if _iMMOQUESTS
        UCE_IncreaseQuestKillCounterFor(victim);
#else
		for (int i = 0; i < quests.Count; ++i) {
		  if (!quests[i].completed)
			  quests[i].OnKilled(this, i, victim);
	  }
#endif
    }

    // -----------------------------------------------------------------------------------
    // OnDamageDealtToPlayer
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void OnDamageDealtToPlayer(Player player)
    {
        if (!player.IsOffender() && !player.IsMurderer())
        {
            // did we kill him? then start/reset murder status
            // did we just attack him? then start/reset offender status
            // (unless we are already a murderer)
            if (player.health == 0)
            {
                StartMurderer();
#if _iMMOQUESTS
                QuestsOnKilled(player);
#endif
            }
            else if (!IsMurderer()) StartOffender();
        }
    }

    // ==================================== FUNCTIONS ====================================

    // -----------------------------------------------------------------------------------
    // UCE_checkQuestCompletion
    // @Server
    // -----------------------------------------------------------------------------------
    public void UCE_checkQuestCompletion(int index)
    {
        if (UCE_HasActiveQuest(UCE_quests[index].name) && UCE_CanCompleteQuest(UCE_quests[index].name))
        {
            UCE_ShowPopup(questCompletePopup.message + UCE_quests[index].name, questCompletePopup.iconId, questCompletePopup.soundId);

            if (UCE_quests[index].autoCompleteQuest)
                UCE_FinishQuest(index);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetQuestIndexByName
    // -----------------------------------------------------------------------------------
    public int UCE_GetQuestIndexByName(string questName)
    {
        return UCE_quests.FindIndex(quest => quest.name == questName);
    }

    // -----------------------------------------------------------------------------------
    // UCE_HasCompletedQuest
    // -----------------------------------------------------------------------------------
    public bool UCE_HasCompletedQuest(string questName)
    {
        return UCE_quests.Any(q => q.name == questName && q.completed);
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanRestartQuest
    // -----------------------------------------------------------------------------------
    public bool UCE_CanRestartQuest(UCE_ScriptableQuest quest)
    {
        int idx = UCE_GetQuestIndexByName(quest.name);

        if (idx == -1) return true;

        UCE_Quest tmp_quest = UCE_quests[idx];

        if (UCE_CanCompleteQuest(quest.name) ||
            UCE_HasActiveQuest(quest.name) ||
            quest.repeatable <= 0 ||
            (quest.repeatable > 0 && tmp_quest.getLastCompleted() < tmp_quest.repeatable && tmp_quest.completedAgain)
        )
            return false;

        return true;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HasActiveQuest
    // -----------------------------------------------------------------------------------
    public bool UCE_HasActiveQuest(string questName)
    {
        return UCE_quests.Any(q => q.name == questName && !q.completed
                || (q.name == questName && !q.completed && !q.completedAgain)
                || (q.name == questName && q.completed && !q.completedAgain));
    }

    // -----------------------------------------------------------------------------------
    // UCE_IncreaseQuestNpcCounterFor
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    public void UCE_IncreaseQuestNpcCounterFor(Npc npc)
    {
        for (int i = 0; i < UCE_quests.Count; ++i)
        {
            // active quest and not completed yet?
            if ((!UCE_quests[i].completed || !UCE_quests[i].completedAgain) &&
                UCE_quests[i].visitTarget.Length > 0 &&
                UCE_quests[i].visitTarget.Any(x => x.name.GetDeterministicHashCode() == npc.name.GetDeterministicHashCode()) &&
                !UCE_quests[i].visitedTarget.Any(x => x == npc.name.GetDeterministicHashCode())
                )
            {
                int index = i;
                Cmd_UCE_IncreaseQuestNpcCounterFor(index, npc.name.GetDeterministicHashCode());
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_IncreaseQuestNpcCounterFor
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_IncreaseQuestNpcCounterFor(int index, int hash)
    {
        UCE_Quest quest = UCE_quests[index];
        bool bChanged = false;
        for (int j = 0; j < UCE_quests[index].visitTarget.Length; ++j)
        {
            if (UCE_quests[index].visitTarget[j].name.GetDeterministicHashCode() == hash &&
                quest.visitedTarget[j] != hash
                )
            {
                quest.visitedTarget[j] = hash;
                quest.visitedCount++;
                UCE_quests[index] = quest;
                bChanged = true;
                break;
            }
        }

        if (bChanged) UCE_checkQuestCompletion(index);
    }

    // -----------------------------------------------------------------------------------
    // UCE_IncreaseQuestKillCounterFor
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_IncreaseQuestKillCounterFor(Entity victim)
    {
        if (victim == null) return;

        if (victim is Monster)
        {
            UCE_IncreaseQuestMonsterKillCounterFor((Monster)victim);
        }
        else if (victim is Player)
        {
            UCE_IncreaseQuestPlayerKillCounterFor((Player)victim);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_IncreaseQuestMonsterKillCounterFor
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_IncreaseQuestMonsterKillCounterFor(Monster victim)
    {
        for (int i = 0; i < UCE_quests.Count; ++i)
        {
            int index = i;

            if ((!UCE_quests[index].completed || !UCE_quests[index].completedAgain) &&
                UCE_quests[index].killTarget.Length > 0 &&
                UCE_quests[index].killTarget.Any(x => x.target.name == victim.name)
                )
            {
                UCE_Quest quest = UCE_quests[index];
                bool bChanged = false;

                for (int j = 0; j < quest.killTarget.Length; ++j)
                {
                    int idx = j;
                    if (quest.killTarget[idx].target.name == victim.name &&
                        quest.killedTarget[idx] < quest.killTarget[idx].amount)
                    {
                        quest.killedTarget[idx]++;
                        bChanged = true;
                        break;
                    }
                }

                UCE_quests[index] = quest;
                if (bChanged) UCE_checkQuestCompletion(index);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_IncreaseQuestPlayerKillCounterFor
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_IncreaseQuestPlayerKillCounterFor(Player victim)
    {
#if _iMMOPVP && _iMMOQUESTS
        for (int i = 0; i < UCE_quests.Count; ++i)
        {
            int index = i;

            if ((!UCE_quests[index].completed || !UCE_quests[index].completedAgain) &&
                UCE_quests[index].pvpTarget.Length > 0
                )
            {
                UCE_Quest quest = UCE_quests[index];

                for (int j = 0; j < quest.pvpTarget.Length; ++j)
                {
                    int idx = j;

                    if (UCE_CheckPvPTarget(victim, quest.pvpTarget[idx]))
                    {
                        quest.pvpedTarget[idx]++;
                        break;
                    }
                }

                UCE_quests[index] = quest;
                UCE_checkQuestCompletion(index);
            }
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckPvPTarget
    // -----------------------------------------------------------------------------------
#if _iMMOPVP && _iMMOQUESTS

    public bool UCE_CheckPvPTarget(Player victim, UCE_pvpTarget target)
    {
        // -- Check Level Range

        if (target.levelRange != 0)
        {
            int minLevel = Mathf.Max(1, level - target.levelRange);
            int maxLevel = level + target.levelRange;

            if (victim.level < minLevel ||
                victim.level > maxLevel)
                return false;
        }

        // -- Check Type

        if (target.type != UCE_pvpTarget.pvpType.Any)
        {
            if (target.type == UCE_pvpTarget.pvpType.MyParty ||
                target.type == UCE_pvpTarget.pvpType.OtherParty)
            {
                if (!InParty() || !victim.InParty())
                    return false;

                if (target.type == UCE_pvpTarget.pvpType.AnyParty &&
                    !victim.InParty())
                    return false;

                if (target.type == UCE_pvpTarget.pvpType.MyParty &&
                    (!party.Contains(victim.name) ||
                    victim.party.Contains(name)))
                    return false;

                if (target.type == UCE_pvpTarget.pvpType.OtherParty &&
                    (!party.Contains(victim.name) ||
                    victim.party.Contains(name)))
                    return false;
            }

            if (target.type == UCE_pvpTarget.pvpType.MyGuild ||
                target.type == UCE_pvpTarget.pvpType.OtherGuild)
            {
                if (!InGuild() || !victim.InGuild())
                    return false;

                if (target.type == UCE_pvpTarget.pvpType.AnyGuild &&
                    !victim.InGuild())
                    return false;

                if (target.type == UCE_pvpTarget.pvpType.MyGuild &&
                    guild.name != victim.guild.name)
                    return false;

                if (target.type == UCE_pvpTarget.pvpType.OtherGuild &&
                    guild.name == victim.guild.name)
                    return false;
            }

            if (target.type == UCE_pvpTarget.pvpType.MyRealm ||
                target.type == UCE_pvpTarget.pvpType.OtherRealm)
            {
                //if (Realm == 0 || alliedRealm == 0 || victim.Realm == 0 || victim.alliedRealm == 0)
                //	return false;

                if (target.type == UCE_pvpTarget.pvpType.MyRealm &&
                    !UCE_getAlliedRealms(victim))
                    return false;

                if (target.type == UCE_pvpTarget.pvpType.OtherRealm &&
                    UCE_getAlliedRealms(victim))
                    return false;
            }
        }

        return true;
    }

#endif

    // -----------------------------------------------------------------------------------
    // UCE_IncreaseHarvestNodeCounterFor
    // -----------------------------------------------------------------------------------
#if _iMMOHARVESTING && _iMMOQUESTS

    [Server]
    public void UCE_IncreaseHarvestNodeCounterFor(UCE_HarvestingProfessionTemplate profession)
    {
        for (int i = 0; i < UCE_quests.Count; ++i)
        {
            if ((!UCE_quests[i].completed || !UCE_quests[i].completedAgain) &&
                UCE_quests[i].harvestTarget.Length > 0 &&
                UCE_quests[i].harvestTarget.Any(x => x.target == profession)
                )
            {
                UCE_Quest quest = UCE_quests[i];
                bool bChanged = false;

                for (int j = 0; j < quest.harvestTarget.Length; ++j)
                {
                    if (quest.harvestTarget[j].target == profession &&
                        quest.harvestedTarget[j] < quest.harvestTarget[j].amount)
                    {
                        int idx = j;
                        quest.harvestedTarget[idx]++;
                        bChanged = true;
                        break;
                    }
                }

                UCE_quests[i] = quest;
                if (bChanged) UCE_checkQuestCompletion(i);
            }
        }
    }

#endif
    // -----------------------------------------------------------------------------------
    // UCE_IncreaseCraftCounterFor
    // -----------------------------------------------------------------------------------
#if _iMMOCRAFTING && _iMMOQUESTS

    [Server]
    public void UCE_IncreaseCraftCounterFor(UCE_Tmpl_Recipe recipe)
    {
        for (int i = 0; i < UCE_quests.Count; ++i)
        {
            if ((!UCE_quests[i].completed || !UCE_quests[i].completedAgain) &&
                UCE_quests[i].craftTarget.Length > 0 &&
                UCE_quests[i].craftTarget.Any(x => x.target == recipe)
                )
            {
                UCE_Quest quest = UCE_quests[i];
                bool bChanged = false;

                for (int j = 0; j < quest.craftTarget.Length; ++j)
                {
                    if (quest.craftTarget[j].target == recipe &&
                        quest.craftedTarget[j] < quest.craftTarget[j].amount)
                    {
                        int idx = j;
                        quest.craftedTarget[idx]++;
                        bChanged = true;
                        break;
                    }
                }

                UCE_quests[i] = quest;
                if (bChanged) UCE_checkQuestCompletion(i);
            }
        }
    }

#endif

    // -----------------------------------------------------------------------------------
    // UCE_IncreaseQuestLootCounterFor
    // -----------------------------------------------------------------------------------
#if _iMMOCHEST && _iMMOQUESTS

    [Server]
    public void UCE_IncreaseQuestLootCounterFor(string lootcrateName)
    {
        for (int i = 0; i < UCE_quests.Count; ++i)
        {
            if ((!UCE_quests[i].completed || !UCE_quests[i].completedAgain) &&
                UCE_quests[i].lootTarget.Length > 0 &&
                UCE_quests[i].lootTarget.Any(x => x.target.name == lootcrateName)
                )
            {
                UCE_Quest quest = UCE_quests[i];
                bool bChanged = false;

                for (int j = 0; j < quest.lootTarget.Length; ++j)
                {
                    if (quest.lootTarget[j].target.name == lootcrateName &&
                        quest.lootedTarget[j] < quest.lootTarget[j].amount)
                    {
                        int idx = j;
                        quest.lootedTarget[idx]++;
                        bChanged = true;
                        break;
                    }
                }

                UCE_quests[i] = quest;
                if (bChanged) UCE_checkQuestCompletion(i);
            }
        }
    }

#endif

    // -----------------------------------------------------------------------------------
    // UCE_CanAcceptQuest
    // -----------------------------------------------------------------------------------
    public bool UCE_CanAcceptQuest(UCE_ScriptableQuest quest)
    {
        return UCE_quests.Count(q => !q.completed) < activeQuestLimit &&
               quest.questRequirements.checkRequirements(this) &&
               UCE_CanRestartQuest(quest)
               ;
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_AcceptQuest
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_AcceptQuest(int npcQuestIndex)
    {
        if (state == "IDLE" &&
            target != null &&
            isAlive &&
            target.isAlive &&
            target is Npc &&
            0 <= npcQuestIndex && npcQuestIndex < ((Npc)target).UCE_quests.Length &&
            Utils.ClosestDistance(this, target) <= interactionRange &&
            UCE_CanAcceptQuest(((Npc)target).UCE_quests[npcQuestIndex]))
        {
            int idx = UCE_GetQuestIndexByName(((Npc)target).UCE_quests[npcQuestIndex].name);

            if (idx == -1)
            {
                UCE_ScriptableQuest quest = ((Npc)target).UCE_quests[npcQuestIndex];
                UCE_quests.Add(new UCE_Quest(quest));

                // -- accept items
                if (quest.acceptItems != null && quest.acceptItems.Length > 0)
                {
                    foreach (UCE_rewardItem rewardItem in quest.acceptItems)
                        InventoryAdd(new Item(rewardItem.item), rewardItem.amount);
                }
            }
            else
            {
                UCE_Quest quest = UCE_quests[idx];
                quest.resetQuest();
                quest.completedAgain = false;
                quest.lastCompleted = "";
                UCE_quests[idx] = quest;

                // -- accept items
                if (quest.acceptItems != null && quest.acceptItems.Length > 0)
                {
                    foreach (UCE_rewardItem rewardItem in quest.acceptItems)
                        InventoryAdd(new Item(rewardItem.item), rewardItem.amount);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_AcceptQuest
    // -----------------------------------------------------------------------------------
    public void UCE_AcceptQuest(string questName)
    {

        int idx = UCE_GetQuestIndexByName(questName);

        // -- only if we don't have the quest already
        if (idx == -1)
        {

            UCE_ScriptableQuest newQuest;

            if (UCE_ScriptableQuest.dict.TryGetValue(questName.GetDeterministicHashCode(), out newQuest))
            {

                if (UCE_CanAcceptQuest(newQuest))
                {

                    UCE_quests.Add(new UCE_Quest(newQuest));

                    // -- accept items
                    if (newQuest.acceptItems != null && newQuest.acceptItems.Length > 0)
                    {
                        foreach (UCE_rewardItem rewardItem in newQuest.acceptItems)
                            InventoryAdd(new Item(rewardItem.item), rewardItem.amount);
                    }

                    UCE_ShowPopup(shareQuestMessage + newQuest.name);

                }

            }

        }

    }

    // -----------------------------------------------------------------------------------
    // UCE_CanCompleteQuest
    // -----------------------------------------------------------------------------------
    public bool UCE_CanCompleteQuest(string questName)
    {
        int index = UCE_GetQuestIndexByName(questName);

        if (index != -1 &&
            (!UCE_quests[index].completed || UCE_quests[index].repeatable > 0 && !UCE_quests[index].completedAgain)
            )
        {
            UCE_Quest quest = UCE_quests[index];

            // -- check explored areas
            int explored = 0;
#if _iMMOEXPLORATION
            foreach (UCE_Area_Exploration area in quest.exploreTarget)
            {
                if (UCE_HasExploredArea(area))
                    explored++;
            }
#endif

            // -- check faction requirement
            bool factionRequirementsMet = true;
#if _iMMOFACTIONS
            factionRequirementsMet = UCE_CheckFactionRating(quest.factionRequirement);
#endif

            // -- validate the rest
            if (quest.IsFulfilled(checkGatheredItems(quest), explored, factionRequirementsMet))
                return true;
        }
        return false;
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_CancelQuest
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_CancelQuest(string questName)
    {
        int index = UCE_GetQuestIndexByName(questName);

        UCE_Quest quest = UCE_quests[index];

        if (!UCE_HasCompletedQuest(questName))
        {
            // -- remove accept items
            if (quest.acceptItems.Length > 0)
            {
                foreach (UCE_rewardItem rewardItem in quest.acceptItems)
                    InventoryRemove(new Item(rewardItem.item), rewardItem.amount);
            }

            UCE_quests.RemoveAt(index);
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_ShareQuest
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_ShareQuest(string questName)
    {
        if (!InParty()) return;

        if (!UCE_HasCompletedQuest(questName))
            UCE_ShareQuest(questName);
    }

    // -----------------------------------------------------------------------------------
    // UCE_ShareQuest
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    protected void UCE_ShareQuest(string questName)
    {

        List<Player> closeMembers = InParty() ? GetPartyMembersInProximity() : new List<Player>();

        foreach (Player member in closeMembers)
            member.UCE_AcceptQuest(questName);

    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_CompleteQuest
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_CompleteQuest(int npcQuestIndex)
    {
        if (state == "IDLE" &&
            isAlive &&
            target != null &&
            target.isAlive &&
            target is Npc &&
            0 <= npcQuestIndex && npcQuestIndex < ((Npc)target).UCE_quests.Length &&
            Utils.ClosestDistance(this, target) <= interactionRange)
        {
            UCE_ScriptableQuest npcQuest = ((Npc)target).UCE_quests[npcQuestIndex];
            int index = UCE_GetQuestIndexByName(npcQuest.name);

            UCE_FinishQuest(index);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_FinishQuest
    // @Server
    // -----------------------------------------------------------------------------------
    protected void UCE_FinishQuest(int index)
    {
        if (index != -1)
        {
            UCE_Quest quest = UCE_quests[index];

            if (UCE_CanCompleteQuest(quest.name))
            {
                // -- remove accept items (optional)
                if (quest.removeAtCompletion && quest.acceptItems.Length > 0)
                {
                    foreach (UCE_rewardItem rewardItem in quest.acceptItems)
                        InventoryRemove(new Item(rewardItem.item), rewardItem.amount);
                }

                // -- remove gathered items
                if (!quest.DontDestroyGathered)
                {
                    foreach (UCE_gatherTarget gatherTarget in quest.gatherTarget)
                    {
                        InventoryRemove(new Item(gatherTarget.target), gatherTarget.amount);
                    }
                }

                // -- determine the correct reward
                if (quest.questRewards.Length > 0)
                {
                    UCE_QuestReward reward = getQuestReward(quest);

                    // -- gain basic rewards
                    gold += reward.rewardGold;
                    experience += reward.rewardExperience;
                    coins += reward.rewardCoins;

                    // -- reward items
                    if (reward.rewardItem.Length > 0)
                    {
                        foreach (UCE_rewardItem rewardItem in reward.rewardItem)
                            InventoryAdd(new Item(rewardItem.item), rewardItem.amount);
                    }

                    // -- unlock travelroutes
#if _iMMOTRAVEL
                    foreach (UCE_Unlockroute route in reward.rewardUnlockroutes)
                        UCE_UnlockTravelroute(route);
#endif

                    // -- reward honor currency
#if _iMMOHONORSHOP
                    foreach (UCE_HonorShopCurrencyCost currency in reward.honorCurrency)
                        UCE_AddHonorCurrency(currency.honorCurrency, currency.amount);
#endif

                    // -- apply realm change
#if _iMMOPVP
                    UCE_setRealm(reward.changeRealm, reward.changeAlliedRealm);
#endif
                }

                // -- apply faction modifiers
#if _iMMOFACTIONS
                foreach (UCE_FactionModifier factionModifier in quest.factionModifiers)
                {
                    UCE_AddFactionRating(factionModifier.faction, factionModifier.amount);
                }
#endif

                // -- apply world events
#if _iMMOWORLDEVENTS
                if (quest.worldEvent != null)
                    UCE_ModifyWorldEventCount(quest.worldEvent, quest.worldEventModifier);
#endif

                // -- complete quest
                quest.completed = true;
                quest.counter++;

                if (quest.repeatable > 0)
                {
                    quest.resetQuest();
                    quest.completedAgain = true;
                    quest.lastCompleted = DateTime.UtcNow.ToString("s");
                }

                UCE_quests[index] = quest;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // getQuestReward
    // -----------------------------------------------------------------------------------
    public UCE_QuestReward getQuestReward(UCE_Quest quest)
    {
        if (quest.questRewards.Length == 1)
            return quest.questRewards[0];

        // -- check class based rewards

        for (int i = 0; i < quest.questRewards.Length; i++)
        {
            if (quest.questRewards[i].availableToClass != null && quest.questRewards[i].availableToClass.Length > 0)
            {
                if (UCE_checkHasClass(quest.questRewards[i].availableToClass))
                    return quest.questRewards[i];
            }
        }

        // -- check randomized rewards

        foreach (UCE_QuestReward questReward in quest.questRewards)
        {
            if (UnityEngine.Random.value <= questReward.rewardChance)
                return questReward;
        }

        // -- return the very first reward if no one is found
        return quest.questRewards[0];
    }

    // -----------------------------------------------------------------------------------
    // getHasEnoughSpace
    // -----------------------------------------------------------------------------------
    public bool getHasEnoughSpace(UCE_Quest quest)
    {
        if (quest.questRewards.Length > 0)
        {
            foreach (UCE_QuestReward questReward in quest.questRewards)
            {
                if (InventorySlotsFree() < questReward.rewardItem.Length)
                    return false;
            }
        }

        return true;
    }

    // -----------------------------------------------------------------------------------
    // checkGatheredItems
    // -----------------------------------------------------------------------------------
    public int[] checkGatheredItems(UCE_Quest quest)
    {
        int[] gathered = new int[10];
        int j = 0;

        for (int i = 0; i < quest.gatherTarget.Length; i++)
        {
            j = i;
            gathered[j] = Mathf.Min(InventoryCount(new Item(quest.gatherTarget[j].target)), quest.gatherTarget[j].amount);
        }

        return gathered;
    }

    // -----------------------------------------------------------------------------------
}
