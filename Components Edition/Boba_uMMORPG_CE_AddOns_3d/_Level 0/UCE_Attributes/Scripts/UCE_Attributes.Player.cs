// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

// PLAYER

public partial class Player
{
#if _iMMOATTRIBUTES

    public UCE_PlayerAttributes playerAttributes;

    // -----------------------------------------------------------------------------------
    // OnServerCharacterCreate_UCE_Attributes
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerCharacterCreate")]
    private void OnServerCharacterCreate_UCE_Attributes(Player player)
    {

        // -- this is to make sure the maximum value is calculated before loading to the player

        player.health.current   = player.health.max;
        player.mana.current     = player.mana.max;

#if _iMMOSTAMINA
        player.stamina  = player.staminaMax;
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Attributes
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Attributes(Player player)
    {

        // -- this is to make sure the maximum value is calculated before loading to the player

        int tmpHealth   = player.health.max;
        int tmpMana     = player.mana.max;

#if _iMMOSTAMINA
    	int tmpStamina  = player.staminaMax;
#endif
    }

    // =============================== OTHER FUNCTIONS ===================================

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public int UCE_AttributesSpendable()
    {
        int pointsSpent = (from UCE_Attribute in playerAttributes.UCE_Attributes
                           select UCE_Attribute.points).Sum();

        int totalPoints = 0;

        //prevent divide by zero error
        if (playerAttributes.everyXLevels > 0)
        {
            //adjust for starting reward level
            totalPoints = level.current - (playerAttributes.startingRewardLevel - 1);
            //divide so we get points only every x levels
            totalPoints = Mathf.CeilToInt((float)totalPoints / (float)playerAttributes.everyXLevels);
            //adjust if less than zero and multiply by the number of points per level
            totalPoints = Mathf.Max(totalPoints, 0) * playerAttributes.rewardPoints;
            //add starting points
            totalPoints += playerAttributes.startingAttributePoints;
        }

        //final available points is total the client should have so far minus the number they have spent
        return totalPoints - pointsSpent;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_IncreaseAttribute(int index)
    {
        // validate.
        // If we have health and we have greater than zero spendable points and we can see the attribute passed over, increment it
        if (isAlive &&
            UCE_AttributesSpendable() > 0 &&
            0 <= index && index < playerAttributes.UCE_Attributes.Count())
        {
            UCE_Attribute attr = playerAttributes.UCE_Attributes[index];
            attr.points += 1;
            playerAttributes.UCE_Attributes[index] = attr;
        }
    }


#endif

    // -----------------------------------------------------------------------------------
}
