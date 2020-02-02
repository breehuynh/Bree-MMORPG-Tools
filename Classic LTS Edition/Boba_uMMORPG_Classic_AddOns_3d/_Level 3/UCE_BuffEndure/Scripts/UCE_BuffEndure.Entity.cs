// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;
using UnityEngine;

// ENTITY

public partial class Entity
{
    public int health
    {
        get { return Mathf.Min(_health, healthMax); } // min in case hp>hpmax after buff ends etc.
        set
        {
#if _iMMOBUFFENDURE
            if (buffs.Any(x => x.endure))
                _health = Mathf.Clamp(value, 1, healthMax);
            else
#endif
                _health = Mathf.Clamp(value, 0, healthMax);
        }
    }

    // -----------------------------------------------------------------------------------
}
