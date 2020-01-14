// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if _iMMOCRAFTING

// PLAYER

public partial class Player
{
    [Header("[-=-=-=- UCE CRAFTING [See Tooltips] -=-=-=-]")]
    [Tooltip("[Optional] Default recipes the player starts the game with.")]
    public UCE_Tmpl_Recipe[] startingRecipes;

    [Tooltip("[Optional] Default crafts the player starts the game with.")]
    public UCE_DefaultCraftingProfession[] startingCrafts;


    [Tooltip("[Optional] Popup text, sound and icons (as defined in Tools).")]
    public UCE_CraftingPopupMessages craftingPopupMessages;

    [HideInInspector] public SyncListUCE_CraftingProfession UCE_Crafts;
    [HideInInspector] public SyncListString UCE_recipes = new SyncListString();
    [HideInInspector] public UCE_InteractableWorkbench UCE_selectedWorkbench;

    protected bool craftBooster = false;
    protected int craftAmount = 1;
    protected UCE_Tmpl_Recipe UCE_myRecipe;
    protected UCE_UI_Crafting _UCE_UI_Crafting;

    // -----------------------------------------------------------------------------------
    // OnSelect_UCE_Crafting_Workbench
    // Selection handling for a clicked workbench, client side only
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    public void UCE_OnSelect_InteractableWorkbench(UCE_InteractableWorkbench _UCE_selectedWorkbench)
    {
        UCE_selectedWorkbench = _UCE_selectedWorkbench;

        UCE_CraftingProfessionRequirement requiredProfession = getRequiredCraftingProfession();

        if (requiredProfession != null)
        {
            List<UCE_Tmpl_Recipe> recipes = new List<UCE_Tmpl_Recipe>();

            // -- filter recipes that match the workbench's profession
            // -- filter recipes that match the players recipe list
            recipes.AddRange(
                    UCE_Tmpl_Recipe.dict.Values.ToList().Where(
                        x => x.requiredCraft == requiredProfession.template &&
                        UCE_recipes.Any(r => r == x.name)
                        )
                    );

            if (recipes.Count > 0)
            {
                if (!_UCE_UI_Crafting)
                    _UCE_UI_Crafting = FindObjectOfType<UCE_UI_Crafting>();

                _UCE_UI_Crafting.Show(UCE_selectedWorkbench.gameObject, requiredProfession.template, recipes);
            }
            else
            {
                UCE_PopupShow(UCE_selectedWorkbench.nothingMessage);
            }
        }
        else
        {
            UCE_PopupShow(UCE_selectedWorkbench.nothingMessage);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_Crafting_LearnRecipe
    // -----------------------------------------------------------------------------------
    public bool UCE_Crafting_LearnRecipe(UCE_Tmpl_Recipe recipe)
    {
        if (!UCE_recipes.Any(s => s == recipe.name))
        {
            UCE_recipes.Add(recipe.name);
            UCE_ShowPopup(craftingPopupMessages.learnedMessage + recipe.name, craftingPopupMessages.learnedIconId, craftingPopupMessages.learnedSoundId);
            return true;
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_Crafting_LearnRecipe
    // -----------------------------------------------------------------------------------
    public bool UCE_Crafting_LearnRecipe(UCE_Tmpl_Recipe[] recipes)
    {
        bool valid = false;

        foreach (UCE_Tmpl_Recipe recipe in recipes)
            valid = UCE_Crafting_LearnRecipe(recipe);

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_Crafting_isBusy
    // -----------------------------------------------------------------------------------
    public bool UCE_Crafting_isBusy()
    {
        return UCE_myRecipe != null && UCE_checkTimer();
    }

    // -----------------------------------------------------------------------------------
    // UCE_Crafting_CraftValidation
    // -----------------------------------------------------------------------------------
    public bool UCE_Crafting_CraftValidation(UCE_Tmpl_Recipe recipe, int amount, bool boost, bool craftable = true)
    {
        bool valid = true;

        // ----- set crafting booster
        craftBooster = boost;

        // ----- check profession level
        if (!UCE_HasCraftingProfessionLevel(recipe.requiredCraft, recipe.minSkillLevel))
            valid = false;

        if (!craftable) return valid;

        // ----- check ingredients
        foreach (UCE_CraftingRecipeIngredient ingredient in recipe.ingredients)
        {
            if (InventoryCount(new Item(ingredient.item)) < (ingredient.amount * amount))
                valid = false;
        }

        // ----- check mana
        if (mana < recipe.manaCost)
            valid = false;

        // ----- check tools
        int check = 0;
        foreach (UCE_CraftingRecipeTool tool in recipe.tools)
        {
            if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                check++;
        }

        if (recipe.requiresAllTools)
        {
            if (check < recipe.tools.Length) valid = false;
        }
        else
        {
            if (check <= 0 && recipe.tools.Length > 0) valid = false;
        }

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // OnDamageDealt_UCE_cancelCrafting
    // Custom Hook
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDamageDealt")]
    private void OnDamageDealt_UCE_cancelCrafting()
    {
        UCE_cancelCrafting();

        if (isServer)
            Target_UCE_Crafting_cancelCraftingClient(connectionToClient);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Crafting_unlearnRecipe
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_Crafting_unlearnRecipe(string recipeName)
    {
        UCE_recipes.Remove(recipeName);
    }

    // -----------------------------------------------------------------------------------
    // UCE_Crafting_startCrafting
    // -----------------------------------------------------------------------------------
    public void UCE_Crafting_startCrafting(UCE_Tmpl_Recipe recipe, int amount, bool boost)
    {
        UCE_myRecipe = recipe;
        craftAmount = amount;

        if (UCE_WorkbenchValidation() && UCE_Crafting_CraftValidation(UCE_myRecipe, amount, boost))
        {
            UCE_addTask();

            float duration = UCE_CraftingDuration(UCE_myRecipe, boost) * amount;
            UCE_setTimer(duration);
            UCE_CastbarShow(UCE_myRecipe.name, duration);

            StartAnimation(UCE_CraftingAnimation().playerAnimation, UCE_CraftingAnimation().startPlayerSound);
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Crafting_finishCrafting
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_Crafting_finishCrafting(GameObject _UCE_selectedWorkbench, string recipeName, int amount, bool boost)
    {
        UCE_selectedWorkbench = _UCE_selectedWorkbench.GetComponent<UCE_InteractableWorkbench>();

        UCE_CraftingResult craftingResult = UCE_CraftingResult.None;

        // -- validation (in case something changed during the craft duration)

        if (UCE_selectedWorkbench && UCE_Tmpl_Recipe.dict.TryGetValue(recipeName.GetDeterministicHashCode(), out UCE_myRecipe))
        {
            if (UCE_Crafting_CraftValidation(UCE_myRecipe, amount, boost))
            {
                UCE_removeTask();

                // --------------------------------------------------------------------------- Experience

                var prof = UCE_getCraftingProfession(UCE_myRecipe.requiredCraft);
                int oldLevel = prof.level;
                int exp = UCE_CraftingExperience(UCE_myRecipe, amount, boost);
                prof.experience += exp;
                UCE_setCraftingProfession(prof);

#if _iMMOTOOLS
                if (oldLevel < prof.level)
                    UCE_ShowPopup(UCE_selectedWorkbench.levelUpMessage + prof.templateName + " [L" + prof.level + "]");
#endif

                // --------------------------------------------------------------------------- Skill Check

                float successProbability = UCE_CraftingProbability(UCE_myRecipe, boost);
                float criticalProbability = UCE_CraftingCriticalProbability(UCE_myRecipe, boost);

                for (int i = 1; i <= amount; i++)

                    if (UnityEngine.Random.value <= successProbability)
                    {
                        // ---- Success

                        craftingResult = UCE_CraftingResult.Success;

                        experience += UCE_myRecipe.experience;
                        skillExperience += UCE_myRecipe.skillExp;

                        float critProba = UCE_myRecipe.criticalProbability += UCE_myRecipe.criticalResultPerSkillLevel * oldLevel;

                        // ---- gain default or critical item

                        if (UnityEngine.Random.value <= criticalProbability)
                        {
                            craftingResult = UCE_CraftingResult.CriticalSuccess;

                            if (UCE_myRecipe.criticalResult.Length > 0)
                            {
                                foreach (UCE_CraftingRecipeIngredient result in UCE_myRecipe.criticalResult)
                                    InventoryAdd(new Item(result.item), result.amount);
                            }
                            else
                            {
                                foreach (UCE_CraftingRecipeIngredient result in UCE_myRecipe.defaultResult)
                                    InventoryAdd(new Item(result.item), result.amount);
                            }
                        }
                        else
                        {
                            foreach (UCE_CraftingRecipeIngredient result in UCE_myRecipe.defaultResult)
                                InventoryAdd(new Item(result.item), result.amount);
                        }

#if _iMMOCRAFTING && _iMMOQUESTS
                        UCE_IncreaseCraftCounterFor(UCE_myRecipe);
#endif
                    }
                    else
                    {
                        // --------------------------------------------------------------------------- Failure

                        craftingResult = UCE_CraftingResult.Failure;

                        // --------------------------------------------------------------------------- [Optional] Gain failure item

                        float failProba = UCE_myRecipe.failureProbability += UCE_myRecipe.failureResultPerSkillLevel * oldLevel;

                        if (UnityEngine.Random.value <= failProba)
                        {
                            foreach (UCE_CraftingRecipeIngredient result in UCE_myRecipe.failureResult)
                                InventoryAdd(new Item(result.item), result.amount);
                        }
                    }

                // --------------------------------------------------------------------------- Deplete Ingredients

                foreach (UCE_CraftingRecipeIngredient ingredient in UCE_myRecipe.ingredients)
                {
                    if (craftingResult == UCE_CraftingResult.Failure && ingredient.DontDestroyOnFailure) continue;
                    if (craftingResult == UCE_CraftingResult.CriticalSuccess && ingredient.DontDestroyOnCriticalSuccess) continue;
                    InventoryRemove(new Item(ingredient.item), (ingredient.amount * amount));
                }

                // --------------------------------------------------------------------------- Deplete other Costs (mana etc.)

                mana -= UCE_myRecipe.manaCost;

                // --------------------------------------------------------------------------- Check Tool breakage

                foreach (UCE_CraftingRecipeTool tool in UCE_myRecipe.tools)
                {
                    if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                    {
                        if (UnityEngine.Random.value <= tool.toolDestroyChance)
                        {
                            if (tool.equippedItem)
                                UCE_removeEquipment(tool.requiredItem);
                            else
                                InventoryRemove(new Item(tool.requiredItem), 1);
                            UCE_TargetAddMessage(craftingPopupMessages.breakMessage);
                        }
                    }
                }

                if (boost)
                {
                    foreach (UCE_CraftingRecipeTool tool in UCE_myRecipe.optionalTools)
                    {
                        if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= amount) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                        {
                            if (UnityEngine.Random.value <= tool.toolDestroyChance)
                            {
                                if (tool.equippedItem)
                                    UCE_removeEquipment(tool.requiredItem);
                                else
                                    InventoryRemove(new Item(tool.requiredItem), amount);
                                UCE_TargetAddMessage(craftingPopupMessages.boosterMessage);
                            }
                        }
                    }
                }

                // --------------------------------------------------------------------------- Popup Message

                if (craftingResult == UCE_CraftingResult.Failure)
                {
                    UCE_ShowPopup(craftingPopupMessages.failMessage, (byte)craftingPopupMessages.failIconId, (byte)craftingPopupMessages.failSoundId);
                }
                else if (craftingResult == UCE_CraftingResult.Success)
                {
                    UCE_ShowPopup(craftingPopupMessages.successMessage, (byte)craftingPopupMessages.successIconId, (byte)craftingPopupMessages.successSoundId);
                }
                else if (craftingResult == UCE_CraftingResult.CriticalSuccess)
                {
                    UCE_ShowPopup(craftingPopupMessages.critMessage, (byte)craftingPopupMessages.critIconId, (byte)craftingPopupMessages.critSoundId);
                }

                // --------------------------------------------------------------------------- Cleanup

                UCE_selectedWorkbench.OnCrafted();

                Target_UCE_Crafting_cancelCraftingClient(connectionToClient);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_Crafting_finishCraftingClient
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void Target_UCE_Crafting_cancelCraftingClient(NetworkConnection target)
    {
        UCE_cancelCrafting();
    }

    // -----------------------------------------------------------------------------------
    // UCE_CraftingProfessionTemplate
    // -----------------------------------------------------------------------------------
    public UCE_CraftingProfessionTemplate UCE_CraftingAnimation()
    {
        UCE_CraftingProfessionRequirement requiredProfession = getRequiredCraftingProfession();

        if (requiredProfession != null)
            return requiredProfession.template;

        return null;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CraftingValidation
    // -----------------------------------------------------------------------------------
    public bool UCE_WorkbenchValidation()
    {
        bool bValid = (
                UCE_selectedWorkbench != null &&
                UCE_selectedWorkbench.checkInteractionRange(this) &&
                UCE_selectedWorkbench.interactionRequirements.checkState(this)
                );

        if (!bValid)
            UCE_cancelCrafting();

        return bValid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_finishCraftingClient
    // -----------------------------------------------------------------------------------
    public void UCE_cancelCrafting()
    {
        if (UCE_selectedWorkbench != null || UCE_myRecipe != null)
        {
            UCE_stopTimer();
            UCE_removeTask();
            UCE_CastbarHide();

            if (UCE_myRecipe != null && UCE_CraftingAnimation() != null)
                StopAnimation(UCE_CraftingAnimation().playerAnimation, UCE_CraftingAnimation().stopPlayerSound);

            craftBooster = false;
            craftAmount = 1;

            UCE_myRecipe = null;
            UCE_selectedWorkbench = null;
        }
    }

    // -----------------------------------------------------------------------------------
    // LateUpdate_UCE_Workbench
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    [DevExtMethods("LateUpdate")]
    private void LateUpdate_UCE_Workbench()
    {
        if (UCE_WorkbenchValidation())
        {
            if (UCE_myRecipe && UCE_checkTimer())
            {
                Cmd_UCE_Crafting_finishCrafting(UCE_selectedWorkbench.gameObject, UCE_myRecipe.name, craftAmount, craftBooster);
                UCE_cancelCrafting();
            }
        }
    }

    // ============================== DURATION/PROBABILITY ===============================

    // -----------------------------------------------------------------------------------
    // UCE_Crafting_CanBoost
    // -----------------------------------------------------------------------------------
    public bool UCE_Crafting_CanBoost(UCE_Tmpl_Recipe recipe, int amount)
    {
        if (recipe.optionalTools.Length <= 0) return false;

        bool bValid = false;

        foreach (UCE_CraftingRecipeTool tool in recipe.optionalTools)
        {
            if (
                (!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= amount) ||
                (tool.equippedItem && tool.requiredItem.maxStack == 1 && UCE_checkHasEquipment(tool.requiredItem)) ||
                (tool.equippedItem && tool.requiredItem.maxStack > 1 && UCE_checkDepletableEquipment(tool.requiredItem))
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
    // UCE_CraftingProbability
    // -----------------------------------------------------------------------------------
    public float UCE_CraftingProbability(UCE_Tmpl_Recipe recipe, bool boost)
    {
        float proba = 0f;
        int level = 0;

        if (recipe != null)
        {
            proba = recipe.probability;

            // -- Modificator: Skill

            if (recipe.requiredCraft)
                level = UCE_getCraftingProfessionLevel(recipe.requiredCraft);

            proba += level * recipe.probabilityPerSkillLevel;

            // -- Modificator: Required Tools

            foreach (UCE_CraftingRecipeTool tool in recipe.tools)
            {
                if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                {
                    proba += tool.modifyProbability;
                    if (!recipe.requiresAllTools)
                        break;
                }
            }

            // -- Modificator: Optional Tools

            if (boost)
            {
                foreach (UCE_CraftingRecipeTool tool in recipe.optionalTools)
                {
                    if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                    {
                        proba += tool.modifyProbability;
                        break;
                    }
                }
            }
        }

        return proba;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CraftingCriticalProbability
    // -----------------------------------------------------------------------------------
    public float UCE_CraftingCriticalProbability(UCE_Tmpl_Recipe recipe, bool boost)
    {
        float proba = 0f;
        int level = 0;

        if (recipe != null)
        {
            proba = recipe.criticalProbability;

            // -- Modificator: Skill

            if (recipe.requiredCraft)
                level = UCE_getCraftingProfessionLevel(recipe.requiredCraft);

            proba += level * recipe.probabilityPerSkillLevel;

            // -- Modificator: Required Tools

            foreach (UCE_CraftingRecipeTool tool in recipe.tools)
            {
                if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                {
                    proba += tool.modifyCriticalProbability;
                    if (!recipe.requiresAllTools)
                        break;
                }
            }

            // -- Modificator: Optional Tools

            if (boost)
            {
                foreach (UCE_CraftingRecipeTool tool in recipe.optionalTools)
                {
                    if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                    {
                        proba += tool.modifyCriticalProbability;
                        break;
                    }
                }
            }
        }

        return proba;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CraftingDuration
    // -----------------------------------------------------------------------------------
    public float UCE_CraftingDuration(UCE_Tmpl_Recipe recipe, bool boost)
    {
        float duration = 0f;
        int level = 0;

        if (recipe != null)
        {
            duration = recipe.duration;

            // -- Modificator: Skill

            if (recipe.requiredCraft)
                level = UCE_getCraftingProfessionLevel(recipe.requiredCraft);

            duration += level * recipe.durationPerSkillLevel;

            // -- Modificator: Required Tools

            foreach (UCE_CraftingRecipeTool tool in recipe.tools)
            {
                if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                {
                    duration += tool.modifyDuration;
                    if (!recipe.requiresAllTools)
                        break;
                }
            }

            // -- Modificator: Optional Tools

            if (boost)
            {
                foreach (UCE_CraftingRecipeTool tool in recipe.optionalTools)
                {
                    if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                    {
                        duration += tool.modifyDuration;
                        break;
                    }
                }
            }
        }

        return duration;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CraftingExperience
    // -----------------------------------------------------------------------------------
    public int UCE_CraftingExperience(UCE_Tmpl_Recipe recipe, int amount, bool boost)
    {
        int experience = 0;

        if (recipe != null)
        {
            experience = UnityEngine.Random.Range(recipe.ProfessionExperienceRewardMin, recipe.ProfessionExperienceRewardMax) * amount;

            // -- Modificator: Required Tools

            foreach (UCE_CraftingRecipeTool tool in recipe.tools)
            {
                if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                {
                    experience += UnityEngine.Random.Range(tool.modifyExperienceMin, tool.modifyExperienceMax);
                    if (!recipe.requiresAllTools)
                        break;
                }
            }

            // -- Modificator: Optional Tools

            if (boost)
            {
                foreach (UCE_CraftingRecipeTool tool in recipe.optionalTools)
                {
                    if ((!tool.equippedItem && InventoryCount(new Item(tool.requiredItem)) >= 1) || (tool.equippedItem && UCE_checkHasEquipment(tool.requiredItem)))
                    {
                        experience += UnityEngine.Random.Range(tool.modifyExperienceMin, tool.modifyExperienceMax);
                        break;
                    }
                }
            }
        }

        return experience;
    }

    // ============================== PROFESSION ===============================

    // -----------------------------------------------------------------------------------
    // UCE_getCraftingProfession
    // -----------------------------------------------------------------------------------
    public UCE_CraftingProfession UCE_getCraftingProfession(UCE_CraftingProfessionTemplate aProf)
    {
        return UCE_Crafts.First(pr => pr.templateName == aProf.name);
    }

    // -----------------------------------------------------------------------------------
    // UCE_getCraftingProfessionLevel
    // -----------------------------------------------------------------------------------
    public int UCE_getCraftingProfessionLevel(UCE_CraftingProfessionTemplate aProf)
    {
        return UCE_Crafts.First(pr => pr.templateName == aProf.name).level;
    }

    // -----------------------------------------------------------------------------------
    // UCE_getCraftingExp
    // -----------------------------------------------------------------------------------
    public long UCE_getCraftingExp(UCE_CraftingProfession aProf)
    {
        int id = UCE_Crafts.FindIndex(prof => prof.templateName == aProf.templateName);
        return UCE_Crafts[id].experience;
    }

    // -----------------------------------------------------------------------------------
    // UCE_setCraftingProfession
    // -----------------------------------------------------------------------------------
    public void UCE_setCraftingProfession(UCE_CraftingProfession aProf)
    {
        int id = UCE_Crafts.FindIndex(pr => pr.templateName == aProf.template.name);
        UCE_Crafts[id] = aProf;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HasCraftingProfession
    // -----------------------------------------------------------------------------------
    public bool UCE_HasCraftingProfession(UCE_CraftingProfessionTemplate aProf)
    {
        return UCE_Crafts.Any(prof => prof.templateName == aProf.name);
    }

    // -----------------------------------------------------------------------------------
    // getRequiredCraftingProfession
    // Returns the required profession to access the selected workbench
    // -----------------------------------------------------------------------------------
    public UCE_CraftingProfessionRequirement getRequiredCraftingProfession()
    {
        if (UCE_selectedWorkbench == null) return null;

        foreach (UCE_CraftingProfessionRequirement tmpl in UCE_selectedWorkbench.interactionRequirements.craftProfessionRequirements)
        {
            if (tmpl != null && UCE_HasCraftingProfessionLevel(tmpl.template, tmpl.level))
            {
                return tmpl;
            }
        }

        return null;
    }

    // -----------------------------------------------------------------------------------
    // HasCraftingProfessions
    // -----------------------------------------------------------------------------------
    public bool UCE_HasCraftingProfessions(UCE_CraftingProfessionRequirement[] tmpls, bool requiresAll = false)
    {
        if (tmpls == null || tmpls.Length == 0) return true;

        bool valid = false;

        foreach (UCE_CraftingProfessionRequirement tmpl in tmpls)
        {
            if (UCE_HasCraftingProfessionLevel(tmpl.template, tmpl.level))
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
    // UCE_HasCraftingProfessionLevel
    // -----------------------------------------------------------------------------------
    public bool UCE_HasCraftingProfessionLevel(UCE_CraftingProfessionTemplate aProf, int level)
    {
        if (aProf == null || level <= 0) return true;

        if (UCE_HasCraftingProfession(aProf))
        {
            var tmpProf = UCE_getCraftingProfession(aProf);
            if (tmpProf.level >= level) return true;
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
}

#endif
