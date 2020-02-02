// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using System.Collections.Generic;
using Mirror;

// UCE NETWORK SPAWNER

public class UCE_NetworkSpawner : MonoBehaviour
{
    public NetworkSpawn[] networkSpawnObjects;

    protected bool init;
    protected NetworkManagerMMO manager;
    protected List<GameObject> cacheSpawnPrefabs = new List<GameObject>();

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        if (!init)
        {
            manager = FindObjectOfType<NetworkManagerMMO>();

            cacheSpawnPrefabs = manager.spawnPrefabs;

            // make sure we are the server and have objects in the spawnable prefabs list
            if (cacheSpawnPrefabs.Count == 0 || !NetworkServer.active || init) return;

            OnSpawn();
            init = true;
        }
    }

    // -----------------------------------------------------------------------------------
    // OnSpawn
    // -----------------------------------------------------------------------------------
    protected void OnSpawn()
    {
        foreach (NetworkSpawn networkSpawn in networkSpawnObjects)
        {
            // make sure spawnable object is not null and is in the spawnable prefabs list
            if (networkSpawn.gameObject != null && cacheSpawnPrefabs.Contains(networkSpawn.gameObject))
            {
                GameObject gob = Instantiate(networkSpawn.gameObject, networkSpawn.spawnPosition.position, networkSpawn.spawnPosition.rotation);
                manager.UCE_NetworkSpawn(gob);
            }
        }

        Destroy(this.gameObject);
    }

    // -----------------------------------------------------------------------------------
}
