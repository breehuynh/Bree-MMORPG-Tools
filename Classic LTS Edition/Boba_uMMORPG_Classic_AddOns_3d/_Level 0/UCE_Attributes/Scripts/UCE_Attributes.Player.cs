// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

// PLAYER

public partial class Player
{
#if _iMMOATTRIBUTES

    [Header("[-=-=-=- UCE ATTRIBUTES -=-=-=-]")]
    public UCE_playerAttributes playerAttributes;

    protected int[] _cacheStatInt = new int[8]; //padding
    protected float[] _cacheStatFloat = new float[32]; //padding
    protected float[] _cacheStatTimer = new float[32]; //padding
    protected Dictionary<string, UCE_AttributeCache> _attributeCache = new Dictionary<string, UCE_AttributeCache>();

    public SyncListUCE_Attribute UCE_Attributes = new SyncListUCE_Attribute();

    // -----------------------------------------------------------------------------------
    // OnServerCharacterCreate_UCE_Attributes
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerCharacterCreate")]
    private void OnServerCharacterCreate_UCE_Attributes(Player player)
    {

        // -- this is to make sure the maximum value is calculated before loading to the player

        player.health   = player.healthMax;
        player.mana     = player.manaMax;

#if _iMMOSTAMINA
        player.stamina  = player.staminaMax;
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Attributes
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Attributes(Player player)
    {

        // -- this is to make sure the maximum value is calculated before loading to the player

        int tmpHealth   = player.healthMax;
        int tmpMana     = player.manaMax;

#if _iMMOSTAMINA
    	int tmpStamina  = player.staminaMax;
#endif
    }
    
    // ============================== ATTRIBUTE GETTERS ==================================

    // -----------------------------------------------------------------------------------
    // healthMax
    // -----------------------------------------------------------------------------------
    public override int healthMax
    {
        get
        {
            if (Time.time < _cacheStatTimer[0])
                return _cacheStatInt[0];

            // -- Bonus: Equipment
            int equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).healthBonus;

            // -- Bonus: Traits
            int iTraitBonus = 0;
#if _iMMOTRAITS
            iTraitBonus = UCE_Traits.Sum(trait => trait.healthBonus);
#endif

            // -- Bonus: Equipment Sets
            int iSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                                          select (slot.item).setIndividualBonusHealth(equipment)).DefaultIfEmpty(0).Sum());
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                                          select (slot.item).setPartialBonusHealth(equipment)).DefaultIfEmpty(0).FirstOrDefault());
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                                          select (slot.item).setCompleteBonusHealth(equipment)).DefaultIfEmpty(0).FirstOrDefault());
#endif
            // -- Bonus: Attributes
            int attrBonus = CalcHealthMax();

            _cacheStatTimer[0] = Time.time + cacheTimerInterval;

            _cacheStatInt[0] = base.healthMax + equipmentBonus + iTraitBonus + iSetBonus + attrBonus;

            return Mathf.Max(0, _cacheStatInt[0]);
        }
    }

    // -----------------------------------------------------------------------------------
    // manaMax
    // -----------------------------------------------------------------------------------
    public override int manaMax
    {
        get
        {
            if (Time.time < _cacheStatTimer[1])
                return _cacheStatInt[1];

            // -- Bonus: Equipment
            int equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).manaBonus;

            // -- Bonus: Traits
            int iTraitBonus = 0;
#if _iMMOTRAITS
            iTraitBonus = UCE_Traits.Sum(trait => trait.manaBonus);
#endif

            // -- Bonus: Equipment Sets
            int iSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                                          select (slot.item).setIndividualBonusMana(equipment)).DefaultIfEmpty(0).Sum());
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                                          select (slot.item).setPartialBonusMana(equipment)).DefaultIfEmpty(0).FirstOrDefault());
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                                          select (slot.item).setCompleteBonusMana(equipment)).DefaultIfEmpty(0).FirstOrDefault());
#endif
            // -- Bonus: Attributes
            int attrBonus = CalcManaMax();

            _cacheStatTimer[1] = Time.time + cacheTimerInterval;

            _cacheStatInt[1] = base.manaMax + equipmentBonus + iTraitBonus + iSetBonus + attrBonus;

            return Mathf.Max(0, _cacheStatInt[1]);
        }
    }

    // -----------------------------------------------------------------------------------
    // damage
    // -----------------------------------------------------------------------------------
    public override int damage
    {
        get
        {
            if (Time.time < _cacheStatTimer[2])
                return _cacheStatInt[2];

            // -- Bonus: Equipment
            int equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).damageBonus;

            // -- Bonus: Traits
            int iTraitBonus = 0;
#if _iMMOTRAITS
            iTraitBonus = UCE_Traits.Sum(trait => trait.damageBonus);
#endif

            // -- Bonus: Equipment Sets
            int iSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                                          select (slot.item).setIndividualBonusDamage(equipment)).DefaultIfEmpty(0).Sum());
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                                          select (slot.item).setPartialBonusDamage(equipment)).DefaultIfEmpty(0).FirstOrDefault());
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                                          select (slot.item).setCompleteBonusDamage(equipment)).DefaultIfEmpty(0).FirstOrDefault());
#endif

            // -- Bonus: Attributes
            int attrBonus = CalcDamage();

            _cacheStatTimer[2] = Time.time + cacheTimerInterval;

            _cacheStatInt[2] = base.damage + equipmentBonus + iTraitBonus + iSetBonus + attrBonus;

            return Mathf.Max(0, _cacheStatInt[2]);
        }
    }

    // -----------------------------------------------------------------------------------
    // defense
    // -----------------------------------------------------------------------------------
    public override int defense
    {
        get
        {
            if (Time.time < _cacheStatTimer[3])
                return _cacheStatInt[3];

            // -- Bonus: Equipment
            int equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).defenseBonus;

            // -- Bonus: Traits
            int iTraitBonus = 0;
#if _iMMOTRAITS
            iTraitBonus = UCE_Traits.Sum(trait => trait.defenseBonus);
#endif

            // -- Bonus: Equipment Sets
            int iSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                                          select (slot.item).setIndividualBonusDefense(equipment)).DefaultIfEmpty(0).Sum());
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                                          select (slot.item).setPartialBonusDefense(equipment)).DefaultIfEmpty(0).FirstOrDefault());
            iSetBonus += Convert.ToInt32((from slot in equipment
                                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                                          select (slot.item).setCompleteBonusDefense(equipment)).DefaultIfEmpty(0).FirstOrDefault());
