// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using System.Collections;

// PLAYER

public partial class Player
{
    // -----------------------------------------------------------------------------------
    // OnSelect_UCE_IndicatorProjector
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    [DevExtMethods("OnSelect")]
    private void OnSelect_UCE_IndicatorProjector(Entity entity)
    {
        if (entity != null &&
            entity.GetComponent<UCE_IndicatorProjector>() != null &&
            Utils.ClosestDistance(this, entity) <= interactionRange &&
            state == "IDLE"
            )
        {
            UCE_IndicatorProjector ip = entity.GetComponent<UCE_IndicatorProjector>();

            ip.Show();
        }
    }

    // -----------------------------------------------------------------------------------
}
