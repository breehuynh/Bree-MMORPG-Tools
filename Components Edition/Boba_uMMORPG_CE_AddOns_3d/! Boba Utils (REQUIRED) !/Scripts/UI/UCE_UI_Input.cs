// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UCE UI INPUT

public class UCE_UI_Input : MonoBehaviour
{
    public GameObject panel;
    public Text messageText;
    public Text amountText;
    public Slider amountSlider;
    public Button buttonConfirm;

    private UCE_UI_InputCallback instance;

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(string message, int minAmount, int maxAmount, UCE_UI_InputCallback callbackObject)
    {
        instance = callbackObject;
        messageText.text = message;
        amountSlider.value = 0;
        amountSlider.minValue = minAmount;
        amountSlider.maxValue = maxAmount;
        amountText.text = amountSlider.value.ToString() + "/" + maxAmount.ToString();
        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // SliderValueChanged
    // -----------------------------------------------------------------------------------
    public void SliderValueChanged()
    {
        amountText.text = amountSlider.value.ToString() + "/" + amountSlider.maxValue.ToString();
    }

    // -----------------------------------------------------------------------------------
    // Confirm
    // -----------------------------------------------------------------------------------
    public void Confirm()
    {
        instance.ConfirmInput((int)amountSlider.value);
        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
    // Hide
    // -----------------------------------------------------------------------------------
    public void Hide()
    {
        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
}
