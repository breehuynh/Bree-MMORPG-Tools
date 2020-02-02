// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;

// DESTROY ON CLIENT

public class UCE_DestroyOnClient : MonoBehaviour
{
    // -------------------------------------------------------------------------------
    // Start
    // -------------------------------------------------------------------------------
    private void Start()
    {
        NetworkBehaviour source = GetComponentInParent<NetworkBehaviour>();

        if (source && !NetworkServer.active && source.isClient)
            Destroy(this.gameObject);
    }

    // -------------------------------------------------------------------------------
}
