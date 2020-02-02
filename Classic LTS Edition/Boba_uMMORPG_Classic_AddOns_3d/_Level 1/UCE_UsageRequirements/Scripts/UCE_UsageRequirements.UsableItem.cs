// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

public abstract partial class UsableItem : ScriptableItem
{
    [Header("-=-=- UCE Usage Requirements -=-=-")]
    public UCE_Requirements usageRequirements;

    [Tooltip("Can only be used while the player is inside a usage area of the same ID (0 = disabled)")]
    public int usageAreaId;

    // -----------------------------------------------------------------------------------
    // UCE_CanUse
    // -----------------------------------------------------------------------------------
    public virtual bool UCE_CanUse(Player player)
    {
        return (usageAreaId <= 0 || player.UCE_usageAreaId == usageAreaId) &&
                usageRequirements.checkRequirements(player)
                ;
    }

    // -----------------------------------------------------------------------------------
}
