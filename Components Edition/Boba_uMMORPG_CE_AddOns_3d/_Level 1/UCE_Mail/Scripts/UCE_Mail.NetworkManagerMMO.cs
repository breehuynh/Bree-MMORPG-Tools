// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if _MYSQL
using MySql.Data;								// From MySql.Data.dll in Plugins folder
using MySql.Data.MySqlClient;                   // From MySql.Data.dll in Plugins folder
#elif _SQLITE
#endif

// NETWORK MANAGER MMO

public partial class NetworkManagerMMO : NetworkManager
{
    [Header("[-=-=- UCE MAIL -=-=-]")]
    public UCE_Tmpl_MailSettings mailSettings;

    // -----------------------------------------------------------------------------------
    // OnStartServer_MailChecker
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartServer")]
    private void OnStartServer_MailChecker()
    {
        if (mailSettings)
            StartCoroutine(UpdateMailStatus());
        else
            Debug.LogWarning("You forgot to assign Mail Settings to NetworkManager!");
    }

    // -----------------------------------------------------------------------------------
    // UpdateMailStatus
    // -----------------------------------------------------------------------------------
    private IEnumerator UpdateMailStatus()
    {
        yield return null;
#if _SERVER
        //get the last known ID known on server startup
        //new messages are considered to be any after this point so we can notify people of new messages

#if _MYSQL && _SERVER
		long maxID = Convert.ToInt32(Database.singleton.ExecuteScalarMySql("SELECT IFNULL (id, 0) FROM (SELECT MAX(id)  AS id FROM mail) AS id"));
#elif _SQLITE && _SERVER
        var results = Database.singleton.UCE_connection.FindWithQuery<Database.mail>("SELECT IFNULL(id, 0) FROM (SELECT MAX(id) AS id FROM mail)");
        long maxID = results.id;
#endif

        while (true)
        {
            yield return new WaitForSeconds(mailSettings.mailCheckSeconds);

            //check for new messages
            List<MailMessage> newMessages = Database.singleton.Mail_CheckForNewMessages(maxID);

            foreach (MailMessage message in newMessages)
            {
                //if the player is online, add to their synclist
                if (Player.onlinePlayers.ContainsKey(message.to))
                {
                    Player.onlinePlayers[message.to].mailMessages.Add(message);
                }

                if (message.id > maxID)
                {
                    maxID = message.id;
                }
            }
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}
