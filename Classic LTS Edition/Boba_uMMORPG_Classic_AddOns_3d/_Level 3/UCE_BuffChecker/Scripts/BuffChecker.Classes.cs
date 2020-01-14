// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BuffCheckerEntry

[System.Serializable]
public class BuffCheckerEntry
{
    [Tooltip("[Optional] The buff that must be active to toggle the game objects (set to null to use state)")]
    public BuffSkill buffSkill;

    [Tooltip("[Optional] The state the entity has to be in, in order to toggle the game objects (set to empty to use buff)")]
    public string state;

    [Tooltip("[Optional] One or more game objects that are children of the entity, will be toggled active/inactive")]
    public GameObject[] gameObjects;

    [Tooltip("[Optional] Will set an animation parameter Integer to that value, can be used to additional animations (set -1 to disable)")]
    public int animationIndex = -1;

    // -----------------------------------------------------------------------------------
    // isActive
    // -----------------------------------------------------------------------------------
    public bool isActive(Entity entity)
    {
        if (buffSkill)
        {
            int index = entity.buffs.FindIndex(s => s.name == buffSkill.name);

            if (index != -1)
            {
                Buff buff = entity.buffs[index];
                return buff.BuffTimeRemaining() > 0;
            }
        }

        return !string.IsNullOrWhiteSpace(state) && entity.state == state;
    }

    // -----------------------------------------------------------------------------------
    // ToggleGameObject
    // -----------------------------------------------------------------------------------
    public void ToggleGameObject(bool bActive = false)
    {
        foreach (GameObject gameObject in gameObjects)
            gameObject.SetActive(bActive);
    }

    // -----------------------------------------------------------------------------------
}
