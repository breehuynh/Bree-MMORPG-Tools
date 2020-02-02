// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

// PLAYER

public partial class Player
{
    // -----------------------------------------------------------------------------------
    // UCE_Ammunition_HasEquipmentOfCategory
    // -----------------------------------------------------------------------------------
    public bool UCE_Ammunition_HasEquipmentOfCategory(string categoryName)
    {
        return equipment.FindIndex(slot => slot.amount > 0 &&
            ((EquipmentItem)slot.item.data).category.StartsWith(categoryName)
        ) != -1;
    }

    // -----------------------------------------------------------------------------------
    // UCE_Ammunition_checkHasSkill
    // -----------------------------------------------------------------------------------
    public bool UCE_Ammunition_checkHasSkill(ScriptableSkill skill, int level)
    {
        if (skill == null || level <= 0) return true;
        return skills.Any(s => s.name == skill.name && s.level >= level);
    }

    // -----------------------------------------------------------------------------------
    // UCE_Ammunition_checkHasEquipment
    // -----------------------------------------------------------------------------------
    public bool UCE_Ammunition_checkHasEquipment(ScriptableItem item)
    {
        if (item == null) return true;

        foreach (ItemSlot slot in equipment)
            if (slot.amount > 0 && slot.item.data == item) return true;

        return false;
    }
}
