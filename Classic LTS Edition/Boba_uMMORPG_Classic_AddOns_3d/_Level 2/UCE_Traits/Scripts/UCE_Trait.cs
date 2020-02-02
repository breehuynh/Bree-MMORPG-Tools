// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Linq;

// UCE TRAIT

[System.Serializable]
public partial struct UCE_Trait
{
    public string name;

    // constructors
    public UCE_Trait(UCE_TraitTemplate template)
    {
        name = template.name;
    }

    // does the template still exist?
    public bool TemplateExists()
    {
        return UCE_TraitTemplate.dict.ContainsKey(name.GetStableHashCode());
    }

    // attribute property access
    public UCE_TraitTemplate data
    {
        get { return UCE_TraitTemplate.dict[name.GetStableHashCode()]; }
    }

    // --------------------------------- STAT MODIFIERS ----------------------------------

    // Attribute Modifiers
#if _iMMOATTRIBUTES

#endif

    // Elemental Resistances
#if _iMMOELEMENTS

    public float GetResistance(UCE_ElementTemplate element)
    {
        if (data.statModifiers.elementalResistances.Any(x => x.template == element))
            return data.statModifiers.elementalResistances.FirstOrDefault(x => x.template == element).value;
        else
            return 0;
    }

#endif

    public int healthBonus { get { return data.statModifiers.healthBonus; } }
    public int manaBonus { get { return data.statModifiers.manaBonus; } }
#if _iMMOSTAMINA
    public int staminaBonus { get { return data.statModifiers.staminaBonus; } }
#endif
    public int damageBonus { get { return data.statModifiers.damageBonus; } }
    public int defenseBonus { get { return data.statModifiers.defenseBonus; } }
    public float blockChanceBonus { get { return data.statModifiers.blockChanceBonus; } }
    public float criticalChanceBonus { get { return data.statModifiers.criticalChanceBonus; } }

#if _iMMOATTRIBUTES
    public float bonusBlockFactor { get { return data.statModifiers.bonusBlockFactor; } }
    public float bonusCriticalFactor { get { return data.statModifiers.bonusCriticalFactor; } }
    public float bonusDrainHealthFactor { get { return data.statModifiers.bonusDrainHealthFactor; } }
    public float bonusDrainManaFactor { get { return data.statModifiers.bonusDrainManaFactor; } }
    public float bonusReflectDamageFactor { get { return data.statModifiers.bonusReflectDamageFactor; } }
    public float bonusDefenseBreakFactor { get { return data.statModifiers.bonusDefenseBreakFactor; } }
    public float bonusBlockBreakFactor { get { return data.statModifiers.bonusBlockBreakFactor; } }
    public float bonusCriticalEvasion { get { return data.statModifiers.bonusCriticalEvasion; } }
    public float bonusAccuracy { get { return data.statModifiers.bonusAccuracy; } }
    public float bonusResistance { get { return data.statModifiers.bonusResistance; } }

    public float bonusAbsorbHealthFactor { get { return data.statModifiers.bonusAbsorbHealthFactor; } }
    public float bonusAbsorbManaFactor { get { return data.statModifiers.bonusAbsorbManaFactor; } }

#endif

    // -----------------------------------------------------------------------------------

    /*
    public string ToolTip(int gathered = 0) {
        // we use a StringBuilder so that addons can modify tooltips later too
        // ('string' itself can't be passed as a mutable object)
        StringBuilder tip = new StringBuilder(template.toolTip);
        tip.Replace("{NAME}", name);
        tip.Replace("{PERCENTHEALTH}", percentHealth.ToString("0.0"));
        tip.Replace("{FLATHEALTH}", flatHealth.ToString());
        tip.Replace("{PERCENTMANA}", percentMana.ToString("0.0"));
        tip.Replace("{FLATMANA}", flatMana.ToString());
        tip.Replace("{PERCENTDAMAGE}", percentDamage.ToString("0.0"));
        tip.Replace("{FLATDAMAGE}", flatDamage.ToString());
        tip.Replace("{PERCENTDEFENSE}", percentDefense.ToString("0.0"));
        tip.Replace("{FLATDEFENSE}", flatDefense.ToString());
        tip.Replace("{PERCENTBLOCK}", percentBlock.ToString("0.0"));
        tip.Replace("{PERCENTCRITICAL}", percentCritical.ToString("0.0"));

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("ToolTip", tip);       
        Utils.InvokeMany(typeof(UCE_Attribute), this, "ToolTip_", tip);

        return tip.ToString();
    }
	*/
}

public class SyncListUCE_Trait : SyncList<UCE_Trait> { }
