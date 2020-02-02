// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using Mirror;

// UCE ATTRIBUTE

[System.Serializable]
public partial struct UCE_Attribute
{
    public string name;
    public int points;

    // constructor
    public UCE_Attribute(UCE_AttributeTemplate template)
    {
        name = template.name;
        points = 0;
    }

    // does the template still exist?
    public bool TemplateExists()
    {
        return UCE_AttributeTemplate.dict.ContainsKey(name.GetStableHashCode());
    }

    // attribute property access
    public UCE_AttributeTemplate template
    {
        get { return UCE_AttributeTemplate.dict[name.GetStableHashCode()]; }
    }

    public float    percentHealth                   { get { return template.percentHealth; } }
    public int      flatHealth                      { get { return template.flatHealth; } }
    public float    percentMana                     { get { return template.percentMana; } }
    public int      flatMana                        { get { return template.flatMana; } }
#if _iMMOSTAMINA
    public float    percentStamina                  { get { return template.percentStamina; } }
    public int      flatStamina                     { get { return template.flatStamina; } }
#endif
    public float    percentDamage                   { get { return template.percentDamage; } }
    public int      flatDamage                      { get { return template.flatDamage; } }
    public float    percentDefense                  { get { return template.percentDefense; } }
    public int      flatDefense                     { get { return template.flatDefense; } }
    public float    percentBlock                    { get { return template.percentBlock; } }
    public float    flatBlock                       { get { return template.flatBlock; } }
    public float    percentCritical                 { get { return template.percentCritical; } }
    public float    flatCritical                    { get { return template.flatCritical; } }
    public float    percentBlockFactor              { get { return template.percentBlockFactor; } }
    public float    flatBlockFactor                 { get { return template.flatBlockFactor; } }
    public float    percentCriticalFactor           { get { return template.percentCriticalFactor; } }
    public float    flatCriticalFactor              { get { return template.flatCriticalFactor; } }
    public float    percentAccuracy                 { get { return template.percentAccuracy; } }
    public float    flatAccuracy                    { get { return template.flatAccuracy; } }
    public float    percentResistance               { get { return template.percentResistance; } }
    public float    flatResistance                  { get { return template.flatResistance; } }
    public float    percentDrainHealthFactor        { get { return template.percentDrainHealthFactor; } }
    public float    flatDrainHealthFactor           { get { return template.flatDrainHealthFactor; } }
    public float    percentDrainManaFactor          { get { return template.percentDrainManaFactor; } }
    public float    flatDrainManaFactor             { get { return template.flatDrainManaFactor; } }
    public float    percentReflectDamageFactor      { get { return template.percentReflectDamageFactor; } }
    public float    flatReflectDamageFactor         { get { return template.flatReflectDamageFactor; } }
    public float    percentDefenseBreakFactor       { get { return template.percentDefenseBreakFactor; } }
    public float    flatDefenseBreakFactor          { get { return template.flatDefenseBreakFactor; } }
    public float    percentBlockBreakFactor         { get { return template.percentBlockBreakFactor; } }
    public float    flatBlockBreakFactor            { get { return template.flatBlockBreakFactor; } }
    public float    percentCriticalEvasion          { get { return template.percentCriticalEvasion; } }
    public float    flatCriticalEvasion             { get { return template.flatCriticalEvasion; } }
    
    public float    percentAbsorbHealthFactor       { get { return template.percentAbsorbHealthFactor; } }
    public float    flatAbsorbHealthFactor          { get { return template.flatAbsorbHealthFactor; } }
    public float    percentAbsorbManaFactor         { get { return template.percentAbsorbManaFactor; } }
    public float    flatAbsorbManaFactor            { get { return template.flatAbsorbManaFactor; } }

