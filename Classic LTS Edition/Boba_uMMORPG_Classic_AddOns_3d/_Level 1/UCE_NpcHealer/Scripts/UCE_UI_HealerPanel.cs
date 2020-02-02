// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// Note: this script has to be on an always-active UI parent, so that we can
// always find it from other code. (GameObject.Find doesn't find inactive ones)
using UnityEngine;
using UnityEngine.UI;

// UCE_UI_HealerPanel

public partial class UCE_UI_HealerPanel : MonoBehaviour
{
    public GameObject panel;
    public Text descriptionText;
    public Button acceptButton;
    public Button declineButton;

    public string healerText;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        // use collider point(s) to also work with big entities
        if (player.target != null && player.target is Npc &&
            Utils.ClosestDistance(player, player.target) <= player.interactionRange)
        {
            Npc npc = (Npc)player.target;

            descriptionText.text = healerText + npc.healingServices.getCost(player);

            acceptButton.interactable = npc.healingServices.Valid(player);

            acceptButton.onClick.SetListener(() =>
            {
                player.Cmd_UCE_Healer();
                panel.SetActive(false);
            });

            declineButton.onClick.SetListener(() =>
            {
                panel.SetActive(false);
            });
        }
        else panel.SetActive(false); // hide
    }

    // -----------------------------------------------------------------------------------
}
