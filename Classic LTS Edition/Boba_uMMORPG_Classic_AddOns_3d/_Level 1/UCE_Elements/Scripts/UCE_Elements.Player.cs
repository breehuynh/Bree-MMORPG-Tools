// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// PLAYER

public partial class Player
{
    // -----------------------------------------------------------------------------------
    // OnDealDamage_UCE_Elements
    // -----------------------------------------------------------------------------------
    public UCE_ElementTemplate UCE_getAttackElement()
    {
        if (UCE_GetWeapon() != null && UCE_GetWeapon().elementalAttack != null) return UCE_GetWeapon().elementalAttack;

        foreach (ItemSlot slot in equipment)
        {
            if (slot.amount > 0 && ((EquipmentItem)slot.item.data).elementalAttack != null)
                return ((EquipmentItem)slot.item.data).elementalAttack;
        }

        return null;
    }

    // -----------------------------------------------------------------------------------
    // OnDealDamage_UCE_Elements
    // -----------------------------------------------------------------------------------
    public override float UCE_CalculateElementalResistance(UCE_ElementTemplate element, bool bCache = true)
    {
        float fValue = 0f;
        UCE_ElementCache elementCache = null;

        // -- Check Caching
        if (_elementsCache != null) _elementsCache.TryGetValue(element.name, out elementCache);
        if (bCache && elementCache != null && Time.time < elementCache.timer)
            return elementCache.value;

        // ------------------------------- Calculation -----------------------------------

        // -- Bonus: Base Resistance
        float fResistance = base.UCE_CalculateElementalResistance(element, false);

        // -- Bonus: Equipment
        float fEquipmentBonus = (from slot in equipment
                                 where slot.amount > 0
                                 select ((EquipmentItem)slot.item.data).GetResistance(element)).Sum();

        // -- Bonus: Equipment Sets
        float fSetBonus = 0f;
#if _iMMOTRAITS && _iMMOEQUIPMENTSETS && _iMMOELEMENTS
        // -- Equipment Bonus (Set Bonus)
        foreach (ItemSlot slot in equipment)
        {
            fSetBonus += slot.item.setBonusElementIndividual(slot, equipment, element);
        }

        // -- Equipment Bonus (Set Bonus)
        foreach (ItemSlot slot in equipment)
        {
            float tmpPointsP = slot.item.setBonusElementPartial(slot, equipment, element);
            fSetBonus += tmpPointsP;
            if (tmpPointsP > 0) break;
        }

        // -- Equipment Bonus (Set Bonus)
        foreach (ItemSlot slot in equipment)
        {
            float tmpPointsC = slot.item.setBonusElementComplete(slot, equipment, element);
            fSetBonus += tmpPointsC;
            if (tmpPointsC > 0) break;
        }
#endif

        // -- Bonus: Traits
        float fTraitBonus = 0f;
#if _iMMOTRAITS && _iMMOELEMENTS
        fTraitBonus = UCE_Traits.Sum(trait => trait.GetResistance(element));
#endif

        fValue = fResistance - (fEquipmentBonus + fTraitBonus + fSetBonus);

        // ----------------------------- Calculation End ---------------------------------

        // -- Update Caching
        if (bCache && elementCache != null)
        {
            elementCache.timer = Time.time + cacheTimerInterval;
            elementCache.value = fValue;
            _elementsCache[element.name] = elementCache;
        }
        else if (bCache)
        {
            elementCache = new UCE_ElementCache();
            elementCache.timer = Time.time + cacheTimerInterval;
            elementCache.value = fValue;
            _elementsCache.Add(element.name, elementCache);
        }

        return fValue;
    }

    // -----------------------------------------------------------------------------------
}
