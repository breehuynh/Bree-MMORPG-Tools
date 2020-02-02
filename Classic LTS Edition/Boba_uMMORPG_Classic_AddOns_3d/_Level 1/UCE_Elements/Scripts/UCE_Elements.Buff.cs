// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;

// BUFF

public partial struct Buff
{
    public LevelBasedElement[] elementalResistances { get { return data.elementalResistances; } }

    public float GetResistance(UCE_ElementTemplate element)
    {
        return elementalResistances.FirstOrDefault(x => x.template == element).Get(level);
    }
}