#endif

            // -- Bonus: Attributes
            int attrBonus = CalcDefense();

            _cacheStatTimer[3] = Time.time + cacheTimerInterval;

            _cacheStatInt[3] = base.defense + equipmentBonus + iTraitBonus + iSetBonus + attrBonus;

            return Mathf.Max(0, _cacheStatInt[3]);
        }
    }

    // -----------------------------------------------------------------------------------
    // blockChance
    // -----------------------------------------------------------------------------------
    public override float blockChance
    {
        get
        {
            if (Time.time < _cacheStatTimer[4])
                return _cacheStatFloat[0];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).blockChanceBonus;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.blockChanceBonus);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusBlockChance(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusBlockChance(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusBlockChance(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcBlock();

            _cacheStatTimer[4] = Time.time + cacheTimerInterval;

            _cacheStatFloat[0] = base.blockChance + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return Mathf.Max(0, _cacheStatFloat[0]);
        }
    }

    // -----------------------------------------------------------------------------------
    // criticalChance
    // -----------------------------------------------------------------------------------
    public override float criticalChance
    {
        get
        {
            if (Time.time < _cacheStatTimer[5])
                return _cacheStatFloat[1];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).criticalChanceBonus;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.criticalChanceBonus);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusCriticalChance(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusCriticalChance(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusCriticalChance(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcCritical();

            _cacheStatTimer[5] = Time.time + cacheTimerInterval;

            _cacheStatFloat[1] = base.criticalChance + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return Mathf.Max(0, _cacheStatFloat[1]);
        }
    }

    // ================================== EXTRA STATS ====================================

    // -----------------------------------------------------------------------------------
    // blockFactor
    // -----------------------------------------------------------------------------------
    public override float blockFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[6])
                return _cacheStatFloat[2];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusBlockFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusBlockFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusBlockFactor(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusBlockFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusBlockFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcBlockFactor();

            _cacheStatTimer[6] = Time.time + cacheTimerInterval;

            _cacheStatFloat[2] = base.blockFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return Mathf.Max(0, _cacheStatFloat[2]);
        }
    }

    // -----------------------------------------------------------------------------------
    // criticalFactor
    // -----------------------------------------------------------------------------------
    public override float criticalFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[7])
                return _cacheStatFloat[3];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusCriticalFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusCriticalFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusCriticalFactor(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusCriticalFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusCriticalFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcCriticalFactor();

            _cacheStatTimer[7] = Time.time + cacheTimerInterval;

            _cacheStatFloat[3] = base.criticalFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return Mathf.Max(0, _cacheStatFloat[3]);
        }
    }

    // -----------------------------------------------------------------------------------
    // accuracy
    // -----------------------------------------------------------------------------------
    public override float accuracy
    {
        get
        {
            if (Time.time < _cacheStatTimer[8])
                return _cacheStatFloat[4];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusAccuracy;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusAccuracy);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusAccuracy(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusAccuracy(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusAccuracy(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcAccuracy();

            _cacheStatTimer[8] = Time.time + cacheTimerInterval;

            _cacheStatFloat[4] = base.accuracy + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[4];
        }
    }

    // -----------------------------------------------------------------------------------
    // resistance
    // -----------------------------------------------------------------------------------
    public override float resistance
    {
        get
        {
            if (Time.time < _cacheStatTimer[9])
                return _cacheStatFloat[5];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusResistance;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusResistance);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusResistance(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusResistance(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusResistance(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcResistance();

            _cacheStatTimer[9] = Time.time + cacheTimerInterval;

            _cacheStatFloat[5] = base.resistance + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[5];
        }
    }

    // -----------------------------------------------------------------------------------
    // drainHealthFactor
    // -----------------------------------------------------------------------------------
    public override float drainHealthFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[10])
                return _cacheStatFloat[6];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusDrainHealthFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusDrainHealthFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusDrainHealthFactor(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusDrainHealthFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusDrainHealthFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcDrainHealthFactor();

            _cacheStatTimer[10] = Time.time + cacheTimerInterval;

            _cacheStatFloat[6] = base.drainHealthFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[6];
        }
    }

    // -----------------------------------------------------------------------------------
    // drainManaFactor
    // -----------------------------------------------------------------------------------
    public override float drainManaFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[11])
                return _cacheStatFloat[7];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusDrainManaFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusDrainManaFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusDrainManaFactor(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusDrainManaFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusDrainManaFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcDrainManaFactor();

            _cacheStatTimer[11] = Time.time + cacheTimerInterval;

            _cacheStatFloat[7] = base.drainManaFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[7];
        }
    }

    // -----------------------------------------------------------------------------------
    // reflectDamageFactor
    // -----------------------------------------------------------------------------------
    public override float reflectDamageFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[12])
                return _cacheStatFloat[8];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusReflectDamageFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusReflectDamageFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusReflectDamageFactor(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusReflectDamageFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusReflectDamageFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcReflectDamageFactor();

            _cacheStatTimer[12] = Time.time + cacheTimerInterval;

            _cacheStatFloat[8] = base.reflectDamageFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[8];
        }
    }

    // -----------------------------------------------------------------------------------
    // defenseBreakFactor
    // -----------------------------------------------------------------------------------
    public override float defenseBreakFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[13])
                return _cacheStatFloat[9];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusDefenseBreakFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusDefenseBreakFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusDefenseBreakFactor(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusDefenseBreakFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusDefenseBreakFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcDefenseBreakFactor();

            _cacheStatTimer[13] = Time.time + cacheTimerInterval;

            _cacheStatFloat[9] = base.defenseBreakFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[9];
        }
    }

    // -----------------------------------------------------------------------------------
    // blockBreakFactor
    // -----------------------------------------------------------------------------------
    public override float blockBreakFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[14])
                return _cacheStatFloat[10];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusBlockBreakFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusBlockBreakFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusBlockBreakFactor(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusBlockBreakFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusBlockBreakFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcBlockBreakFactor();

            _cacheStatTimer[14] = Time.time + cacheTimerInterval;

            _cacheStatFloat[10] = base.blockBreakFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[10];
        }
    }

    // -----------------------------------------------------------------------------------
    // criticalEvasion
    // -----------------------------------------------------------------------------------
    public override float criticalEvasion
    {
        get
        {
            if (Time.time < _cacheStatTimer[15])
                return _cacheStatFloat[11];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusCriticalEvasion;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusCriticalEvasion);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusCriticalEvasion(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusCriticalEvasion(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusCriticalEvasion(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcCriticalEvasion();

            _cacheStatTimer[15] = Time.time + cacheTimerInterval;

            _cacheStatFloat[11] = base.criticalEvasion + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[11];
        }
    }

    // -----------------------------------------------------------------------------------
    // absorbHealthFactor
    // -----------------------------------------------------------------------------------
    public override float absorbHealthFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[16])
                return _cacheStatFloat[12];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusAbsorbHealthFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusAbsorbHealthFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusAbsorbHealthFactor(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusAbsorbHealthFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusAbsorbHealthFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcAbsorbHealthFactor();

            _cacheStatTimer[16] = Time.time + cacheTimerInterval;

            _cacheStatFloat[12] = base.absorbHealthFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[12];
        }
    }

    // -----------------------------------------------------------------------------------
    // absorbManaFactor
    // -----------------------------------------------------------------------------------
    public override float absorbManaFactor
    {
        get
        {
            if (Time.time < _cacheStatTimer[17])
                return _cacheStatFloat[13];

            // -- Bonus: Equipment
            float equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).bonusAbsorbManaFactor;

            // -- Bonus: Traits
            float fTraitBonus = 0f;
#if _iMMOTRAITS
            fTraitBonus = UCE_Traits.Sum(trait => trait.bonusAbsorbManaFactor);
#endif

            // -- Bonus: Equipment Sets
            float fSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                          select (slot.item).setIndividualBonusAbsorbManaFactor(equipment)).DefaultIfEmpty(0).Sum();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                          select (slot.item).setPartialBonusAbsorbManaFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
            fSetBonus += (from slot in equipment
                          where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                          select (slot.item).setCompleteBonusAbsorbManaFactor(equipment)).DefaultIfEmpty(0).FirstOrDefault();
