// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if _iMMOASSETBUNDLEMANAGER
using Jacovone.AssetBundleMagic;
#endif

// UCE ATTRIBUTE

[CreateAssetMenu(fileName = "New UCE Attribute", menuName = "UCE Templates/New UCE Attribute", order = 999)]
public partial class UCE_AttributeTemplate : ScriptableObject
{
    [Header("-=-=- UCE PLAYER ATTRIBUTE -=-=-")]
    [TextArea(1, 30)] public string toolTip;

    public Sprite image;

    [Header("Health {PERCENTHEALTH} {FLATHEALTH}")]
    [Tooltip("[Optional] Health - increases max health on self")]
    [Range(-999f, 999f)] public float percentHealth = 0f;

    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public int flatHealth = 0;

    [Header("Mana {PERCENTMANA} {FLATMANA}")]
    [Tooltip("[Optional] Mana - increases max mana on self")]
    [Range(-999f, 999f)] public float percentMana = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public int flatMana = 0;

#if _iMMOSTAMINA
    [Header("Stamina {PERCENTSTAMINA} {FLATSTAMINA}")]
    [Tooltip("[Optional] Stamina - increases max stamina on self")]
    [Range(-999f, 999f)] public float percentStamina = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public int flatStamina = 0;
#endif

    [Header("Damage {PERCENTDAMAGE} {FLATDAMAGE}")]
    [Tooltip("[Optional] Damage - increases damage on self")]
    [Range(-999f, 999f)] public float percentDamage = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public int flatDamage = 0;

    [Header("Defense {PERCENTDEFENSE} {FLATDEFENSE}")]
    [Tooltip("[Optional] Defense - increases defense on self")]
    [Range(-999f, 999f)] public float percentDefense = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public int flatDefense = 0;

    [Header("Block Chance {PERCENTBLOCK} {FLATBLOCK}")]
    [Tooltip("[Optional] Block Chance - increases chance to degrade incoming attack to blocked hit (damage reduction)")]
    [Range(-999f, 999f)] public float percentBlock = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatBlock = 0;

    [Header("Critical Chance {PERCENTCRITICAL} {FLATCRITICAL}")]
    [Tooltip("[Optional] Critical Chance - increases chance to upgrade outgoing attack to critical hit (damage increase)")]
    [Range(-999f, 999f)] public float percentCritical = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatCritical = 0;

    [Header("Block Factor {PERCENTBLOCKFACTOR} {FLATBLOCKFACTOR}")]
    [Tooltip("[Optional] Block Factor - increases the damage reduction when blocking an attack (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentBlockFactor = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatBlockFactor = 0;

    [Header("Critical Factor {PERCENTCRITICALFACTOR} {FLATCRITICALFACTOR}")]
    [Tooltip("[Optional] Critical Factor - increases the damage of a critical hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentCriticalFactor = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatCriticalFactor = 0;

    [Header("Accuracy {PERCENTACCURACY} {FLATACCURACY}")]
    [Tooltip("[Optional] Accuracy - increases chance to inflict a (negative) buff on a target (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentAccuracy = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatAccuracy = 0;

    [Header("Resistance {PERCENTRESISTANCE} {FLATRESISTANCE}")]
    [Tooltip("[Optional] Resistance - increases chance to resist a (negative) buff being inflicted on self (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentResistance = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatResistance = 0;

    [Header("Drain Health Factor {PERCENTDRAINHEALTHFACTOR} {FLATDRAINHEALTHFACTOR}")]
    [Tooltip("[Optional] Drain Health Factor - drains a percentage of health from the target with every hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentDrainHealthFactor = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatDrainHealthFactor = 0;

    [Header("Drain Mana Factor {PERCENTDRAINMANAFACTOR} {FLATDRAINMANAFACTOR}")]
    [Tooltip("[Optional] Drain Mana Factor - drains a percentage of mana from the target with every hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentDrainManaFactor = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatDrainManaFactor = 0;

    [Header("Reflect Damage Factor {PERCENTREFLECTDAMAGEFACTOR} {FLATREFLECTDAMAGEFACTOR}")]
    [Tooltip("[Optional] Reflect Damage Factor - reflects a percentage of damage dealt to self back onto the attacker (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentReflectDamageFactor = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatReflectDamageFactor = 0;

    [Header("Defense Break Factor {PERCENTDEFENSEBREAKFACTOR} {FLATDEFENSEBREAKFACTOR}")]
    [Tooltip("[Optional] Defense Break Factor - reduces the targets defense (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentDefenseBreakFactor = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatDefenseBreakFactor = 0;

    [Header("Block Break Factor {PERCENTBLOCKBREAKFACTOR} {FLATBLOCKBREAKFACTOR}")]
    [Tooltip("[Optional] Block Break Factor - reduces the targets block chance (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentBlockBreakFactor = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatBlockBreakFactor = 0;

    [Header("Critical Evasion {PERCENTCRITICALEVASION} {FLATCRITICALEVASION}")]
    [Tooltip("[Optional] Critical Evasion - reduces the chance of receiving a critical hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentCriticalEvasion = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatCriticalEvasion = 0;

    [Header("Absorb Health Factor {PERCENTABSORBHEALTHFACTOR} {FLATABSORBHEALTHFACTOR}")]
    [Tooltip("[Optional] Absorb Health Factor - regnerate a percentage of health on the self when taking a hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentAbsorbHealthFactor = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatAbsorbHealthFactor = 0;

    [Header("Absorb Mana Factor {PERCENTABSORBMANAFACTOR} {FLATABSORBMANAFACTOR}")]
    [Tooltip("[Optional] Absorb Mana Factor - regnerate a percentage of mana on the self when taking a hit (0.01=1%, 0.5=50%, 1=100%)")]
    [Range(-999f, 999f)] public float percentAbsorbManaFactor = 0f;
    [Tooltip("[Note] FLAT = adds fixed value to total. PERCENT = adds percentage of base value to total")]
    public float flatAbsorbManaFactor = 0;

    // -----------------------------------------------------------------------------------
    // Caching
    // -----------------------------------------------------------------------------------
    private static Dictionary<int, UCE_AttributeTemplate> _cache;

    public static Dictionary<int, UCE_AttributeTemplate> dict
    {
        get
        {
            if (_cache == null) {
                UCE_ScripableObjectEntry entry = UCE_TemplateConfiguration.singleton.GetEntry(typeof(UCE_AttributeTemplate));
                string folderName = entry != null ? entry.folderName : "";
#if _iMMOASSETBUNDLEMANAGER
                if (entry != null && entry.loadFromAssetBundle)
                    _cache = AssetBundleMagic.LoadBundle(entry.bundleName).LoadAllAssets<UCE_AttributeTemplate>().ToDictionary(x => x.name.GetStableHashCode(), x => x);
                else
                    _cache = Resources.LoadAll<UCE_AttributeTemplate>(folderName).ToDictionary(x => x.name.GetStableHashCode(), x => x);
#else
                _cache = Resources.LoadAll<UCE_AttributeTemplate>(UCE_TemplateConfiguration.singleton.GetTemplatePath(typeof(UCE_AttributeTemplate))).ToDictionary(x => x.name.GetStableHashCode(), x => x);
#endif
            }

            return _cache;

        }
    }

    // -----------------------------------------------------------------------------------

}
