using UnityEngine;
using UnityEngine.UI;

public partial class UIStamina : MonoBehaviour
{
    public GameObject panel;
    public Slider staminaSlider;
    public Text staminaStatus;

    void Update()
    {
        Player player = Player.localPlayer;
        if (player)
        {
            panel.SetActive(true);

            staminaSlider.value = player.StaminaPercent();
            staminaStatus.text = player.stamina + " / " + player.staminaMax;
                     
        }
    }
}
