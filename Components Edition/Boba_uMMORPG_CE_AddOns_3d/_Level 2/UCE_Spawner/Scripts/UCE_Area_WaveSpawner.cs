// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// ===================================================================================
// WAVE SPAWNER AREA
// ===================================================================================
[RequireComponent(typeof(SphereCollider))]
public class UCE_Area_WaveSpawner : NetworkBehaviour
{
    [Header("[UCE WAVE SPAWNER]")]
    [Tooltip("One click de-activation")]
    public bool isActive = true;

    [Tooltip("Does this spawner trigger when the server is launched (Once)?")]
    public bool triggerOnServerLaunch = true;

    [Tooltip("Does this spawner trigger when a player enters the area (repeating)?")]
    public bool triggerOnPlayerEnter = false;

    [Tooltip("[Optional] Optional activation requirements for the entering player")]
    public UCE_ActivationRequirements enterActivationRequirements;

    [Tooltip("Do the monsters unspawn when the last player leaves the area (repeating)?")]
    public bool unspawnOnPlayerExit = false;

    [Tooltip("Delay timer after a player exits the area (default 4 seconds)")]
    public float unspawnDelay = 4f;

    [Header("[Spawn Destinations]")]
    [Tooltip("[Optional] Choose one or more SpawnerArea's as destination. Leave empty to use this area as destination.")]
    public UCE_SpawnDestination[] spawnDestinations;

    [Tooltip("[Optional] Select a Layer to prevent objects being spawned on that layer")]
    public LayerMask doNotSpawnAt;

#if _iMMOMONSTERWAYPOINTS

    [Header("[Dynamic Waypoints (replace default waypoints)]")]
    public Transform[] waypoints;

#endif

    [Header("[Waves]")]
    public UCE_WaveSpawnList[] spawnLists;

    [Header("[Rewards]")]

    [Tooltip("[Optional] A popup with this message will be sent to all players inside the spawner area when all waves are beaten.")]
    public string completedAllWavesMessage;

    [Tooltip("[Optional] All players inside the spawner area will receive these rewards when all waves are beaten.")]
    public UCE_InteractionRewards completionRewards;

    protected Transform center;
    protected float radius;
    protected int x, y;
    protected List<Player> players;

    protected int maxIterationCycles = 75;
    protected bool _isCompleted = false;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    private void Start()
    {
        if (!isActive) return;

        players = new List<Player>();

        if (spawnLists.Length > 0)
        {
            for (int i = 0; i < spawnLists.Length; i++)
            {
                spawnLists[i].Prepare(this, i);
            }
        }

        if (triggerOnServerLaunch)
        {
            foreach (UCE_WaveSpawnList spawnList in spawnLists)
            {
                spawnList.Refresh(1);
                spawnList.Spawn();
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // isBeaten
    // -----------------------------------------------------------------------------------
    public bool IsCompleted()
    {
        return spawnLists.All(x => x.IsCompleted());
    }

    // -------------------------------------------------------------------------------
    // OnTriggerEnter
    // -------------------------------------------------------------------------------
    [ServerCallback]
    private void OnTriggerEnter(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();
        if (player && enterActivationRequirements.checkRequirements(player))
        {
            if (isActive && triggerOnPlayerEnter)
            {
                // -- clear existing objects if we are the first player in a while
                if (players.Count == 0)
                {
                    _isCompleted = false;
                    unspawnGameObjects();
                    players.Clear();
                }

                // -- spawn new objects
                foreach (UCE_WaveSpawnList spawnList in spawnLists)
                {
                    spawnList.Refresh(player.level.current);
                    spawnList.Spawn();
                }

                players.Add(player);
                CancelInvoke("unspawnGameObjects");
            }
        }
    }

    // -------------------------------------------------------------------------------
    // OnTriggerExit
    // -------------------------------------------------------------------------------
    [ServerCallback]
    private void OnTriggerExit(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();
        if (player && player.isAlive)
        {
            if (isActive && unspawnOnPlayerExit)
            {
                players.Remove(player);
                if (players.Count <= 0)
                {
                    Invoke("unspawnGameObjects", unspawnDelay);
                    players.Clear();
                }
            }
        }
    }

    // -------------------------------------------------------------------------------
    // unspawnGameObjects
    // -------------------------------------------------------------------------------
    [Server]
    private void unspawnGameObjects()
    {
        CancelInvoke("unspawnGameObjects");
        foreach (UCE_WaveSpawnList spawnList in spawnLists)
            spawnList.Unspawn();
    }

    // -------------------------------------------------------------------------------
    // notifyPlayersInArea
    // -------------------------------------------------------------------------------
    [ServerCallback]
    public void notifyPlayersInArea(string msg)
    {
        foreach (Player player in players)
        {
            if (player)
            {
                player.UCE_ShowPopup(msg);
            }
        }
    }

    // -------------------------------------------------------------------------------
    // getPlayerCountInArea
    // -------------------------------------------------------------------------------
    public int getPlayerCountInArea()
    {
        return players.Count;
    }

    // -------------------------------------------------------------------------------
    // SpawnChild
    // -------------------------------------------------------------------------------
    [Server]
    public void SpawnOnChild(UCE_WaveSpawnList child)
    {
        StartCoroutine(child.SpawnObject());
    }

    // -------------------------------------------------------------------------------
    // InstantiateOnChild
    // -------------------------------------------------------------------------------
    [Server]
    public GameObject InstantiateOnChild(GameObject go, Vector3 pos, Quaternion rot)
    {
        GameObject gob = Instantiate(go, pos, rot);

#if _iMMOMONSTERWAYPOINTS
        if (gob.GetComponent<Monster>() && waypoints.Length > 0)
            gob.GetComponent<Monster>().waypoints = waypoints;
#endif

        return gob;
    }

    // -------------------------------------------------------------------------------
    // updateMemberPopulation
    // -------------------------------------------------------------------------------
    [ServerCallback]
    public void updateMemberPopulation(int nameHash, int waveIndex)
    {
        // -- update member population
        spawnLists[waveIndex].updateMemberPopulation(nameHash);

        // -- check for spawner completion
        if (IsCompleted() && !_isCompleted)
        {
            _isCompleted = true;

            if (!string.IsNullOrWhiteSpace(completedAllWavesMessage))
                notifyPlayersInArea(completedAllWavesMessage);

            foreach (Player player in players)
            {
                if (player)
                {
                    completionRewards.gainRewards(player);
                }
            }
        }
    }

    // -------------------------------------------------------------------------------
    // getRandomSpawnArea
    // -------------------------------------------------------------------------------
    public Transform getRandomSpawnArea()
    {
        if (spawnDestinations == null || spawnDestinations.Length == 0)
            return this.GetComponent<Transform>();

        foreach (UCE_SpawnDestination spawnDestination in spawnDestinations)
        {
            if (Random.value <= spawnDestination.probability)
                return spawnDestination.transform;
        }

        return this.GetComponent<Transform>();
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
    public Vector3 getRandomSpawnVector(Transform spawnArea, float radius)
    {
        return new Vector3(spawnArea.position.x + Random.Range(radius * -1, radius), spawnArea.position.y, spawnArea.position.z + Random.Range(radius * -1, radius));
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
            spawnPosition = getRandomSpawnVector(spawnArea, radius);

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
