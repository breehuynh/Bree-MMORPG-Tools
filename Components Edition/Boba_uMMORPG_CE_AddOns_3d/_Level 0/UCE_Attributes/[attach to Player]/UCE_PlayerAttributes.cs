using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public partial class UCE_PlayerAttributes : IHealthBonus, IManaBonus, ICombatBonus
{

#if _iMMOATTRIBUTES

    [Header("[-=-=-=- UCE ATTRIBUTES -=-=-=-]")]
    public Entity entity;

    protected int[] _cacheStatInt = new int[8]; //padding
    protected float[] _cacheStatFloat = new float[32]; //padding
    protected float[] _cacheStatTimer = new float[32]; //padding
    protected Dictionary<string, UCE_AttributeCache> _attributeCache = new Dictionary<string, UCE_AttributeCache>();

    public SyncListUCE_Attribute UCE_Attributes = new SyncListUCE_Attribute();

    // ============================== ATTRIBUTE GETTERS ==================================

    public int GetHealthBonus(int baseHealth)
    {
        if (Time.time < _cacheStatTimer[0])
            return _cacheStatInt[0];

        // -- Bonus: Traits
        int iTraitBonus = 0;
#if _iMMOTRAITS
        if (entity is Player)
            iTraitBonus = ((Player) entity).UCE_Traits.Sum(trait => trait.healthBonus);
#endif

        // -- Bonus: Equipment Sets
        int iSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                                          select (slot.item).setIndividualBonusHealth(entity.equipment.slots)).DefaultIfEmpty(0).Sum());
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                                          select (slot.item).setPartialBonusHealth(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault());
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                                          select (slot.item).setCompleteBonusHealth(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault());
#endif
        // -- Bonus: Attributes
        int attrBonus = CalcHealthMax();

        _cacheStatTimer[0] = Time.time + entity.cacheTimerInterval;

        _cacheStatInt[0] = iTraitBonus + iSetBonus + attrBonus;

        return Mathf.Max(0, _cacheStatInt[0]);
    }

    public int GetHealthRecoveryBonus()
    {
        return 0;
    }

    // -----------------------------------------------------------------------------------
    // manaMax
    // -----------------------------------------------------------------------------------

    public int GetManaBonus(int baseMana)
    {
        if (Time.time < _cacheStatTimer[1])
            return _cacheStatInt[1];


        // -- Bonus: Traits
        int iTraitBonus = 0;
#if _iMMOTRAITS
        if (entity is Player)
            iTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.manaBonus);
#endif

        // -- Bonus: Equipment Sets
        int iSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                                          select (slot.item).setIndividualBonusMana(entity.equipment.slots)).DefaultIfEmpty(0).Sum());
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                                          select (slot.item).setPartialBonusMana(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault());
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                                          select (slot.item).setCompleteBonusMana(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault());
#endif
        // -- Bonus: Attributes
        int attrBonus = CalcManaMax();

        _cacheStatTimer[1] = Time.time + entity.cacheTimerInterval;

        _cacheStatInt[1] = iTraitBonus + iSetBonus + attrBonus;

        return Mathf.Max(0, _cacheStatInt[1]);
    }

    public int GetManaRecoveryBonus()
    {
        return 0;
    }

    // -----------------------------------------------------------------------------------
    // damage
    // -----------------------------------------------------------------------------------

    public int GetDamageBonus()
    {
        if (Time.time < _cacheStatTimer[2])
            return _cacheStatInt[2];

        // -- Bonus: Traits
        int iTraitBonus = 0;
#if _iMMOTRAITS
        if (entity is Player)
            iTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.damageBonus);
#endif

        // -- Bonus: Equipment Sets
        int iSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                                          select (slot.item).setIndividualBonusDamage(entity.equipment.slots)).DefaultIfEmpty(0).Sum());
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                                          select (slot.item).setPartialBonusDamage(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault());
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                                          select (slot.item).setCompleteBonusDamage(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault());
#endif

        // -- Bonus: Attributes
        int attrBonus = CalcDamage();

        _cacheStatTimer[2] = Time.time + entity.cacheTimerInterval;

        _cacheStatInt[2] = iTraitBonus + iSetBonus + attrBonus;

        return Mathf.Max(0, _cacheStatInt[2]);
    }

    // -----------------------------------------------------------------------------------
    // defense
    // -----------------------------------------------------------------------------------

    public int GetDefenseBonus()
    {
        if (Time.time < _cacheStatTimer[3])
            return _cacheStatInt[3];

        // -- Bonus: Traits
        int iTraitBonus = 0;
#if _iMMOTRAITS
        if (entity is Player)
            iTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.defenseBonus);
