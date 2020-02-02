// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Text;
using UnityEngine;

#if _iMMOEQUIPMENTSETS

// ITEM

public partial struct Item
{
    // -----------------------------------------------------------------------------------
    // UCE_validIndividualSetBonus
    // -----------------------------------------------------------------------------------
    public bool UCE_validIndividualSetBonus(SyncListItemSlot equipment)
    {
        int counter = 0;

        for (int j = 0; j < ((EquipmentItem)data).setItems.Length; ++j)
        {
            if (((EquipmentItem)data).setItems[j] != null)
            {
                for (int i = 0; i < equipment.Count; ++i)
                {
                    if (equipment[i].amount > 0)
                    {
                        if (equipment[i].item.data.name == ((EquipmentItem)data).setItems[j].name)
                        {
                            counter++;
                            //break;
                        }
                    }
                }
            }
        }

        if (counter >= ((EquipmentItem)data).setItems.Length)
        {
            return true;
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_validPartialSetBonus
    // -----------------------------------------------------------------------------------
    public bool UCE_validPartialSetBonus(SyncListItemSlot equipment)
    {
        int counter = 0;

        if (((EquipmentItem)data).equipmentSet != null)
        {
            for (int j = 0; j < ((EquipmentItem)data).equipmentSet.setItems.Length; ++j)
            {
                if (((EquipmentItem)data).equipmentSet.setItems[j] != null)
                {
                    for (int i = 0; i < equipment.Count; ++i)
                    {
                        if (equipment[i].amount > 0)
                        {
                            if (equipment[i].item.data.name == ((EquipmentItem)data).equipmentSet.setItems[j].name)
                            {
                                counter++;
                                //break;
                            }
                        }
                    }
                }
            }

            if (counter >= ((EquipmentItem)data).equipmentSet.partialSetItemsCount)
            {
                return true;
            }
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_validCompleteSetBonus
    // -----------------------------------------------------------------------------------
    public bool UCE_validCompleteSetBonus(SyncListItemSlot equipment)
    {
        int counter = 0;

        if (((EquipmentItem)data).equipmentSet != null)
        {
            for (int j = 0; j < ((EquipmentItem)data).equipmentSet.setItems.Length; ++j)
            {
                if (((EquipmentItem)data).equipmentSet.setItems[j] != null)
                {
                    for (int i = 0; i < equipment.Count; ++i)
                    {
                        if (equipment[i].amount > 0)
                        {
                            if (equipment[i].item.data.name == ((EquipmentItem)data).equipmentSet.setItems[j].name)
                            {
                                counter++;
                                //break;
                            }
                        }
                    }
                }
            }

            if (counter >= ((EquipmentItem)data).equipmentSet.setItems.Length)
            {
                return true;
            }
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_hasIndividualSetBonus
    // -----------------------------------------------------------------------------------
    public bool UCE_hasIndividualSetBonus()
    {
        return
                data is EquipmentItem &&
                ((EquipmentItem)data).UCE_hasIndividualSetBonus;
    }

    // -----------------------------------------------------------------------------------
    // UCE_hasPartialSetBonus
    // -----------------------------------------------------------------------------------
    public bool UCE_hasPartialSetBonus()
    {
        return
                data is EquipmentItem &&
                ((EquipmentItem)data).equipmentSet != null &&
                ((EquipmentItem)data).equipmentSet.UCE_hasPartialSetBonus;
    }

    // -----------------------------------------------------------------------------------
    // UCE_hasCompleteSetBonus
    // -----------------------------------------------------------------------------------
    public bool UCE_hasCompleteSetBonus()
    {
        return
                data is EquipmentItem &&
                ((EquipmentItem)data).equipmentSet != null &&
                ((EquipmentItem)data).equipmentSet.UCE_hasCompleteSetBonus;
    }

    // -----------------------------------------------------------------------------------
    // setBonusAttributeIndividual
    // -----------------------------------------------------------------------------------
#if _iMMOATTRIBUTES

    public int setBonusAttributeIndividual(ItemSlot slot, SyncListItemSlot equipment, UCE_Attribute attribute)
    {
        int iPoints = 0;

        // -- Individual Bonus (Applied per Item)
        if (slot.amount > 0 && UCE_hasIndividualSetBonus() && UCE_validIndividualSetBonus(equipment))
        {
            foreach (UCE_AttributeModifier modifier in ((EquipmentItem)data).individualStatModifiers.UCE_AttributeModifiers)
            {
                if (modifier.template == attribute.template)
                {
                    iPoints += Convert.ToInt32(attribute.points * modifier.percentBonus);
                    iPoints += modifier.flatBonus;
                }
            }
        }

        return iPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // setBonusAttributePartial
    // -----------------------------------------------------------------------------------
#if _iMMOATTRIBUTES

    public int setBonusAttributePartial(ItemSlot slot, SyncListItemSlot equipment, UCE_Attribute attribute)
    {
        int iPoints = 0;

        // -- Partial Bonus (Applied Once)
        if (slot.amount > 0 && UCE_hasPartialSetBonus() && UCE_validPartialSetBonus(equipment))
        {
            foreach (UCE_AttributeModifier modifier in ((EquipmentItem)data).equipmentSet.partialStatModifiers.UCE_AttributeModifiers)
            {
                if (modifier.template == attribute.template)
                {
                    iPoints += Convert.ToInt32(attribute.points * modifier.percentBonus);
                    iPoints += modifier.flatBonus;
                    break;
                }
            }
        }

        return iPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // setBonusAttributeComplete
    // -----------------------------------------------------------------------------------
#if _iMMOATTRIBUTES

    public int setBonusAttributeComplete(ItemSlot slot, SyncListItemSlot equipment, UCE_Attribute attribute)
    {
        int iPoints = 0;

        // -- Complete Bonus (Applied Once)
        if (slot.amount > 0 && UCE_hasCompleteSetBonus() && UCE_validCompleteSetBonus(equipment))
        {
            foreach (UCE_AttributeModifier modifier in ((EquipmentItem)data).equipmentSet.completeStatModifiers.UCE_AttributeModifiers)
            {
                if (modifier.template == attribute.template)
                {
                    iPoints += Convert.ToInt32(attribute.points * modifier.percentBonus);
                    iPoints += modifier.flatBonus;
                    break;
                }
            }
        }

        return iPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // setBonusElementIndividual
    // -----------------------------------------------------------------------------------
#if _iMMOELEMENTS

    public float setBonusElementIndividual(ItemSlot slot, SyncListItemSlot equipment, UCE_ElementTemplate element)
    {
        float fPoints = 0;

        // -- Individual Bonus (Applied per Item)
        if (slot.amount > 0 && UCE_hasIndividualSetBonus() && UCE_validIndividualSetBonus(equipment))
        {
            foreach (UCE_ElementModifier modifier in ((EquipmentItem)data).individualStatModifiers.elementalResistances)
            {
                if (modifier.template == element)
                {
                    fPoints += modifier.value;
                }
            }
        }

        return fPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // setBonusElementPartial
    // -----------------------------------------------------------------------------------
#if _iMMOELEMENTS

    public float setBonusElementPartial(ItemSlot slot, SyncListItemSlot equipment, UCE_ElementTemplate element)
    {
        float fPoints = 0;

        // -- Partial Bonus (Applied Once)
        if (slot.amount > 0 && UCE_hasPartialSetBonus() && UCE_validPartialSetBonus(equipment))
        {
            foreach (UCE_ElementModifier modifier in ((EquipmentItem)data).equipmentSet.partialStatModifiers.elementalResistances)
            {
                if (modifier.template == element)
                {
                    fPoints += modifier.value;
                    break;
                }
            }
        }

        return fPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // setBonusElementComplete
    // -----------------------------------------------------------------------------------
#if _iMMOELEMENTS

    public float setBonusElementComplete(ItemSlot slot, SyncListItemSlot equipment, UCE_ElementTemplate element)
    {
        float fPoints = 0;

        // -- Complete Bonus (Applied Once)
        if (slot.amount > 0 && UCE_hasCompleteSetBonus() && UCE_validCompleteSetBonus(equipment))
        {
            foreach (UCE_ElementModifier modifier in ((EquipmentItem)data).equipmentSet.completeStatModifiers.elementalResistances)
            {
                if (modifier.template == element)
                {
                    fPoints += modifier.value;
                    break;
                }
            }
        }

        return fPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // GETTERS
    // -----------------------------------------------------------------------------------

    public int setIndividualBonusHealth(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.healthBonus : 0;
    }

    public int setPartialBonusHealth(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.healthBonus : 0;
    }

    public int setCompleteBonusHealth(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.healthBonus : 0;
    }

    public int setIndividualBonusMana(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.manaBonus : 0;
    }

    public int setPartialBonusMana(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.manaBonus : 0;
    }

    public int setCompleteBonusMana(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.manaBonus : 0;
    }

#if _iMMOSTAMINA
    public int setIndividualBonusStamina(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.staminaBonus : 0;
    }

    public int setPartialBonusStamina(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.staminaBonus : 0;
    }

    public int setCompleteBonusStamina(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.staminaBonus : 0;
    }
#endif

    public int setIndividualBonusDamage(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.damageBonus : 0;
    }

    public int setPartialBonusDamage(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.damageBonus : 0;
    }

    public int setCompleteBonusDamage(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.damageBonus : 0;
    }

    public int setIndividualBonusDefense(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.defenseBonus : 0;
    }

    public int setPartialBonusDefense(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.defenseBonus : 0;
    }

    public int setCompleteBonusDefense(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.defenseBonus : 0;
    }

    public float setIndividualBonusBlockChance(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.blockChanceBonus : 0;
    }

    public float setPartialBonusBlockChance(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.blockChanceBonus : 0;
    }

    public float setCompleteBonusBlockChance(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.blockChanceBonus : 0;
    }

    public float setIndividualBonusCriticalChance(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.criticalChanceBonus : 0;
    }

    public float setPartialBonusCriticalChance(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.criticalChanceBonus : 0;
    }

    public float setCompleteBonusCriticalChance(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.criticalChanceBonus : 0;
    }

#if _iMMOATTRIBUTES

    public float setIndividualBonusBlockFactor(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusBlockFactor : 0;
    }

    public float setPartialBonusBlockFactor(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusBlockFactor : 0;
    }

    public float setCompleteBonusBlockFactor(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusBlockFactor : 0;
    }

    public float setIndividualBonusCriticalFactor(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusCriticalFactor : 0;
    }

    public float setPartialBonusCriticalFactor(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusCriticalFactor : 0;
    }

    public float setCompleteBonusCriticalFactor(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusCriticalFactor : 0;
    }

    public float setIndividualBonusAccuracy(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusAccuracy : 0;
    }

    public float setPartialBonusAccuracy(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAccuracy : 0;
    }

    public float setCompleteBonusAccuracy(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAccuracy : 0;
    }

    public float setIndividualBonusResistance(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusResistance : 0;
    }

    public float setPartialBonusResistance(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusResistance : 0;
    }

    public float setCompleteBonusResistance(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusResistance : 0;
    }

    public float setIndividualBonusDrainHealthFactor(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusDrainHealthFactor : 0;
    }

    public float setPartialBonusDrainHealthFactor(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDrainHealthFactor : 0;
    }

    public float setCompleteBonusDrainHealthFactor(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDrainHealthFactor : 0;
    }

    public float setIndividualBonusDrainManaFactor(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusDrainManaFactor : 0;
    }

    public float setPartialBonusDrainManaFactor(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDrainManaFactor : 0;
    }

    public float setCompleteBonusDrainManaFactor(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDrainManaFactor : 0;
    }

    public float setIndividualBonusReflectDamageFactor(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusReflectDamageFactor : 0;
    }

    public float setPartialBonusReflectDamageFactor(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusReflectDamageFactor : 0;
    }

    public float setCompleteBonusReflectDamageFactor(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusReflectDamageFactor : 0;
    }

    public float setIndividualBonusDefenseBreakFactor(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusDefenseBreakFactor : 0;
    }

    public float setPartialBonusDefenseBreakFactor(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDefenseBreakFactor : 0;
    }

    public float setCompleteBonusDefenseBreakFactor(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDefenseBreakFactor : 0;
    }

    public float setIndividualBonusBlockBreakFactor(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusBlockBreakFactor : 0;
    }

    public float setPartialBonusBlockBreakFactor(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusBlockBreakFactor : 0;
    }

    public float setCompleteBonusBlockBreakFactor(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusBlockBreakFactor : 0;
    }

    public float setIndividualBonusCriticalEvasion(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusCriticalEvasion : 0;
    }

    public float setPartialBonusCriticalEvasion(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusCriticalEvasion : 0;
    }

    public float setCompleteBonusCriticalEvasion(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusCriticalEvasion : 0;
    }

    public float setIndividualBonusAbsorbHealthFactor(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusAbsorbHealthFactor : 0;
    }

    public float setPartialBonusAbsorbHealthFactor(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAbsorbHealthFactor : 0;
    }

    public float setCompleteBonusAbsorbHealthFactor(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAbsorbHealthFactor : 0;
    }

    public float setIndividualBonusAbsorbManaFactor(SyncListItemSlot equipment)
    {
        return UCE_validIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusAbsorbManaFactor : 0;
    }

    public float setPartialBonusAbsorbManaFactor(SyncListItemSlot equipment)
    {
        return UCE_validPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAbsorbManaFactor : 0;
    }

    public float setCompleteBonusAbsorbManaFactor(SyncListItemSlot equipment)
    {
        return UCE_validCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAbsorbManaFactor : 0;
    }

#endif

    // -----------------------------------------------------------------------------------
    // TOOLTIP
    // -----------------------------------------------------------------------------------
    [DevExtMethods("ToolTip")]
    private void ToolTip_UCE_EquipmentSets(StringBuilder tip)
    {
        if (!(data is EquipmentItem)) return;

        // --------------------------------------------------------------------------- Equipment Set Name
        if (((EquipmentItem)data).equipmentSet != null)
        {
            tip.Append("\n");
            tip.Append("<b>" + ((EquipmentItem)data).equipmentSet.name + "</b>\n");
        }

        // --------------------------------------------------------------------------- Individual Set Bonus
        if (UCE_hasIndividualSetBonus())
        {
            tip.Append("\n");
            tip.Append("<b>-=- Individual Set Bonus -=-</b>\n");
            tip.Append(((EquipmentItem)data).setItems.Length + " / " + ((EquipmentItem)data).setItems.Length + " Item(s) equipped:\n");
            for (int j = 0; j < ((EquipmentItem)data).setItems.Length; ++j)
            {
                if (((EquipmentItem)data).setItems[j] != null) tip.Append("* " + ((EquipmentItem)data).setItems[j].name + "\n");
            }

#if _iMMOATTRIBUTES
            foreach (UCE_AttributeModifier modifier in ((EquipmentItem)data).individualStatModifiers.UCE_AttributeModifiers)
            {
                tip.Append(modifier.template.name + " Bonus: " + modifier.flatBonus.ToString() + "/ " + Mathf.RoundToInt(modifier.percentBonus * 100).ToString() + "%\n");
            }
#endif

#if _iMMOELEMENTS
            foreach (UCE_ElementModifier modifier in ((EquipmentItem)data).individualStatModifiers.elementalResistances)
            {
                tip.Append(modifier.template.name + " Resistance: " + Mathf.RoundToInt(modifier.value * 100).ToString() + "%\n");
            }
#endif

            if (((EquipmentItem)data).individualStatModifiers.healthBonus != 0) tip.Append("Health Bonus:" + ((EquipmentItem)data).individualStatModifiers.healthBonus.ToString() + "\n");
            if (((EquipmentItem)data).individualStatModifiers.manaBonus != 0) tip.Append("Mana Bonus:" + ((EquipmentItem)data).individualStatModifiers.manaBonus.ToString() + "\n");
            if (((EquipmentItem)data).individualStatModifiers.damageBonus != 0) tip.Append("Damage Bonus:" + ((EquipmentItem)data).individualStatModifiers.damageBonus.ToString() + "\n");
            if (((EquipmentItem)data).individualStatModifiers.defenseBonus != 0) tip.Append("Defense Bonus:" + ((EquipmentItem)data).individualStatModifiers.defenseBonus.ToString() + "\n");

            if (((EquipmentItem)data).individualStatModifiers.blockChanceBonus != 0) tip.Append("Block Chance Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.blockChanceBonus * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.criticalChanceBonus != 0) tip.Append("Critical Chance Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.criticalChanceBonus * 100).ToString() + "%\n");
#if _iMMOATTRIBUTES
            if (((EquipmentItem)data).individualStatModifiers.bonusBlockFactor != 0) tip.Append("Block Factor Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusBlockFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.bonusCriticalFactor != 0) tip.Append("Critical Factor Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusCriticalFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.bonusDrainHealthFactor != 0) tip.Append("Drain Health Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusDrainHealthFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.bonusDrainManaFactor != 0) tip.Append("Drain Mana Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusDrainManaFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.bonusReflectDamageFactor != 0) tip.Append("Reflect Damage Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusReflectDamageFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.bonusDefenseBreakFactor != 0) tip.Append("Defense Break Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusDefenseBreakFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.bonusBlockBreakFactor != 0) tip.Append("Block Break Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusBlockBreakFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.bonusCriticalEvasion != 0) tip.Append("Critical Evasion Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusCriticalEvasion * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.bonusAccuracy != 0) tip.Append("Accuracy Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusAccuracy * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.bonusResistance != 0) tip.Append("Resistance Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusResistance * 100).ToString() + "%\n");

            if (((EquipmentItem)data).individualStatModifiers.bonusAbsorbHealthFactor != 0) tip.Append("Absorb Health Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusAbsorbHealthFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).individualStatModifiers.bonusAbsorbManaFactor != 0) tip.Append("Absorb Mana Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).individualStatModifiers.bonusAbsorbManaFactor * 100).ToString() + "%\n");

#endif
        }

        // --------------------------------------------------------------------------- Partial Set Bonus
        if (UCE_hasPartialSetBonus())
        {
            tip.Append("\n");
            tip.Append("<b>-=- Partial Set Bonus -=-</b>\n");
            tip.Append(((EquipmentItem)data).equipmentSet.partialSetItemsCount + " / " + ((EquipmentItem)data).equipmentSet.setItems.Length + " Item(s) equipped.\n");

#if _iMMOATTRIBUTES
            foreach (UCE_AttributeModifier modifier in ((EquipmentItem)data).equipmentSet.partialStatModifiers.UCE_AttributeModifiers)
            {
                tip.Append(modifier.template.name + " Bonus: " + modifier.flatBonus.ToString() + "/ " + Mathf.RoundToInt(modifier.percentBonus * 100).ToString() + "%\n");
            }
#endif

#if _iMMOELEMENTS
            foreach (UCE_ElementModifier modifier in ((EquipmentItem)data).equipmentSet.partialStatModifiers.elementalResistances)
            {
                tip.Append(modifier.template.name + " Resistance: " + Mathf.RoundToInt(modifier.value * 100).ToString() + "%\n");
            }
#endif

            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.healthBonus != 0) tip.Append("Health Bonus:" + ((EquipmentItem)data).equipmentSet.partialStatModifiers.healthBonus.ToString() + "\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.manaBonus != 0) tip.Append("Mana Bonus:" + ((EquipmentItem)data).equipmentSet.partialStatModifiers.manaBonus.ToString() + "\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.damageBonus != 0) tip.Append("Damage Bonus:" + ((EquipmentItem)data).equipmentSet.partialStatModifiers.damageBonus.ToString() + "\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.defenseBonus != 0) tip.Append("Defense Bonus:" + ((EquipmentItem)data).equipmentSet.partialStatModifiers.defenseBonus.ToString() + "\n");

            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.blockChanceBonus != 0) tip.Append("Block Chance Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.blockChanceBonus * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.criticalChanceBonus != 0) tip.Append("Critical Chance Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.criticalChanceBonus * 100).ToString() + "%\n");
#if _iMMOATTRIBUTES
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusBlockFactor != 0) tip.Append("Block Factor Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusBlockFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusCriticalFactor != 0) tip.Append("Critical Factor Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusCriticalFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDrainHealthFactor != 0) tip.Append("Drain Health Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDrainHealthFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDrainManaFactor != 0) tip.Append("Drain Mana Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDrainManaFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusReflectDamageFactor != 0) tip.Append("Reflect Damage Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusReflectDamageFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDefenseBreakFactor != 0) tip.Append("Defense Break Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDefenseBreakFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusBlockBreakFactor != 0) tip.Append("Block Break Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusBlockBreakFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusCriticalEvasion != 0) tip.Append("Critical Evasion Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusCriticalEvasion * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAccuracy != 0) tip.Append("Accuracy Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAccuracy * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusResistance != 0) tip.Append("Resistance Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusResistance * 100).ToString() + "%\n");

            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAbsorbHealthFactor != 0) tip.Append("Absorb Health Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAbsorbHealthFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAbsorbManaFactor != 0) tip.Append("Absorb Mana Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAbsorbManaFactor * 100).ToString() + "%\n");

#endif
        }

        // --------------------------------------------------------------------------- Complete Set Bonus
        if (UCE_hasCompleteSetBonus())
        {
            tip.Append("\n");
            tip.Append("<b>-=- Complete Set Bonus -=-</b>\n");
            tip.Append(((EquipmentItem)data).equipmentSet.setItems.Length + " / " + ((EquipmentItem)data).equipmentSet.setItems.Length + " Item(s) equipped:.\n");
            
            for (int j = 0; j < ((EquipmentItem)data).equipmentSet.setItems.Length; ++j)
            {
                if (((EquipmentItem)data).equipmentSet.setItems[j] != null) tip.Append("* " + ((EquipmentItem)data).equipmentSet.setItems[j].name + "\n");
            }

#if _iMMOATTRIBUTES
            foreach (UCE_AttributeModifier modifier in ((EquipmentItem)data).equipmentSet.completeStatModifiers.UCE_AttributeModifiers)
            {
                tip.Append(modifier.template.name + " Bonus: " + modifier.flatBonus.ToString() + "/ " + Mathf.RoundToInt(modifier.percentBonus * 100).ToString() + "%\n");
            }
#endif

#if _iMMOELEMENTS
            foreach (UCE_ElementModifier modifier in ((EquipmentItem)data).equipmentSet.completeStatModifiers.elementalResistances)
            {
                tip.Append(modifier.template.name + " Resistance: " + Mathf.RoundToInt(modifier.value * 100).ToString() + "%\n");
            }
#endif

            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.healthBonus != 0) tip.Append("Health Bonus:" + ((EquipmentItem)data).equipmentSet.completeStatModifiers.healthBonus.ToString() + "\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.manaBonus != 0) tip.Append("Mana Bonus:" + ((EquipmentItem)data).equipmentSet.completeStatModifiers.manaBonus.ToString() + "\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.damageBonus != 0) tip.Append("Damage Bonus:" + ((EquipmentItem)data).equipmentSet.completeStatModifiers.damageBonus.ToString() + "\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.defenseBonus != 0) tip.Append("Defense Bonus:" + ((EquipmentItem)data).equipmentSet.completeStatModifiers.defenseBonus.ToString() + "\n");

            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.blockChanceBonus != 0) tip.Append("Block Chance Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.blockChanceBonus * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.criticalChanceBonus != 0) tip.Append("Critical Chance Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.criticalChanceBonus * 100).ToString() + "%\n");
#if _iMMOATTRIBUTES
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusBlockFactor != 0) tip.Append("Block Factor Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusBlockFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusCriticalFactor != 0) tip.Append("Critical Factor Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusCriticalFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDrainHealthFactor != 0) tip.Append("Drain Health Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDrainHealthFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDrainManaFactor != 0) tip.Append("Drain Mana Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDrainManaFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusReflectDamageFactor != 0) tip.Append("Reflect Damage Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusReflectDamageFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDefenseBreakFactor != 0) tip.Append("Defense Break Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDefenseBreakFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusBlockBreakFactor != 0) tip.Append("Block Break Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusBlockBreakFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusCriticalEvasion != 0) tip.Append("Critical Evasion Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusCriticalEvasion * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAccuracy != 0) tip.Append("Accuracy Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAccuracy * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusResistance != 0) tip.Append("Resistance Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusResistance * 100).ToString() + "%\n");

            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAbsorbHealthFactor != 0) tip.Append("Absorb Health Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAbsorbHealthFactor * 100).ToString() + "%\n");
            if (((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAbsorbManaFactor != 0) tip.Append("Absorb Mana Bonus:" + Mathf.RoundToInt(((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAbsorbManaFactor * 100).ToString() + "%\n");

#endif
        }
    }
}

#endif