#endif

            // -- Bonus: Attributes
            float attrBonus = CalcAbsorbManaFactor();

            _cacheStatTimer[17] = Time.time + cacheTimerInterval;

            _cacheStatFloat[13] = base.drainManaFactor + equipmentBonus + fTraitBonus + fSetBonus + attrBonus;

            return _cacheStatFloat[13];
        }
    }

#if _iMMOSTAMINA
    // -----------------------------------------------------------------------------------
    // staminaMax
    // -----------------------------------------------------------------------------------
    public override int staminaMax
    {
        get
        {
            
            if (Time.time < _cacheStatTimer[18])
                return _cacheStatInt[4];

            // -- Bonus: Equipment
            int equipmentBonus = 0;
            foreach (ItemSlot slot in equipment)
                if (slot.amount > 0)
                    equipmentBonus += ((EquipmentItem)slot.item.data).staminaBonus;

            // -- Bonus: Traits
            int iTraitBonus = 0;
#if _iMMOTRAITS
            iTraitBonus = UCE_Traits.Sum(trait => trait.staminaBonus);
#endif

            // -- Bonus: Equipment Sets
            int iSetBonus = 0;
#if _iMMOEQUIPMENTSETS
            iSetBonus += Convert.ToInt32((from slot in equipment
                                  where slot.amount > 0 && slot.item.UCE_hasIndividualSetBonus()
                                  select (slot.item).setIndividualBonusStamina(equipment)).DefaultIfEmpty(0).Sum());
            iSetBonus += Convert.ToInt32((from slot in equipment
                                  where slot.amount > 0 && slot.item.UCE_hasPartialSetBonus()
                                  select (slot.item).setPartialBonusStamina(equipment)).DefaultIfEmpty(0).FirstOrDefault());
            iSetBonus += Convert.ToInt32((from slot in equipment
                                  where slot.amount > 0 && slot.item.UCE_hasCompleteSetBonus()
                                  select (slot.item).setCompleteBonusStamina(equipment)).DefaultIfEmpty(0).FirstOrDefault());
#endif
            // -- Bonus: Attributes
            int attrBonus = CalcStaminaMax();
            
            _cacheStatTimer[18] = Time.time + cacheTimerInterval;
            
            _cacheStatInt[4] = base.staminaMax + equipmentBonus + iTraitBonus + iSetBonus + attrBonus;
            
            return Mathf.Max(0, _cacheStatInt[4]);
            
        }
    }