#endif

        // -- Bonus: Equipment Sets
        int iSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                                          select (slot.item).setIndividualBonusDefense(entity.equipment.slots)).DefaultIfEmpty(0).Sum());
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                                          select (slot.item).setPartialBonusDefense(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault());
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                                          select (slot.item).setCompleteBonusDefense(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault());
#endif

        // -- Bonus: Attributes
        int attrBonus = CalcDefense();

        _cacheStatTimer[3] = Time.time + entity.cacheTimerInterval;

        _cacheStatInt[3] = iTraitBonus + iSetBonus + attrBonus;

        return Mathf.Max(0, _cacheStatInt[3]);
}


    // -----------------------------------------------------------------------------------
    // criticalChance
    // -----------------------------------------------------------------------------------

    public float GetCriticalChanceBonus()
    {
        if (Time.time < _cacheStatTimer[5])
            return _cacheStatFloat[1];

        // -- Bonus: Traits
        float fTraitBonus = 0f;
#if _iMMOTRAITS
        if (entity is Player)
            fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.criticalChanceBonus);
#endif

        // -- Bonus: Equipment Sets
        float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusCriticalChance(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusCriticalChance(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusCriticalChance(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

        // -- Bonus: Attributes
        float attrBonus = CalcCritical();

        _cacheStatTimer[5] = Time.time + entity.cacheTimerInterval;

        _cacheStatFloat[1] = fTraitBonus + fSetBonus + attrBonus;

        return Mathf.Max(0, _cacheStatFloat[1]);
    }

    // -----------------------------------------------------------------------------------
    // blockChance
    // -----------------------------------------------------------------------------------

    public float GetBlockChanceBonus()
    {
        if (Time.time < _cacheStatTimer[4])
            return _cacheStatFloat[0];


        // -- Bonus: Traits
        float fTraitBonus = 0f;
#if _iMMOTRAITS
        if (entity is Player)
            fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.blockChanceBonus);
#endif

        // -- Bonus: Equipment Sets
        float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusBlockChance(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusBlockChance(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusBlockChance(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

        // -- Bonus: Attributes
        float attrBonus = CalcBlock();

        _cacheStatTimer[4] = Time.time + entity.cacheTimerInterval;

        _cacheStatFloat[0] =  fTraitBonus + fSetBonus + attrBonus;

        return Mathf.Max(0, _cacheStatFloat[0]);
    }

    // ================================== EXTRA STATS ====================================

    // -----------------------------------------------------------------------------------
    // blockFactor
    // -----------------------------------------------------------------------------------
    public float blockFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[6])
                return _cacheStatFloat[2];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusBlockFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusBlockFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusBlockFactor(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusBlockFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusBlockFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcBlockFactor();

            _cacheStatTimer[6] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[2] = entity.blockFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return Mathf.Max(0, _cacheStatFloat[2]);
        }
    }

    // -----------------------------------------------------------------------------------
    // criticalFactor
    // -----------------------------------------------------------------------------------
    public float criticalFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[7])
                return _cacheStatFloat[3];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusCriticalFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusCriticalFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusCriticalFactor(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusCriticalFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusCriticalFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcCriticalFactor();

            _cacheStatTimer[7] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[3] = entity.criticalFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return Mathf.Max(0, _cacheStatFloat[3]);
        }
    }

    // -----------------------------------------------------------------------------------
    // accuracy
    // -----------------------------------------------------------------------------------
    public float accuracy
    {
        get
        {
            if (Time.time < _cacheStatTimer[8])
                return _cacheStatFloat[4];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusAccuracy;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusAccuracy);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusAccuracy(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusAccuracy(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusAccuracy(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcAccuracy();

            _cacheStatTimer[8] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[4] = entity.accuracy + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[4];
        }
    }

    // -----------------------------------------------------------------------------------
    // resistance
    // -----------------------------------------------------------------------------------
    public float resistance
    {
        get
        {
            if (Time.time < _cacheStatTimer[9])
                return _cacheStatFloat[5];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusResistance;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusResistance);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusResistance(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusResistance(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusResistance(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcResistance();

            _cacheStatTimer[9] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[5] = entity.resistance + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[5];
        }
    }

    // -----------------------------------------------------------------------------------
    // drainHealthFactor
    // -----------------------------------------------------------------------------------
    public float drainHealthFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[10])
                return _cacheStatFloat[6];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusDrainHealthFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusDrainHealthFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusDrainHealthFactor(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusDrainHealthFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusDrainHealthFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcDrainHealthFactor();

            _cacheStatTimer[10] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[6] = entity.drainHealthFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[6];
        }
    }

    // -----------------------------------------------------------------------------------
    // drainManaFactor
    // -----------------------------------------------------------------------------------
    public float drainManaFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[11])
                return _cacheStatFloat[7];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusDrainManaFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusDrainManaFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusDrainManaFactor(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusDrainManaFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusDrainManaFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcDrainManaFactor();

            _cacheStatTimer[11] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[7] = entity.drainManaFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[7];
        }
    }

    // -----------------------------------------------------------------------------------
    // reflectDamageFactor
    // -----------------------------------------------------------------------------------
    public float reflectDamageFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[12])
                return _cacheStatFloat[8];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusReflectDamageFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusReflectDamageFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusReflectDamageFactor(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusReflectDamageFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusReflectDamageFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcReflectDamageFactor();

            _cacheStatTimer[12] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[8] = entity.reflectDamageFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[8];
        }
    }

    // -----------------------------------------------------------------------------------
    // defenseBreakFactor
    // -----------------------------------------------------------------------------------
    public float defenseBreakFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[13])
                return _cacheStatFloat[9];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusDefenseBreakFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusDefenseBreakFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusDefenseBreakFactor(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusDefenseBreakFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusDefenseBreakFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcDefenseBreakFactor();

            _cacheStatTimer[13] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[9] = entity.defenseBreakFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[9];
        }
    }

    // -----------------------------------------------------------------------------------
    // blockBreakFactor
    // -----------------------------------------------------------------------------------
    public float blockBreakFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[14])
                return _cacheStatFloat[10];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusBlockBreakFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusBlockBreakFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusBlockBreakFactor(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusBlockBreakFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusBlockBreakFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcBlockBreakFactor();

            _cacheStatTimer[14] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[10] = entity.blockBreakFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[10];
        }
    }

    // -----------------------------------------------------------------------------------
    // criticalEvasion
    // -----------------------------------------------------------------------------------
    public float criticalEvasion
    {
        get
        {
            if (Time.time < _cacheStatTimer[15])
                return _cacheStatFloat[11];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusCriticalEvasion;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusCriticalEvasion);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusCriticalEvasion(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusCriticalEvasion(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusCriticalEvasion(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcCriticalEvasion();

            _cacheStatTimer[15] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[11] = entity.criticalEvasion + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[11];
        }
    }

    // -----------------------------------------------------------------------------------
    // absorbHealthFactor
    // -----------------------------------------------------------------------------------
    public float absorbHealthFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[16])
                return _cacheStatFloat[12];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusAbsorbHealthFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusAbsorbHealthFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusAbsorbHealthFactor(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusAbsorbHealthFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusAbsorbHealthFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcAbsorbHealthFactor();

            _cacheStatTimer[16] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[12] = entity.absorbHealthFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[12];
        }
    }

    // -----------------------------------------------------------------------------------
    // absorbManaFactor
    // -----------------------------------------------------------------------------------
    public float absorbManaFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[17])
                return _cacheStatFloat[13];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusAbsorbManaFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            if (entity is Player)
                fTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.bonusAbsorbManaFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusAbsorbManaFactor(entity.equipment.slots)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusAbsorbManaFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in entity.equipment.slots
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusAbsorbManaFactor(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcAbsorbManaFactor();

            _cacheStatTimer[17] = Time.time + entity.cacheTimerInterval;

            _cacheStatFloat[13] = entity.drainManaFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[13];
        }
    }

