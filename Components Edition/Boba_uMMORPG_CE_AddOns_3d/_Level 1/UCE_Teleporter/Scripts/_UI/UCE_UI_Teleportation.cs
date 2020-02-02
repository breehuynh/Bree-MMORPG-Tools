// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// TELEPORTATION UI
// ===================================================================================
#if _iMMOTELEPORTER

public partial class UCE_UI_Teleportation : MonoBehaviour
{
    public GameObject panel;
    public UCE_Slot_Teleportation slotPrefab;
    public Transform content;

    public string labelTeleport = "Teleport to: ";

    // -----------------------------------------------------------------------------------
    // Update
    // @Client
    // -----------------------------------------------------------------------------------
    public void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (panel.activeSelf &&
            player.target != null &&
            player.target is Npc &&
            Utils.ClosestDistance(player, player.target) <= player.interactionRange
            )
        {
            Npc npc = (Npc)player.target;

            UIUtils.BalancePrefabs(slotPrefab.gameObject, npc.teleportationDestinations.Length, content);

            for (int i = 0; i < npc.teleportationDestinations.Length; ++i)
            {
                int index = i;

                UCE_Slot_Teleportation slot = content.GetChild(index).GetComponent<UCE_Slot_Teleportation>();

                slot.actionButton.interactable = npc.teleportationDestinations[index].teleportationRequirement.checkRequirements(player);

                slot.actionButton.GetComponentInChildren<Text>().text = labelTeleport + npc.teleportationDestinations[index].teleportationTarget.name;

                slot.actionButton.onClick.SetListener(() =>
                {
                    player.Cmd_NpcWarp(index);
                    panel.SetActive(false);
                });
            }

            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }

    // -----------------------------------------------------------------------------------
}

#endif
