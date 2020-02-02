// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using UnityEngine;

// PLAYER

public partial class Player
{
    [HideInInspector] public double UCE_SecondsPassed;

    // -----------------------------------------------------------------------------------
    // UCE_RecoverOverTime
    // -----------------------------------------------------------------------------------
    public void UCE_RecoverOverTime(UCE_Area_Sanctuary sanctuary)
    {
        if (sanctuary == null) return;

        if (UCE_SecondsPassed > 0 && 
            (sanctuary.SecondsPerHealth > 0 || 
            sanctuary.SecondsPerMana > 0 ||
#if _iMMOSTAMINA
            sanctuary.SecondsPerStamina > 0 ||
#endif
            sanctuary.SecondsPerExp > 0 || 
            sanctuary.SecondsPerSkillExp > 0 || 
            sanctuary.SecondsPerGold > 0 || 
            sanctuary.SecondsPerCoins > 0)
            )
        {
            int gainHealth      = 0;
            int gainMana        = 0;
            int gainStamina     = 0;
            int gainExp         = 0;
            int gainSkillExp    = 0;
            int gainGold        = 0;
            int gainCoins       = 0;

            if (sanctuary.SecondsPerHealth > 0)     gainHealth = Mathf.Max(0, Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerHealth));
            if (sanctuary.SecondsPerMana > 0)       gainMana = Mathf.Max(0, Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerMana));
#if _iMMOSTAMINA
            if (sanctuary.SecondsPerStamina > 0)    gainStamina = Mathf.Max(0, Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerStamina));
#endif
            if (sanctuary.SecondsPerSkillExp > 0)   gainSkillExp = sanctuary.MaxSkillExp > 0 ? Mathf.Clamp(Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerSkillExp), 0, sanctuary.MaxSkillExp) : Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerSkillExp);
            if (sanctuary.SecondsPerExp > 0)        gainExp = sanctuary.MaxExp > 0 ? Mathf.Clamp(Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerExp), 0, sanctuary.MaxExp) : Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerExp);
            if (sanctuary.SecondsPerGold > 0)       gainGold = sanctuary.MaxGold > 0 ? Mathf.Clamp(Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerGold), 0, sanctuary.MaxGold) : Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerGold);
            if (sanctuary.SecondsPerCoins > 0)      gainCoins = sanctuary.MaxCoins > 0 ? Mathf.Clamp(Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerCoins), 0, sanctuary.MaxCoins) : Convert.ToInt32(UCE_SecondsPassed / sanctuary.SecondsPerCoins);

            if (gainHealth > 0 && health < healthMax)
            {
                health += gainHealth;
                UCE_TargetAddMessage(sanctuary.MSG_HEALTH + gainHealth.ToString());
            }

            if (gainMana > 0 && mana < manaMax)
            {
                mana += gainMana;
                UCE_TargetAddMessage(sanctuary.MSG_MANA + gainMana.ToString());
            }

#if _iMMOSTAMINA
            if (gainStamina > 0 && stamina < staminaMax)
            {
                stamina += gainStamina;
                UCE_TargetAddMessage(sanctuary.MSG_STAMINA + gainStamina.ToString());
            }
#endif

            if (gainExp > 0)
            {
                experience += gainExp;
                UCE_TargetAddMessage(sanctuary.MSG_EXP + gainExp.ToString());
            }

            if (gainSkillExp > 0)
            {
                skillExperience += gainSkillExp;
                UCE_TargetAddMessage(sanctuary.MSG_SKILLEXP + gainSkillExp.ToString());
            }

            if (gainGold > 0)
            {
                gold += gainGold;
                UCE_TargetAddMessage(sanctuary.MSG_GOLD + gainGold.ToString());
            }

            if (gainCoins > 0)
            {
                coins += gainCoins;
                UCE_TargetAddMessage(sanctuary.MSG_COINS + gainCoins.ToString());
            }

#if _iMMOHONORSHOP
            foreach (UCE_Sanctuary_HonorCurrency currency in sanctuary.honorCurrencies)
            {
                if (currency.SecondsPerUnit > 0 && currency.honorCurrency != null)
                {
                    int gainCurrency = currency.MaxUnits > 0 ? Mathf.Clamp(Convert.ToInt32(UCE_SecondsPassed / currency.SecondsPerUnit), 0, currency.MaxUnits) : Convert.ToInt32(UCE_SecondsPassed / currency.SecondsPerUnit);

                    if (gainCurrency > 0)
                    {
                        UCE_AddHonorCurrency(currency.honorCurrency, gainCurrency);
                        UCE_TargetAddMessage(sanctuary.MSG_CURRENCY + gainCurrency.ToString() + " " + currency.honorCurrency.name);
                    }
                }
            }
#endif

            UCE_SecondsPassed = 0;
        }
    }
}
