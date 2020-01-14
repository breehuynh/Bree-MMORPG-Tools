// =======================================================================================
// Created and maintained by Boba
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

    public bool     blockMoraleRecovery            => data.blockMoraleRecovery;
    public int      bonusMoraleMax                 => data.bonusMoraleMax.Get(level);
    public float    bonusMoralePercentPerSecond    => data.bonusMoralePercentPerSecond.Get(level);

}
