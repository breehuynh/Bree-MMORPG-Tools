// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using UnityEngine;

// -----------------------------------------------------------------------------------
// UCE_PlaceableObjectUpgradeCost
// -----------------------------------------------------------------------------------
[Serializable]
public partial class UCE_PlaceableObjectUpgradeCost
{
    [Tooltip("Minimum level of owner in order to upgrade")]
    public int minLevel;

    [Tooltip("Next level upgrade: Duration in seconds")]
    public float duration;

    [Tooltip("Next level upgrade: Gold Cost")]
    public long gold;

    [Tooltip("Next level upgrade: Coins Cost")]
    public long coins;

    [Tooltip("Next level upgrade: Items Cost")]
    public UCE_ItemRequirement[] requiredItems;
}
