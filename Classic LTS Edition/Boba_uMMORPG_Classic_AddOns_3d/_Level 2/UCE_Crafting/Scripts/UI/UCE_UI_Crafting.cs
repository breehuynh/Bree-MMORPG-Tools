// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using UnityEngine;

#if _iMMOCRAFTING

// UCE CRAFTING

public partial class UCE_UI_Crafting : MonoBehaviour
{
    public GameObject panel;
    public UCE_UI_CraftingUnlearn unlearnPanel;
    public UCE_Slot_Crafting slotPrefab;
    public Transform content;
    public UCE_UI_CraftingButton[] categoryButtons;
    public string categoryAll = "All";

    private string currentCategory;
    private List<UCE_Tmpl_Recipe> recipes;
    private UCE_CraftingProfessionTemplate profession;

    protected GameObject instance;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Refresh();
    }

    // -----------------------------------------------------------------------------------
    // Refresh
    // -----------------------------------------------------------------------------------
    public void Refresh()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (panel.activeSelf &&
            instance != null &&
            UCE_Tools.UCE_CheckSelectionHandling(instance)
            )
        {
            int rcount = getRecipeCount();
            int t_index = -1;
            int r_index = 0;

            UIUtils.BalancePrefabs(slotPrefab.gameObject, rcount, content);

            for (int i = 0; i < recipes.Count; ++i)
            {
                if (canCraft(i))
                {
                    r_index = i;
                    t_index++;

                    UCE_Tmpl_Recipe recipe = recipes[r_index];

                    UCE_Slot_Crafting slot = content.GetChild(t_index).GetComponent<UCE_Slot_Crafting>();

                    slot.Show(recipe, player.UCE_Crafting_CanBoost(recipe, slot.amount));

                    if (player.UCE_Crafting_CraftValidation(recipe, slot.amount, slot.boost) && !player.UCE_Crafting_isBusy())
                    {
                        slot.actionButton.interactable = true;
                    }
                    else
                    {
                        slot.actionButton.interactable = false;
                    }

                    slot.unlearnButton.interactable = true;
                    slot.unlearnButton.onClick.SetListener(() =>
                    {
                        unlearnPanel.Show(recipe);
                    });

                    slot.actionButton.onClick.SetListener(() =>
                    {
                        player.UCE_Crafting_startCrafting(recipe, slot.amount, slot.boost);
                        panel.SetActive(false);
                    });
                }
            }
        }
        else
        {
            currentCategory = "";
            panel.SetActive(false); // hide
        }
    }

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(GameObject _instance, UCE_CraftingProfessionTemplate p, List<UCE_Tmpl_Recipe> r)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        instance = _instance;
        profession = p;

        for (int i = 0; i < categoryButtons.Length; ++i)
        {
            if (profession.categories.Length >= i)
            {
                categoryButtons[i].gameObject.SetActive(true);
                categoryButtons[i].SetCategory(profession.categories[i]);
            }
            else
            {
                categoryButtons[i].gameObject.SetActive(false);
            }
        }

        recipes = new List<UCE_Tmpl_Recipe>();
        recipes.Clear();
        recipes = r;

        currentCategory = categoryAll;

        changeCategory(currentCategory);
    }

    // -----------------------------------------------------------------------------------
    // changeCategory
    // -----------------------------------------------------------------------------------
    public void changeCategory(string newCategory)
    {
        currentCategory = newCategory;

        for (int i = 0; i < content.childCount; ++i)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        Invoke("Refresh", .05f);

        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // getRecipeCount
    // -----------------------------------------------------------------------------------
    private int getRecipeCount()
    {
        int count = 0;

        for (int i = 0; i < recipes.Count; ++i)
        {
            if (canCraft(i))
                count++;
        }

        return count;
    }

    // -----------------------------------------------------------------------------------
    // canCraft
    // -----------------------------------------------------------------------------------
    private bool canCraft(int index)
    {
        Player player = Player.localPlayer;
        if (!player) return false;

        return ((recipes[index].category == currentCategory || currentCategory == categoryAll) &&
            player.UCE_Crafting_CraftValidation(recipes[index], 1, false));
    }

    // -----------------------------------------------------------------------------------
}

#endif
