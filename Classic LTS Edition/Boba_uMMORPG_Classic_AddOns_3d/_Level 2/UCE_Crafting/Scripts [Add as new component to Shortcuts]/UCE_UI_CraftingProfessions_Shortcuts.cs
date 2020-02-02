// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// SHORTCUTS

public partial class UCE_UI_CraftingProfessions_Shortcuts : MonoBehaviour
{
    public Button CraftingProfessionsButton;
    public GameObject CraftingProfessionsPanel;

    public Button CraftingRecipesButton;
    public GameObject CraftingRecipesPanel;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    public void Update()
    {
        CraftingProfessionsButton.onClick.SetListener(() =>
        {
            CraftingProfessionsPanel.SetActive(!CraftingProfessionsPanel.activeSelf);
        });

        CraftingRecipesButton.onClick.SetListener(() =>
        {
            CraftingRecipesPanel.SetActive(!CraftingRecipesPanel.activeSelf);
        });
    }

    // -----------------------------------------------------------------------------------
}
