// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class SwitchServerMsg : MessageBase
{
    public string scenePath;
    public string characterName;
}

// NETWORK MANAGER MMO

public partial class NetworkManagerMMO
{
    // -----------------------------------------------------------------------------------
    // OnClientConnect_UCE_NetworkZones
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnClientConnect")]
    private void OnClientConnect_UCE_NetworkZones(NetworkConnection conn)
    {
        if (GetComponent<UCE_NetworkZone>())
            NetworkClient.RegisterHandler<SwitchServerMsg>(GetComponent<UCE_NetworkZone>().OnClientSwitchServerRequested);
    }

    // -----------------------------------------------------------------------------------
    // OnStartServer_UCE_NetworkZones
    // spawn instance processes (if any)
    // @Server
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartServer")]
    private void OnStartServer_UCE_NetworkZones()
    {
#if !UNITY_EDITOR
		if (GetComponent<UCE_NetworkZone>() != null)
    		GetComponent<UCE_NetworkZone>().SpawnProcesses();
#endif
    }

    // -----------------------------------------------------------------------------------
    // OnClientCharactersAvailable_UCE_NetworkZones
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnClientCharactersAvailable")]
    private void OnClientCharactersAvailable_UCE_NetworkZones(CharactersAvailableMsg message)
    {
        int index = message.characters.ToList().FindIndex(c => c.name == UCE_NetworkZone.autoSelectCharacter);

        if (index != -1)
        {
            // send character select message
            print("[Zones]: autoselect " + UCE_NetworkZone.autoSelectCharacter + "(" + index + ")");

            byte[] extra = BitConverter.GetBytes(index);
            ClientScene.AddPlayer(NetworkClient.connection, extra);

            // clear auto select
            UCE_NetworkZone.autoSelectCharacter = "";
        }
    }

    // -----------------------------------------------------------------------------------
    // OnServerAddPlayer_UCE_NetworkZones
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerAddPlayer")]
    private void OnServerAddPlayer_UCE_NetworkZones(string account, GameObject player, NetworkConnection conn, AddPlayerMessage message)
    {
        // where was the player saved the last time?
        string lastScene = Database.singleton.GetCharacterScene(player.name);

        if (lastScene != "" && lastScene != SceneManager.GetActiveScene().name)
        {
            print("[Zones]: " + player.name + " was last saved on another scene, transferring to: " + lastScene);

            // ask client to switch server
            conn.Send(
                new SwitchServerMsg
                {
                    scenePath = lastScene,
                    characterName = player.name
                }
            );

            // immediately destroy so nothing messes with the new
            // position and so it's not saved again etc.
            NetworkServer.Destroy(player);
        }
    }

    // -----------------------------------------------------------------------------------
    // OnServerCharacterCreate_UCE_NetworkZone
    // Save starting scene of the player only when that player is created
    // @Server
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerCharacterCreate")]
    private void OnServerCharacterCreate_UCE_NetworkZone(CharacterCreateMsg message, Player player)
    {
        if (player.startingScene == null) return;
        Database.singleton.SaveCharacterScene(player.name, player.startingScene.SceneName);
    }

    // -----------------------------------------------------------------------------------
    // UCE_NetworkSpawn
    // -----------------------------------------------------------------------------------
    public void UCE_NetworkSpawn(GameObject gob)
    {
        NetworkServer.Spawn(gob);
    }

    // -----------------------------------------------------------------------------------
}
