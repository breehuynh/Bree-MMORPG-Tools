// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;
using UnityEngine;

// UCE HEALING SERVICES

[System.Serializable]
public partial class UCE_HealingServices
{
    [Tooltip("One click deactivation")]
    public bool offersHealing;

    [Tooltip("Fixed gold Cost to heal or base cost for healing")]
    public float goldCost;

    [Tooltip("When checked, the cost above is the cost per point of healing")]
    public bool scaleCost;

    [Tooltip("Does it fully recover health?")]
    public bool healHealth;

    [Tooltip("Does it fully recover mana?")]
    public bool healMana;

    [Tooltip("Does it remove buffs (except Offender/Murderer etc.)?")]
    public bool removeBuffs;

    [Tooltip("Does it remove nerfs (buffs set to disadvantageous) (except Offender/Murderer etc.)?")]
    public bool removeNerfs;

#if _iMMOCURSEDEQUIPMENT

    [Tooltip("Does it force-unequip all cursed items up to this curse level from the player?")]
    [Range(0, 9)] public int unequipMaxCursedLevel;

#endif

    // -----------------------------------------------------------------------------------
    // Valid
    // -----------------------------------------------------------------------------------
    public bool Valid(Player player)
    {
        if (!offersHealing) return false;

        bool valid = (player != null && player.isAlive && getCost(player) <= player.gold);

        if (valid)
        {
            valid = false;

            if (healHealth)
                valid = (player.health < player.healthMax) ? true : valid;

            if (healMana)
                valid = (player.mana < player.manaMax) ? true : valid;

            if (removeBuffs)
                valid = player.buffs.Any(x => !x.data.disadvantageous) ? true : valid;

            if (removeNerfs)
                valid = player.buffs.Any(x => x.data.disadvantageous) ? true : valid;

#if _iMMOCURSEDEQUIPMENT
            if (unequipMaxCursedLevel > 0)
                valid = player.equipment.Any(x => x.amount > 0 && ((EquipmentItem)x.item.data).cursedLevel > 0) ? true : valid;
#endif
        }

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // getCost
    // -----------------------------------------------------------------------------------
    public int getCost(Player player)
    {
        if (!scaleCost) return (int)goldCost;

        int price = 0;

        if (healHealth && scaleCost)
            price += (int)((player.healthMax - player.health) * goldCost);

        if (healMana && scaleCost)
            price += (int)((player.manaMax - player.mana) * goldCost);

        if (removeBuffs && scaleCost)
            price += (int)(player.UCE_getBuffCount() * goldCost);

        if (removeNerfs && scaleCost)
            price += (int)(player.UCE_getNerfCount() * goldCost);

        return price;
    }
}
