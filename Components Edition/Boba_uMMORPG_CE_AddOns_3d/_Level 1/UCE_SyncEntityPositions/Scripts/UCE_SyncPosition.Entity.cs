// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using Mirror;
using UnityEngine;

public partial class Entity
{
    private float rubberbandTimer;
    private Vector3 oldPosition;
    private Vector3 rubberbandPosition;
    private bool isRubberbanding;

    public void LateUpdate_Zindex()
    {
        if (isClient)
        {
            if (isRubberbanding)
            {
                float timepassed = Time.time - rubberbandTimer;
                float d = timepassed / 0.1f;
                if (timepassed < 0.1f)
                {
                    transform.position = Vector3.Lerp(oldPosition, rubberbandPosition, d);
                }
                else
                {
                    movement.Warp(rubberbandPosition);
                    isRubberbanding = false;
                }
            }
        }
    }

    [Server]
    public void ResetMovementAll()
    {
    	if (movement.CanNavigate())
    	{
        	movement.Reset();
        	RpcResetMovement(transform.position);
    		return;
    	}
    	movement.Reset();
    	
    }

    [ClientRpc]
    public void RpcResetMovement(Vector3 resetPosition)
    {
        rubberbandTimer = Time.time;
        oldPosition = transform.position;
        rubberbandPosition = resetPosition;
        isRubberbanding = true;
    }
}