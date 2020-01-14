// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;

// ENTITY

public partial class Entity
{
    public bool _invincible = false;

    // -----------------------------------------------------------------------------------
    // invincible
    // -----------------------------------------------------------------------------------
    public virtual bool invincible
    {
        get
        {
#if _iMMOBUFFINVINCIBILITY
            return _invincible || buffs.Any(x => x.invincibility);
#else
			return _invincible;
#endif
        }
    }

    // -----------------------------------------------------------------------------------
}
