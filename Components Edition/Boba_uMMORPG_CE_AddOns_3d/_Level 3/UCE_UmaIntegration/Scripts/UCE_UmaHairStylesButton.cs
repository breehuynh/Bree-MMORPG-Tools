// =======================================================================================
// Maintained by bobatea#9400 on Discord
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

public class UCE_UmaHairStylesButton : MonoBehaviour
{
    [Header("Decrease Hairstyle = false")]
    public bool increase = true;
    public TextMeshProUGUI indexText;
    private UCE_UI_CharacterCreation creator;
    private Button changeButton;

    private void Start()
    {
        creator = FindObjectOfType<UCE_UI_CharacterCreation>();
        changeButton = gameObject.GetComponent<Button>();
        changeButton.onClick.SetListener(ChangeHair);
    }

    private int index = 0;

    public void ChangeHair()
    {
        if (creator.dca == null) return;

        bool male = creator.dca.activeRace.name == "HumanMale" ? true : false;

        if (male) // Male
        {
            if (increase) // Increase
                if (creator.maleIndex >= creator.maleHairStyles.Count - 1)
                {
                    creator.maleIndex = 0;
                    index = creator.maleIndex;
                }
                else { creator.maleIndex += 1; index = creator.maleIndex; }
            if (!increase) // Decrease
                if (creator.maleIndex == 0)
                {
                    creator.maleIndex = creator.maleHairStyles.Count - 1;
                    index = creator.maleIndex;
                }
                else { creator.maleIndex -= 1; index = creator.maleIndex; }
        }
        if (!male) // Female
        {
            if (increase) // Increase
                if (creator.femaleIndex >= creator.femaleHairStyles.Count - 1)
                {
                    creator.femaleIndex = 0;
                    index = creator.femaleIndex;
                }
                else { creator.femaleIndex += 1; index = creator.femaleIndex; }
            if (!increase) // Decrease
                if (creator.femaleIndex == 0)
                {
                    creator.femaleIndex = creator.femaleHairStyles.Count - 1;
                    index = creator.femaleIndex;
                }
                else { creator.femaleIndex -= 1; index = creator.femaleIndex; }
        }
        creator.SelectHair(index);

        if (indexText != null)
            indexText.text = (index + 1).ToString();
    }
}

#endif