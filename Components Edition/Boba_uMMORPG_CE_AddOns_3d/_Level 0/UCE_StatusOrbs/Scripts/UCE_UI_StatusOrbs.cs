// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UCE_UI_StatusOrbs : MonoBehaviour
{
    [Header("=-=-= UCE Status Orbs =-=-=")]
    [SerializeField] private bool showOrbs = true;

    [SerializeField] private GameObject panel = null;
    [SerializeField] private Text healthOrbText = null, manaOrbText = null;
    [SerializeField] private Image healthOrbImage = null, manaOrbImage = null;

    private void Update()
    {
        // Grab our local player.
        Player player = Player.localPlayer;

        // If our player is found and we're showing orbs.
        // Then update our health and mana for text and the images.
        if (player != null && showOrbs)
        {
            panel.SetActive(true);
            healthOrbText.text = player.health + " / " + player.health.max;
            manaOrbText.text = player.mana + " / " + player.mana.max;
            healthOrbImage.fillAmount = player.health.Percent();
            manaOrbImage.fillAmount = player.mana.Percent();
        }
        else panel.SetActive(false);
    }
}
