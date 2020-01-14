// =======================================================================================
// Created and maintained by Boba
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
    [ClientRpc]
    public void RpcBackstabStartTeleport(Vector3 position, Vector3 enemyPos)
    {
        LookAtY(enemyPos);
        agent.Warp(position);
    }
}