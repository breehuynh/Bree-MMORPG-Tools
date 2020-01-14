// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// PVP ZONE AREA

[RequireComponent(typeof(SphereCollider))]
public partial class UCE_Area_PVPZone : NetworkBehaviour
{
    [Header("[SETTINGS (Set ProxChecker VisRange to 2x Radius of Area or bigger)]")]
    public UCE_PVPZONE_Settings pvpZone_settings;

    [Header("[POPUPS]")]
    public UCE_PopupClass enterPopup;

    public UCE_PopupClass exitPopup;

    [Header("[EDITOR]")]
    public Color gizmoColor = new Color(0, 1, 1, 0.25f);

    public Color gizmoWireColor = new Color(1, 1, 1, 0.8f);

    // -----------------------------------------------------------------------------------
    // OnDrawGizmos
    // @Editor
    // -----------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        SphereCollider collider = GetComponent<SphereCollider>();

        // we need to set the gizmo matrix for proper scale & rotation
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(collider.center, collider.radius);
        Gizmos.color = gizmoWireColor;
        Gizmos.DrawWireSphere(collider.center, collider.radius);
        Gizmos.matrix = Matrix4x4.identity;
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerEnter
    // @Client / @Server
    // -----------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();
        if (player)
        {
            player.UCE_PvpRegionEnter(pvpZone_settings);
            if (pvpZone_settings.requirements.checkRequirements(player))
                player.UCE_ShowPopup(enterPopup.message, enterPopup.iconId, enterPopup.soundId);
        }
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerExit
    // @Client / @Server
    // -----------------------------------------------------------------------------------
    private void OnTriggerExit(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();
        if (player)
        {
            player.UCE_PvpRegionLeave();
            if (pvpZone_settings.requirements.checkRequirements(player))
                player.UCE_ShowPopup(exitPopup.message, exitPopup.iconId, exitPopup.soundId);
        }
    }

    // -----------------------------------------------------------------------------------
}
