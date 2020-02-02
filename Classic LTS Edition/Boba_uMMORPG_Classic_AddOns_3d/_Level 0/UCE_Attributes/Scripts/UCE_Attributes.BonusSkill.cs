// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using UnityEngine;
using Mirror;

// BONUS SKILL

public abstract partial class BonusSkill : ScriptableSkill
{
    [Header("[-=-=- UCE ATTRIBUTE MODIFIERS -=-=-]")]
    public UCE_AttributeModifier[] UCE_AttributeModifiers = { };

    [Header("[-=-=- UCE SECONDARY STAT MODIFIERS -=-=-]")]
    [Tooltip("[Optional] Block Factor - increases the damage reduction when blocking an attack (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusBlockFactor;

    [Tooltip("[Optional] Critical Factor - increases the damage of a critical hit (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusCriticalFactor = new LinearFloat { baseValue = 1.5f };

    [Tooltip("[Optional] Accuracy - increases chance to inflict a (negative) buff on a target (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusAccuracy;

    [Tooltip("[Optional] Resistance - increases chance to resist a (negative) buff being inflicted on self (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusResistance;

    [Tooltip("[Optional] Drain Health Factor - drains a percentage of health from the target with every hit (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusDrainHealthFactor;

    [Tooltip("[Optional] Drain Mana Factor - drains a percentage of mana from the target with every hit (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusDrainManaFactor;

    [Tooltip("[Optional] Reflect Damage Factor - reflects a percentage of damage dealt to self back onto the attacker (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusReflectDamageFactor;

    [Tooltip("[Optional] Defense Break Factor - reduces the targets defense (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusDefenseBreakFactor;

    [Tooltip("[Optional] Block Break Factor - reduces the targets block chance (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusBlockBreakFactor;

    [Tooltip("[Optional] Critical Evasion - reduces the chance of receiving a critical hit (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusCriticalEvasion;

    [Tooltip("[Optional] Absorb Health Factor - regnerate a percentage of health on the self when taking a hit (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusAbsorbHealthFactor;

    [Tooltip("[Optional] Absorb Mana Factor - regnerate a percentage of mana on the self when taking a hit (0.01=1%, 0.5=50%, 1=100%)")]
    public LinearFloat bonusAbsorbManaFactor;
}
