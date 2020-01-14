// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

public class UCE_UI_Help : MonoBehaviour
{
    #region Variables

    public KeyCode hotKey = KeyCode.L;          //The key used to open the reports menu.
    public GameObject panel;                    //The reports menu to be used.
    public InputField InputFieldTitle;          //Header that will be placed on the report.
    public InputField InputFieldReportDetails;  //Information that will be placed on the report.
    public Button ButtonSendReport;             //Button that will send the information to your database.
    public Text ReporterName;                   //The name of the player that's sending in the report.

    #endregion Variables

    #region Functions

    // Use this for initialization

    private void Start()
    {
        ButtonSendReport.onClick.SetListener(() => SendBugButtonEvent());   //Watches for someone to click on the send report button.
    }

    // Update is called once per frame
    private void Update()
    {
        var player = Player.localPlayer;                            //Grabs the player from utils.
        if (!player) return;                                        //If no player was found don't continue.

        ReporterName.text = player.account;                         //Set the players account name as the reporting name.

        if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())  //Only open the menu if the hotkey is pressed and nothing else is going on.
            panel.SetActive(!panel.activeSelf);                     //Set the report menu to active.
    }

    //Tells the client to send the server database the report.
    public void SendBugButtonEvent()
    {
        var player = Player.localPlayer;                                                                                //Grabs the player from utils.
        if (!player) return;                                                                                            //If no player was found don't continue.

        if (!string.IsNullOrWhiteSpace(InputFieldReportDetails.text) && !string.IsNullOrWhiteSpace(InputFieldTitle.text)) //Make sure the details section and title of the report are filled out.
        {
            player.CmdSendBugReport(InputFieldTitle.text, InputFieldReportDetails.text);                                //Send the command to submit the bug with the information provided.

            InputFieldReportDetails.text = "";                                                                          //Reset the details field to blank.
            InputFieldTitle.text = "";                                                                                  //Reset the title field to blank.

            panel.SetActive(false);                                                                                     //Set the reports menu to not active.
        }
    }

    #endregion Functions
}