#if _iMMOSTAMINA
    // -----------------------------------------------------------------------------------
    // staminaMax
    // -----------------------------------------------------------------------------------
    public int staminaMax
    {
        get
        {
            
            if (Time.time < _cacheStatTimer[18])
                return _cacheStatInt[4];

            // -- Bonus: Equipment
            int equipmentBonus = 0;
            foreach (ItemSlot slot in entity.equipment.slots)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).staminaBonus;

            // -- Bonus: Traits
            int iTraitBonus = 0;
#if _iMMOTRAITS
            if (entity is Player)
                iTraitBonus = ((Player)entity).UCE_Traits.Sum(trait => trait.staminaBonus);
#endif

            // -- Bonus: Equipment Sets
            int iSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                                  select (slot.item).setIndividualBonusStamina(entity.equipment.slots)).DefaultIfEmpty(0).Sum());
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                                  select (slot.item).setPartialBonusStamina(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault());
            iSetBonus += Convert.ToInt32((from slot in entity.equipment.slots
                                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                                  select (slot.item).setCompleteBonusStamina(entity.equipment.slots)).DefaultIfEmpty(0).FirstOrDefault());
#endif
            // -- Bonus: Attributes
            int attrBonus = CalcStaminaMax();
            
            _cacheStatTimer[18] = Time.time + entity.cacheTimerInterval;
            
            _cacheStatInt[4] = entity.staminaMax + equipmentBonus + iTraitBonus + iSetBonus + attrBonus;
            
            return Mathf.Max(0, _cacheStatInt[4]);
            
        }
    }
