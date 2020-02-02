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

public class UCE_UI_DropdownPet : MonoBehaviour
{
    public GameObject petDropdown;
    public Button[] activeButtons;
    private GraphicRaycaster m_Raycaster;
    private EventSystem m_EventSystem;
    private Player player;
    private PointerEventData m_PointerEventData;

    private void Start()
    {
        m_Raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
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
                if (result.gameObject.name == "PanelNameAndLevel")
                {
                    petDropdown.GetComponent<RectTransform>().position = result.screenPosition;
                    petDropdown.SetActive(true);
                }
            }
        }

        // If the dropdown is shown then check to see what buttons can be used.
        if (petDropdown.activeSelf)
        {
            Pet pet = player.petControl.activePet;

            // attack button
            activeButtons[0].GetComponentInChildren<Text>().fontStyle = pet.autoAttack ? FontStyle.Bold : FontStyle.Normal;
            activeButtons[0].onClick.SetListener(() =>
            {
                pet.CmdSetAutoAttack(!pet.autoAttack);
                petDropdown.SetActive(false);
            });

            // defend button
            activeButtons[1].GetComponentInChildren<Text>().fontStyle = pet.defendOwner ? FontStyle.Bold : FontStyle.Normal;
            activeButtons[1].onClick.SetListener(() =>
            {
                pet.CmdSetDefendOwner(!pet.defendOwner);
                petDropdown.SetActive(false);
            });

            // unsummon button
            activeButtons[2].onClick.SetListener(() =>
            {
                if (player.petControl.CanUnsummon()) player.petControl.CmdUnsummon();
                petDropdown.SetActive(false);
            });

            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            // Check results of our UI raycast if its a valid player then show the dropdown.
            int foundMenu = 0;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "PanelNameAndLevel" || result.gameObject.name == "PetDropdown")
                    foundMenu++;
            }

            if (foundMenu == 0)
                petDropdown.SetActive(false);
        }
    }
}
