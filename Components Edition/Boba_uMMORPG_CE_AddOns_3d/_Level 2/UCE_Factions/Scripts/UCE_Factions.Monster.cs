// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// PLAYER

public partial class Monster
{
    [Header("-=-=-=- UCE FACTION -=-=-=-")]
    public UCE_Tmpl_Faction myFaction;

    // -----------------------------------------------------------------------------------
    // UCE_checkFactionThreshold
    // -----------------------------------------------------------------------------------
    public bool UCE_checkFactionThreshold(Entity entity)
    {
#if _iMMOPVP
        if (
            entity is Player &&
            myFaction != null &&
            ((Player)entity).UCE_GetFactionRating(myFaction) <= myFaction.aggressiveThreshold)
        {
            return false;
        }
#endif
        return true;
    }

    // -----------------------------------------------------------------------------------
}
