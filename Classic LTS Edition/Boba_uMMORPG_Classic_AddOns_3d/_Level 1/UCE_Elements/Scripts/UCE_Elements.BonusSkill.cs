// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;
using UnityEngine;

public abstract partial class BonusSkill : ScriptableSkill
{
    [Header("-=-=- UCE ELEMENTAL RESISTANCES -=-=-")]
    public LevelBasedElement[] elementalResistances;

    // GetResistance

    public float GetResistance(UCE_ElementTemplate element, int level)
    {
        return elementalResistances.FirstOrDefault(x => x.template == element).Get(level);
    }
}
