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

public class UCE_UI_DropdownTarget : MonoBehaviour
{
    public GameObject targetDropdown;
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

        // On right mouse click check if on a component that has a dropdown.
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            // Check results of our UI raycast if its a valid target then show the dropdown.
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "TargetPanel")
                {
                    targetDropdown.GetComponent<RectTransform>().position = result.screenPosition;
                    targetDropdown.SetActive(true);
                }
            }
        }

        // If the dropdown is shown then check to see what buttons can be used.
        if (targetDropdown.activeSelf)
        {
            Entity target = player.nextTarget ?? player.target;
            float distance = Utils.ClosestDistance(player, target);

            // trade button
            if (target is Player)
            {
                activeButtons[0].gameObject.SetActive(true);
                activeButtons[0].interactable = player.trading.CanStartTradeWith(target);
                activeButtons[0].onClick.SetListener(() =>
                {
                    player.trading.CmdSendRequest();
                    targetDropdown.SetActive(false);
                });
            }
            else activeButtons[0].gameObject.SetActive(false);

            // guild invite button
            if (target is Player && player.guild.InGuild())
            {
                activeButtons[2].gameObject.SetActive(true);
                activeButtons[2].interactable = !((Player)target).guild.InGuild() &&
                                                 player.guild.guild.CanInvite(player.name, target.name) &&
                                                 NetworkTime.time >= player.nextRiskyActionTime &&
                                                 distance <= player.interactionRange;
                activeButtons[2].onClick.SetListener(() =>
                {
                    player.guild.CmdInviteTarget();
                    targetDropdown.SetActive(false);
                });
            }
            else activeButtons[2].gameObject.SetActive(false);

            // party invite button
            if (target is Player)
            {
                activeButtons[1].gameObject.SetActive(true);
                activeButtons[1].interactable = (!player.party.InParty() || !player.party.party.IsFull()) &&
                                                 !((Player)target).party.InParty() &&
                                                 NetworkTime.time >= player.nextRiskyActionTime &&
                                                 distance <= player.interactionRange;
                activeButtons[1].onClick.SetListener(() =>
                {
                    player.party.CmdInvite(target.name);
                    targetDropdown.SetActive(false);
                });
            }
            else activeButtons[1].gameObject.SetActive(false);

#if _iMMOPVP
            if (player.target && player.target is Player && player.target != player && player.UCE_SameRealm((Player)player.target))
            {
                activeButtons[3].gameObject.SetActive(true);
                activeButtons[3].interactable = player.UCE_Friends.FindIndex(x => x.name == ((Player)(player.target)).name) == -1 ? true : false;
                activeButtons[3].onClick.SetListener(() =>
                {
                    player.Cmd_UCE_AddFriend(((Player)(player.target)).name);
                    targetDropdown.SetActive(false);
                });
            }
            else activeButtons[3].gameObject.SetActive(false);

#elif _iMMOFRIENDS
 		    if (player.target && player.target is Player && player.target != player) {
                activeButtons[3].gameObject.SetActive(true);
                activeButtons[3].interactable = player.UCE_Friends.FindIndex(x=> x.name == ((Player)(player.target)).name) == -1  ? true : false;
                activeButtons[3].onClick.SetListener(() => {
                    player.Cmd_UCE_AddFriend(((Player)(player.target)).name);
                    targetDropdown.SetActive(false);
                });
            }
            else activeButtons[3].gameObject.SetActive(false);

#endif

            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            // Check results of our UI raycast if its a valid player then show the dropdown.
            int foundMenu = 0;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "TargetPanel" || result.gameObject.name == "TargetDropdown")
                    foundMenu++;
            }

            if (foundMenu == 0)
                targetDropdown.SetActive(false);
        }
    }
}
