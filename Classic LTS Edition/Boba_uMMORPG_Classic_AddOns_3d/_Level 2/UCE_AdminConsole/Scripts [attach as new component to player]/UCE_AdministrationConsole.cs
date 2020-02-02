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
using System.Linq;
using System.Reflection;
using UnityEngine;

// UCE ADMINISTRATION - CONSOLE

public partial class UCE_AdministrationConsole : NetworkBehaviour
{
    public UCE_AdminCommandList adminCommands;

    protected Player player;
    protected UCE_Tmpl_AdminCommand currentCommandTmpl;
    protected NetworkManagerMMO _NetworkManagerMMO;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    private void Start()
    {
        _NetworkManagerMMO = FindObjectOfType<NetworkManagerMMO>();
        player = GetComponentInParent<Player>();
    }

    // ==================================== GENERAL  =====================================

    // -----------------------------------------------------------------------------------
    // ProcessCommand
    // -----------------------------------------------------------------------------------
    public void ProcessCommand(string commandText)
    {
        if (string.IsNullOrWhiteSpace(commandText) || player.UCE_adminLevel <= 0) return;

        foreach (UCE_Tmpl_AdminCommand command in adminCommands.commands)
        {
            if (commandText.StartsWith(command.commandName) && !string.IsNullOrWhiteSpace(command.functionName))
            {
                if (player.UCE_adminLevel >= command.commandLevel)
                {
                    currentCommandTmpl = command;

                    string[] parsedArgs = getParsed(commandText);

                    if (parsedArgs != null)
                    {
                        player.UCE_AddMessage("[Sys] Executing admin command...");
                        callCommand(parsedArgs);

                        break;
                    }
                }
                else
                {
                    player.UCE_AddMessage("[Sys] You do not have the admin rights for this command!");
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // callCommand
    // -----------------------------------------------------------------------------------
    protected void callCommand(string[] parsedArgs)
    {
        string functionName = "UCE_Admin_" + currentCommandTmpl.functionName;
        Type thisType = this.GetType();
        MethodInfo targetMethod = thisType.GetMethod(functionName);
        targetMethod.Invoke(this, new object[] { parsedArgs });
    }

    // -----------------------------------------------------------------------------------
    // ParseGeneral
    // -----------------------------------------------------------------------------------
    protected string ParseGeneral(string command, string msg)
    {
        return msg.StartsWith(command + " ") ? msg.Substring(command.Length + 1) : "";
    }

    // -----------------------------------------------------------------------------------
    // ParseCommand
    // -----------------------------------------------------------------------------------
    protected string[] ParseCommand(string command, string msg, int spaceCount)
    {
        string[] temp = new string[spaceCount];

        string content = ParseGeneral(command, msg);

        if (content != "")
        {
            int startIndex = 0;

            if (spaceCount > 0)
            {
                for (int no = 0; no < spaceCount; no++)
                {
                    int i = content.IndexOf(" ");
                    if (i >= 0)
                    {
                        if (no != spaceCount - 1)
                        {
                            temp[no] = content.Substring(0, i);
                            content = content.Remove(0, i + 1);
                        }
                        else
                        {
                            temp[no] = content.Substring(startIndex);
                        }
                    }
                    else
                    {
                        temp[no] = content.Substring(startIndex);
                    }
                }
            }
        }
        return temp;
    }

    // -----------------------------------------------------------------------------------
    // getPlayerTargets
    // -----------------------------------------------------------------------------------
    protected List<Player> getPlayerTargets(string targetString, string targetName)
    {
        if (string.IsNullOrWhiteSpace(targetString) || string.IsNullOrWhiteSpace(targetName)) return null;

        List<Player> players = new List<Player>();

        // -- Add online player by name
        if (targetString == adminCommands.tagTargetPlayer && Player.onlinePlayers.ContainsKey(targetName))
        {
            players.Add(Player.onlinePlayers[targetName]);

            // -- Add all online party members
        }
        else if (targetString == adminCommands.tagTargetParty)
        {
            Player player = Player.onlinePlayers[targetName];
            if (player && player.InParty())
            {
                foreach (string name in player.party.members)
                {
                    if (Player.onlinePlayers.ContainsKey(name))
                        players.Add(Player.onlinePlayers[name]);
                }
            }

            // -- Add all online guild members
        }
        else if (targetString == adminCommands.tagTargetGuild)
        {
            Player player = Player.onlinePlayers[targetName];
            if (player && player.InGuild())
            {
                foreach (GuildMember member in player.guild.members)
                {
                    if (Player.onlinePlayers.ContainsKey(member.name))
                        players.Add(Player.onlinePlayers[member.name]);
                }
            }

            // -- Add all online realm members
#if _iMMOPVP
        }
        else if (targetString == adminCommands.tagTargetRealm)
        {
            Player player = Player.onlinePlayers[targetName];
            if (player)
                players.AddRange(Player.onlinePlayers.Values.Where(x => x.UCE_getAlliedRealms(player)).ToList());
#endif

            // -- Add all online members
        }
        else if (targetString == adminCommands.tagTargetAll)
        {
            players.AddRange(Player.onlinePlayers.Values.ToList());
        }

        return players;
    }

    // -----------------------------------------------------------------------------------
    // getItem
    // -----------------------------------------------------------------------------------
    protected ScriptableItem getItem(string itemName)
    {
        ScriptableItem item;
        ScriptableItem.dict.TryGetValue(itemName.GetStableHashCode(), out item);
        return item;
    }

    // -----------------------------------------------------------------------------------
    // getParsed
    // -----------------------------------------------------------------------------------
    protected string[] getParsed(string currentCommandText)
    {
        if (currentCommandTmpl == null || string.IsNullOrWhiteSpace(currentCommandText)) return null;

        string[] parsed = new string[currentCommandTmpl.arguments.Length];

        parsed = ParseCommand(currentCommandTmpl.commandName, currentCommandText, currentCommandTmpl.arguments.Length);

        for (int i = 0; i < currentCommandTmpl.arguments.Length; ++i)
        {
            if (string.IsNullOrWhiteSpace(parsed[i]) ||
                !checkArgument(parsed[i], currentCommandTmpl.arguments[i].argumentType)
            )
            {
                player.UCE_TargetAddMessage("[Sys] Format error, use: " + currentCommandTmpl.getFormat());
                return null;
            }
        }

        return parsed;
    }

    // -----------------------------------------------------------------------------------
    // checkArgument
    // -----------------------------------------------------------------------------------
    protected bool checkArgument(string argument, UCE_AdminCommandArgument.UCE_AdminCommandArgumentType type)
    {
        int n;

        if (type == UCE_AdminCommandArgument.UCE_AdminCommandArgumentType.TargetType)
        {
            return (argument == adminCommands.tagTargetPlayer ||
                    argument == adminCommands.tagTargetParty ||
                    argument == adminCommands.tagTargetGuild ||
                    argument == adminCommands.tagTargetRealm ||
                    argument == adminCommands.tagTargetAll);
        }
        else if (type == UCE_AdminCommandArgument.UCE_AdminCommandArgumentType.PlayerName)
        {
            if (!Player.onlinePlayers.ContainsKey(argument))
            {
                player.UCE_TargetAddMessage("[Sys] Player not online or invalid name");
                return false;
            }
        }
        else if (type == UCE_AdminCommandArgument.UCE_AdminCommandArgumentType.ItemName)
        {
            if (!ScriptableItem.dict.ContainsKey(argument.GetStableHashCode()))
            {
                player.UCE_TargetAddMessage("[Sys] Invalid item name");
                return false;
            }
        }
        else if (type == UCE_AdminCommandArgument.UCE_AdminCommandArgumentType.Integer)
        {
            return int.TryParse(argument, out n);
        }

        return true;
    }

    // ===================================================================================
    // =================================== COMMANDS  =====================================
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_SetAdmin
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_SetAdmin(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];
        int adminValue = int.Parse(parsedArgs[2]);

        if (adminValue < 0 || adminValue > 255) return;

        Cmd_UCE_Admin_SetAdmin(adminTargets, adminTargetName, adminValue);

        player.UCE_AddMessage("[Sys] Target(s) admin level is now: " + adminValue.ToString());
    }

    [Command]
    public void Cmd_UCE_Admin_SetAdmin(string adminTargets, string adminTargetName, int adminValue)
    {
        if (adminValue < 0 || adminValue > 255) return;

        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            string adminAccount = Database.singleton.GetAccountName(plyr.name);
            Database.singleton.SetAdminAccount(adminAccount, adminValue);
            plyr.UCE_adminLevel = adminValue;
            plyr.UCE_TargetAddMessage("[Admin] Your admin level was adjusted to: " + adminValue.ToString());
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_GiveItem
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_GiveItem(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];
        int adminValue = int.Parse(parsedArgs[2]);
        string adminItemName = parsedArgs[3];

        ScriptableItem item = getItem(adminItemName);
        if (item == null) return;

        Cmd_UCE_Admin_GiveItem(adminTargets, adminTargetName, adminValue, adminItemName);

        player.UCE_AddMessage("[Sys] Target(s) received " + item.name + " x" + adminValue.ToString());
    }

    [Command]
    public void Cmd_UCE_Admin_GiveItem(string adminTargets, string adminTargetName, int adminValue, string adminItemName)
    {
        ScriptableItem item = getItem(adminItemName);
        if (item == null) return;

        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            if (plyr.InventoryCanAdd(new Item(item), adminValue) &&
                   plyr.InventoryAdd(new Item(item), adminValue))
            {
                plyr.UCE_TargetAddMessage("[Admin] You just received " + item.name + " x" + adminValue.ToString());
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_GiveGold
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_GiveGold(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];
        int adminValue = int.Parse(parsedArgs[2]);

        if (adminValue == 0) return;

        Cmd_UCE_Admin_GiveGold(adminTargets, adminTargetName, adminValue);

        player.UCE_AddMessage("[Sys] Target(s) received gold: " + adminValue.ToString());
    }

    [Command]
    public void Cmd_UCE_Admin_GiveGold(string adminTargets, string adminTargetName, int adminValue)
    {
        if (adminValue == 0) return;

        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            plyr.gold += adminValue;
            plyr.UCE_TargetAddMessage("[Admin] You just received gold: " + adminValue.ToString());
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_GiveExp
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_GiveExp(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];
        int adminValue = int.Parse(parsedArgs[2]);

        if (adminValue == 0) return;

        Cmd_UCE_Admin_GiveExp(adminTargets, adminTargetName, adminValue);

        player.UCE_AddMessage("[Sys] Target(s) received experience: " + adminValue.ToString());
    }

    [Command]
    public void Cmd_UCE_Admin_GiveExp(string adminTargets, string adminTargetName, int adminValue)
    {
        if (adminValue == 0) return;

        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            plyr.experience += adminValue;
            plyr.UCE_TargetAddMessage("[Admin] You just received experience: " + adminValue.ToString());
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_GiveCoins
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_GiveCoins(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];
        int adminValue = int.Parse(parsedArgs[2]);

        if (adminValue == 0) return;

        Cmd_UCE_Admin_GiveCoins(adminTargets, adminTargetName, adminValue);

        player.UCE_AddMessage("[Sys] Target(s) received coins: " + adminValue.ToString());
    }

    [Command]
    public void Cmd_UCE_Admin_GiveCoins(string adminTargets, string adminTargetName, int adminValue)
    {
        if (adminValue == 0) return;

        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            plyr.coins += adminValue;
            plyr.UCE_TargetAddMessage("[Admin] You just received coins: " + adminValue.ToString());
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_KillPlayer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_KillPlayer(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];

        Cmd_UCE_Admin_KillPlayer(adminTargets, adminTargetName);

        player.UCE_AddMessage("[Sys] Target(s) successfully killed.");
    }

    [Command]
    public void Cmd_UCE_Admin_KillPlayer(string adminTargets, string adminTargetName)
    {
        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            plyr.health = 0;
            plyr.UCE_TargetAddMessage("[Admin] You where just killed by admin!");
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_BanPlayer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_BanAccount(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];

        Cmd_UCE_Admin_BanAccount(adminTargets, adminTargetName);

        player.UCE_AddMessage("[Sys] Target(s) have been banned.");
    }

    [Command]
    public void Cmd_UCE_Admin_BanAccount(string adminTargets, string adminTargetName)
    {
        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            string adminAccount = Database.singleton.GetAccountName(plyr.name);
            plyr.UCE_TargetAddMessage("[Admin] Your account was just banned by admin!");
            Database.singleton.BanAccount(adminAccount);
            plyr.connectionToClient.Disconnect();
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_UnbanPlayer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_UnbanAccount(string[] parsedArgs)
    {
        string adminAccountName = parsedArgs[0];

        if (adminAccountName == "") return;

        Cmd_UCE_Admin_UnbanAccount(adminAccountName);

        player.UCE_AddMessage("[Sys] Account " + adminAccountName + " was unbanned.");
    }

    [Command]
    public void Cmd_UCE_Admin_UnbanAccount(string adminAccountName)
    {
        if (adminAccountName == "") return;

        Database.singleton.UnbanAccount(adminAccountName);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_GetAccountName
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_GetAccountName(string[] parsedArgs)
    {
        string adminPlayerName = parsedArgs[0];

        if (adminPlayerName == "") return;

        Cmd_UCE_Admin_GetAccountName(adminPlayerName);
    }

    [Command]
    public void Cmd_UCE_Admin_GetAccountName(string adminPlayerName)
    {
        if (adminPlayerName == "") return;
        string adminAccountName = Database.singleton.GetAccountName(adminPlayerName);
        player.UCE_TargetAddMessage("[Sys] " + adminPlayerName + "'s account name is: " + adminAccountName);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_CleanDatabase
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_CleanDatabase(string[] parsedArgs)
    {
        Cmd_UCE_Admin_CleanDatabase(parsedArgs);
        player.UCE_AddMessage("[Sys] Starting Database Cleanup...");
    }

    [Command]
    public void Cmd_UCE_Admin_CleanDatabase(string[] parsedArgs)
    {
#if _iMMODBCLEANER
        _NetworkManagerMMO.OnStartServer_UCE_DatabaseCleaner();
#endif
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_TeleportPlayer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_TeleportPlayer(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];

        Cmd_UCE_Admin_TeleportPlayer(adminTargets, adminTargetName);

        player.UCE_AddMessage("[Sys] Target(s) successfully teleported to your location.");
    }

    [Command]
    public void Cmd_UCE_Admin_TeleportPlayer(string adminTargets, string adminTargetName)
    {
        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            plyr.UCE_TargetAddMessage("[Admin] You where summoned to admins location!");
            plyr.UCE_Warp(transform.position);
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_OnlinePlayers
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_OnlinePlayers(string[] parsedArgs)
    {
        Cmd_UCE_Admin_OnlinePlayers(parsedArgs);
    }

    [Command]
    public void Cmd_UCE_Admin_OnlinePlayers(string[] parsedArgs)
    {
        int playerCount = Player.onlinePlayers.Count;

        player.UCE_TargetAddMessage("[Sys] There are currently <" + playerCount + "> players online.");
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_SummonMonster
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_SummonMonster(string[] parsedArgs)
    {
        string adminTargetName = parsedArgs[0];
        int adminValue = int.Parse(parsedArgs[1]);

        if (adminValue <= 0) return;

        Cmd_UCE_Admin_SummonMonster(adminTargetName, adminValue);

        player.UCE_AddMessage("[Sys] You just summoned " + adminValue.ToString() + " " + adminTargetName + "'s at your location.");
    }

    [Command]
    public void Cmd_UCE_Admin_SummonMonster(string adminTargetName, int adminValue)
    {
        if (adminValue <= 0) return;

        Monster monster = _NetworkManagerMMO.cachedMonsters().Find(x => x.name.ToLower() == adminTargetName.ToLower());

        if (monster)
        {
            for (int j = 1; j <= adminValue; ++j)
            {
                GameObject go = Instantiate(monster.gameObject, player.transform.position, player.transform.rotation);
                NetworkServer.Spawn(go);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_SummonNpc
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_SummonNpc(string[] parsedArgs)
    {
        string adminTargetName = parsedArgs[0];

        Cmd_UCE_Admin_SummonNpc(adminTargetName);

        player.UCE_AddMessage("[Sys] You just summoned " + adminTargetName + " at your location.");
    }

    [Command]
    public void Cmd_UCE_Admin_SummonNpc(string adminTargetName)
    {
        Npc npc = _NetworkManagerMMO.cachedNpcs().Find(x => x.name.ToLower() == adminTargetName.ToLower());

        GameObject go = Instantiate(npc.gameObject, player.transform.position, player.transform.rotation);
        NetworkServer.Spawn(go);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_UnsummonEntity
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_UnsummonEntity(string[] parsedArgs)
    {
        if (player.target != null && (player.target is Monster || player.target is Npc))
        {
            Cmd_UCE_Admin_UnsummonEntity(parsedArgs);
            player.UCE_AddMessage("[Sys] You just unsummoned " + player.target.name + ".");
        }
    }

    [Command]
    public void Cmd_UCE_Admin_UnsummonEntity(string[] parsedArgs)
    {
        if (player.target != null && (player.target is Monster || player.target is Npc))
        {
            NetworkServer.Destroy(player.target.gameObject);
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_KickPlayer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_KickPlayer(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];

        Cmd_UCE_Admin_KickPlayer(adminTargets, adminTargetName);

        player.UCE_AddMessage("[Sys] Target(s) successfully kicked.");
    }

    [Command]
    public void Cmd_UCE_Admin_KickPlayer(string adminTargets, string adminTargetName)
    {
        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            plyr.UCE_TargetAddMessage("[Admin] You where kicked by admin!");
            plyr.connectionToClient.Disconnect();
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_DeletePlayer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_DeletePlayer(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];

        Cmd_UCE_Admin_DeletePlayer(adminTargets, adminTargetName);

        player.UCE_AddMessage("[Sys] Target(s) successfully kicked and deleted.");
    }

    [Command]
    public void Cmd_UCE_Admin_DeletePlayer(string adminTargets, string adminTargetName)
    {
        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            plyr.UCE_TargetAddMessage("[Admin] You where deleted by admin!");

            Database.singleton.SetCharacterDeleted(plyr.name, true);
            plyr.connectionToClient.Disconnect();
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_UndeletePlayer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_UndeletePlayer(string[] parsedArgs)
    {
        string adminTargetName = parsedArgs[0];

        if (adminTargetName == "") return;

        Cmd_UCE_Admin_UndeletePlayer(adminTargetName);

        player.UCE_AddMessage("[Sys] Target(s) successfully undeleted.");
    }

    [Command]
    public void Cmd_UCE_Admin_UndeletePlayer(string adminTargetName)
    {
        if (adminTargetName == "") return;
        Database.singleton.SetCharacterDeleted(adminTargetName, false);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Admin_SendMessage
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    public void UCE_Admin_SendMessage(string[] parsedArgs)
    {
        string adminTargets = parsedArgs[0];
        string adminTargetName = parsedArgs[1];
        string adminTargetMessage = parsedArgs[2];

        Cmd_UCE_Admin_SendMessage(adminTargets, adminTargetName, adminTargetMessage);

        player.UCE_AddMessage("[Sys] Target(s) received the message.");
    }

    [Command]
    public void Cmd_UCE_Admin_SendMessage(string adminTargets, string adminTargetName, string adminTargetMessage)
    {
        List<Player> players = new List<Player>();
        players = getPlayerTargets(adminTargets, adminTargetName);

        foreach (Player plyr in players)
        {
            plyr.UCE_ShowPopup(adminTargetMessage);
            plyr.UCE_TargetAddMessage(adminTargetMessage);
        }
    }

    // -----------------------------------------------------------------------------------
}
