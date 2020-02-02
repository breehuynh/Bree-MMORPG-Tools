// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;

// PLAYER

public partial class Player
{
    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Healer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_Healer()
    {
        if (state == "IDLE" &&
            target != null &&
            target.isAlive &&
            isAlive &&
            target is Npc &&
            Utils.ClosestDistance(this, target) <= interactionRange &&
            ((Npc)target).healingServices.Valid(this))
        {
            Npc npc = (Npc)target;

            gold -= npc.healingServices.getCost(this);

            if (npc.healingServices.healHealth)
                health = healthMax;

            if (npc.healingServices.healMana)
                mana = manaMax;

            if (npc.healingServices.removeBuffs)
                UCE_CleanupStatusBuffs();

            if (npc.healingServices.removeNerfs)
                UCE_CleanupStatusNerfs();

#if _iMMOCURSEDEQUIPMENT && _iMMOTOOLS
            if (npc.healingServices.unequipMaxCursedLevel > 0)
                UCE_UnequipCursedEquipment(npc.healingServices.unequipMaxCursedLevel); // in Tools
#endif
        }
    }

    // -----------------------------------------------------------------------------------
}