#endif


    
    // ============================= CALCULATION FUNCTIONS ===============================

    // -----------------------------------------------------------------------------------
    // CalcHealthMax
    // -----------------------------------------------------------------------------------
    public int CalcHealthMax()
    {
        int total = 0;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            int flatBonus = 0;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatHealth * points;
                pctBonus += attrib.percentHealth * points;
            }

            total += flatBonus;
            total += (int)Mathf.Round(entity.health.baseHealth.Get(entity.level.current) * pctBonus);
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcManaMax
    // -----------------------------------------------------------------------------------
    public int CalcManaMax()
    {
        int total = 0;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            int flatBonus = 0;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatMana * points;
                pctBonus += attrib.percentMana * points;
            }

            total += flatBonus;
            total += (int)Mathf.Round(entity.mana.baseMana.Get(entity.level.current) * pctBonus);
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcDamage
    // -----------------------------------------------------------------------------------
    public int CalcDamage()
    {
        int total = 0;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            int flatBonus = 0;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatDamage * points;
                pctBonus += attrib.percentDamage * points;
            }

            total += flatBonus;
            total += (int)Mathf.Round(entity.combat.baseDamage.Get(entity.level.current) * pctBonus);
        }

        return total;
    }
#if _iMMOSTAMINA
     // -----------------------------------------------------------------------------------
    // CalcStaminaMax
    // -----------------------------------------------------------------------------------
    public int CalcStaminaMax()
    {

        int total = 0;

        if (UCE_Attributes.Count > 0)
        {

            int points = 0;
            int flatBonus = 0;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {

                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatStamina * points;
                pctBonus += attrib.percentStamina * points;

            }

            total += flatBonus;
            total += (int)Mathf.Round(entity._staminaMax.Get(entity.level.current) * pctBonus);
        }

        return total;

    }
