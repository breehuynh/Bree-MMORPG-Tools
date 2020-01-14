// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE UI INPUT CALLBACK

public class UCE_UI_InputCallback : MonoBehaviour
{
    private UCE_UI_Input instance;

    [HideInInspector] public int chosenAmount;
    [HideInInspector] public int selectedID;
    [HideInInspector] public bool confirmed = false;

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(string message, int minAmount, int maxAmount, int _selectedID)
    {
        if (instance == null)
            instance = FindObjectOfType<UCE_UI_Input>();

        confirmed = false;
        chosenAmount = 0;
        selectedID = _selectedID;

        instance.Show(message, minAmount, maxAmount, this);
    }

    // -----------------------------------------------------------------------------------
    // ConfirmInput
    // -----------------------------------------------------------------------------------
    public void ConfirmInput(int amount)
    {
        chosenAmount = amount;
        confirmed = true;
    }

    // -----------------------------------------------------------------------------------
    // Reset
    // -----------------------------------------------------------------------------------
    public void Reset()
    {
        confirmed = false;
        chosenAmount = 0;
        selectedID = -1;

        if (instance != null)
            instance.Hide();
    }

    // -----------------------------------------------------------------------------------
}
