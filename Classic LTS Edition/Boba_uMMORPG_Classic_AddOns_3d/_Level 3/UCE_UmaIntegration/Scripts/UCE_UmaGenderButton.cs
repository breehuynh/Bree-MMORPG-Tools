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
using UMA;
using UMA.CharacterSystem;
using TMPro;
using System;
using UnityEngine.UI;

public class UCE_UmaGenderButton : MonoBehaviour
{
    public UmaGenderButton gender;

    private UCE_UI_CharacterCreation creator;
    private Button changeButton;

    private void Start()
    {
        creator = FindObjectOfType<UCE_UI_CharacterCreation>();
        changeButton = gameObject.GetComponent<Button>();
        changeButton.onClick.SetListener(PerformTask);
    }

    // Start is called before the first frame update
    public void PerformTask()
    {
        if (creator.dca == null) return;
        creator.SwitchGender("Human" + gender.ToString());
    }
}

public enum UmaGenderButton
{
    Male,
    Female
}

#endif