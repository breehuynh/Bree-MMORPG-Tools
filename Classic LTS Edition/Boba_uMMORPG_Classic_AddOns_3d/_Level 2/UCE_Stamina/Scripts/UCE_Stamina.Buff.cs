// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Mirror;

// BUFF

public partial struct Buff
{

    public bool     blockStaminaRecovery            => data.blockStaminaRecovery;
    public int      bonusStaminaMax                 => data.bonusStaminaMax.Get(level);
    public float    bonusStaminaPercentPerSecond    => data.bonusStaminaPercentPerSecond.Get(level);

}
