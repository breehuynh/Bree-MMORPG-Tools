// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
#if _UMA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UMA;

public class UCE_UmaChangeClothingButton : MonoBehaviour
{
    [Header("Decrease Clothing = false")]
    public bool increase = true;
    public TextMeshProUGUI indexText;
    private UCE_UI_CharacterCreation creator;
    private Button changeButton;

    private void Start()
    {
        creator = FindObjectOfType<UCE_UI_CharacterCreation>();
        changeButton = gameObject.GetComponent<Button>();
        changeButton.onClick.SetListener(ChangeClothing);
    }

    private int index = 0;

    public void ChangeClothing()
    {
        if (creator.dca == null) return;

        bool male = creator.dca.activeRace.name == "HumanMale" ? true : false;

        if (male) // Male
        {
            if (increase) // Increase
                if (creator.maleClothingIndex >= creator.maleClothing.Count - 1)
                {
                    creator.maleClothingIndex = 0;
                    index = creator.maleClothingIndex;
                }
                else { creator.maleClothingIndex += 1; index = creator.maleClothingIndex; }
            if (!increase) // Decrease
                if (creator.maleClothingIndex == 0)
                {
                    creator.maleClothingIndex = creator.maleClothing.Count - 1;
                    index = creator.maleClothingIndex;
                }
                else { creator.maleClothingIndex -= 1; index = creator.maleClothingIndex; }
        }
        if (!male) // Female
        {
            if (increase) // Increase
                if (creator.femaleClothingIndex >= creator.femaleClothing.Count - 1)
                {
                    creator.femaleClothingIndex = 0;
                    index = creator.femaleClothingIndex;
                }
                else { creator.femaleClothingIndex += 1; index = creator.femaleClothingIndex; }
            if (!increase) // Decrease
                if (creator.femaleClothingIndex == 0)
                {
                    creator.femaleClothingIndex = creator.femaleClothing.Count - 1;
                    index = creator.femaleClothingIndex;
                }
                else { index = creator.femaleClothingIndex; index = creator.femaleClothingIndex; }
        }
        creator.SelectClothing(index);

        if (indexText != null)
            indexText.text = (index + 1).ToString();
    }
}

#endif