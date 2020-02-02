// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE COST - CLASS

[System.Serializable]
public class UCE_Cost
{
    [Tooltip("[Optional] These items will be removed from players inventory")]
    public UCE_ItemRequirement[] itemCost;
    public long goldCost = 0;
    public long coinCost = 0;
    public int healthCost = 0;
    public int manaCost = 0;
    public int experienceCost = 0;
    public int skillExperienceCost = 0;
#if _iMMOHONORSHOP

    [Tooltip("[Optional] Honor Currency costs to use the teleporter")]
    public UCE_HonorShopCurrencyCost[] honorCurrencyCost;
#endif

    // -----------------------------------------------------------------------------------
    // checkCost
    // -----------------------------------------------------------------------------------
    public bool checkCost(Player player)
    {
        if (!player) return false;

        bool valid = true;

        valid = (itemCost.Length == 0 || player.UCE_checkHasItems(player, itemCost)) ? valid : false;
        valid = (goldCost == 0 || player.gold >= goldCost) ? valid : false;
        valid = (coinCost == 0 || player.itemMall.coins >= coinCost) ? valid : false;
        valid = (healthCost == 0 || player.health.current > healthCost) ? valid : false;
        valid = (manaCost == 0 || player.mana.current >= manaCost) ? valid : false;
        valid = (experienceCost == 0 || player.experience.current >= experienceCost) ? valid : false;
        valid = (skillExperienceCost == 0 || ((PlayerSkills)player.skills).skillExperience >= skillExperienceCost) ? valid : false;

#if _iMMOHONORSHOP
        valid = (player.UCE_CheckHonorCurrencyCost(honorCurrencyCost)) ? valid : false;
#endif

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // payCost
    // -----------------------------------------------------------------------------------
    public void payCost(Player player)
    {
        if (!player || !checkCost(player)) return;

        player.UCE_removeItems(player, itemCost);

        player.gold -= goldCost;
        player.itemMall.coins -= coinCost;
        player.health.current -= healthCost;
        player.mana.current -= manaCost;
        player.experience.current -= experienceCost;
        ((PlayerSkills)player.skills).skillExperience -= skillExperienceCost;

#if _iMMOHONORSHOP
        player.UCE_PayHonorCurrencyCost(honorCurrencyCost);
#endif
    }
}
