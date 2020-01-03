// The Monster class has a few different features that all aim to make monsters
// behave as realistically as possible.
//
// - **States:** first of all, the monster has several different states like
// IDLE, ATTACKING, MOVING and DEATH. The monster will randomly move around in
// a certain movement radius and try to attack any players in its aggro range.
//
// - **Aggro:** To save computations, we let Unity take care of finding players
// in the aggro range by simply adding a AggroArea _(see AggroArea.cs)_ sphere
// to the monster's children in the Hierarchy. We then use the OnTrigger
// functions to find players that are in the aggro area. The monster will always
// move to the nearest aggro player and then attack it as long as the player is
// in the follow radius. If the player happens to walk out of the follow
// radius then the monster will walk back to the start position quickly.
//
// - **Respawning:** The monsters have a _respawn_ property that can be set to
// true in order to make the monster respawn after it died. We developed the
// respawn system with simplicity in mind, there are no extra spawner objects
// needed. As soon as a monster dies, it will make itself invisible for a while
// and then go back to the starting position to respawn. This feature allows the
// developer to quickly drag monster Prefabs into the scene and place them
// anywhere, without worrying about spawners and spawn areas.
//
// - **Loot:** Dead monsters can also generate loot, based on the _lootItems_
// list. Each monster has a list of items with their dropchance, so that loot
// will always be generated randomly. Monsters can also randomly generate loot
// gold between a minimum and a maximum amount.
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(MonsterSkills))]
[RequireComponent(typeof(NavMeshMovement))]
[RequireComponent(typeof(NetworkNavMeshAgent))]
public partial class SmartMonster : Monster
{
    [Header("Patrol Path")]
    public PatrolPath patrolPath;

    [HideInInspector] public float timeSinceLastSawPlayer = Mathf.Infinity;
    [HideInInspector] public float timeSinceArrivedAtWaypoint = Mathf.Infinity;
    [HideInInspector] public int currentWaypointIndex = 0;
    // attack //////////////////////////////////////////////////////////////////
    // CanAttack check
    // we use 'is' instead of 'GetType' so that it works for inherited types too
    public override bool CanAttack(Entity entity)
    {
        bool baseCanAttack = health.current > 0 &&
                             entity.health.current > 0 &&
                             entity != this &&
                             !inSafeZone && !entity.inSafeZone;
        return baseCanAttack &&
               (entity is Player ||
                entity is Pet ||
                entity is Mount ||
                entity is SmartNpc);
    }

    public void SetTimeSinceArrivedAtWaypoint(float time)
    {
        timeSinceArrivedAtWaypoint = time;
    }

    public void CycleWaypoint()
    {
        currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
    }
}
