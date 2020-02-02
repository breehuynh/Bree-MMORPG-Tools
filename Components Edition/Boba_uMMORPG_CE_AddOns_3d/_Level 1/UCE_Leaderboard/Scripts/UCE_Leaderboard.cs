// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;
using UnityEngine;

// ===================================================================================
// UCE Leaderboard
// ===================================================================================
public class UCE_Leaderboard : MonoBehaviour
{
    [SerializeField] private KeyCode hotkey = KeyCode.L;
    [SerializeField] private Transform trnGrid = null;
    [SerializeField] private GameObject gobEntry = null;
    [SerializeField] private GameObject gobLeaderboardPanel = null;

    private float updateRate = 30f, updateNext = 0f;
    private Player player;

    private int sortCategory;           // 0 = rank / 1 = level / 2 = gold
    private int sortMode;               // 0 = up / 1 = down

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        if (player == null) player = Player.localPlayer;
        if (player == null) return;

        if (Input.GetKeyDown(hotkey) && !UIUtils.AnyInputActive())
            gobLeaderboardPanel.SetActive(!gobLeaderboardPanel.activeSelf);

        if (Time.time > updateNext && gobLeaderboardPanel.activeSelf)
        {
            player.Cmd_UCE_AllPlayersOnline();
            updateNext = Time.time + updateRate;
        }

        if (gobLeaderboardPanel.activeSelf)
        {
            if (player.currentOnlinePlayers.Count <= 0) return;

            player.currentOnlinePlayers.OrderBy(x => x.level);

            // Destroy all entries before adding new entries, otherwise endless building list.
            foreach (Transform child in trnGrid)
                Destroy(child.gameObject);

            // Add each player to our leaderboard and set their information.

            for (int i = 1; i <= player.currentOnlinePlayers.Count; i++)
            {
                GameObject entry = Instantiate(gobEntry, trnGrid);
                entry.name = "Entry: Place " + i;
                UCE_LeaderboardEntry lEntry = entry.GetComponent<UCE_LeaderboardEntry>();

                lEntry.txtRank.text = i.ToString();
                lEntry.txtName.text = player.currentOnlinePlayers[i - 1].name;
                lEntry.txtLevel.text = player.currentOnlinePlayers[i - 1].level.ToString();
                lEntry.txtGold.text = player.currentOnlinePlayers[i - 1].gold.ToString();
                /*
				lEntry.txtStatistcOne.text = rankPlayers[i - 1].strength.ToString();
				lEntry.txtStatisticTwo.text = rankPlayers[i - 1].intelligence.ToString();
				lEntry.txtStatisticThree.text = rankPlayers[i - 1].defense.ToString();
				*/
            }
        }
    }

    // -----------------------------------------------------------------------------------
}

// ===================================================================================
