// =======================================================================================
// Created and maintained by Boba
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
    [Header("[-=-=-=- UCE MINION -=-=-=-=-]")]
    public GameObject myMaster;

    public bool followMaster;
    public bool boundToMaster;

    // -----------------------------------------------------------------------------------
    // LateUpdate_UCE_Minion
    // -----------------------------------------------------------------------------------
    [DevExtMethods("LateUpdate")]
    private void LateUpdate_UCE_Minion()
    {
        if (myMaster != null && isAlive)
        {
            if (followMaster)
                startPosition = myMaster.transform.position;

            if (boundToMaster && myMaster.GetComponent<Entity>() != null)
            {
                if (!myMaster.GetComponent<Entity>().isAlive)
                    health = 0;
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
