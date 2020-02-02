// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Linq;
using UnityEngine;

#if _iMMOHARVESTING

// PLAYER

public partial class Player
{
    [HideInInspector] public SyncListUCE_HarvestingProfession UCE_Professions;

    protected UCE_UI_HarvestingLoot UCE_harvestingUIInstance;
    [HideInInspector] public UCE_ResourceNode UCE_selectedResourceNode;

    protected bool harvestBooster = false;

    // -----------------------------------------------------------------------------------
    // UCE_OnSelect_ResourceNode
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    public void UCE_OnSelect_ResourceNode(UCE_ResourceNode _UCE_selectedResourceNode)
    {
        if (UCE_harvestingUIInstance)
            UCE_harvestingUIInstance.Hide(false);

        UCE_selectedResourceNode = _UCE_selectedResourceNode;
        movement.LookAtY(UCE_selectedResourceNode.gameObject.transform.position);
        Cmd_UCE_checkResourceNodeAccess(UCE_selectedResourceNode.gameObject);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_checkResourceNodeAccess
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    protected void Cmd_UCE_checkResourceNodeAccess(GameObject _UCE_selectedResourceNode)
    {
        UCE_selectedResourceNode = _UCE_selectedResourceNode.GetComponent<UCE_ResourceNode>();

        if (UCE_ResourceNodeValidation() && UCE_HarvestingValidation())
        {
            if (UCE_selectedResourceNode.IsDepleted() && !UCE_selectedResourceNode.HasResources())
            {
                UCE_ShowPrompt(UCE_selectedResourceNode.depletedMessage);
            }
            else
            {
                if (!UCE_selectedResourceNode.HasResources())
                    UCE_selectedResourceNode.OnRefill();

                UCE_setTimer(UCE_HarvestingDuration(harvestBooster));
                Target_UCE_startResourceNodeAccess(connectionToClient);
            }
        }
        else
        {
            if (UCE_selectedResourceNode != null && UCE_selectedResourceNode.checkInteractionRange(this))
            {
                if (UCE_selectedResourceNode.IsDepleted())
                {
                    UCE_ShowPrompt(UCE_selectedResourceNode.depletedMessage);
                }
                else
                {
                    UCE_ShowPrompt(UCE_selectedResourceNode.requirementsMessage);
                }
                UCE_cancelResourceNode();
            }
            else
            {
                movement.Navigate(this.collider.ClosestPointOnBounds(transform.position),0);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // LateUpdate_UCE_Harvesting
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    [DevExtMethods("LateUpdate")]
    private void LateUpdate_UCE_Harvesting()
    {
        if (UCE_ResourceNodeValidation() && UCE_checkTimer())
        {
            UCE_stopTimer();
            UCE_removeTask();
            UCE_CastbarHide();

            UCE_HarvestingProfessionTemplate requiredProfession = getHarvestingProfessionTemplate();

            StopAnimation(requiredProfession.animatorState, requiredProfession.stopPlayerSound);

            Destroy(indicator);

            Cmd_UCE_FinishHarvest();
        }
        else if (!UCE_ResourceNodeValidation() && UCE_timerRunning)
        {
            UCE_cancelHarvesting();
        }
    }

    // -----------------------------------------------------------------------------------
    // OnDamageDealt_UCE_cancelHarvesting
    // Custom Hook
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDamageDealt")]
    private void OnDamageDealt_UCE_cancelHarvesting()
    {
        UCE_cancelHarvesting();

        if (isServer)
            Target_UCE_Harvesting_cancelHarvestingClient(connectionToClient);
    }

    // -----------------------------------------------------------------------------------
    // UCE_cancelHarvesting
    // -----------------------------------------------------------------------------------
    public void UCE_cancelHarvesting()
    {
        if (UCE_selectedResourceNode != null)
        {
            UCE_stopTimer();
            UCE_removeTask();
            UCE_CastbarHide();

            UCE_HarvestingProfessionTemplate requiredProfession = getHarvestingProfessionTemplate();

            if (requiredProfession != null)
                StopAnimation(requiredProfession.animatorState, requiredProfession.stopPlayerSound);

            UCE_selectedResourceNode = null;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_ResourceNodeValidation (Client/Server)
    // -----------------------------------------------------------------------------------
    public bool UCE_ResourceNodeValidation()
    {
        bool bValid = (
                UCE_selectedResourceNode != null &&
                (!UCE_selectedResourceNode.IsDepleted() || UCE_selectedResourceNode.HasResources()) &&
                UCE_selectedResourceNode.checkInteractionRange(this) &&
                UCE_selectedResourceNode.interactionRequirements.checkState(this)
                );

        // ---- Cancel
        /*
        if (!bValid)
        	UCE_cancelHarvesting();
        */
        return bValid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HarvestingValidation (Client/Server)
    // -----------------------------------------------------------------------------------
    public bool UCE_HarvestingValidation()
    {
        bool bValid = UCE_ResourceNodeValidation() && mana.current >= UCE_selectedResourceNode.manaCost;

        if (bValid)
        {
            // ----- check tools

            UCE_HarvestingProfessionTemplate requiredProfession = getHarvestingProfessionTemplate();

            int check = 0;
            foreach (UCE_HarvestingTool tool in requiredProfession.tools)
            {
                if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                    check++;
            }

            if (requiredProfession.requiresAllTools)
            {
                if (check < requiredProfession.tools.Length) bValid = false;
            }
            else
            {
                if (check <= 0 && requiredProfession.tools.Length > 0) bValid = false;
            }
        }

        // ---- Cancel
        /*
        if (!bValid)
        	UCE_cancelHarvesting();
        */
        return bValid;
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_startResourceNodeAccess
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void Target_UCE_startResourceNodeAccess(NetworkConnection target)
    {
        if (UCE_ResourceNodeValidation() && UCE_HarvestingValidation())
        {
            UCE_addTask();
            UCE_setTimer(UCE_HarvestingDuration(harvestBooster));
            UCE_CastbarShow(UCE_selectedResourceNode.accessLabel, UCE_HarvestingDuration(harvestBooster));

            movement.Reset();
            movement.LookAtY(UCE_selectedResourceNode.transform.position);

            UCE_HarvestingProfessionTemplate requiredProfession = getHarvestingProfessionTemplate();

            StartAnimation(requiredProfession.animatorState, requiredProfession.startPlayerSound);
        }
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_Harvesting_cancelHarvestingClient
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void Target_UCE_Harvesting_cancelHarvestingClient(NetworkConnection target)
    {
        UCE_cancelHarvesting();
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_finishResourceNodeAccess
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void Target_UCE_finishResourceNodeAccess(NetworkConnection target)
    {
        if (UCE_ResourceNodeValidation())
        {
            if (!UCE_harvestingUIInstance)
                UCE_harvestingUIInstance = FindObjectOfType<UCE_UI_HarvestingLoot>();

            UCE_harvestingUIInstance.Show();
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_FinishHarvest (Server)
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_FinishHarvest()
    {
        if (UCE_HarvestingValidation())
        {
            UCE_removeTask();
            UCE_stopTimer();

            UCE_HarvestingResult harvestingResult = UCE_HarvestingResult.None;

            // --------------------------------------------------------------------------- Experience

            UCE_HarvestingProfessionRequirement requiredProfession = getRequiredHarvestingProfession();
            UCE_HarvestingProfession profession = getHarvestingProfession(requiredProfession);

            harvestBooster = UCE_Harvesting_CanBoost();

            float harvestChance = UCE_HarvestingProbability(harvestBooster);
            float harvestCritChance = UCE_HarvestingCriticalProbability(harvestBooster);
            int nodeLevel = requiredProfession.level;
            int oldLevel = profession.level;
            int exp = UCE_HarvestingExperience(harvestBooster);
            profession.experience += exp;

            SetHarvestingProfession(profession);

            if (exp > 0)
                UCE_TargetAddMessage(exp.ToString() + " " + profession.template.name + " " + UCE_selectedResourceNode.experienceMessage);

#if _iMMOTOOLS
            if (oldLevel < profession.level)
                UCE_ShowPopup(UCE_selectedResourceNode.levelUpMessage + profession.templateName + " [L" + profession.level + "]");
#endif

            // --------------------------------------------------------------------------- Skill Check

            if (UnityEngine.Random.value <= harvestChance)
            {
                harvestingResult = UCE_HarvestingResult.Success;

                if (UnityEngine.Random.value <= harvestCritChance)
                    harvestingResult = UCE_HarvestingResult.CriticalSuccess;
            }
            else
            {
                harvestingResult = UCE_HarvestingResult.Failure;
            }

            // --------------------------------------------------------------------------- Deplete other Costs (mana etc.)

            mana.current -= UCE_selectedResourceNode.manaCost;

            // --------------------------------------------------------------------------- Check Tool breakage

            foreach (UCE_HarvestingTool tool in profession.template.tools)
            {
                if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                {
                    if (UnityEngine.Random.value <= tool.toolDestroyChance)
                    {
                        if (tool.equippedItem)
                            UCE_removeEquipment(tool.requiredItem);
                        else
                            inventory.Remove(new Item(tool.requiredItem), 1);
                        UCE_TargetAddMessage(UCE_selectedResourceNode.breakMessage);
                    }
                }
            }

            if (harvestBooster)
            {
                foreach (UCE_HarvestingTool tool in profession.template.optionalTools)
                {
                    if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                    {
                        if (UnityEngine.Random.value <= tool.toolDestroyChance)
                        {
                            if (tool.equippedItem)
                                UCE_removeEquipment(tool.requiredItem);
                            else
                                inventory.Remove(new Item(tool.requiredItem), 1);
                            UCE_TargetAddMessage(UCE_selectedResourceNode.boosterMessage);
                        }
                    }
                }
            }

            // --------------------------------------------------------------------------- Resources

            if (harvestingResult != UCE_HarvestingResult.Failure)
            {
                if (harvestingResult == UCE_HarvestingResult.CriticalSuccess)
                {
                    UCE_selectedResourceNode.OnCritical();
                    UCE_TargetAddMessage(UCE_selectedResourceNode.criticalSuccessMessage);
                }
                else
                {
                    UCE_TargetAddMessage(UCE_selectedResourceNode.successMessage);
                }

                Target_UCE_finishResourceNodeAccess(connectionToClient);

                UCE_selectedResourceNode.OnHarvested();
                UCE_selectedResourceNode.OnDepleted();

#if _iMMOHARVESTING && _iMMOQUESTS
                UCE_IncreaseHarvestNodeCounterFor(profession.template);
#endif
            }
            else
            {
                UCE_ShowPrompt(UCE_selectedResourceNode.failedMessage);
            }

            // --------------------------------------------------------------------------- Cleanup

            //UCE_cancelHarvesting();
            harvestBooster = false;
        }
    }

    // -----------------------------------------------------------------------------------
    // OnDamageDealt_UCE_cancelResourceNode
    // Custom Hook
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDamageDealt")]
    private void OnDamageDealt_UCE_cancelResourceNode()
    {
        UCE_cancelResourceNode();
    }

    // -----------------------------------------------------------------------------------
    // UCE_cancelResourceNode
    // -----------------------------------------------------------------------------------
    public void UCE_cancelResourceNode()
    {
        UCE_stopTimer();
        UCE_removeTask();
        UCE_CastbarHide();
        UCE_selectedResourceNode = null;
        harvestBooster = false;
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_TakeHarvestingResources
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_TakeHarvestingResources(int index)
    {
        if (
            UCE_ResourceNodeValidation() &&
            0 <= index && index < UCE_selectedResourceNode.inventory.Count &&
            UCE_selectedResourceNode.inventory[index].amount > 0)
        {
            ItemSlot slot = UCE_selectedResourceNode.inventory[index];

            // try to add it to the inventory, clear monster slot if it worked
            if (inventory.Add(slot.item, slot.amount))
            {
                slot.amount = 0;
                UCE_selectedResourceNode.inventory[index] = slot;
            }

            // check if resource is depleted (otherwise it takes too long to update)
            if (UCE_selectedResourceNode.EventDepleted())
                UCE_selectedResourceNode.OnDepleted();
        }
    }

    // ============================== DURATION/PROBABILITY ===============================

    // -----------------------------------------------------------------------------------
    // UCE_Harvesting_CanBoost
    // -----------------------------------------------------------------------------------
    public bool UCE_Harvesting_CanBoost()
    {
        UCE_HarvestingProfessionRequirement requiredProfession = getRequiredHarvestingProfession();
        UCE_HarvestingProfession profession = getHarvestingProfession(requiredProfession);

        if (profession.template.optionalTools.Length <= 0) return false;

        bool bValid = false;

        foreach (UCE_HarvestingTool tool in profession.template.optionalTools)
        {
            if (
                (!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) ||
                (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem))
                )
            {
                bValid = true;
            }
            else
            {
                bValid = false;
            }
        }

        return bValid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HarvestingProbability
    // -----------------------------------------------------------------------------------
    public float UCE_HarvestingProbability(bool boost)
    {
        float proba = 0f;

        UCE_HarvestingProfessionRequirement requiredProfession = getRequiredHarvestingProfession();
        UCE_HarvestingProfession profession = getHarvestingProfession(requiredProfession);

        // -- Modificator: Profession Skill

        proba = profession.template.baseHarvestChance;

        // -- Modificator: Node Level vs. Skill Level

        if (profession.level > requiredProfession.level)
            proba += (profession.level - requiredProfession.level) * profession.template.probabilityPerSkillLevel;

        // -- Modificator: Required Tools

        foreach (UCE_HarvestingTool tool in profession.template.tools)
        {
            if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
            {
                proba += tool.modifyProbability;
                if (!profession.template.requiresAllTools)
                    break;
            }
        }

        // -- Modificator: Optional Tools

        if (boost)
        {
            foreach (UCE_HarvestingTool tool in profession.template.optionalTools)
            {
                if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                {
                    proba += tool.modifyProbability;
                    break;
                }
            }
        }

        return proba;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HarvestingCriticalProbability
    // -----------------------------------------------------------------------------------
    public float UCE_HarvestingCriticalProbability(bool boost)
    {
        float proba = 0f;

        UCE_HarvestingProfessionRequirement requiredProfession = getRequiredHarvestingProfession();
        UCE_HarvestingProfession profession = getHarvestingProfession(requiredProfession);

        // -- Modificator: Profession Skill

        proba = profession.template.criticalHarvestChance + (profession.level * profession.template.criticalProbabilityPerSkillLevel);

        // -- Modificator: Required Tools

        foreach (UCE_HarvestingTool tool in profession.template.tools)
        {
            if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
            {
                proba += tool.modifyCriticalProbability;
                if (!profession.template.requiresAllTools)
                    break;
            }
        }

        // -- Modificator: Optional Tools

        if (boost)
        {
            foreach (UCE_HarvestingTool tool in profession.template.optionalTools)
            {
                if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                {
                    proba += tool.modifyCriticalProbability;
                    break;
                }
            }
        }

        return proba;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HarvestingDuration
    // -----------------------------------------------------------------------------------
    public float UCE_HarvestingDuration(bool boost)
    {
        float duration = 0f;
        int level = 0;

        UCE_HarvestingProfessionRequirement requiredProfession = getRequiredHarvestingProfession();
        UCE_HarvestingProfession profession = getHarvestingProfession(requiredProfession);

        duration = UCE_selectedResourceNode.harvestDuration;

        // -- Modificator: Skill

        duration += level * profession.template.durationPerSkillLevel;

        // -- Modificator: Required Tools

        foreach (UCE_HarvestingTool tool in profession.template.tools)
        {
            if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
            {
                duration += tool.modifyDuration;
                if (!profession.template.requiresAllTools)
                    break;
            }
        }

        // -- Modificator: Optional Tools

        if (boost)
        {
            foreach (UCE_HarvestingTool tool in profession.template.optionalTools)
            {
                if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                {
                    duration += tool.modifyDuration;
                    break;
                }
            }
        }

        return duration;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HarvestingExperience
    // -----------------------------------------------------------------------------------
    public int UCE_HarvestingExperience(bool boost)
    {
        int exp = 0;

        UCE_HarvestingProfessionRequirement requiredProfession = getRequiredHarvestingProfession();
        UCE_HarvestingProfession profession = getHarvestingProfession(requiredProfession);

        // -- Modificator: Resource Node

        exp = UnityEngine.Random.Range(UCE_selectedResourceNode.ProfessionExperienceRewardMin, UCE_selectedResourceNode.ProfessionExperienceRewardMax);

        // -- Modificator: Required Tools

        foreach (UCE_HarvestingTool tool in profession.template.tools)
        {
            if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
            {
                exp += UnityEngine.Random.Range(tool.modifyExperienceMin, tool.modifyExperienceMax);
                if (!profession.template.requiresAllTools)
                    break;
            }
        }

        // -- Modificator: Optional Tools

        if (boost)
        {
            foreach (UCE_HarvestingTool tool in profession.template.optionalTools)
            {
                if ((!tool.equippedItem && inventory.Count(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                {
                    exp += UnityEngine.Random.Range(tool.modifyExperienceMin, tool.modifyExperienceMax);
                    break;
                }
            }
        }

        return exp;
    }

    // ================================== PROFESSIONS ====================================

    // -----------------------------------------------------------------------------------
    // getHarvestingProfession
    // -----------------------------------------------------------------------------------
    public UCE_HarvestingProfession getHarvestingProfession(UCE_HarvestingProfessionTemplate tmpl)
    {
        if (HasHarvestingProfession(tmpl))
        {
            int id = UCE_Professions.FindIndex(x => x.templateName == tmpl.name);
            return UCE_Professions[id];
        }

        return new UCE_HarvestingProfession();
    }

    // -----------------------------------------------------------------------------------
    // getHarvestingProfession
    // Returns the required profession of the player, that the player is capable of using
    // -----------------------------------------------------------------------------------
    public UCE_HarvestingProfession getHarvestingProfession(UCE_HarvestingProfessionRequirement[] tmpls)
    {
        foreach (UCE_HarvestingProfessionRequirement tmpl in tmpls)
        {
            if (HasHarvestingProfessionLevel(tmpl.template, tmpl.level))
            {
                int id = UCE_Professions.FindIndex(x => x.templateName == tmpl.template.name);
                return UCE_Professions[id];
            }
        }

        return new UCE_HarvestingProfession();
    }

    // -----------------------------------------------------------------------------------
    // getHarvestingProfession
    // Returns the required profession of the player, that the player is capable of using
    // -----------------------------------------------------------------------------------
    public UCE_HarvestingProfession getHarvestingProfession(UCE_HarvestingProfessionRequirement tmpl)
    {
        if (HasHarvestingProfessionLevel(tmpl.template, tmpl.level))
        {
            int id = UCE_Professions.FindIndex(x => x.templateName == tmpl.template.name);
            return UCE_Professions[id];
        }

        return new UCE_HarvestingProfession();
    }

    // -----------------------------------------------------------------------------------
    // getRequiredHarvestingProfession
    // Returns the required profession to harvest the selected resource node
    // -----------------------------------------------------------------------------------
    public UCE_HarvestingProfessionRequirement getRequiredHarvestingProfession()
    {
        if (UCE_selectedResourceNode == null) return null;

        foreach (UCE_HarvestingProfessionRequirement tmpl in UCE_selectedResourceNode.interactionRequirements.harvestProfessionRequirements)
        {
            if (HasHarvestingProfessionLevel(tmpl.template, tmpl.level))
            {
                return tmpl;
            }
        }

        return null;
    }

    // -----------------------------------------------------------------------------------
    // getHarvestingProfessionTemplate
    // Returns the required template to harvest the selected resource node
    // -----------------------------------------------------------------------------------
    public UCE_HarvestingProfessionTemplate getHarvestingProfessionTemplate()
    {
        if (UCE_selectedResourceNode == null) return null;

        foreach (UCE_HarvestingProfessionRequirement tmpl in UCE_selectedResourceNode.interactionRequirements.harvestProfessionRequirements)
        {
            if (HasHarvestingProfessionLevel(tmpl.template, tmpl.level))
            {
                return tmpl.template;
            }
        }

        return null;
    }

    // -----------------------------------------------------------------------------------
    // getProfessionExp
    // -----------------------------------------------------------------------------------
    public long getHarvestingProfessionExp(UCE_HarvestingProfession aProf)
    {
        int id = UCE_Professions.FindIndex(prof => prof.templateName == aProf.templateName);
        return UCE_Professions[id].experience;
    }

    // -----------------------------------------------------------------------------------
    // SetProfession
    // -----------------------------------------------------------------------------------
    public void SetHarvestingProfession(UCE_HarvestingProfession aProf)
    {
        int id = UCE_Professions.FindIndex(pr => pr.templateName == aProf.template.name);
        UCE_Professions[id] = aProf;
    }

    // -----------------------------------------------------------------------------------
    // HasHarvestingProfession
    // -----------------------------------------------------------------------------------
    public bool HasHarvestingProfession(UCE_HarvestingProfessionTemplate profession)
    {
        return UCE_Professions.Any(x => x.templateName == profession.name);
    }

    // -----------------------------------------------------------------------------------
    // UCE_HarvestingProfession
    // -----------------------------------------------------------------------------------
    public UCE_HarvestingProfession UCE_getHarvestingProfession(UCE_HarvestingProfessionTemplate tmpl)
    {
        return UCE_Professions.First(x => x.templateName == tmpl.name);
    }

    // -----------------------------------------------------------------------------------
    // HasHarvestingProfessions
    // -----------------------------------------------------------------------------------
    public bool HasHarvestingProfessions(UCE_HarvestingProfessionRequirement[] tmpls, bool requiresAll = false)
    {
        if (tmpls == null || tmpls.Length == 0) return true;

        bool valid = false;

        foreach (UCE_HarvestingProfessionRequirement tmpl in tmpls)
        {
            if (HasHarvestingProfessionLevel(tmpl.template, tmpl.level))
            {
                valid = true;
                if (!requiresAll) return valid;
            }
            else
            {
                valid = false;
            }
        }

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // HasHarvestingProfessionLevel
    // -----------------------------------------------------------------------------------
    public bool HasHarvestingProfessionLevel(UCE_HarvestingProfessionTemplate aProf, int level)
    {
        if (HasHarvestingProfession(aProf))
        {
            var tmpProf = getHarvestingProfession(aProf);
            if (tmpProf.level >= level) return true;
        }
        return false;
    }

    // -----------------------------------------------------------------------------------
}

#endif
