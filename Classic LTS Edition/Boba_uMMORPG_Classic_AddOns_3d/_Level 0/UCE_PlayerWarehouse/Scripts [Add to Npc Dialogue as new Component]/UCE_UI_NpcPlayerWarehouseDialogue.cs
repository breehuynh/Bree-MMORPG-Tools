// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UCE UI NPC PLAYER WAREHOUSE DIALOGUE

public class UCE_UI_NpcPlayerWarehouseDialogue : MonoBehaviour
{
    public GameObject npcDialoguePanel;
    public Button playerWarehouseButton;
    public UCE_UI_PlayerWarehouse playerWarehousePanel;
    public GameObject inventoryPanel;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (player.target != null && player.target is Npc &&
            Utils.ClosestDistance(player, player.target) <= player.interactionRange)
        {
            Npc npc = (Npc)player.target;

            playerWarehouseButton.gameObject.SetActive(npc.offersPlayerWarehouse);

            playerWarehouseButton.onClick.SetListener(() =>
            {
                npcDialoguePanel.SetActive(false);
                inventoryPanel.SetActive(true);
                playerWarehousePanel.Show();
            });
        }
        else
        {
            playerWarehousePanel.Hide();
        }
    }

    // -----------------------------------------------------------------------------------
}
