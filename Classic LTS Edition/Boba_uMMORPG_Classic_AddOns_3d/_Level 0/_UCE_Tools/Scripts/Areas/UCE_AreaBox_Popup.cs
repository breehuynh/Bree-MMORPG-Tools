// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// ===================================================================================
// POPUP AREA - BOX
// ===================================================================================
[RequireComponent(typeof(BoxCollider))]
public partial class UCE_AreaBox_Popup : NetworkBehaviour
{
#if _iMMOPVP

    [Tooltip("Show the messages only to members or allies of this realm")]
    public int realmId;
    public int alliedRealmId;
#endif
    public string messageOnEnter;
    public string messageOnExit;
    [Range(0, 255)] public byte iconId;
    [Range(0, 255)] public byte soundId;

    // -----------------------------------------------------------------------------------
    // OnTriggerEnter
    // @Client
    // -----------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider co)
    {
        if (messageOnEnter != "")
        {
            Player player = co.GetComponentInParent<Player>();
            if (player)
            {
#if _iMMOPVP
                if (player.UCE_getAlliedRealms(realmId, alliedRealmId))
                {
#endif
                player.UCE_ShowPopup(messageOnEnter, iconId, soundId);
#if _iMMOPVP
                }
#endif
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerExit
    // @Client
    // -----------------------------------------------------------------------------------
    private void OnTriggerExit(Collider co)
    {
        if (messageOnExit != "")
        {
            Player player = co.GetComponentInParent<Player>();
            if (player)
            {
#if _iMMOPVP
                if (player.UCE_getAlliedRealms(realmId, alliedRealmId))
                {
#endif
                player.UCE_ShowPopup(messageOnExit, iconId, soundId);
#if _iMMOPVP
                }
#endif
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
