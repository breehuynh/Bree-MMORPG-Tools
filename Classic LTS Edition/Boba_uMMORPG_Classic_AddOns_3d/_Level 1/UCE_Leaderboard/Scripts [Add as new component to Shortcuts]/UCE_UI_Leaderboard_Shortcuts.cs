// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// SHORTCUTS

public partial class UCE_UI_Leaderboard_Shortcuts : MonoBehaviour
{
    public Button LeaderboardButton;
    public GameObject LeaderboardPanel;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    public void Update()
    {
        LeaderboardButton.onClick.SetListener(() =>
        {
            LeaderboardPanel.SetActive(!LeaderboardPanel.activeSelf);
        });
    }

    // -----------------------------------------------------------------------------------
}
