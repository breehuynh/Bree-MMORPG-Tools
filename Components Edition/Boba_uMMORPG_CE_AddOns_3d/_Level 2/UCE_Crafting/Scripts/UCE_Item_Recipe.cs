// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using UnityEngine;

#if _iMMOCRAFTING

// RECIPE ITEM

[CreateAssetMenu(fileName = "New UCE Recipe", menuName = "UCE Item/UCE RecipeItem", order = 999)]
public class UCE_Item_Recipe : UsableItem
{
    [Header("-=-=-=- Recipe Item -=-=-=-")]
    public UCE_Tmpl_Recipe[] learnedRecipes;

    public bool hasUnlimitedUse;

    public string tooltipHeader = "Learned Recipes:";

    // -----------------------------------------------------------------------------------
    // Use
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory.slots[inventoryIndex];

        if (player.UCE_Crafting_LearnRecipe(learnedRecipes))
        {
            // always call base function too
            base.Use(player, inventoryIndex);

            //decrease amount on use if has no unlimited amount
            if (hasUnlimitedUse == false)
            {
                slot.DecreaseAmount(1);
                player.inventory.slots[inventoryIndex] = slot;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // ToolTip
    // -----------------------------------------------------------------------------------
    public override string ToolTip()
    {
        StringBuilder tip = new StringBuilder(base.ToolTip());

        string recipeNames = tooltipHeader;

        foreach (UCE_Tmpl_Recipe recipe in learnedRecipes)
            recipeNames += "* " + recipe.name + "\n";

        tip.Replace("{SIMPLERECIPE}", recipeNames);

        return tip.ToString();
    }

    // -----------------------------------------------------------------------------------
}

#endif
