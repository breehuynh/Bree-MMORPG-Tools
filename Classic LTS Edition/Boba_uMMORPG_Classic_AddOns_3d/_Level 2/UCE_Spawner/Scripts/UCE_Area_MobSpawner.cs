// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// MOB SPAWNER AREA

[RequireComponent(typeof(SphereCollider))]
public class UCE_Area_MobSpawner : UCE_Area_Spawner
{
    [Header("-=-=-=- SPAWN LIST -=-=-=-")]
    public UCE_MobSpawnableGameObject[] spawnableGameObjects;

    [Tooltip("[Optional] Spawner only triggers when X or more players are inside its collider")]
    public int minPlayerCount;

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
        if (players.Count < minPlayerCount) return;

        foreach (UCE_MobSpawnableGameObject spawnableEntity in spawnableGameObjects)
        {
            if (spawnableEntity.probability > 0 && UnityEngine.Random.value <= spawnableEntity.probability)
            {
                Vector3 spawnPosition;
                if (spawnableEntity.transform == null)
                    spawnPosition = getRandomSpawnPosition();
                else
                    spawnPosition = spawnableEntity.transform.position;

                GameObject go = spawnGameObject(spawnableEntity.gameobject, spawnPosition, getRandomSpawnRotation());

                if (go.GetComponent<Entity>() != null)
                {
                    if (spawnableEntity.scaleLevel == ScaleLevel.Add)
                    {
                        go.GetComponent<Entity>().level += spawnableEntity.levelAdjustment;
                    }
                    else if (spawnableEntity.scaleLevel == ScaleLevel.Average)
                    {
                        go.GetComponent<Entity>().level = (int)(go.GetComponent<Monster>().level + spawnableEntity.levelAdjustment) / 2;
                    }
                    else if (spawnableEntity.scaleLevel == ScaleLevel.Override)
                    {
                        go.GetComponent<Entity>().level = spawnableEntity.levelAdjustment;
                    }
                    else if (spawnableEntity.scaleLevel == ScaleLevel.Player)
                    {
                        go.GetComponent<Entity>().level = currentPlayerLevel + spawnableEntity.levelAdjustment;
                    }
                    else if (spawnableEntity.scaleLevel == ScaleLevel.PlayerAverage)
                    {
                        go.GetComponent<Entity>().level = (int)(go.GetComponent<Entity>().level + currentPlayerLevel + spawnableEntity.levelAdjustment) / 2;
                    }
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // OnUnspawn
    // -----------------------------------------------------------------------------------
    protected override void OnUnspawn() { }

    // -----------------------------------------------------------------------------------
}
