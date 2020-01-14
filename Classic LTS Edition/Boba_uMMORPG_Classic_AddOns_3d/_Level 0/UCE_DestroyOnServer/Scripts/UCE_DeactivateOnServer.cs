// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;

// DEACTIVATE ON SERVER

public class UCE_DeactivateOnServer : MonoBehaviour
{
    // -------------------------------------------------------------------------------
    // Start
    // -------------------------------------------------------------------------------
    private void Start()
    {
#if _SERVER && !_CLIENT
        this.gameObject.SetActive(false);
#else
        this.gameObject.SetActive(true);
#endif
    }

    // -------------------------------------------------------------------------------
}