    // -----------------------------------------------------------------------------------
    // ToolTip
    // -----------------------------------------------------------------------------------
    public string ToolTip()
    {
        var tip = new StringBuilder(template.toolTip);

        tip.Replace("{NAME}", name);
        tip.Replace("{PERCENTHEALTH}", (percentHealth * 100).ToString("0.0"));
        tip.Replace("{FLATHEALTH}", flatHealth.ToString());
        tip.Replace("{PERCENTMANA}", (percentMana * 100).ToString("0.0"));
        tip.Replace("{FLATMANA}", flatMana.ToString());
#if _iMMOSTAMINA
        tip.Replace("{PERCENTSTAMINA}",             (percentStamina * 100).ToString("0.0"));
        tip.Replace("{FLATSTAMINA}",                flatStamina.ToString());
#endif
        tip.Replace("{PERCENTDAMAGE}", (percentDamage * 100).ToString("0.0"));
        tip.Replace("{FLATDAMAGE}", flatDamage.ToString());
        tip.Replace("{PERCENTDEFENSE}", (percentDefense * 100).ToString("0.0"));
        tip.Replace("{FLATDEFENSE}", flatDefense.ToString());
        tip.Replace("{PERCENTBLOCK}", (percentBlock * 100).ToString("0.0"));
        tip.Replace("{FLATBLOCK}", flatBlock.ToString());
        tip.Replace("{PERCENTCRITICAL}", (percentCritical * 100).ToString("0.0"));
        tip.Replace("{FLATCRITICAL}", flatCritical.ToString());
        tip.Replace("{PERCENTBLOCKFACTOR}", (percentBlockFactor * 100).ToString("0.0"));
        tip.Replace("{FLATBLOCKFACTOR}", flatBlockFactor.ToString());
        tip.Replace("{PERCENTCRITICALFACTOR}", (percentCriticalFactor * 100).ToString("0.0"));
        tip.Replace("{FLATCRITICALFACTOR}", flatCriticalFactor.ToString());
        tip.Replace("{PERCENTACCURACY}", (percentAccuracy * 100).ToString("0.0"));
        tip.Replace("{FLATACCURACY}", flatAccuracy.ToString());
        tip.Replace("{PERCENTRESISTANCE}", (percentResistance * 100).ToString("0.0"));
        tip.Replace("{FLATRESISTANCE}", flatResistance.ToString());
        tip.Replace("{PERCENTDRAINHEALTHFACTOR}", (percentDrainHealthFactor * 100).ToString("0.0"));
        tip.Replace("{FLATDRAINHEALTHFACTOR}", flatDrainHealthFactor.ToString());
        tip.Replace("{PERCENTDRAINMANAFACTOR}", (percentDrainManaFactor * 100).ToString("0.0"));
        tip.Replace("{FLATDRAINMANAFACTOR}", flatDrainManaFactor.ToString());
        tip.Replace("{PERCENTREFLECTDAMAGEFACTOR}", (percentReflectDamageFactor * 100).ToString("0.0"));
        tip.Replace("{FLATREFLECTDAMAGEFACTOR}", flatReflectDamageFactor.ToString());
        tip.Replace("{PERCENTDEFENSEBREAKFACTOR}", (percentDefenseBreakFactor * 100).ToString("0.0"));
        tip.Replace("{FLATDEFENSEBREAKFACTOR}", flatDefenseBreakFactor.ToString());
        tip.Replace("{PERCENTBLOCKBREAKFACTOR}", (percentBlockBreakFactor * 100).ToString("0.0"));
        tip.Replace("{FLATBLOCKBREAKFACTOR}", flatBlockBreakFactor.ToString());
        tip.Replace("{PERCENTCRITICALEVASION}", (percentCriticalEvasion * 100).ToString("0.0"));
        tip.Replace("{FLATCRITICALEVASION}", flatCriticalEvasion.ToString());

        tip.Replace("{PERCENTABSORBHEALTHFACTOR}", (percentAbsorbHealthFactor * 100).ToString("0.0"));
        tip.Replace("{FLATABSORBHEALTHFACTOR}", flatAbsorbHealthFactor.ToString());
        tip.Replace("{PERCENTABSORBMANAFACTOR}", (percentAbsorbManaFactor * 100).ToString("0.0"));
        tip.Replace("{FLATABSORBMANAFACTOR}", flatAbsorbManaFactor.ToString());

        //this.InvokeInstanceDevExtMethods("ToolTip", tip);
        Utils.InvokeMany(typeof(UCE_Attribute), this, "ToolTip_", tip);

        return tip.ToString();
    }

    // -----------------------------------------------------------------------------------
}

public class SyncListUCE_Attribute : SyncList<UCE_Attribute> { }
