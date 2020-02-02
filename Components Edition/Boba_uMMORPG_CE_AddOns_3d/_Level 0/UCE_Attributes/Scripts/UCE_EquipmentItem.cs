// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using UnityEngine;

// EQUIPMENT ITEM

#if _iMMOATTRIBUTES

public partial class EquipmentItem
{
    [Header("[-=-=- UCE Attribute Modifiers -=-=-]")]
    public UCE_AttributeModifier[] UCE_AttributeModifiers = { };

    [Header("[-=-=- UCE Extra Stats -=-=-]")]
    [Tooltip("[Optional] Accuracy - increases chance to inflict a (negative) buff on a target (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusAccuracy;

    [Tooltip("[Optional] Resistances - increases chance to resist a (negative) buff being inflicted on self (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusResistance;

    [Tooltip("[Optional] Block Factor - increases the power of blocking an attack (damage reduction) (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusBlockFactor;

    [Tooltip("[Optional] Critical Factor - increases the damage of a critical hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusCriticalFactor;

    [Tooltip("[Optional] Drain Health Factor - drains a percentage of health from the target with every hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusDrainHealthFactor;

    [Tooltip("[Optional] Drain Mana Factor - drains a percentage of mana from the target with every hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusDrainManaFactor;

    [Tooltip("[Optional] Reflect Damage Factor - reflects a percentage of damage dealt to self back onto the attacker (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusReflectDamageFactor;

    [Tooltip("[Optional] Defense Break Factor - reduces the targets defense (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusDefenseBreakFactor;

    [Tooltip("[Optional] Block Break Factor - reduces the targets block chance (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusBlockBreakFactor;

    [Tooltip("[Optional] Critical Evasion - reduces the chance of receiving a critical hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusCriticalEvasion;

    [Tooltip("[Optional] Absorb Health Factor - regenerates a percentage of health on every received hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusAbsorbHealthFactor;

    [Tooltip("[Optional] Absorb Mana Factor - regenerates a percentage of health on every received hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999, 999)] public float bonusAbsorbManaFactor;

    /*
		Tooltip
		Tooltips on partial classes not possible by uMMORPG design at the moment, will
		require another core modification that sets a hook

	*/
}

#endif
