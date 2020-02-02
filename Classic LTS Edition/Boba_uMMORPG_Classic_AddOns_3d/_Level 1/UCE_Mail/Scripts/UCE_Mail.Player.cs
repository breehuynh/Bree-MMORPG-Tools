// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

// PLAYER

public partial class Player
{
    public SyncListMailMessage mailMessages = new SyncListMailMessage();

    [Header("[-=-=- UCE MAIL -=-=-]")]
    public UCE_Tmpl_MailSettings mailSettings;

    private UCE_UI_SendMail mail;

    // -----------------------------------------------------------------------------------
    // UnreadMailCount
    // -----------------------------------------------------------------------------------
    public int UnreadMailCount()
    {
        int count = 0;
        foreach (MailMessage message in mailMessages)
        {
            if (message.read == 0)
                count++;
        }
        return count;
    }

    // -----------------------------------------------------------------------------------
    // CmdMail_Send
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdMail_Send(string messageTo, string subject, string body, int itemIndex)
    {
        string errors = "";

        // ----- check if mail can be sent

        if (string.IsNullOrEmpty(messageTo))
        {
            errors += mailSettings.labelRecipient;
        }
        if (string.IsNullOrEmpty(subject))
        {
            errors += mailSettings.labelSubject;
        }
        if (string.IsNullOrEmpty(body))
        {
            errors += mailSettings.labelBody;
        }
        if (!mailSettings.costPerMail.checkCost(this))
        {
            errors += mailSettings.labelCost;
        }
        if (itemIndex != -1)
        {
            if (inventory[itemIndex].amount < 1)
                errors += "Missing item to send!\n";
        }

        // ----- begin send mail

        //if no errors yet, perform more complicated checks
        if (string.IsNullOrEmpty(errors))
        {
            long expiration = Mail_CalculateExpiration();

            mailSettings.costPerMail.payCost(this);

            string itemName = "";

            if (itemIndex != -1)
            {
                itemName = inventory[itemIndex].item.data.name;
                Item item = new Item(inventory[itemIndex].item.data);
                InventoryRemove(item, 1);
            }

            Database.singleton.Mail_CreateMessage(name, messageTo, subject, body, itemName, expiration);

            //commit player immediately so if server went offline, any items/gold are not recovered
            Database.singleton.CharacterSave(this, true);

            TargetMail_SendResults(connectionToClient, "Mail Sent", true);
        }
        else
        {
            TargetMail_SendResults(connectionToClient, errors, false);
        }

        nextRiskyActionTime = NetworkTime.time + mailSettings.mailWaitSeconds;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public long Mail_CalculateExpiration()
    {
        long expiration = 0;

        switch (mailSettings.expiresPart)
        {
            case DateInterval.Seconds:
                expiration = mailSettings.expiresAmount;
                break;

            case DateInterval.Minutes:
                expiration = mailSettings.expiresAmount * 60;
                break;

            case DateInterval.Hours:
                expiration = mailSettings.expiresAmount * 3600;
                break;

            case DateInterval.Days:
                expiration = mailSettings.expiresAmount * 86400;
                break;

            case DateInterval.Months:
                expiration = mailSettings.expiresAmount * 86400 * 30;
                break;

            case DateInterval.Years:
                expiration = mailSettings.expiresAmount * 86400 * 365;
                break;
        }

        return expiration * 1000; //convert to milliseconds
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void TargetMail_SendResults(NetworkConnection target, string status, bool success)
    {
        if (mail == null)
            mail = FindObjectOfType<UCE_UI_SendMail>();

        if (mail != null)
        {
            mail.MailMessageSent(status);
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdMail_Search(string searchString)
    {
        List<MailSearch> result = Database.singleton.Mail_SearchForCharacter(searchString, name);
        //serialize the result to string (easiest)
        string[] serialized = new string[result.Count];
        for (int i = 0; i < result.Count; i++)
        {
            serialized[i] = result[i].name + "|" + result[i].level + "|";
            if (result[i].guild != null)
            {
                serialized[i] += result[i].guild;
            }
        }

        TargetMail_SearchResults(connectionToClient, String.Join("&", serialized));

        nextRiskyActionTime = NetworkTime.time + mailSettings.mailWaitSeconds;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void TargetMail_SearchResults(NetworkConnection target, string searchResults)
    {
        if (mail == null)
            mail = FindObjectOfType<UCE_UI_SendMail>();

        List<MailSearch> results = new List<MailSearch>();

        if (!string.IsNullOrEmpty(searchResults))
        {
            string[] tmp = searchResults.Split('&');

            for (int i = 0; i < tmp.Length; i++)
            {
                string[] tmp2 = tmp[i].Split('|');

                MailSearch res = new MailSearch();
                res.name = tmp2[0];
                int.TryParse(tmp2[1], out res.level);
                res.guild = tmp2[2];
                results.Add(res);
            }
        }

        if (mail != null)
        {
            mail.UpdateSearchResults(results);
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdMail_ReadMessage(int index)
    {
        if (index >= 0 && index < mailMessages.Count)
        {
            MailMessage message = mailMessages[index];
            message.read = Epoch.Current();
            mailMessages[index] = message;
            Database.singleton.Mail_UpdateMessage(message);
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdMail_TakeItem(int index)
    {
        if (index >= 0 && index < mailMessages.Count)
        {
            MailMessage message = mailMessages[index];
            if (InventoryCanAdd(new Item(message.item), 1))
            {
                InventoryAdd(new Item(message.item), 1);
                message.read = Epoch.Current();
                message.item = null;
                mailMessages[index] = message;
                Database.singleton.Mail_UpdateMessage(message);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void CmdMail_DeleteMessage(int index)
    {
        if (index >= 0 && index < mailMessages.Count)
        {
            MailMessage message = mailMessages[index];
            message.deleted = Epoch.Current();
            mailMessages.RemoveAt(index);
            Database.singleton.Mail_UpdateMessage(message);
        }

        nextRiskyActionTime = NetworkTime.time + mailSettings.mailWaitSeconds;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_InventorySlot_MailItemSlot(int[] slotIndices)
    {
        // slotIndices[0] = slotFrom; slotIndices[1] = slotTo
        ItemSlot slot = inventory[slotIndices[0]];
        if (slot.item.tradable && slot.amount > 0 && !slot.item.summoned)
        {
            UCE_UI_SendMail.singleton.itemIndex = slotIndices[0];
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDragAndClear")]
    private void OnDragAndClear_MailItemSlot(int slotIndex)
    {
        UCE_UI_SendMail.singleton.itemIndex = -1;
    }

    // -----------------------------------------------------------------------------------
}