#endif

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
        foreach (Buff buff in buffs)
        {
            if (buff.data.UCE_AttributeModifiers.Length > 0)
            {
                var validItems = buff.data.UCE_AttributeModifiers.ToList().Where(attr => attr.template == attrib.template);
                validItems.ToList().ForEach(attr => { points += attr.flatBonus; points += (int)Math.Round(attr.percentBonus * _points); });
            }
        }

        // -- Skill Bonus (Passives)
        foreach (Skill skill in skills)
        {
            if (skill.level > 0 && skill.data is PassiveSkill && ((PassiveSkill)skill.data).UCE_AttributeModifiers.Length > 0)
            {
                var validItems = ((PassiveSkill)skill.data).UCE_AttributeModifiers.ToList().Where(attr => attr.template == attrib.template);
                validItems.ToList().ForEach(attr => { points += attr.flatBonus; points += (int)Math.Round(attr.percentBonus * _points); });
            }
        }

        // -- Equipment Bonus
        foreach (ItemSlot slot in equipment)
        {
            if (slot.amount > 0 && ((EquipmentItem)slot.item.data).UCE_AttributeModifiers.Length > 0)
            {
                var validItems = ((EquipmentItem)slot.item.data).UCE_AttributeModifiers.ToList().Where(attr => attr.template == attrib.template);
                validItems.ToList().ForEach(attr => { points += attr.flatBonus; points += (int)Math.Round(attr.percentBonus * _points); });
            }
        }

