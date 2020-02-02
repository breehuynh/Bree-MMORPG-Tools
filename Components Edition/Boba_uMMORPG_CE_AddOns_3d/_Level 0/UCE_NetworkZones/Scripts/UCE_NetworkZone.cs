// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// Spawns one server zone process per scene.
// Also shuts down other zones when the main one died or was terminated.
//
// IMPORTANT: we do not EVER set manager.onlineScene/offlineScene. This part of
// UNET is broken and will cause all kinds of random issues when the server
// forces the client to reload the scene while receiving initialization messages
// already. Instead we always load the scene manually, then connect to the
// server afterwards.
using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

// UCE NETWORK ZONE

[RequireComponent(typeof(NetworkManager))]
[RequireComponent(typeof(TelepathyTransport))]
public class UCE_NetworkZone : MonoBehaviour
{
    public bool active = true;

    // components
    public NetworkManager manager;

    public TelepathyTransport transport;

    public UnityScene[] scenesToSpawn;

    // write online time to db every 'writeInterval'
    // die if not online for 'writeInterval * timeoutMultiplier'
    [Header("AliveCheck")]
    [Range(1, 10)] public float writeInterval = 3;

    [Range(2, 10)] public float timeoutMultiplier = 10;
    public float timeoutInterval { get { return writeInterval * timeoutMultiplier; } }

    [HideInInspector] public bool isSibling;

    // original network port
    private ushort originalPort;

    public static string autoSelectCharacter = "";
    public static bool autoConnectClient;

    // -----------------------------------------------------------------------------------
    // ParseSceneIndexFromArgs
    // -----------------------------------------------------------------------------------
    public int ParseSceneIndexFromArgs()
    {
        // note: args are null on android
        String[] args = System.Environment.GetCommandLineArgs();
        if (args != null)
        {
            int index = args.ToList().FindIndex(arg => arg == "-scenePath");
            return 0 <= index && index < args.Length - 1 ? args[index + 1].ToInt() : 0;
        }
        return 0;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public string ArgsString()
    {
        // note: first arg is always process name or empty
        // note: args are null on android
        String[] args = System.Environment.GetCommandLineArgs();
        return args != null ? String.Join(" ", args.Skip(1).ToArray()) : "";
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    // full path to game executable
    // -> osx: ../game.app/Contents/MacOS/game
    // -> GetCurrentProcess, Application.dataPaht etc. all aren't good for this
    public string processPath
    {
        get
        {
            // note: args are null on android
            String[] args = System.Environment.GetCommandLineArgs();
            return args != null ? args[0] : "";
        }
    }

    // -----------------------------------------------------------------------------------
    // Awake
    // before NetworkServer.Start
    // -----------------------------------------------------------------------------------
    private void Awake()
    {
        if (!active) return;

        originalPort = transport.port;

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (FindObjectsOfType<UCE_NetworkZone>().Length > 1)
        {
            print("Multiple NetworkZone components in the Scene will cause Deadlocks!");
            return;
        }

        // -- Setup Sibling

        int index = ParseSceneIndexFromArgs();
        if (index > 0)
        {
            isSibling = true;
            string sceneName = scenesToSpawn[index].SceneName;

            print("[Zones] setting requested port: +" + index);
            transport.port = (ushort)(originalPort + index);

            print("[Zones] changing server scene: " + sceneName);
            manager.onlineScene = sceneName; // loads scene automatically
            manager.StartServer();
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void SpawnProcesses()
    {
        // only if we are the main scene (if no -scene parameter was passed)
        if (ParseSceneIndexFromArgs() == 0)
        {
            print("[Zones]: main process starts online writer...");
            InvokeRepeating("WriteOnlineTime", 0, writeInterval);

            print("[Zones]: main process spawns siblings...");

            // -- Spawn Siblings

            for (int i = 1; i < scenesToSpawn.Length; ++i)
            {
                int index = i;
                Process p = new Process();
                p.StartInfo.FileName = processPath;
                p.StartInfo.Arguments = ArgsString() + " -scenePath " + index.ToString();
                print("[Zones]: spawning: " + p.StartInfo.FileName + "; args=" + p.StartInfo.Arguments);
                p.Start();
            }
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void OnClientSwitchServerRequested(NetworkConnection connection, SwitchServerMsg message)
    {
        print("OnClientSwitchServerRequested: " + message.scenePath);

        // only on client
        if (!NetworkServer.active)
        {
            UCE_UI_Tools.FadeOutScreen(false);

            print("[Zones]: disconnecting from current server");
            manager.StopClient();

            NetworkClient.Shutdown();
            NetworkManager.Shutdown(); // clears singleton too
            NetworkManager.singleton = manager; // setup singleton again

            print("[Zones]: loading required scene: " + message.scenePath);
            autoSelectCharacter = message.characterName;

            string sceneName = Path.GetFileNameWithoutExtension(message.scenePath);

            SceneManager.LoadScene(sceneName);
            autoConnectClient = true;
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnityEngine.Debug.LogWarning("OnSceneLoaded: " + scene.name);

        // server started and loaded the requested scene?
        int index = ParseSceneIndexFromArgs();

        // Server
        if (NetworkServer.active && scenesToSpawn[index] == scene.name)
        {
            // write online time every few seconds and check main zone alive every few seconds
            print("[Zones]: starting online alive check for zone: " + scene.name);
            InvokeRepeating("AliveCheck", timeoutInterval, timeoutInterval);
        }

        // Client
        // client started and needs to automatically connect?
        if (autoConnectClient)
        {
            int sceneIndex = scenesToSpawn.ToList().FindIndex(x => x.SceneName == scene.name);
            transport.port = (ushort)(originalPort + sceneIndex);
            print("[Zones]: automatically connecting client to new server at port: " + transport.port);
            //manager.onlineScene = scene.name; DO NOT DO THIS! will cause client to reload scene again, causing unet bugs
            manager.StartClient(); // loads new scene automatically
            UCE_UI_Tools.FadeInScreen(1.0f);
            autoConnectClient = false;
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    private void WriteOnlineTime()
    {
        Database.singleton.SaveMainZoneOnlineTime();
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    private void AliveCheck()
    {
        double mainZoneOnline = Database.singleton.TimeElapsedSinceMainZoneOnline();

        if (mainZoneOnline > timeoutInterval)
        {
            print("[Zones]: alive check failed, time to die for: " + SceneManager.GetActiveScene().name);
            Application.Quit();
        }
    }

    // -----------------------------------------------------------------------------------
}
