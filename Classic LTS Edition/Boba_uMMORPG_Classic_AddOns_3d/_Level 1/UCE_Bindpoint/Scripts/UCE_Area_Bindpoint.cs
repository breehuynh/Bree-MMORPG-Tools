// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// BINDPOINT AREA

[RequireComponent(typeof(SphereCollider))]
public class UCE_Area_Bindpoint : NetworkBehaviour
{
    [Header("-=-=-=- Bindpoint -=-=-=-")]
    public Transform bindpoint;

    [Header("-=-=-=- Popups -=-=-=-")]
    public UCE_PopupClass enterPopup;

    [Header("-=-=-=- Editor -=-=-=-")]
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

    // -------------------------------------------------------------------------------
    // OnTriggerEnter
    // -------------------------------------------------------------------------------
    [ServerCallback]
    private void OnTriggerEnter(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();
        if (player && bindpoint != null && player.UCE_myBindpoint.name != bindpoint.gameObject.name)
        {
            player.UCE_SetBindpointFromArea(bindpoint.gameObject.name, bindpoint.position.x, bindpoint.position.y, bindpoint.position.z);
            player.UCE_ShowPopup(enterPopup.message, enterPopup.iconId, enterPopup.soundId);
        }
    }

    // -------------------------------------------------------------------------------
}
