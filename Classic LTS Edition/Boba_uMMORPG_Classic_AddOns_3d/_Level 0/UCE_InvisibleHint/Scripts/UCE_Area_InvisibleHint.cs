// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System.Collections;

// ===================================================================================
// INVISIBLE HINT AREA
// ===================================================================================
[RequireComponent(typeof(BoxCollider))]
public class UCE_Area_InvisibleHint : UCE_InteractableAreaBox
{
    [Tooltip("[Optional] One click deactivation")]
    public bool isActive = true;

    [Tooltip("[Required] Text to display while in the area"), TextArea(1, 30)]
    public string textToDisplay = "";

    [Tooltip("[Optional] Automatically hide after x seconds?")]
    public float hideAfter;

    // -----------------------------------------------------------------------------------
    // OnInteractClient
    // -----------------------------------------------------------------------------------
    //[ClientCallback]
    public override void OnInteractClient(Player player)
    {
        if (isActive && textToDisplay != "")
            player.UCE_InvisibleHint_Show(textToDisplay, hideAfter);
    }

    // -------------------------------------------------------------------------------
    // OnTriggerExit
    // -------------------------------------------------------------------------------
    [ClientCallback]
    private void OnTriggerExit(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();

        if (player && player == Player.localPlayer && textToDisplay != "")
            player.UCE_InvisibleHint_Hide();
    }

    // -------------------------------------------------------------------------------
}
