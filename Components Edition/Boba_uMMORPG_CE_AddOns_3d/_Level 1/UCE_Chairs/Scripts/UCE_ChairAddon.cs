// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// Created and maintained by Pierce

using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;

public partial class Player
{
    //[SyncVar] int example;

    [SyncVar, HideInInspector]
    public bool isSeated = false;

    [HideInInspector]
    public UCE_ChairAddon currentChair;

    //////////////////////////////////////////////////////
    // Used to Update Player Rotation to Chair Rotation //
    //////////////////////////////////////////////////////
    [Command]
    public void CmdrotatePlayer(Quaternion rot)
    {
        this.transform.rotation = rot;
        //this.agent.updateRotation = true;
        RpcRotatePlayer(rot);
    }

    ///////////////////////////////////////////////////////
    // Used to Update All Players Nearby of new rotation //
    ///////////////////////////////////////////////////////
    [ClientRpc]
    public void RpcRotatePlayer(Quaternion rot)
    {
        this.transform.rotation = rot;
        //this.agent.updateRotation = true;
    }

    ///////////////////////////////////////////////////////
    // Could be touched up in the future,                //
    // Waits 0.5f Seconds to get to desitionation then   //
    // Rotates the player to the Chairs Parent Rotation  //
    ///////////////////////////////////////////////////////
    public IEnumerator startSitting()
    {
        yield return new WaitForSeconds(0.1f);
        this.movement.Reset();
        this.movement.Navigate(currentChair.transform.position, 0);
        yield return new WaitForSeconds(0.5f);
        this.CmdrotatePlayer(currentChair.transform.rotation);
        this.CmdSit();
    }

    /////////////////////////////////////////////////////////
    // Used for Base Addon LateUpdate Function,            //
    // Sets Animator and If you move you'll cancel sitting //
    /////////////////////////////////////////////////////////
    private void LateUpdate_Uce_ChairAddon()
    {
        animator.SetBool("Sit", isSeated);
        if (isSeated)
        {
            if (state == "DEAD")
            {
                if (currentChair != null)
                {
                    CmdStand(currentChair.gameObject);
                }
            }
            if (movement.GetVelocity().magnitude > 0)
            {
                if (currentChair != null)
                {
                    CmdStand(currentChair.gameObject);
                }
            }
        }
    }

    ///////////////////////////////////////////////////////////
    // Tells Server to change - isSeated - to True           //
    ///////////////////////////////////////////////////////////
    [Command]
    public void CmdSit()
    {
        isSeated = true;
    }

    ////////////////////////////////////////////////////////////
    // Tells Server to change - isSeated - to False and       //
    // Tells chair to become available for other players      //
    ////////////////////////////////////////////////////////////
    [Command]
    public void CmdStand(GameObject go)
    {
        isSeated = false;

        if (go.GetComponent<UCE_ChairAddon>() == null) return;

        UCE_ChairAddon temp = go.GetComponent<UCE_ChairAddon>();
        temp.inUse = false;
    }
}

public partial class UCE_ChairAddon : UCE_InteractableObject
{
    [SyncVar]
    public bool inUse = false;

    [ClientCallback]
    public override void OnInteractClient(Player player)
    {
        if (!check(player)) return;
        player.currentChair = this;
        player.StartCoroutine(player.startSitting());
    }

    [ServerCallback]
    public override void OnInteractServer(Player player)
    {
        if (!check(player)) return;

        player.currentChair = this;
        inUse = true;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Used to check if Seat is taken, or Player is already seated; just incase. //
    ///////////////////////////////////////////////////////////////////////////////
    private bool check(Player player)
    {
        if (inUse)
        {
            return false;
        }
        if (player.isSeated == true)
        {
            return false;
        }
        return true;
    }
}
