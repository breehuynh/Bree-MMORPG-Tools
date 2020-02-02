// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// NetworkManagerMMO

public partial class NetworkManagerMMO
{

    // -----------------------------------------------------------------------------------
    // Start_UCE_Tools
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Start")]
    private void Start_UCE_Tools()
    {
#if _SERVER && !_CLIENT
        StartServer();
#endif
    }

    // -----------------------------------------------------------------------------------
}
