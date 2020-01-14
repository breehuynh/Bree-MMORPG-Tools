using System;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// =======================================================================================
// ENTITY.STAMINA
// =======================================================================================
public partial class Entity
{

    [Header("STAMINA")]
    public bool _staminaRecovery = true;
    [SerializeField] protected LinearInt _staminaRecoveryRate = new LinearInt { baseValue = -1 };
    [SerializeField] protected LinearInt _staminaMax = new LinearInt { baseValue = 100 };
    [SyncVar] protected int _stamina = 1;

    // -----------------------------------------------------------------------------------
    // StaminaPercent
    // -----------------------------------------------------------------------------------
    public float StaminaPercent()
    {
        return (stamina != 0 && staminaMax != 0) ? (float)stamina / (float)staminaMax : 0;
    }

    // -----------------------------------------------------------------------------------
    // staminaMax
    // -----------------------------------------------------------------------------------
    public virtual int staminaMax
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int passiveBonus = 0;
            foreach (Skill skill in skills)
                if (skill.level > 0 && skill.data is PassiveSkill)
                    passiveBonus += ((PassiveSkill)skill.data).bonusStaminaMax.Get(skill.level);

            int buffBonus = 0;
            for (int i = 0; i < buffs.Count; ++i)
                buffBonus += buffs[i].bonusStaminaMax;

            // base + passives + buffs
            return _staminaMax.Get(level) + passiveBonus + buffBonus;
        }
    }

    // -----------------------------------------------------------------------------------
    // stamina
    // -----------------------------------------------------------------------------------
    public virtual int stamina
    {
        get { return Mathf.Min(_stamina, staminaMax); } // min in case hp>hpmax after buff ends etc.
        set { _stamina = Mathf.Clamp(value, 0, staminaMax); }
    }

    // -----------------------------------------------------------------------------------
    // staminaRecovery
    // -----------------------------------------------------------------------------------
    public virtual bool staminaRecovery
    {
        get
        {
            return staminaRecoveryRate < 0 || (_staminaRecovery && !buffs.Any(x => x.blockStaminaRecovery));
        }
    }

    // -----------------------------------------------------------------------------------
    // staminaRecoveryRate
    // -----------------------------------------------------------------------------------
    public int staminaRecoveryRate
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            float passivePercent = 0;
            foreach (Skill skill in skills)
                if (skill.level > 0 && skill.data is PassiveSkill)
                    passivePercent += ((PassiveSkill)skill.data).bonusStaminaPercentPerSecond.Get(skill.level);

            float buffPercent = 0;
            foreach (Buff buff in buffs)
                buffPercent += buff.bonusStaminaPercentPerSecond;

            // base + passives + buffs
            return _staminaRecoveryRate.Get(level) + Convert.ToInt32(passivePercent * staminaMax) + Convert.ToInt32(buffPercent * staminaMax);
        }
    }

    // -----------------------------------------------------------------------------------

}

// =======================================================================================