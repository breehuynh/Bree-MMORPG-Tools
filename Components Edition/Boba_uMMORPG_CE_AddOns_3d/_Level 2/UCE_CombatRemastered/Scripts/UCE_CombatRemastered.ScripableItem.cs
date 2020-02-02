// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EquipmentItem
{
    // Create our weapon types to match our skill types.
    public enum EquipType { None, Unarmed, Slash1Hand, Pierce1Hand, Bludgeon1Hand, Slash2Hand, Pierce2Hand, Bludgeon2Hand, RangedThrown, RangedBow, RangedGun, Shield, Dual, Cloth, Leather, Plate }

    // Create a selectable type for each weapon.
    [Header("UCE Combat Remastered - Required Type")]
    public EquipType equipType = EquipType.None;
}
