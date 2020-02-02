// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// INTERACTABLE LOTTERY

public partial class UCE_InteractableLottery : UCE_InteractableObject
{
    [Header("-=-=-=- UCE Lottery Object -=-=-=-")]
    public UCE_InteractionRewards rewards;

    // -----------------------------------------------------------------------------------
    // OnInteractServer
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public override void OnInteractServer(Player player)
    {
        rewards.gainRewards(player);
    }
}
