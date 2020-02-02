// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using UnityEngine;

// UCE ITEM DROP CHANCE

[Serializable]
public class UCE_ItemDropChance
{
    public ScriptableItem item;
    [Range(0, 1)] public float probability;
    public UCE_ActivationRequirements dropRequirements;
}
