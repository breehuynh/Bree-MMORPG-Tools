// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// MONSTER

public partial class Monster
{
    [Header("[UCE MONSTER WAYPOINTS]")]
    [Tooltip("[Optional] Waypoints to walk to from top (first) to bottom (last).")]
    public Transform[] waypoints;

    [Tooltip("[Optional] true = keep patrolling between waypoints, false = walk to waypoints just once, then randomly")]
    public bool patrol = true;

    [Tooltip("[Optional] true = turns final waypoint into start position, false = returns to start position after final waypoint reached")]
    public bool stickToLastWaypoint = true;

    [Tooltip("[Optional] 0 = Waypoints only, 1 = Random only, or any in-between.")]
    [Range(0, 1)] public float WaypointToRandomRatio;

    protected int destPoint = 0;
    protected bool destReached = false;

    // -----------------------------------------------------------------------------------
    // OnEventMoveRandomly_
    // -----------------------------------------------------------------------------------
    [Server]
    [DevExtMethods("OnEventMoveRandomly")]
    private void OnEventMoveRandomly_UCE_MonsterWayPoints()
    {
        if (UnityEngine.Random.value <= WaypointToRandomRatio || !DoPatrol())
        {
            GotoRandomPoint();
        }
        else
        {
            GotoNextWayPoint();
        }
    }

    // -----------------------------------------------------------------------------------
    // GotoNextWayPoint
    // -----------------------------------------------------------------------------------
    public void GotoNextWayPoint()
    {
        if (!DoPatrol())
        {
            GotoRandomPoint();
            return;
        }

        agent.destination = waypoints[destPoint].position;

        if (destPoint + 1 > waypoints.Length - 1)
        {
            destReached = true;
            startPosition = waypoints[waypoints.Length - 1].position;
        }

        destPoint = (destPoint + 1) % waypoints.Length;
    }

    // -----------------------------------------------------------------------------------
    // GotoRandomPoint
    // -----------------------------------------------------------------------------------
    public void GotoRandomPoint()
    {
        Vector2 circle2D = Random.insideUnitCircle * moveDistance;
        agent.stoppingDistance = 0;
        agent.destination = startPosition + new Vector3(circle2D.x, 0, circle2D.y);
    }

    // -----------------------------------------------------------------------------------
    // DoPatrol
    // -----------------------------------------------------------------------------------
    protected bool DoPatrol()
    {
        return (
                waypoints.Length > 0 &&
                ((!patrol && !destReached) || patrol)
                );
    }

    // -----------------------------------------------------------------------------------
}
