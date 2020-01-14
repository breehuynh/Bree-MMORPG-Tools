// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Linq;
using UnityEngine;

// NPC

public partial class Npc
{
    [Header("-=-=- UCE Walking NPC -=-=-")]
    [Tooltip("[Optional] Probability to move random or patrol to next waypoint.")]
    [Range(0, 1)] public float moveProbability = 0.1f;

    [Tooltip("[Optional] Maximum move range for random movement only.")]
    public float moveDistance = 10;

    [Tooltip("[Optional] Patrols between waypoints.")]
    public Transform[] waypoints;

    [Tooltip("[Optional] 0 = Waypoints only, 1 = Random only, or any in-between.")]
    [Range(0, 1)] public float WaypointToRandomRatio;

    [Tooltip("[Optional] How close must a player be in order for the Npc to supend movement?")]
    public float playerDetectionRadius = 3.0f;

    protected int destPoint = 0;
    protected Vector3 startPosition;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    protected override void Start()
    {
        base.Awake();
        startPosition = transform.position;

        agent.stoppingDistance = 0;
    }

    // -----------------------------------------------------------------------------------
    // FSM EVENTS
    // -----------------------------------------------------------------------------------

    private bool EventMoveEnd()
    {
        return state == "MOVING" && !IsMoving();
    }

    private bool EventMoveRandomly()
    {
        return UnityEngine.Random.value <= moveProbability * Time.deltaTime;
    }

    // -----------------------------------------------------------------------------------
    // LateUpdate
    // -----------------------------------------------------------------------------------
    private void LateUpdate()
    {
        // only if worth updating right now (e.g. a player is around)
        if (!IsWorthUpdating()) return;

        if (isClient) // no need for animations on the server
        {
            if (animator.parameters.Any(x => x.name == "MOVING"))
                animator.SetBool("MOVING", state == "MOVING" && agent.velocity != Vector3.zero);
        }

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("LateUpdate"); 
        Utils.InvokeMany(typeof(Npc), this, "LateUpdate_");
    }

    // -----------------------------------------------------------------------------------
    // UpdateServer_IDLE
    // -----------------------------------------------------------------------------------
    [Server]
    [DevExtMethods("UpdateServer")]
    private string UpdateServer_IDLE()
    {
        if (EventMoveRandomly())
        {
            if (GetPlayersNearby(playerDetectionRadius)) return "IDLE";

            if (UnityEngine.Random.value <= WaypointToRandomRatio || waypoints.Length == 0)
            {
                GotoRandomPoint();
            }
            else
            {
                GotoNextWayPoint();
            }

            return "MOVING";
        }

        if (EventMoveEnd()) { }

        return "IDLE";
    }

    // -----------------------------------------------------------------------------------
    // UpdateServer_MOVING
    // -----------------------------------------------------------------------------------
    [Server]
    [DevExtMethods("UpdateServer")]
    private string UpdateServer_MOVING()
    {
        if (EventMoveEnd())
        {
            return "IDLE";
        }

        if (EventMoveRandomly()) { }

        return "MOVING";
    }

    // -----------------------------------------------------------------------------------
    // UpdateServer
    // -----------------------------------------------------------------------------------
    [Server]
    protected override string UpdateServer()
    {
        if (state == "IDLE") return UpdateServer_IDLE();
        if (state == "MOVING") return UpdateServer_MOVING();
        return "IDLE";
    }

    // -----------------------------------------------------------------------------------
    // GetPlayersNearby
    // -----------------------------------------------------------------------------------
    public bool GetPlayersNearby(float fRadius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, fRadius);
        foreach (Collider co in colliders)
        {
            Player player = co.GetComponentInParent<Player>();
            if (player != null)
                return true;
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    // GotoNextWayPoint
    // -----------------------------------------------------------------------------------
    public void GotoNextWayPoint()
    {
        if (waypoints.Length == 0) return;
        agent.destination = waypoints[destPoint].position;
        destPoint = (destPoint + 1) % waypoints.Length;
    }

    // -----------------------------------------------------------------------------------
    // GotoRandomPoint
    // -----------------------------------------------------------------------------------
    public void GotoRandomPoint()
    {
        Vector2 circle2D = UnityEngine.Random.insideUnitCircle * moveDistance;
        agent.destination = startPosition + new Vector3(circle2D.x, 0, circle2D.y);
    }

    // -----------------------------------------------------------------------------------
}
