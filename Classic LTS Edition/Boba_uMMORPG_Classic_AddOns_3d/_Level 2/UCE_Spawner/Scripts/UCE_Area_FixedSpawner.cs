// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// FIXED SPAWNER AREA

[RequireComponent(typeof(SphereCollider))]
public class UCE_Area_FixedSpawner : UCE_Area_Spawner
{
    [Header("-=-=-=- SPAWN LIST -=-=-=-")]
    public UCE_FixedSpawnableGameObject[] spawnableGameObjects;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public override void Start()
    {
        base.Start();
        UpdateMaxGameObjects();
    }

    // -----------------------------------------------------------------------------------
    // UpdateMaxGameObjects
    // -----------------------------------------------------------------------------------
    protected override void UpdateMaxGameObjects()
    {
        maxGameObjects = spawnableGameObjects.Length;
    }

    // -----------------------------------------------------------------------------------
    // OnSpawn
    // -----------------------------------------------------------------------------------
    protected override void OnSpawn()
    {
        foreach (UCE_FixedSpawnableGameObject spawnableEntity in spawnableGameObjects)
            spawnGameObject(spawnableEntity.gameobject, spawnableEntity.transform.position, spawnableEntity.transform.rotation);
    }

    // -----------------------------------------------------------------------------------
    // OnUnspawn
    // -----------------------------------------------------------------------------------
    protected override void OnUnspawn() { }

    // -----------------------------------------------------------------------------------
}
