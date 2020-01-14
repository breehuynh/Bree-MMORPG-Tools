// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

// PLAYER

public partial class Player
{
    // -----------------------------------------------------------------------------------
    // OnStartLocalPlayer_UCE_CantGoNaked
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartLocalPlayer")]
    private void OnStartLocalPlayer_UCE_CantGoNaked()
    {
        OnEquipmentChanged_UCE_CantGoNaked();
    }

    // -----------------------------------------------------------------------------------
    // OnEquipmentChanged_UCE_CantGoNaked
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnEquipmentChanged")]
    private void OnEquipmentChanged_UCE_CantGoNaked()
    {
        for (int i = 0; i < equipment.Count; ++i)
        {
            ItemSlot slot = equipment[i];
            EquipmentInfo info = equipmentInfo[i];

            // clear previous one in any case (when overwriting or clearing)
            if (info.nakedLocation != null && info.nakedLocation.childCount > 0) Destroy(info.nakedLocation.GetChild(0).gameObject);

            if (slot.amount <= 0 && info.nakedModel != null)
            {
                GameObject go = Instantiate(info.nakedModel);
                go.transform.SetParent(info.nakedLocation, false);

                // is it a skinned mesh with an animator?
                Animator anim = go.GetComponent<Animator>();
                if (anim != null)
                {
                    // assign main animation controller to it
                    anim.runtimeAnimatorController = animator.runtimeAnimatorController;

                    // restart all animators, so that skinned mesh equipment will be
                    // in sync with the main animation
                    RebindAnimators();
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
