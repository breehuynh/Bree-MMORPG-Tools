// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using UnityEngine;

public partial class Player
{
    #region Variables

    private int timeToReport = 15;   //The wait time between reports.

    public SyncListReport reports = new SyncListReport();       //List used to load the reports from the database.

    public ChannelInfo help = new ChannelInfo("", "(Help)", "(Help)", null);

    #endregion Variables

    #region Functions

    //Sends the command to the server database telling about the report and informs the player.
    [Command]
    public void CmdSendBugReport(string _title, string _message)
    {
        UCE_HelpMember report = new UCE_HelpMember();                                                                   //Setup the reporting member to be used.
        if (reports.Count > 0)                                                                                          //If the reports are not empty then continue.
        {
            var lastBugReportTime = DateTime.Parse(reports[reports.Count - 1].time);                                    //Set the time the bug has been reported.

            if (DateTime.UtcNow.Subtract(lastBugReportTime).Minutes < UCE_Help.ReportIntervalMinutes)                   //Check if enough time has passed from last posting before allowing another post.
            {
                //Inform the player in chat that they can't report another ticket for the interval time.
                TargetHelpResponse(netIdentity, "(Info)", "", "Can't report right now; because you reported a problem less than " + timeToReport.ToString() + " minutes ago.", "");
                return;
            }
            else
            {
                report.readBefore = false;                                                                              //Set the read option to false.
                report.senderAcc = account;                                                                             //Set the account information.
                report.senderCharacter = name;                                                                          //Set the character name.
                report.title = _title;                                                                                  //Set the title information.
                report.message = _message;                                                                              //Set the details message information.
                report.solved = false;                                                                                  //Set the solved option to false.
                report.time = DateTime.UtcNow.ToString();                                                               //Set the date and time information.
                report.position = transform.position.ToString();                                                        //Set the position of the player information.

                Database.singleton.SaveReports(report);                                                                           //Send the information to the server database.

                reports.Add(report);                                                                                    //Add the report to our list to view from other addon.
                TargetHelpResponse(netIdentity, "(Info)", "", "Report sent successfully.", "");
            }
        }
        else
        {
            report.readBefore = false;                                                                                  //Set the read option to false.
            report.senderAcc = account;                                                                                 //Set the account information.
            report.senderCharacter = name;                                                                              //Set the character name.
            report.title = _title;                                                                                      //Set the title information.
            report.message = _message;                                                                                  //Set the details message information.
            report.solved = false;                                                                                      //Set the solved option to false.
            report.time = DateTime.UtcNow.ToString();                                                                   //Set the date and time information.
            report.position = transform.position.ToString();                                                            //Set the position of the player information.

            Database.singleton.SaveReports(report);                                                                     //Send the information to the server database.

            reports.Add(report);                                                                                        //Add the report to our list to view from other addon.
            TargetHelpResponse(netIdentity, "(Info)", "", "Report sent successfully.", "");                             //Tell the player in chat that the report was successful.
        }
    }

    // Sends the response back to the player reporting the issue.
    [TargetRpc]
    private void TargetHelpResponse(NetworkIdentity identity, string sender, string identifier, string message, string replyPrefix)
    {
        UIChat.singleton.AddMessage(new ChatMessage(sender, identifier, message, replyPrefix, help.textPrefab));
    }

    #endregion Functions
}
