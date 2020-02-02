// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;
using UnityEngine.AI;

// START SPAWNER AREA

[RequireComponent(typeof(SphereCollider))]
public class UCE_Area_StartSpawner : NetworkBehaviour
{
    [Header("-=-=-=- UCE SPAWNER -=-=-=-")]
    [Tooltip("[Optional] Select a Layer to prevent objects being spawned on that layer")]
    public LayerMask doNotSpawnAt;

    [Header("-=-=-=- SPAWN LIST -=-=-=-")]
    [Tooltip("[Optional] Limit the total amount of objects spawned (set to 0 disable)")]
    [Range(0, 999)] public int maxObjects;

    public UCE_StartSpawnableGameObject[] spawnableGameObjects;

    protected int maxIterationCycles = 75;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public void Start()
    {
        OnSpawn();
        NetworkServer.Destroy(this.gameObject);
    }

    // -----------------------------------------------------------------------------------
    // OnSpawn
    // -----------------------------------------------------------------------------------
    protected void OnSpawn()
    {
        int i = 0;

        foreach (UCE_StartSpawnableGameObject spawnableObject in spawnableGameObjects)
        {
            if (spawnableObject.probability > 0 && UnityEngine.Random.value <= spawnableObject.probability)
            {
                spawnGameObject(spawnableObject.gameobject, getRandomSpawnPosition(), getRandomSpawnRotation());
                i++;

                if (maxObjects > 0 && i >= maxObjects)
                    break;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // spawnGameObjects
    // -----------------------------------------------------------------------------------
    [Server]
    protected GameObject spawnGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject go = Instantiate(prefab, position, rotation);
        go.name = prefab.name; 													// avoid "(Clone)"
        NetworkServer.Spawn(go);
        return go;
    }

    // -----------------------------------------------------------------------------------
    // getRandomSpawnPosition
    // -----------------------------------------------------------------------------------
    public Quaternion getRandomSpawnRotation()
    {
        return Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
    }

    // -----------------------------------------------------------------------------------
    // getRandomSpawnVector
    // -----------------------------------------------------------------------------------
    public Vector3 getRandomSpawnVector(float radius)
    {
        return new Vector3(this.gameObject.transform.position.x + Random.Range(radius * -1, radius), 1.0f, this.gameObject.transform.position.z + Random.Range(radius * -1, radius));
    }

    // -----------------------------------------------------------------------------------
    // getRandomSpawnPosition
    // -----------------------------------------------------------------------------------
    public Vector3 getRandomSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;

        int i = 0;
        bool pass = false;
        float radius = GetComponent<SphereCollider>().radius;
        NavMeshHit hit;

        while (!pass)
        {
            i++;
            spawnPosition = getRandomSpawnVector(radius);

            if (NavMesh.SamplePosition(spawnPosition, out hit, radius, NavMesh.AllAreas) &&
                !Physics.Raycast(spawnPosition, Vector3.down, radius, doNotSpawnAt.value)
            )
            {
                return spawnPosition;
            }

            if (i > maxIterationCycles)
                break;                                                              //emergency break in case of nothing found after x passes
        }

        return spawnPosition;
    }

    // -----------------------------------------------------------------------------------
}
