// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// TRAVELROUTE - BOX COLLIDER

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NetworkIdentity))]
public class UCE_AreaBox_Travelroute : NetworkBehaviour
{
    [Header("-=-=-=- UCE TRAVELROUTES -=-=-=-")]
    [Tooltip("[Optional] One click deactivation")]
    public bool isActive = true;

    protected UCE_UI_Travelroute instance;

    [Tooltip("[Required] Travelroutes available on enter")]
    public UCE_Travelroute[] Travelroutes;

    [Tooltip("[Optional] Travelroutes unlocked on enter")]
    public UCE_Unlockroute[] Unlockroutes;

    [Header("-=-=- Editor -=-=-")]
    public Color gizmoColor = new Color(0, 1, 1, 0.25f);

    public Color gizmoWireColor = new Color(1, 1, 1, 0.8f);

    // -----------------------------------------------------------------------------------
    // OnDrawGizmos
    // @Editor
    // -----------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        BoxCollider collider = GetComponent<BoxCollider>();

        // we need to set the gizmo matrix for proper scale & rotation
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(collider.center, collider.size);
        Gizmos.color = gizmoWireColor;
        Gizmos.DrawWireCube(collider.center, collider.size);
        Gizmos.matrix = Matrix4x4.identity;
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerEnter
    // @Client
    // -----------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();

        if (player != null && player.isAlive && isActive)
        {
            player.UCE_myTravelrouteArea = this;
            player.UCE_UnlockTravelroutes();

            if (instance == null)
                instance = FindObjectOfType<UCE_UI_Travelroute>();

            instance.Show();
        }
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerExit
    // @Client
    // -----------------------------------------------------------------------------------
    private void OnTriggerExit(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();

        if (player != null && player.isAlive && isActive)
        {
            player.UCE_myTravelrouteArea = null;

            if (instance == null)
                instance = FindObjectOfType<UCE_UI_Travelroute>();

            instance.Hide();
        }
    }

    // -----------------------------------------------------------------------------------
}
