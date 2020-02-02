// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;

// Entity

public partial class Entity
{

    protected bool bRecoveringHealthMana = false;
    protected bool bRecoveringStamina = false;

    // -----------------------------------------------------------------------------------
    // Update_UCE_SmartInvoke
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Update")]
    private void Update_UCE_SmartInvoke()
    {
        if (!isServer) return;
        UCE_CheckRecovery();
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckRecovery
    // -----------------------------------------------------------------------------------
    protected void UCE_CheckRecovery()
    {

        // -> check for health and mana recovery
        if (
            !bRecoveringHealthMana &&
            enabled &&
            isAlive &&
            !(this is Npc) &&
            (
            health.current < health.max ||
            mana.current < mana.max
            )
            )
        {
            InvokeRepeating(nameof(UCE_RecoverHealthMana), 1, 1);
            bRecoveringHealthMana = true;
        }

#if _iMMOSTAMINA
        // -> special case to check for stamina recovery
        if (
            !bRecoveringStamina &&
            enabled &&
            isAlive &&
            !(this is Npc) &&
            state != "IDLE"
            )
        {
            InvokeRepeating(nameof(UCE_RecoverStamina), 1, 1);
            bRecoveringStamina = true;
        }
#endif

    }

    // -----------------------------------------------------------------------------------
    // UCE_RecoverHealthMana
    // -----------------------------------------------------------------------------------
    protected void UCE_RecoverHealthMana()
    {
    
        if (
            !enabled ||
            !isAlive ||
            (!healthRecovery && !manaRecovery) ||
            (health.current >= health.max && mana.current >= mana.max) ||
            (health.recoveryRate == 0 && mana.recoveryRate == 0)
            )
        {
            CancelInvoke(nameof(UCE_RecoverHealthMana));
            bRecoveringHealthMana = false;
            return;
        }

        if (healthRecovery &&
            (health.recoveryRate < 0 ||
            health.recoveryRate > 0
            ))
            health.current += health.recoveryRate;

        if (manaRecovery && 
            (mana.recoveryRate < 0 ||
            mana.recoveryRate > 0
            ))
            mana.current += mana.recoveryRate;

    }

    // -----------------------------------------------------------------------------------
    // UCE_RecoverStamina
    // -----------------------------------------------------------------------------------
    protected void UCE_RecoverStamina()
    {
#if _iMMOSTAMINA
        if (
            !enabled ||
            !isAlive ||
            !staminaRecovery ||
            staminaRecoveryRate == 0 ||
            state == "IDLE"
            )
        {
            CancelInvoke(nameof(UCE_RecoverStamina));
            bRecoveringStamina = false;
            return;
        }


        if (staminaRecovery)
            stamina += staminaRecoveryRate;
#endif
    }

    // -----------------------------------------------------------------------------------
}
