// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// MONSTER

public partial class Monster
{
    // ================================= MISC FUNCS ======================================
    // A bunch of very common utility functions that are missing on the core asset
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // UCE_CanAttack
    // Replaces the built-in CanAttack check. This one can be expanded, the built-in one not.
    // -----------------------------------------------------------------------------------
    public override bool UCE_CanAttack(Entity entity)
    {
        return
            base.UCE_CanAttack(entity)
#if _iMMOPVP
            && UCE_getAttackAllowance(entity)
#endif
            ;
    }

    // -----------------------------------------------------------------------------------
}
