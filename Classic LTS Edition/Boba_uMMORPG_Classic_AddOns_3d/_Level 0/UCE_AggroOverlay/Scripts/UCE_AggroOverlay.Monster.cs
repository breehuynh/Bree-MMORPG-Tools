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

// MONSTER

public partial class Monster
{
    public UCE_AggroOverlay aggroOverlay;

    // -----------------------------------------------------------------------------------
    // OnClientAggro_UCE_AggroOverlay
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    [DevExtMethods("OnClientAggro")]
    private void OnClientAggro_UCE_AggroOverlay(Entity entity)
    {
        if (aggroOverlay == null) return;

        if (
            target != entity ||
            !(entity is Player)
            )
            aggroOverlay.Hide();
        else if (target == null ||
                entity is Player)
            aggroOverlay.Show();
    }

    // -----------------------------------------------------------------------------------
}
