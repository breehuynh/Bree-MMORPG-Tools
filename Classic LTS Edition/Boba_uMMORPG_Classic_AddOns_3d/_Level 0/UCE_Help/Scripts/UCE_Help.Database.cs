// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
//using UnityEngine;
//using System.IO;

#if _MYSQL && _SERVER
using MySql.Data;
using MySql.Data.MySqlClient;
#elif _SQLITE && _SERVER
using SQLite;
#endif

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{

#if _SQLITE && _SERVER
	// -----------------------------------------------------------------------------------
    // UCE Reports
    // -----------------------------------------------------------------------------------
    class UCE_reports
    {
        public string senderAcc { get; set; }
        public string senderCharacter { get; set; }
        public bool readBefore { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public bool solved { get; set; }
        public string time { get; set; }
        public string position { get; set; }
    }
    #endif

    #region Functions

    // -----------------------------------------------------------------------------------
    // Connect_Reports
    // Sets up the database to accept reports.
    // Create the reports table if it wasn't there for any reason.
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_Reports()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"
							CREATE TABLE IF NOT EXISTS UCE_reports(
                           	senderAcc VARCHAR(32) NOT NULL,
                            senderCharacter VARCHAR(32) NOT NULL,
                           	readBefore INTEGER(16) NOT NULL,
                           	title VARCHAR(32) NOT NULL,
                           	message VARCHAR(512) NOT NULL,
                           	solved INTEGER(16) NOT NULL,
                           	time VARCHAR(128) NOT NULL,
                           	position VARCHAR(256) NOT NULL
                  			) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<UCE_reports>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_Reports
    // Loads the reports when they're called. Currently not used by anything, here for future addon.
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_Reports(Player player)
    {
#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql("SELECT senderCharacter, readBefore, title, message, solved, time, position FROM UCE_reports WHERE senderAcc=@senderAcc;", new MySqlParameter("@senderAcc", player.account));
        if (table.Count > 0)                                //If the table has anything then continue.
        {
            for (int i = 0; i < table.Count; i++)           //Loop through the table to gather the information.
            {
                var row = table[i];                         //Grab each row from the table.
                var report = new UCE_HelpMember();          //Make the report.
                report.senderAcc = player.account;          //Set the account information.
                report.senderCharacter = player.name;       //Set the character name of sender.
                report.readBefore = ((int)row[1]) != 0;     //Set the read option.
                report.title = (string)row[2];              //Set the title.
                report.message = (string)row[3];            //Set the details message.
                report.solved = ((int)row[4]) != 0;         //Set the solved or not option.
                report.time = (string)row[5];               //Set the time and date.
                report.position = (string)row[6];           //Set the position the player was on the map.
                player.reports.Add(report);                 //Add the report to a list to pull with other addon.
            }
        }
#elif _SQLITE && _SERVER
        var table = connection.Query<UCE_reports>("SELECT senderCharacter, readBefore, title, message, solved, time, position FROM 'UCE_reports' WHERE senderAcc=?", player.account);
        if (table.Count > 0)                                //If the table has anything then continue.
        {
            for (int i = 0; i < table.Count; i++)           //Loop through the table to gather the information.
            {
                var row = table[i];                         //Grab each row from the table.
                var report = new UCE_HelpMember();          //Make the report.
                report.senderAcc = player.account;          //Set the account information.
                report.senderCharacter = player.name;       //Set the character name of sender.
                report.readBefore = row.readBefore;         //Set the read option.
                report.title = row.time;                    //Set the title.
                report.message = row.message;               //Set the details message.
                report.solved = row.solved;                 //Set the solved or not option.
                report.time = row.time;                     //Set the time and date.
                report.position = row.position;             //Set the position the player was on the map.
                player.reports.Add(report);                 //Add the report to a list to pull with other addon.
            }
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // SaveReports
    //Saves the information provided to the database.
    // -----------------------------------------------------------------------------------
    public void SaveReports(UCE_HelpMember report)
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql("INSERT INTO UCE_reports VALUES (@senderAcc, @senderCharacter, @readBefore, @title, @message, @solved, @time, @position)",
                        new MySqlParameter("@senderAcc", report.senderAcc),
                        new MySqlParameter("@senderCharacter", report.senderCharacter),
                        new MySqlParameter("@readBefore", report.readBefore ? 1 : 0),
                        new MySqlParameter("@title", report.title),
                        new MySqlParameter("@message", report.message),
                        new MySqlParameter("@solved", report.solved ? 1 : 0),
                        new MySqlParameter("@time", report.time),
                        new MySqlParameter("@position", report.position));
#elif _SQLITE && _SERVER
        connection.Insert(new UCE_reports
        {
            senderAcc = report.senderAcc,
            senderCharacter = report.senderCharacter,
            readBefore = report.readBefore,
            title = report.title,
            message = report.message,
            solved = report.solved,
            time = report.time,
            position = report.position
        });
#endif
    }

    // -----------------------------------------------------------------------------------

    #endregion Functions
}
