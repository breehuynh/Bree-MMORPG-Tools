using System;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// =======================================================================================
// ENTITY.MORALE
// =======================================================================================
public partial class Entity
{

    [Header("MORALE")]
    public bool _moraleRecovery = true;
    [SerializeField] protected LinearInt _moraleRecoveryRate = new LinearInt { baseValue = 1 };
    [SerializeField] protected LinearInt _moraleMax = new LinearInt { baseValue = 100 };
    [SyncVar] int _morale = 1;

    // -----------------------------------------------------------------------------------
    // MoralePercent
    // -----------------------------------------------------------------------------------
    public float MoralePercent()
    {
        return (morale != 0 && moraleMax != 0) ? (float)morale / (float)moraleMax : 0;
    }

    // -----------------------------------------------------------------------------------
    // moraleMax
    // -----------------------------------------------------------------------------------
    public virtual int moraleMax
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int passiveBonus = 0;
            foreach (Skill skill in skills)
                if (skill.level > 0 && skill.data is PassiveSkill)
                    passiveBonus += ((PassiveSkill)skill.data).bonusMoraleMax.Get(skill.level);

            int buffBonus = 0;
            for (int i = 0; i < buffs.Count; ++i)
                buffBonus += buffs[i].bonusMoraleMax;

            // base + passives + buffs
            return _moraleMax.Get(level) + passiveBonus + buffBonus;
        }
    }

    // -----------------------------------------------------------------------------------
    // morale
    // -----------------------------------------------------------------------------------
    public int morale
    {
        get { return Mathf.Min(_morale, moraleMax); }
        set { _morale = Mathf.Clamp(value, 0, moraleMax); }
    }

    // -----------------------------------------------------------------------------------
    // moraleRecovery
    // -----------------------------------------------------------------------------------
    public virtual bool moraleRecovery
    {
        get
        {
            return moraleRecoveryRate < 0 || (_moraleRecovery && !buffs.Any(x => x.blockMoraleRecovery));
        }
    }

    // -----------------------------------------------------------------------------------
    // moraleRecoveryRate
    // -----------------------------------------------------------------------------------
    public int moraleRecoveryRate
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            float passivePercent = 0;
            foreach (Skill skill in skills)
                if (skill.level > 0 && skill.data is PassiveSkill)
                    passivePercent += ((PassiveSkill)skill.data).bonusMoralePercentPerSecond.Get(skill.level);

            float buffPercent = 0;
            foreach (Buff buff in buffs)
                buffPercent += buff.bonusMoralePercentPerSecond;

            // base + passives + buffs
            return _moraleRecoveryRate.Get(level) + Convert.ToInt32(passivePercent * moraleMax) + Convert.ToInt32(buffPercent * moraleMax);
        }
    }

    // -----------------------------------------------------------------------------------

}

// =======================================================================================