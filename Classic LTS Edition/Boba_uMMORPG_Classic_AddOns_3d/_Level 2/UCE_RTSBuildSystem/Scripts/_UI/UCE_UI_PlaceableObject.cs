// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// PLACEABLE OBJECT UI

public partial class UCE_UI_PlaceableObject : MonoBehaviour
{
    public GameObject panel;
    public UCE_UI_PlaceableObjectUpgrade upgradePanel;
    public UCE_UI_PlaceableObjectUnspawn unspawnPanel;

    public Button upgradeButton;
    public Button unspawnButton;
    public GameObject slotPrefab;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        //if (!panel.activeSelf) return;

        Player player = Player.localPlayer;
        if (!player) return;

        if (player.UCE_myPlaceableObject != null && player.UCE_myPlaceableObject.ownerCharacter == player.name)
        {
            // -- Upgrade Structure
            upgradeButton.gameObject.SetActive(player.UCE_CanUpgradePlaceableObject(player.UCE_myPlaceableObject));
            upgradeButton.onClick.SetListener(() =>
            {
                upgradePanel.Show();
                panel.SetActive(false);
            });

            // -- Pickup Structure
            unspawnButton.gameObject.SetActive(player.UCE_myPlaceableObject.pickupable);
            unspawnButton.onClick.SetListener(() =>
            {
                unspawnPanel.Show();
                panel.SetActive(false);
            });
        }
        else
        {
            panel.SetActive(false);
        }
    }

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show()
    {
        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
}
