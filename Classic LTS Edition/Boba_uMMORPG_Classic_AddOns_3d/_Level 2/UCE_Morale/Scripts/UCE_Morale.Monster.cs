// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// =======================================================================================
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using System.Collections;

// =======================================================================================
// MONSTER
// =======================================================================================
public partial class Monster {

    // -----------------------------------------------------------------------------------
    // OnDamageDealt_UCE_Morale
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDamageDealt")]
    private void OnDamageDealt_UCE_Morale(int amount)
    {
        morale -= amount;
    }





    // -----------------------------------------------------------------------------------

}

// =======================================================================================
