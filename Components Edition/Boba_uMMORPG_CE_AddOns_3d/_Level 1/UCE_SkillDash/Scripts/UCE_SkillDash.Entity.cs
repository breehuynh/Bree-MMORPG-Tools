// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public partial class Entity
{
    private Vector3 dashNewPosition;
    private float dashTimer;
    private float dashMaxTime;
    private bool isDash;
    private Vector3 dashOldPosition;

    [HideInInspector] public Vector3 LookDirection = Vector3.forward;

    public void OnStartLocalPlayer_Dash()
    {
        LookDirection = Vector3.forward;
    }

    [Server]
    public void StartDash(Vector3 newPos, float time)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPos, out hit, 2, NavMesh.AllAreas))
        {
            Debug.DrawLine(movement.transform.position, hit.position, Color.white, 3f);
            int layerMask = 1 << 8;
            layerMask = ~layerMask;
            RaycastHit hit2;
            if (Physics.Linecast(movement.transform.position + (Vector3.up * 0.5f), hit.position + (Vector3.up * 0.5f), out hit2, layerMask))
            {
                Debug.DrawLine(movement.transform.position + (Vector3.up * 0.5f), (hit2.point + (Vector3.up * 0.5f)) + (movement.transform.position - hit2.point).normalized, Color.red, 3f);
                if (NavMesh.SamplePosition((hit2.point - (Vector3.up * 0.5f)) + (movement.transform.position - hit2.point).normalized, out hit, 1, NavMesh.AllAreas))
                {
                    Debug.DrawLine(movement.transform.position, hit.position, Color.green, 3f);
                    dashNewPosition = hit.position;
                }
                else
                {
                    return;
                }
            }
            else
            {
                dashNewPosition = hit.position;
            }
            dashTimer = Time.time;
            dashMaxTime = time;
            isDash = true;
            dashOldPosition = movement.transform.position;
            RpcStartDash(dashNewPosition, time);
        }
    }

    [ClientRpc]
    private void RpcStartDash(Vector3 newPos, float time)
    {
        dashNewPosition = newPos;
        dashTimer = Time.time;
        dashMaxTime = time;
        isDash = true;
        dashOldPosition = movement.transform.position;
    }

    public void LateUpdate_Dash()
    {
        if (isServer && movement.GetVelocity().magnitude != 0 && movement.GetVelocity().normalized != Vector3.zero)
        {
            LookDirection = movement.GetVelocity().normalized;
        }

        if (isDash)
        {
            if (this is Player)
            {
     
                //((Player)this).rubberbanding.enabled = false;
            }
            float timepassed = Time.time - dashTimer;
            float d = timepassed / dashMaxTime;
            if (timepassed < dashMaxTime)
            {
                transform.position = Vector3.Lerp(dashOldPosition, dashNewPosition, d);
            }
            else
            {
                if (isServer)
                {
                    movement.Warp(dashNewPosition);
                }
                transform.position = dashNewPosition;
                isDash = false;
            }
        }
        else
        {
            if (this is Player)
            {
                //((Player)this).rubberbanding.enabled = true;
            }
        }
    }
}