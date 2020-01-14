// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UCE_UI_MainMenu : MonoBehaviour
{
    #region Variables

    [Header("UCE Main Menu")]
    public KeyCode hotKey = KeyCode.Escape;                         //Key used to open the main menu.

    private GameObject mainMenuPanel;                               //The panel for the main menu.
    private NetworkManagerMMO network;                              //The network manager for your game.
    private NetworkAuthenticatorMMO auth;                           //The network authenticator for your game.
    private UILogin loginUI;                                        //The login ui for your game.
    public Button changeCharacterButton;                            //The button used for changing your character.
    public Button logoutButton;                                     //The button used for logging a player out.
    public Button exitButton;                                       //The button used for exiting the game.
    public GameObject[] closeWindows;                               //A holder for all windows you want closed when main menu is opened.
    private UIPopup popup;                                          //A holder for our popup controls.

    [Tooltip("How long before a player can logout or change characters after combat, making this value to small may cause issues with logging back in.")]
    public int combatTimer = 5;                                     //How long before a player can logout or change characters after combat.

    [Tooltip("How long it takes to logout or change characters, making this value to small may cause issues with logging back in.")]
    public int logoutTimer = 20;                                    //How long it takes to perform the logout/change character.

    private double allowedLogoutTime;
    private double remainingLogoutTime;
    private Coroutine characterCoroutine;
    private Coroutine logoutCoroutine;

    #region _iMMOSETTINGS

#if _iMMOSETTINGS

    public Button settingsButton;                                   //The button associated with opening DaggerOptions.
    public GameObject settingsPanel;                                 //The panel for the options menu.

#endif

    #endregion _iMMOSETTINGS

    #endregion Variables

    #region Functions

    // Grabs our hidden components controls.

    private void Start()
    {
        network = GameObject.Find("NetworkManager").GetComponent<NetworkManagerMMO>();
        auth = network.GetComponent<NetworkAuthenticatorMMO>();
        popup = GameObject.Find("Popup").GetComponent<UIPopup>();
        loginUI = GameObject.Find("Login").GetComponent<UILogin>();
        mainMenuPanel = transform.GetChild(0).gameObject;

#if _iMMOSETTINGS

        settingsButton = transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Button>();
        settingsPanel = GameObject.Find("UCE_UI_Settings");

#endif
    }

    //Activates this function when the main menu is open.
    private void OnEnable()
    {
        #region _iMMOSETTINGS

#if _iMMOSETTINGS
        settingsButton.onClick.AddListener(delegate { OnSettings(); });                                 //Watches for someone to click the settings button.
#endif

        #endregion _iMMOSETTINGS

        changeCharacterButton.onClick.AddListener(delegate { characterCoroutine = StartCoroutine(OnChangeCharacter()); });   //Watches for someone to click the change character button.
        logoutButton.onClick.AddListener(delegate { logoutCoroutine = StartCoroutine(OnLogout()); });                     //Watches for someone to click the log out button.
        exitButton.onClick.AddListener(delegate { OnExitGame(); });                                     //Watches for someone to click the exit game button.
    }

    //Initiates every frame.
    private void Update()
    {
        var player = Player.localPlayer;                                                            //Grabs the player name based on DaggerUtils.
        if (!player) return;                                                                        //If the player isn't online then return nothing.

        // hotkey (not while typing in chat, etc.)
        if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())                                   //If the hotkey is pressed and input is their continue.
            mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);                                     //Open the main menu if the main menu is open close it.

        // If main menu is opened close all attached windows.
        if (mainMenuPanel.activeSelf)
        {
            allowedLogoutTime = player.lastCombatTime + combatTimer;
            remainingLogoutTime = NetworkTime.time < allowedLogoutTime ? (allowedLogoutTime - NetworkTime.time) : 0;

            changeCharacterButton.interactable = remainingLogoutTime <= 0 ? true : false;
            logoutButton.interactable = remainingLogoutTime <= 0 ? true : false;

            foreach (GameObject go in closeWindows)
                go.SetActive(false);
        }
    }

    #region _iMMOSETTINGS

#if _iMMOSETTINGS

    //Called by UCE Settings to active the options menu.
    public void OnSettings()
    {
        //Opens the options menu.
        settingsPanel.GetComponent<CanvasGroup>().alpha = 1;
        settingsPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

#endif

    #endregion _iMMOSETTINGS

    //Takes the player back to the character selection screen to choose a new character.
    private IEnumerator OnChangeCharacter()
    {
        network.changingCharacters = true;                                                              //Set our value for changing characters to true.
        StartCoroutine(OnLogout());                                                                     //Run the logout function, no reason to repeat ourselves.

        if (remainingLogoutTime <= 0)
        {
            // Wait for disconnect.
            for (int i = 1; i < logoutTimer + 2; i++)
            {
                yield return new WaitForSeconds(1);
            }

            // inputs
            network.StartClient();                                                                      //Start the client back up for relogin.
            auth.loginAccount = loginUI.accountInput.text;                                              //Resend the players login account information to server.
            auth.loginPassword = loginUI.passwordInput.text;                                            //Resend the players password information to server.

            // copy servers to dropdown; copy selected one to networkmanager ip/port.
            loginUI.serverDropdown.interactable = !network.isNetworkActive;                             //Copy the servers dropdown.
            network.networkAddress = network.serverList[loginUI.serverDropdown.value].ip;               //Copy the servers dropdown ip address to the network manager.
            yield return new WaitForSeconds(1);

            network.changingCharacters = false;                                                         //Set our changing characters back to false, as we're done.
        }
    }

    //Logs the current player out and puts them back at the login screen.
    private IEnumerator OnLogout()
    {
        var player = Player.localPlayer;                                                                //Grabs the player name based on DaggerUtils.
        if (!player) yield return null;                                                                 //If the player isn't online then return nothing.

        //wait for disconnect
        //Wait for 20 seconds if the client hasn't disconnected we will return to login screen.
        mainMenuPanel.SetActive(false);
        popup.panel.SetActive(true);
        popup.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
        popup.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);

        popup.transform.GetChild(0).transform.GetChild(2).GetComponent<Button>().onClick.SetListener(delegate { StopAllCoroutines(); });

        for (int i = 1; i < logoutTimer + 1; i++)
        {
            popup.messageText.text = "Time til logged out: " + ((logoutTimer + 1) - i);
            yield return new WaitForSeconds(1);

            allowedLogoutTime = player.lastCombatTime + combatTimer;
            remainingLogoutTime = NetworkTime.time < allowedLogoutTime ? (allowedLogoutTime - NetworkTime.time) : 0;

            if (remainingLogoutTime > 0) break;
        }

        if (remainingLogoutTime <= 0)
        {
            if (NetworkServer.active) StopAllCoroutines();                                              //If this is the server then don't continue.

            CancelInvoke();
            network.OnClientDisconnect(player.connectionToServer);                                      //Checks to make sure the player has disconnected.

            if (NetworkServer.active) network.StopHost();                                               //Check if the player is the host if so stop the host as well.

            mainMenuPanel.SetActive(false);                                                             //Closes the main menu.

            network.state = NetworkState.Offline;
        }

        popup.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        popup.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
        popup.panel.SetActive(false);
    }

    //Closes the game session out, completely.
    public void OnExitGame()
    {
        Application.Quit();                                                                         //Closes the game.
    }

    #endregion Functions
}
