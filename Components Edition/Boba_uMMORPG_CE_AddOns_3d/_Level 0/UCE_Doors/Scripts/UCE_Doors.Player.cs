// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System.Collections;
using System;
using System.Linq;

#if _iMMODOORS

// PLAYER

public partial class Player
{
    [HideInInspector] public UCE_Doors UCE_selectedDoor;

    // -----------------------------------------------------------------------------------
    // UCE_OnSelect_Door
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    public void UCE_OnSelect_Door(UCE_Doors _UCE_selectedDoors)
    {
        UCE_selectedDoor = _UCE_selectedDoors;
        movement.LookAtY(UCE_selectedDoor.gameObject.transform.position);
        Cmd_UCE_checkDoorAccess(UCE_selectedDoor.gameObject);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_DoorAccess
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    protected void Cmd_UCE_checkDoorAccess(GameObject _UCE_selectedDoor)
    {
        UCE_selectedDoor = _UCE_selectedDoor.GetComponent<UCE_Doors>();

        if (UCE_DoorValidation())
        {
            Target_UCE_startDoorAccess(connectionToClient);
        }
        else
        {
            if (UCE_selectedDoor != null && UCE_selectedDoor.checkInteractionRange(this) && UCE_selectedDoor.lockedMessage != "")
            {
                UCE_ShowPrompt(UCE_selectedDoor.lockedMessage);
            }
            else
            {
                movement.Navigate(this.collider.ClosestPointOnBounds(transform.position), 0);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_startDoorAccess
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    protected void Target_UCE_startDoorAccess(NetworkConnection target)
    {
        if (UCE_DoorValidation())
        {
            UCE_addTask();
            UCE_setTimer(UCE_selectedDoor.accessDuration);
            UCE_CastbarShow(UCE_selectedDoor.accessLabel, UCE_selectedDoor.accessDuration);
            StartAnimation(UCE_selectedDoor.playerAnimation);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_DoorValidation
    // -----------------------------------------------------------------------------------
    public bool UCE_DoorValidation()
    {
        bool bValid = (UCE_selectedDoor != null &&
            UCE_selectedDoor.checkInteractionRange(this) &&
            UCE_selectedDoor.interactionRequirements.checkState(this));

        if (!bValid)
            UCE_cancelDoor();

        return bValid;
    }

    // -----------------------------------------------------------------------------------
    // LateUpdate_UCE_Door
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    [DevExtMethods("LateUpdate")]
    private void LateUpdate_UCE_Door()
    {
        if (UCE_DoorValidation() && UCE_checkTimer())
        {
            UCE_removeTask();
            UCE_stopTimer();
            UCE_CastbarHide();

            Cmd_UCE_finishDoorAccess();

            StopAnimation(UCE_selectedDoor.playerAnimation);
        }
    }

    // -----------------------------------------------------------------------------------
    // OnDamageDealt_UCE_cancelDoor
    // Custom Hook
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDamageDealt")]
    private void OnDamageDealt_UCE_cancelDoor()
    {
        UCE_cancelDoor();
    }

    // -----------------------------------------------------------------------------------
    // UCE_cancelDoor
    // -----------------------------------------------------------------------------------
    public void UCE_cancelDoor()
    {
        if (UCE_selectedDoor != null)
        {
            UCE_stopTimer();
            UCE_removeTask();
            UCE_CastbarHide();

            StopAnimation(UCE_selectedDoor.playerAnimation);

            UCE_selectedDoor = null;
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_finishDoor
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_finishDoorAccess()
    {
        UCE_removeTask();
        UCE_stopTimer();
    }
}

#endif
