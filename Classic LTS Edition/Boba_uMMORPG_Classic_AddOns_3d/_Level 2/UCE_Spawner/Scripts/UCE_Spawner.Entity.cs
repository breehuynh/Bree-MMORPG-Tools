// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// ENTITY

public partial class Entity
{
    [HideInInspector] public UCE_Area_WaveSpawner UCE_parentSpawnArea;
    [HideInInspector] public int UCE_parentWaveIndex;

    // -----------------------------------------------------------------------------------
    // OnDeath
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDeath")]
    private void OnDeath_UCE_WaveSpawnerEntity()
    {
        if (UCE_parentSpawnArea == null) return;
        UCE_parentSpawnArea.updateMemberPopulation(name.GetStableHashCode(), UCE_parentWaveIndex);
        UCE_parentSpawnArea = null;
    }

    // -----------------------------------------------------------------------------------
}
