// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE TOMBSTONE

[System.Serializable]
public partial class UCE_Tombstone
{
    public GameObject[] tombstone;
    [Range(0, 1)] public float tombstoneChance = 1.0f;
    [Range(0, 1)] public float tombstoneFallHeight = 0.5f;
}
