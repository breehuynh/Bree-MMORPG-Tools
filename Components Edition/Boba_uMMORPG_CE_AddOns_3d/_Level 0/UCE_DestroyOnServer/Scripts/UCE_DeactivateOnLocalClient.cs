// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;

// DEACTIVATE ON lOCAL CLIENT

public class UCE_DeactivateOnLocalClient : MonoBehaviour
{
    // -------------------------------------------------------------------------------
    // Start
    // -------------------------------------------------------------------------------
    private void Start()
    {
#if _CIENT && !_SERVER
        Player player = Player.localPlayer;
        if (player)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
#else
        this.gameObject.SetActive(true);
#endif
    }

    // -------------------------------------------------------------------------------
}