#endif

    // -----------------------------------------------------------------------------------
    // CalcDefense
    // -----------------------------------------------------------------------------------
    public int CalcDefense()
    {
        int total = 0;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            int flatBonus = 0;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatDefense * points;
                pctBonus += attrib.percentDefense * points;
            }

            total += flatBonus;
            total += (int)Mathf.Round(entity.combat.baseDefense.Get(entity.level.current) * pctBonus);
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcBlock
    // -----------------------------------------------------------------------------------
    public float CalcBlock()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatBlock * points;
                pctBonus += attrib.percentBlock * points;
            }

            total += flatBonus;
            total += entity.combat.baseBlockChance.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcCritical
    // -----------------------------------------------------------------------------------
    public float CalcCritical()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatCritical * points;
                pctBonus += attrib.percentCritical * points;
            }

            total += flatBonus;
            total += entity.combat.baseCriticalChance.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcBlockFactor
    // -----------------------------------------------------------------------------------
    public float CalcBlockFactor()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatBlockFactor * points;
                pctBonus += attrib.percentBlockFactor * points;
            }

            total += flatBonus;
            total += entity._blockFactor.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcCriticalFactor
    // -----------------------------------------------------------------------------------
    public float CalcCriticalFactor()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatCriticalFactor * points;
                pctBonus += attrib.percentCriticalFactor * points;
            }

            total += flatBonus;
            total += entity._criticalFactor.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcAccuracy
    // -----------------------------------------------------------------------------------
    public float CalcAccuracy()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatAccuracy * points;
                pctBonus += attrib.percentAccuracy * points;
            }

            total += flatBonus;
            total += entity._accuracy.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcResistance
    // -----------------------------------------------------------------------------------
    public float CalcResistance()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatResistance * points;
                pctBonus += attrib.percentResistance * points;
            }

            total += flatBonus;
            total += entity._resistance.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcDrainHealthFactor
    // -----------------------------------------------------------------------------------
    public float CalcDrainHealthFactor()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatDrainHealthFactor * points;
                pctBonus += attrib.percentDrainHealthFactor * points;
            }

            total += flatBonus;
            total += entity._drainHealthFactor.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcDrainManaFactor
    // -----------------------------------------------------------------------------------
    public float CalcDrainManaFactor()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatDrainManaFactor * points;
                pctBonus += attrib.percentDrainManaFactor * points;
            }

            total += flatBonus;
            total += entity._drainManaFactor.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcReflectDamageFactor
    // -----------------------------------------------------------------------------------
    public float CalcReflectDamageFactor()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatReflectDamageFactor * points;
                pctBonus += attrib.percentReflectDamageFactor * points;
            }

            total += flatBonus;
            total += entity._reflectDamageFactor.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcDefenseBreakFactor
    // -----------------------------------------------------------------------------------
    public float CalcDefenseBreakFactor()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatDefenseBreakFactor * points;
                pctBonus += attrib.percentDefenseBreakFactor * points;
            }

            total += flatBonus;
            total += entity._defenseBreakFactor.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcBlockBreakFactor
    // -----------------------------------------------------------------------------------
    public float CalcBlockBreakFactor()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatBlockBreakFactor * points;
                pctBonus += attrib.percentBlockBreakFactor * points;
            }

            total += flatBonus;
            total += entity._blockBreakFactor.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcCriticalEvasion
    // -----------------------------------------------------------------------------------
    public float CalcCriticalEvasion()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatCriticalEvasion * points;
                pctBonus += attrib.percentCriticalEvasion * points;
            }

            total += flatBonus;
            total += entity._criticalEvasion.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcAbsorbHealthFactor
    // -----------------------------------------------------------------------------------
    public float CalcAbsorbHealthFactor()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatAbsorbHealthFactor * points;
                pctBonus += attrib.percentAbsorbHealthFactor * points;
            }

            total += flatBonus;
            total += entity._absorbHealthFactor.Get(entity.level.current) * pctBonus;
        }

        return total;
    }

    // -----------------------------------------------------------------------------------
    // CalcAbsorbManaFactor
    // -----------------------------------------------------------------------------------
    public float CalcAbsorbManaFactor()
    {
        float total = 0f;

        if (UCE_Attributes.Count > 0)
        {
            int points = 0;
            float flatBonus = 0f;
            float pctBonus = 0f;

            foreach (UCE_Attribute attrib in UCE_Attributes)
            {
                points = UCE_calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatAbsorbManaFactor * points;
                pctBonus += attrib.percentAbsorbManaFactor * points;
            }

            total += flatBonus;
            total += entity._absorbManaFactor.Get(entity.level.current) * pctBonus;
        }

        return total;
    }



    // ============================= ATTRIBUTE FUNCTIONS =================================

    // -----------------------------------------------------------------------------------
    // UCE_calculateBonusAttribute
    // -----------------------------------------------------------------------------------
    public int UCE_calculateBonusAttribute(UCE_Attribute attrib)
    {
        int _points = attrib.points;
        int points = 0;
        UCE_AttributeCache attributeCache = null;

        // -- Check Caching
        if (_attributeCache.TryGetValue(attrib.name, out attributeCache) && Time.time < attributeCache.timer)
            return attributeCache.value;

        // ------------------------------- Calculation -----------------------------------

        // -- Buff Bonus
        foreach (Buff buff in entity.skills.buffs)
        {
            if (buff.data.UCE_AttributeModifiers.Length > 0)
            {
                var validItems = buff.data.UCE_AttributeModifiers.ToList().Where(attr => attr.template == attrib.template);
                validItems.ToList().ForEach(attr => { points += attr.flatBonus; points += (int)Math.Round(attr.percentBonus * _points); });
            }
        }

        // -- Skill Bonus (Passives)
        foreach (Skill skill in entity.skills.skills)
        {
            if (skill.level > 0 && skill.data is PassiveSkill && ((PassiveSkill)skill.data).UCE_AttributeModifiers.Length > 0)
            {
                var validItems = ((PassiveSkill)skill.data).UCE_AttributeModifiers.ToList().Where(attr => attr.template == attrib.template);
                validItems.ToList().ForEach(attr => { points += attr.flatBonus; points += (int)Math.Round(attr.percentBonus * _points); });
            }
        }

        // -- Equipment Bonus
        foreach (ItemSlot slot in entity.equipment.slots)
        {
            if (slot.amount > 0 && ((EquipmentItem)slot.item.data).UCE_AttributeModifiers.Length > 0)
            {
                var validItems = ((EquipmentItem)slot.item.data).UCE_AttributeModifiers.ToList().Where(attr => attr.template == attrib.template);
                validItems.ToList().ForEach(attr => { points += attr.flatBonus; points += (int)Math.Round(attr.percentBonus * _points); });
            }
        }

#if _iMMOEQUIPMENTSETS
        // -- Equipment Bonus (Set Bonus)
        foreach (ItemSlot slot in entity.equipment.slots)
        {
            points += slot.item.setBonusAttributeIndividual(slot, entity.equipment.slots, attrib);
        }

        // -- Equipment Bonus (Set Bonus)
        foreach (ItemSlot slot in entity.equipment.slots)
        {
            int tmpPointsP = slot.item.setBonusAttributePartial(slot, entity.equipment.slots, attrib);
            points += tmpPointsP;
            if (tmpPointsP > 0) break;
        }

        // -- Equipment Bonus (Set Bonus)
        foreach (ItemSlot slot in entity.equipment.slots)
        {
            int tmpPointsC = slot.item.setBonusAttributeComplete(slot, entity.equipment.slots, attrib);
            points += tmpPointsC;
            if (tmpPointsC > 0) break;
        }
#endif

#if _iMMOTRAITS
        // -- Trait Bonus
        if (entity is Player)
        foreach (UCE_Trait trait in ((Player)entity).UCE_Traits)
        {
            foreach (UCE_AttributeModifier modifier in trait.data.statModifiers.UCE_AttributeModifiers)
            {
                if (modifier.template == attrib.template)
                {
                    points += modifier.flatBonus;
                    points += (int)Math.Round(modifier.percentBonus * _points);
                }
            }
        }
#endif

        // ----------------------------- Calculation End ---------------------------------

        // -- Update Caching
        if (attributeCache != null)
        {
            attributeCache.timer = Time.time + entity.cacheTimerInterval;
            attributeCache.value = points;
            _attributeCache[attrib.name] = attributeCache;
        }
        else
        {
            attributeCache = new UCE_AttributeCache();
            attributeCache.timer = Time.time + entity.cacheTimerInterval;
            attributeCache.value = points;
            _attributeCache.Add(attrib.name, attributeCache);
        }

        return Mathf.Max(0, points);
    }

#endif

    public float GetBlockChanceBonus()
    {
        return 0;
    }

    public float GetCriticalChanceBonus()
    {
        return 0;
    }

    public int GetDamageBonus()
    {
        return 0;
    }

    public int GetDefenseBonus()
    {
        return 0;
    }
    public int GetHealthBonus(int baseHealth)
    {
        return 0;
    }

    public int GetHealthRecoveryBonus()
    {
        return 0;
    }

    public int GetManaBonus(int baseMana)
    {
        return 0;
    }

    public int GetManaRecoveryBonus()
    {
        return 0;
    }
}