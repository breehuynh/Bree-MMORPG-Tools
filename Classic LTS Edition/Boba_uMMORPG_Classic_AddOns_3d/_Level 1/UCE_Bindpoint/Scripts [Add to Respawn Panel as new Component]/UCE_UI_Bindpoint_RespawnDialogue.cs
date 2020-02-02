// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UI RESPAWN

public partial class UCE_UI_Bindpoint_RespawnDialogue : MonoBehaviour
{
    public GameObject panel;
    public Button RespawnToBindpointButton;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (!panel.activeSelf) return;

        RespawnToBindpointButton.gameObject.SetActive(player.UCE_myBindpoint.Valid);
        RespawnToBindpointButton.onClick.SetListener(() => { player.Cmd_UCE_RespawnToBindpoint(); });
    }

    // -----------------------------------------------------------------------------------
}
