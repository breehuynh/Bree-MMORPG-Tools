// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

#if _iMMOITEMDROP && _iMMOLOOTRULES

// LOOT RULES - PLAYER

[System.Serializable]
public partial class UCE_PlayerLootRules
{
    [Header("[LOOT RULES]")]
    [Tooltip("After x seconds loot rules are set to 'LootEverybody'. Set to 0 to disable")]
    public float LiftRulesAfter;

    public bool LootVictorParty;
    public bool LootVictorGuild;
#if _iMMOPVP
    public bool LootVictorRealm;
#endif
    public bool LootEverybody;

    [Header("[DROP SETTINGS]")]
    [Tooltip("[Optional] Modifies the drop chance of every item equipped")]
    [Range(-1, 1)] public float equipmentDropModifier;

    [Tooltip("[Optional] Modifies the drop chance of every item in the inventory")]
    [Range(-1, 1)] public float inventoryDropModifier;

    [Header("[ITEM DROP]")]
    public float radiusMultiplier = 1;

    [Tooltip("[Required] Drop prefab used for gold drops")]
    public UCE_ItemDrop goldDropPrefab;

    [Tooltip("[Required] Percentage of gold dropped on death")]
    [Range(0, 1)] public float goldPercentage;

    [HideInInspector] public int dropSolverAttempts = 3;
    [HideInInspector] public double deathTime;
    [HideInInspector, SyncVar] public bool lootRulesLifted;

    // -----------------------------------------------------------------------------------
}

#endif
