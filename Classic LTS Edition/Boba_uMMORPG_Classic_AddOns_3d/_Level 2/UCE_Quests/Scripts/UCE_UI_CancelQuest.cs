// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UCE UI CANCEL QUEST

public partial class UCE_UI_CancelQuest : MonoBehaviour
{
    public GameObject panel;
    public GameObject parentPanel;
    public Text text;

    public string cancelText = "Do you want to cancel: ";

    protected string questName;

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(string _questName)
    {
        questName = _questName;
        text.text = cancelText + questName;
        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // onClickYes
    // -----------------------------------------------------------------------------------
    public void onClickYes()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        player.Cmd_UCE_CancelQuest(questName);

        panel.SetActive(false);
        parentPanel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
    // onClickNo
    // -----------------------------------------------------------------------------------
    public void onClickNo()
    {
        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
}
