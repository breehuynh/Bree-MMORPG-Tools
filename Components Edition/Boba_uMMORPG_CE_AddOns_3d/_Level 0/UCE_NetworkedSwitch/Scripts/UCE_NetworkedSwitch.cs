// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System.Linq;

// NETWORKED SWITCH

public partial class UCE_NetworkedSwitch : UCE_InteractableObject
{
    public GameObject[] activatedObjects;
    public bool visible = true;

    // -----------------------------------------------------------------------------------
    // OnInteractClient
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public override void OnInteractClient(Player player) { }

    // -----------------------------------------------------------------------------------
    // OnInteractServer
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public override void OnInteractServer(Player player)
    {
        visible = !visible;

        foreach (GameObject go in activatedObjects)
            go.GetComponent<UCE_ActivateableObject>().Toggle(visible);
    }

    // -----------------------------------------------------------------------------------
}
