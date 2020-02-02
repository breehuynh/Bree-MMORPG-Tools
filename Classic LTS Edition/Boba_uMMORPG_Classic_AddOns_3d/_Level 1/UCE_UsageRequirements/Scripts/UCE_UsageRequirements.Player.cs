// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// PLAYER

public partial class Player : Entity
{
    [HideInInspector] public int UCE_usageAreaId;

    // -----------------------------------------------------------------------------------
    // UCE_UsageAreaEnter
    // -----------------------------------------------------------------------------------
    public void UCE_UsageAreaEnter(int id)
    {
        if (id <= 0) return;
        UCE_usageAreaId = id;
    }

    // -----------------------------------------------------------------------------------
    // UCE_UsageAreaExit
    // -----------------------------------------------------------------------------------
    public void UCE_UsageAreaExit()
    {
        UCE_usageAreaId = 0;
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetEquipmentId
    // -----------------------------------------------------------------------------------
    public bool UCE_GetEquipmentId(int id)
    {
        return equipment.FindIndex(slot => slot.amount > 0 &&
            ((EquipmentItem)slot.item.data).usageEquipmentId == id) != -1;
    }

    // -----------------------------------------------------------------------------------
}
