// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// HARVEST ITEMS

[System.Serializable]
public class UCE_HarvestingHarvestItems
{
    public ScriptableItem template;
    [Range(0, 1)] public float probability;
    [Range(1, 999)] public int minAmount = 1;
    [Range(1, 999)] public int maxAmount = 1;
}
