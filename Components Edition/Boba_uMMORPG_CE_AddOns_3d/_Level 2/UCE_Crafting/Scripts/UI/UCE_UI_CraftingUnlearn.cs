// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

#if _iMMOCRAFTING

// UCE UI CRAFTING UNLEARN

public partial class UCE_UI_CraftingUnlearn : MonoBehaviour
{
    public GameObject panel;
    public GameObject parentPanel;
    public Text text;

    public string unlearnText = "Do you want to unlearn: ";

    [HideInInspector] public UCE_Tmpl_Recipe recipe;

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(UCE_Tmpl_Recipe newRecipe)
    {
        recipe = newRecipe;
        text.text = unlearnText + recipe.name;
        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // onClickUnlearn
    // -----------------------------------------------------------------------------------
    public void onClickUnlearn()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        player.Cmd_UCE_Crafting_unlearnRecipe(recipe.name);

        panel.SetActive(false);
        parentPanel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
    // onClickCancel
    // -----------------------------------------------------------------------------------
    public void onClickCancel()
    {
        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
}

#endif
