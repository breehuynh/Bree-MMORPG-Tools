// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// UCE AREA (SPHERE) INTERACTABLE TELEPORT

[RequireComponent(typeof(SphereCollider))]
public class UCE_AreaSphere_InteractableTeleport : UCE_InteractableAreaSphere
{
    [Header("[-=-=-=- UCE TELEPORTER -=-=-=-]")]
    [Tooltip("[Required] Any on scene Transform or GameObject OR off scene coordinates (requires UCE Network Zones AddOn)")]
    public UCE_TeleportationTarget teleportationTarget;

    // -----------------------------------------------------------------------------------
    // OnInteractServer
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public override void OnInteractServer(Player player)
    {
        teleportationTarget.OnTeleport(player);
    }

    // -----------------------------------------------------------------------------------
}