#if _iMMOEQUIPMENTSETS
        // -- Equipment Bonus (Set Bonus)
        foreach (ItemSlot slot in equipment)
        {
            points += slot.item.setBonusAttributeIndividual(slot, equipment, attrib);
        }

        // -- Equipment Bonus (Set Bonus)
        foreach (ItemSlot slot in equipment)
        {
            int tmpPointsP = slot.item.setBonusAttributePartial(slot, equipment, attrib);
            points += tmpPointsP;
            if (tmpPointsP > 0) break;
        }

        // -- Equipment Bonus (Set Bonus)
        foreach (ItemSlot slot in equipment)
        {
            int tmpPointsC = slot.item.setBonusAttributeComplete(slot, equipment, attrib);
            points += tmpPointsC;
            if (tmpPointsC > 0) break;
        }
#endif

#if _iMMOTRAITS
        // -- Trait Bonus
        foreach (UCE_Trait trait in UCE_Traits)
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
            attributeCache.timer = Time.time + cacheTimerInterval;
            attributeCache.value = points;
            _attributeCache[attrib.name] = attributeCache;
        }
        else
        {
            attributeCache = new UCE_AttributeCache();
            attributeCache.timer = Time.time + cacheTimerInterval;
            attributeCache.value = points;
            _attributeCache.Add(attrib.name, attributeCache);
        }

        return Mathf.Max(0, points);
    }

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
            total += (int)Mathf.Round(_healthMax.Get(level) * pctBonus);
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
            total += (int)Mathf.Round(_manaMax.Get(level) * pctBonus);
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
            total += (int)Mathf.Round(_staminaMax.Get(level) * pctBonus);
        }

        return total;

    }
#endif

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
            total += (int)Mathf.Round(_damage.Get(level) * pctBonus);
        }

        return total;
    }

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
            total += (int)Mathf.Round(_defense.Get(level) * pctBonus);
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
            total += _blockChance.Get(level) * pctBonus;
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
            total += _criticalChance.Get(level) * pctBonus;
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
            total += _blockFactor.Get(level) * pctBonus;
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
            total += _criticalFactor.Get(level) * pctBonus;
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
            total += _accuracy.Get(level) * pctBonus;
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
            total += _resistance.Get(level) * pctBonus;
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
            total += _drainHealthFactor.Get(level) * pctBonus;
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
            total += _drainManaFactor.Get(level) * pctBonus;
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
            total += _reflectDamageFactor.Get(level) * pctBonus;
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
            total += _defenseBreakFactor.Get(level) * pctBonus;
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
            total += _blockBreakFactor.Get(level) * pctBonus;
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
            total += _criticalEvasion.Get(level) * pctBonus;
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
            total += _absorbHealthFactor.Get(level) * pctBonus;
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
            total += _absorbManaFactor.Get(level) * pctBonus;
        }

        return total;
    }

    // =============================== OTHER FUNCTIONS ===================================

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public int UCE_AttributesSpendable()
    {
        int pointsSpent = (from UCE_Attribute in UCE_Attributes
                           select UCE_Attribute.points).Sum();

        int totalPoints = 0;

        //prevent divide by zero error
        if (playerAttributes.everyXLevels > 0)
        {
            //adjust for starting reward level
            totalPoints = level - (playerAttributes.startingRewardLevel - 1);
            //divide so we get points only every x levels
            totalPoints = Mathf.CeilToInt((float)totalPoints / (float)playerAttributes.everyXLevels);
            //adjust if less than zero and multiply by the number of points per level
            totalPoints = Mathf.Max(totalPoints, 0) * playerAttributes.rewardPoints;
            //add starting points
            totalPoints += playerAttributes.startingAttributePoints;
        }

        //final available points is total the client should have so far minus the number they have spent
        return totalPoints - pointsSpent;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_IncreaseAttribute(int index)
    {
        // validate.
        // If we have health and we have greater than zero spendable points and we can see the attribute passed over, increment it
        if (isAlive &&
            UCE_AttributesSpendable() > 0 &&
            0 <= index && index < UCE_Attributes.Count())
        {
            UCE_Attribute attr = UCE_Attributes[index];
            attr.points += 1;
            UCE_Attributes[index] = attr;
        }
    }

#endif

    // -----------------------------------------------------------------------------------
}
