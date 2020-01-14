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

public class UCE_UmaSlider : MonoBehaviour
{
    public float min = 0.4f, max = 0.6f;

    public UmaSliderTypes type;

    private UCE_UI_CharacterCreation creator;
    private Slider changeSlider;

    private void Start()
    {
        creator = FindObjectOfType<UCE_UI_CharacterCreation>();
        changeSlider = gameObject.GetComponent<Slider>();
        changeSlider.minValue = min;
        changeSlider.maxValue = max;
        changeSlider.value = (min + max) / 2;
        changeSlider.onValueChanged.SetListener(PerformTask);
    }

    // Start is called before the first frame update
    public void PerformTask(float value)
    {
        if (creator.dca == null) return;
        if (value > creator.dca.umaData.GetAllDna().Length) return;

        creator.dca.umaData.GetAllDna()[0].SetValue(type.GetHashCode() + (creator.dca.activeRace.name == "HumanMale" ? 3 : 0), value);
        creator.dca.UpdateUMA();
    }
}

public enum UmaSliderTypes
{
    Height,
    Head_Size,
    Head_Width,
    Neck_Thickness,
    Arm_Length,
    Forearm_Length,
    Arm_Width,
    Forearm_Widgth,
    Hand_Size,
    Feet_Size,
    Leg_Seperation,
    Upper_Muscle,
    Lower_Muscle,
    Upper_Weight,
    Lower_Weight,
    Leg_Size,
    Belly_Size,
    Waist,
    Gluteus_Size,
    Ear_Size,
    Ear_Position,
    Ear_Rotation,
    Nose_Size,
    Nose_Curve,
    Nose_Width,
    Nose_Inclination,
    Nose_Flatten,
    Chin_Size,
    Chin_Pronounced,
    Chin_Position,
    Mandible_Size,
    Jaw_Size,
    Jaw_Position,
    Cheek_Size,
    Cheek_Position,
    Low_Cheek_Pronounced,
    Low_Cheeck_Position,
    Forehead_Size,
    Forehead_Position,
    Lip_Size,
    Mouth_Size,
    Eye_Rotation,
    Eye_Size,
    Breast_Size
}

#endif