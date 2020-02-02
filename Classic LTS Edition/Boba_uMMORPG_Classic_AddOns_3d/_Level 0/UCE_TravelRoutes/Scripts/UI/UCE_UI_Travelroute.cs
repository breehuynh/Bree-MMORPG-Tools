// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// TRAVELROUTE UI
// ===================================================================================
public partial class UCE_UI_Travelroute : MonoBehaviour
{
    public GameObject panel;
    public UCE_Slot_Travelroute slotPrefab;
    public Transform content;

    public const string UCE_CURRENCY_LABEL = " Gold ";
    public const string UCE_TRAVEL_LABEL = "Travel ";

    // -----------------------------------------------------------------------------------
    // Show
    // @Client
    // -----------------------------------------------------------------------------------
    public void Show()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        List<UCE_Travelroute> travelroutesAvailable = player.UCE_TravelroutesVisibleForPlayer();
        UIUtils.BalancePrefabs(slotPrefab.gameObject, travelroutesAvailable.Count, content);

        for (int i = 0; i < travelroutesAvailable.Count; ++i)
        {
            bool valid = true;
            int price = 0;
            int index = i;
            UCE_Slot_Travelroute slot = content.GetChild(i).GetComponent<UCE_Slot_Travelroute>();
            string realmstring = "";
            string name = "";

#if _iMMOHONORSHOP
            valid = player.UCE_checkHonorCost(index);
#endif

            if (travelroutesAvailable[i].teleportationTarget.Valid)
            {
                price = player.UCE_getTravelCost(i);
                name = travelroutesAvailable[i].teleportationTarget.name;
            }

            slot.descriptionText.text = name;

            if (player.gold >= price && valid)
            {
                slot.actionButton.interactable = true;
            }
            else
            {
                slot.actionButton.interactable = false;
            }

            if (price > 0)
            {
                slot.actionButton.GetComponentInChildren<Text>().text = price + UCE_CURRENCY_LABEL + realmstring;
            }
            else
            {
                slot.actionButton.GetComponentInChildren<Text>().text = UCE_TRAVEL_LABEL + realmstring;
            }

            slot.actionButton.onClick.SetListener(() =>
            {
                player.Cmd_UCE_WarpTravelroute(index);
                panel.SetActive(false);
            });
        }

        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // Hide
    // @Client
    // -----------------------------------------------------------------------------------
    public void Hide()
    {
        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
    // Update
    // @Client
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (player.UCE_myTravelrouteArea == null)
        {
            Hide();
        }
    }

    // -----------------------------------------------------------------------------------
}
