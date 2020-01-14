// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// ===================================================================================
// HAZARD FLOOR
// ===================================================================================
[RequireComponent(typeof(SphereCollider))]
public class UCE_UsageArea : NetworkBehaviour
{
    public int usageAreaId;

    // -------------------------------------------------------------------------------
    // OnTriggerEnter
    // -------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();
        if (player && usageAreaId > 0)
            player.UCE_UsageAreaEnter(usageAreaId);
    }

    // -------------------------------------------------------------------------------
    // OnTriggerExit
    // -------------------------------------------------------------------------------
    private void OnTriggerExit(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();
        if (player)
            player.UCE_UsageAreaExit();
    }

    // -------------------------------------------------------------------------------
}
