// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;

// ENTITY

public partial class Entity
{
    public bool _manaRecovery = true; // can be disabled in combat etc.

    // -----------------------------------------------------------------------------------
    // manaRecovery
    // -----------------------------------------------------------------------------------
    public virtual bool manaRecovery
    {
        get
        {
#if _iMMOBUFFBLOCKMANARECOVERY
            return mana.recoveryRate < 0 || (_manaRecovery && !skills.buffs.Any(x => x.blockManaRecovery));
#else
			return _manaRecovery;
#endif
        }
    }

    // -----------------------------------------------------------------------------------
}
