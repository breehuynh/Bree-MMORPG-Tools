// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// BASE SPAWNER AREA

[RequireComponent(typeof(SphereCollider))]
public abstract class UCE_Area_Spawner : NetworkBehaviour
{
    public enum ScaleLevel { None, Override, Add, Average, Player, PlayerAverage }

    [Header("-=-=-=- UCE SPAWNER -=-=-=-")]
    [Tooltip("[Optional] Do entities unspawn when the last player leaves the area?")]
    public bool unspawnOnPlayerExit = false;

    [Tooltip("[Optional] Delay timer after a player exits the area (default 4 seconds)")]
    public float unspawnDelay = 4f;

    [Tooltip("[Optional] Select a Layer to prevent objects being spawned on that layer")]
    public LayerMask doNotSpawnAt;

    [Tooltip("[Optional] Optional activation requirements for the entering player")]
    public UCE_ActivationRequirements enterActivationRequirements;

    protected List<Player> players;
    protected List<GameObject> gameObjects;
    protected int maxGameObjects;
    protected int currentPlayerLevel;
    protected int maxIterationCycles = 75;

    protected virtual void OnSpawn()
    {
    }

    protected virtual void OnUnspawn()
    {
    }

    protected virtual void UpdateMaxGameObjects()
    {
    }

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public virtual void Start()
    {
        if (!enterActivationRequirements.hasRequirements()) return;
        enterActivationRequirements.setParent(this.gameObject);
        players = new List<Player>();
        gameObjects = new List<GameObject>();
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerEnter
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    private void OnTriggerEnter(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();

        if (player && enterActivationRequirements.checkRequirements(player))
        {
            if (gameObjects.Count < maxGameObjects)
            {
                CancelInvoke("unspawnGameObjects");

                currentPlayerLevel = player.level;
                players.Add(player);

                OnSpawn();
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerExit
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    private void OnTriggerExit(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();

        if (player && unspawnOnPlayerExit)
        {
            players.Remove(player);
            if (players.Count <= 0)
            {
                if (unspawnDelay > 0)
                    Invoke("unspawnGameObjects", unspawnDelay);
                else
                    unspawnGameObjects();
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // unspawnGameObjects
    // -----------------------------------------------------------------------------------
    [Server]
    protected void unspawnGameObjects()
    {
        foreach (GameObject entity in gameObjects)
        {
            if (entity != null)
                NetworkServer.Destroy(entity);
        }
        gameObjects.Clear();
        players.Clear();
        CancelInvoke("unspawnGameObjects");
        OnUnspawn();
    }

    // -----------------------------------------------------------------------------------
    // unspawnGameObjects
    // -----------------------------------------------------------------------------------
    [Server]
    protected GameObject spawnGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject go = Instantiate(prefab, position, rotation);
        go.name = prefab.name; 													// avoid "(Clone)"
        NetworkServer.Spawn(go);
        gameObjects.Add(go);
        return go;
    }

    // -----------------------------------------------------------------------------------
    // getRandomSpawnPosition
    // -----------------------------------------------------------------------------------
    public Quaternion getRandomSpawnRotation()
    {
        return Quaternion.Euler(new Vector3(90, Random.Range(0, 360), 0));
    }

    // -----------------------------------------------------------------------------------
    // getRandomSpawnVector
    // -----------------------------------------------------------------------------------
    public Vector3 getRandomSpawnVector(float radius)
    {
        return new Vector3(this.gameObject.transform.position.x + Random.Range(radius * -1, radius), this.gameObject.transform.position.y, this.gameObject.transform.position.z + Random.Range(radius * -1, radius));
    }

    // -----------------------------------------------------------------------------------
    // getRandomSpawnPosition
    // -----------------------------------------------------------------------------------
    public Vector3 getRandomSpawnPosition(Transform spawnArea = null)
    {
        Vector3 spawnPosition = Vector3.zero;

        if (spawnArea == null)
            spawnArea = this.gameObject.transform;

        int i = 0;
        bool pass = false;
        float radius = spawnArea.GetComponent<SphereCollider>().radius;
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
            {
                break;                                                              //emergency break in case of nothing found after x passes
            }
        }

        return spawnPosition;
    }

    // -----------------------------------------------------------------------------------
}
