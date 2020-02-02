// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UCE_UI_DropdownPlayer : MonoBehaviour
{
    public GameObject playerDropdown;
    public Button[] activeButtons;
    private GraphicRaycaster m_Raycaster;
    private EventSystem m_EventSystem;
    private Player player;
    private PointerEventData m_PointerEventData;
    private NetworkManagerMMO network;                               //The network manager for your game.
    private UIPopup popup;

    private void Start()
    {
        m_Raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        network = GameObject.Find("NetworkManager").GetComponent<NetworkManagerMMO>();
        popup = GameObject.Find("Popup").GetComponent<UIPopup>();
    }

    private void Update()
    {
        // Checks for the local player if we don't have a player yet.
        player = Player.localPlayer;
        if (player == null) return;

        // Check if our player is in a party.
        if (player.party.InParty()) activeButtons[0].gameObject.SetActive(true);

        // On right mouse click check if on a component that has a dropdown.
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            // Check results of our UI raycast if its a valid player then show the dropdown.
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "HealthManaPanel" || result.gameObject.name == "PortraitPanel")
                {
                    playerDropdown.GetComponent<RectTransform>().position = result.screenPosition;
                    playerDropdown.SetActive(true);
                }
            }
        }

        // If the dropdown is shown then check to see what buttons can be used.
        if (playerDropdown.activeSelf)
        {
            // leave party button
            if (player.party.InParty())
            {
                activeButtons[0].gameObject.SetActive(true);
                activeButtons[0].interactable = player.name != player.party.party.master ? true : false;
                activeButtons[0].onClick.SetListener(() =>
                {
                    if (player.party.party.master == player.name)
                        player.party.CmdDismiss();
                    else
                        player.party.CmdLeave();

                    playerDropdown.SetActive(false);
                });
            }
            else activeButtons[0].gameObject.SetActive(false);

            activeButtons[1].interactable = player.remainingLogoutTime == 0;
            activeButtons[1].onClick.SetListener(() =>
            {
                StartCoroutine(OnLogout());
                playerDropdown.SetActive(false);
            });

            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            // Check results of our UI raycast if its a valid player then show the dropdown.
            int foundMenu = 0;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "HealthManaPanel" || result.gameObject.name == "PortraitPanel" ||
                    result.gameObject.name == "PlayerDropdown")
                    foundMenu++;
            }

            if (foundMenu == 0)
                playerDropdown.SetActive(false);
        }
    }

    //Logs the current player out and puts them back at the login screen.
    private IEnumerator OnLogout()
    {
        var player = Player.localPlayer;                                                            //Grabs the player name based on DaggerUtils.
        if (!player) yield return null;                                                             //If the player isn't online then return nothing.

        CancelInvoke();
        network.OnClientDisconnect(player.connectionToServer);                                      //Checks to make sure the player has disconnected.

        if (NetworkServer.active) network.StopHost();                                               //Check if the player is the host if so stop the host as well.

        //wait for disconnect
        //Wait for 20 seconds if the client hasn't disconnected we will return to login screen.
        popup.panel.SetActive(true);
        popup.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
        popup.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);

        for (int i = 1; i < 21; i++)
        {
            popup.messageText.text = "Time til logged out: " + (21 - i);
            yield return new WaitForSeconds(1);
        }

        popup.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        popup.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
        popup.panel.SetActive(false);

        network.state = NetworkState.Offline;
    }
}
