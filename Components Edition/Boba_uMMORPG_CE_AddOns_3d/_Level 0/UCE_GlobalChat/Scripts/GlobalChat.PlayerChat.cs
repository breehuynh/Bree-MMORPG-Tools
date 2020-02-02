// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using System.Linq;
using Mirror;
using System.Text;
using System.Collections.Generic;

// CHAT

public partial class PlayerChat
{
    public enum GlobalChatError
    {
        None,
        CantFindPlayer,
        TooSoon,
        LevelTooLow,
        MissingItem
    }

    [Header("Global Chat")]
    public ChannelInfo global = new ChannelInfo("/a", "(global)", "(global)", null);

    [Tooltip("Optional. If the player has this item, they can use global chat. If this is null, then it is not required.")]
    public ScriptableItem globalChatItem;

    [Tooltip("Player must be at least this level to use the global chat.")]
    public int globalChatLevel = 1;

    [Tooltip("Number of seconds before the player can use global chat again.")]
    public float rateLimit = 1f;

    private float nextChatTime = 0f;

    public string errorTooSoon = "Must wait at least {RATELIMIT} seconds between messages.";
    public string errorLevelTooLow = "Must be at least level {MINLEVEL} before using global chat.";
    public string errorMissingItem = "You must have purchased {CHATITEM} before you can use global chat.";

    // -----------------------------------------------------------------------------------
    // OnSubmit_GlobalChat
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnSubmit")]
    private void OnSubmit_GlobalChat(string text)
    {
        //if this is a global command
        if (text.StartsWith(global.command))
        {
            //check for an error message with chat request
            GlobalChatError globalChatError = GlobalChatCheckForError();
            //and the user can use the global chat
            if (globalChatError == GlobalChatError.None)
            {
                //get the message part
                string content = ParseGeneral(global.command, text);

                //if message isn't empty
                if (!string.IsNullOrWhiteSpace(content))
                {
                    //so far so good, sending to server
                    CmdMsgGlobalChat(content);

                    //set next time the player can chat
                    nextChatTime = Time.time + rateLimit;
                }
            }
            else
            {
                string errorMessage = "";

                switch (globalChatError)
                {
                    case GlobalChatError.LevelTooLow:
                        errorMessage = BuildErrorMessage(errorLevelTooLow);
                        break;

                    case GlobalChatError.MissingItem:
                        errorMessage = BuildErrorMessage(errorMissingItem);
                        break;

                    case GlobalChatError.TooSoon:
                        errorMessage = BuildErrorMessage(errorTooSoon);
                        break;
                }

                //if the error message is not empty
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    UIChat.singleton.AddMessage(new ChatMessage("", infoChannel.identifierIn, errorMessage, "", global.textPrefab));
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // GlobalChatCheckForError
    // -----------------------------------------------------------------------------------
    public GlobalChatError GlobalChatCheckForError()
    {
        Player player = GetComponent<Player>();

        //was the player found
        if (player != null)
        {
            //is the player at or above the required level
            if (player.level.current >= globalChatLevel)
            {
                //if enoug time has passed since last message
                if (Time.time >= nextChatTime)
                {
                    //is the item null (not required) or found in the player inventory
                    if (globalChatItem == null || player.inventory.Count(new Item(globalChatItem)) > 0)
                    {
                        return GlobalChatError.None;
                    }
                    else
                    {
                        return GlobalChatError.MissingItem;
                    }
                }
                else
                {
                    return GlobalChatError.TooSoon;
                }
            }
            else
            {
                return GlobalChatError.LevelTooLow;
            }
        }
        else
        {
            return GlobalChatError.CantFindPlayer;
        }
    }

    // -----------------------------------------------------------------------------------
    // CmdMsgGlobalChat
    // -----------------------------------------------------------------------------------
    [Command]
    private void CmdMsgGlobalChat(string message)
    {
        if (string.IsNullOrWhiteSpace(message) || message.Length > maxLength || GlobalChatCheckForError() != GlobalChatError.None) return;

        // send message to all online guild members
        foreach (KeyValuePair<string, Player> pair in Player.onlinePlayers)
        {
            PlayerChat chat = pair.Value.GetComponent<PlayerChat>();
            if (chat != null)
            {
                chat.TargetMsgGlobalChat(chat.connectionToClient, name, message);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // TargetMsgGlobalChat
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void TargetMsgGlobalChat(NetworkConnection target, string sender, string message)
    {
        string reply = whisperChannel.command + " " + sender + " "; // whisper
        UIChat.singleton.AddMessage(new ChatMessage(sender, global.identifierIn, message, reply, global.textPrefab));
    }

    // -----------------------------------------------------------------------------------
    // BuildErrorMessage
    // -----------------------------------------------------------------------------------
    public string BuildErrorMessage(string text)
    {
        StringBuilder errorText = new StringBuilder(text);

        errorText.Replace("{NAME}", name);
        errorText.Replace("{RATELIMIT}", rateLimit.ToString("0.#"));
        errorText.Replace("{MINLEVEL}", globalChatLevel.ToString());

        if (globalChatItem != null)
            errorText.Replace("{CHATITEM}", globalChatItem.name);

        return errorText.ToString();
    }

    // -----------------------------------------------------------------------------------
}
