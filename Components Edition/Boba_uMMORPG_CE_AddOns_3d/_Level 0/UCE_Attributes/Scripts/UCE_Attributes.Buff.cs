// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Mirror;

// BUFF

public partial struct Buff
{
    public float bonusBlockFactor { get { return data.bonusBlockFactor.Get(level); } }
    public float bonusCriticalFactor { get { return data.bonusCriticalFactor.Get(level); } }
    public float bonusDrainHealthFactor { get { return data.bonusDrainHealthFactor.Get(level); } }
    public float bonusDrainManaFactor { get { return data.bonusDrainManaFactor.Get(level); } }
    public float bonusReflectDamageFactor { get { return data.bonusReflectDamageFactor.Get(level); } }
    public float bonusDefenseBreakFactor { get { return data.bonusDefenseBreakFactor.Get(level); } }
    public float bonusBlockBreakFactor { get { return data.bonusBlockBreakFactor.Get(level); } }
    public float bonusCriticalEvasion { get { return data.bonusCriticalEvasion.Get(level); } }
    public float bonusAccuracy { get { return data.bonusAccuracy.Get(level); } }
    public float bonusResistance { get { return data.bonusResistance.Get(level); } }
    public float bonusAbsorbHealthFactor { get { return data.bonusAbsorbHealthFactor.Get(level); } }
    public float bonusAbsorbManaFactor { get { return data.bonusAbsorbManaFactor.Get(level); } }
}
