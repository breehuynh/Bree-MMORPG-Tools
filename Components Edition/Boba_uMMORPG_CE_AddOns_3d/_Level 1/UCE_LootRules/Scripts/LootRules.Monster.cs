// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// ===================================================================================
// LOOT RULES - MONSTER
// ===================================================================================
public partial class Monster
{
    [Header("[-=-=-=- Loot Rules (Victor can always loot) -=-=-=-]")]
    [Tooltip("After x seconds loot rules are set to 'LootEverybody'. Set to 0 to disable")]
    public float LiftRulesAfter;

    public bool LootVictorParty;
    public bool LootVictorGuild;
#if _iMMOPVP
    public bool LootVictorRealm;
#endif
    public bool LootEverybody;

    [SyncVar] protected bool lootRulesLifted;

    // -----------------------------------------------------------------------------------
    // Update_UCE_LootRules
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    [DevExtMethods("Update")]
    private void Update_UCE_LootRules()
    {
        if (LiftRulesAfter > 0 && NetworkTime.time > (deathTimeEnd - deathTime) + LiftRulesAfter)
            lootRulesLifted = true;
        else
            lootRulesLifted = false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_ValidateTaggedLooting
    // -----------------------------------------------------------------------------------
    public bool UCE_ValidateTaggedLooting(Player player)
    {
        if (LootEverybody ||
        lastAggressor == null ||
        lastAggressor == player ||
        (LiftRulesAfter > 0 && lootRulesLifted)
        ) return true;
        if (LootVictorParty && UCE_ValidateTaggedLootingParty(player)) return true;
        if (LootVictorGuild && UCE_ValidateTaggedLootingGuild(player)) return true;
#if _iMMOPVP
        if (LootVictorRealm && UCE_ValidateTaggedLootingRealm(player)) return true;
#endif
        return false;
    }

    // -----------------------------------------------------------------------------------
}
