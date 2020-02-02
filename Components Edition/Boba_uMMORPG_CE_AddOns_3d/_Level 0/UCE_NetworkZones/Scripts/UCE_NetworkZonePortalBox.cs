// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

// ===================================================================================
// UCE NETWORK PORTAL (BOX)
// ===================================================================================
public class UCE_NetworkZonePortalBox : UCE_InteractableAreaBox
{
    [Header("[-=-=- UCE NETWORK ZONE PORTAL -=-=-]")]
    public SceneLocation targetScene;

    // -----------------------------------------------------------------------------------
    // OnInteractClient
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public override void OnInteractClient(Player player)
    {
        UCE_UI_Tools.FadeOutScreen(false, 0.25f);
    }

    // -----------------------------------------------------------------------------------
    // OnInteractServer
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public override void OnInteractServer(Player player)
    {
        player.UCE_OnPortal(targetScene);
    }

    // -----------------------------------------------------------------------------------
}
