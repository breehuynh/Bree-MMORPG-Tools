using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "uMMORPG Brain/Brains/Smart Monster", order = 999)]
public class SmartMonsterBrain : CommonBrain
{
    [Header("Movement")]
    [Range(0, 1)] public float moveProbability = 0.1f; // chance per second
    public float moveDistance = 10;
    // smartMonsters should follow their targets even if they run out of the movement
    // radius. the follow dist should always be bigger than the biggest archer's
    // attack range, so that archers will always pull aggro, even when attacking
    // from far away.
    public float followDistance = 20;
    [Range(0.1f, 1)] public float attackToMoveRangeRatio = 0.8f; // move as close as 0.8 * attackRange to a target

    [Header("Smart AI")]
    public bool suspicion = true;
    public float suspicionTime = 3f;
    public bool patrolPath = true;
    [Range(0, 1)] public float patrolSpeedFraction = 0.2f;
    public float waypointTolerance = 1f;
    public float waypointDwellTime = 3f;

    // events //////////////////////////////////////////////////////////////////
    public bool EventDeathTimeElapsed(SmartMonster smartMonster) =>
        smartMonster.state == "DEAD" && NetworkTime.time >= smartMonster.deathTimeEnd;

    public bool EventMoveRandomly(SmartMonster smartMonster) =>
        Random.value <= moveProbability * Time.deltaTime;

    public bool EventRespawnTimeElapsed(SmartMonster smartMonster) =>
        smartMonster.state == "DEAD" && smartMonster.respawn && NetworkTime.time >= smartMonster.respawnTimeEnd;

    public bool EventTargetTooFarToFollow(SmartMonster smartMonster) =>
        smartMonster.target != null &&
        Vector3.Distance(smartMonster.startPosition, Utils.ClosestPoint(smartMonster.target, smartMonster.transform.position)) > followDistance;

    public bool EventPatrolPath(SmartMonster smartMonster) =>
        patrolPath; //&& smartMonster.timeSinceArrivedAtWaypoint > waypointDwellTime;

    // states //////////////////////////////////////////////////////////////////
    string UpdateServer_IDLE(SmartMonster smartMonster)
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventDied(smartMonster))
        {
            // we died.
            return "DEAD";
        }
        if (EventStunned(smartMonster))
        {
            smartMonster.movement.Reset();
            return "STUNNED";
        }
        if (EventTargetDied(smartMonster))
        {
            // we had a target before, but it died now. clear it.
            smartMonster.target = null;
            smartMonster.skills.CancelCast();
            return "IDLE";
        }
        if (EventTargetTooFarToFollow(smartMonster))
        {
            // we had a target before, but it's out of follow range now.
            // clear it and go back to start. don't stay here.
            smartMonster.target = null;
            smartMonster.skills.CancelCast();
            smartMonster.movement.Navigate(smartMonster.startPosition, 0);
            return "MOVING";
        }
        if (EventTargetTooFarToAttack(smartMonster))
        {
            // we had a target before, but it's out of attack range now.
            // follow it. (use collider point(s) to also work with big entities)
            float stoppingDistance = ((MonsterSkills)smartMonster.skills).CurrentCastRange() * attackToMoveRangeRatio;
            Vector3 destination = Utils.ClosestPoint(smartMonster.target, smartMonster.transform.position);
            smartMonster.movement.Navigate(destination, stoppingDistance);
            return "MOVING";
        }
        if (EventTargetEnteredSafeZone(smartMonster))
        {
            // if our target entered the safe zone, we need to be really careful
            // to avoid kiting.
            // -> players could pull a smartMonster near a safe zone and then step in
            //    and out of it before/after attacks without ever getting hit by
            //    the smartMonster
            // -> running back to start won't help, can still kit while running
            // -> warping back to start won't help, we might accidentally placed
            //    a smartMonster in attack range of a safe zone
            // -> the 100% secure way is to die and hide it immediately. many
            //    popular MMOs do it the same way to avoid exploits.
            // => call Entity.OnDeath without rewards etc. and hide immediately
            smartMonster.OnDeath(); // no looting
            smartMonster.respawnTimeEnd = NetworkTime.time + smartMonster.respawnTime; // respawn in a while
            return "DEAD";
        }
        if (EventSkillRequest(smartMonster))
        {
            // we had a target in attack range before and trying to cast a skill
            // on it. check self (alive, mana, weapon etc.) and target
            Skill skill = smartMonster.skills.skills[smartMonster.skills.currentSkill];
            if (smartMonster.skills.CastCheckSelf(skill))
            {
                if (smartMonster.skills.CastCheckTarget(skill))
                {
                    // start casting
                    smartMonster.skills.StartCast(skill);
                    return "CASTING";
                }
                else
                {
                    // invalid target. clear the attempted current skill.
                    smartMonster.target = null;
                    smartMonster.skills.currentSkill = -1;
                    return "IDLE";
                }
            }
            else
            {
                // we can't cast this skill at the moment (cooldown/low mana/...)
                // -> clear the attempted current skill, but keep the target to
                // continue later
                smartMonster.skills.currentSkill = -1;
                return "IDLE";
            }
        }
        if (EventAggro(smartMonster))
        {
            // target in attack range. try to cast a first skill on it
            if (smartMonster.skills.skills.Count > 0) smartMonster.skills.currentSkill = ((MonsterSkills)smartMonster.skills).NextSkill();
            else Debug.LogError(name + " has no skills to attack with.");
            return "IDLE";
        }
        if (EventMoveRandomly(smartMonster))
        {
            // walk to a random position in movement radius (from 'start')
            // note: circle y is 0 because we add it to start.y
            Vector2 circle2D = Random.insideUnitCircle * moveDistance;
            smartMonster.movement.Navigate(smartMonster.startPosition + new Vector3(circle2D.x, 0, circle2D.y), 0);
            return "MOVING";
        }
        if (EventPatrolPath(smartMonster))
        {
            Vector3 currentWaypoint = smartMonster.patrolPath.GetWaypoint(smartMonster.currentWaypointIndex);
            float distanceToWaypoint = Vector3.Distance(smartMonster.transform.position, currentWaypoint);
            bool atWaypoint = distanceToWaypoint < waypointTolerance;
            if (atWaypoint) {
                //smartMonster.SetTimeSinceArrivedAtWaypoint(0);
                smartMonster.CycleWaypoint();
                Vector3 nextPosition = smartMonster.patrolPath.GetWaypoint(smartMonster.currentWaypointIndex);
                smartMonster.movement.Navigate(nextPosition, 0);
                return "MOVING";
            }
            smartMonster.movement.Navigate(currentWaypoint, 0);
            return "MOVING";
        }
        if (EventDeathTimeElapsed(smartMonster)) { } // don't care
        if (EventRespawnTimeElapsed(smartMonster)) { } // don't care
        if (EventMoveEnd(smartMonster)) { } // don't care
        if (EventSkillFinished(smartMonster)) { } // don't care
        if (EventTargetDisappeared(smartMonster)) { } // don't care

        return "IDLE"; // nothing interesting happened
    }

    string UpdateServer_MOVING(SmartMonster smartMonster)
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventDied(smartMonster))
        {
            // we died.
            smartMonster.movement.Reset();
            return "DEAD";
        }
        if (EventStunned(smartMonster))
        {
            smartMonster.movement.Reset();
            return "STUNNED";
        }
        if (EventMoveEnd(smartMonster))
        {
            // we reached our destination.
            return "IDLE";
        }
        if (EventTargetDied(smartMonster))
        {
            // we had a target before, but it died now. clear it.
            smartMonster.target = null;
            smartMonster.skills.CancelCast();
            smartMonster.movement.Reset();
            return "IDLE";
        }
        if (EventTargetTooFarToFollow(smartMonster))
        {
            // we had a target before, but it's out of follow range now.
            // clear it and go back to start. don't stay here.
            smartMonster.target = null;
            smartMonster.skills.CancelCast();
            smartMonster.movement.Navigate(smartMonster.startPosition, 0);
            return "MOVING";
        }
        if (EventTargetTooFarToAttack(smartMonster))
        {
            // we had a target before, but it's out of attack range now.
            // follow it. (use collider point(s) to also work with big entities)
            float stoppingDistance = ((MonsterSkills)smartMonster.skills).CurrentCastRange() * attackToMoveRangeRatio;
            Vector3 destination = Utils.ClosestPoint(smartMonster.target, smartMonster.transform.position);
            smartMonster.movement.Navigate(destination, stoppingDistance);
            return "MOVING";
        }
        if (EventTargetEnteredSafeZone(smartMonster))
        {
            // if our target entered the safe zone, we need to be really careful
            // to avoid kiting.
            // -> players could pull a smartMonster near a safe zone and then step in
            //    and out of it before/after attacks without ever getting hit by
            //    the smartMonster
            // -> running back to start won't help, can still kit while running
            // -> warping back to start won't help, we might accidentally placed
            //    a smartMonster in attack range of a safe zone
            // -> the 100% secure way is to die and hide it immediately. many
            //    popular MMOs do it the same way to avoid exploits.
            // => call Entity.OnDeath without rewards etc. and hide immediately
            smartMonster.OnDeath(); // no looting
            smartMonster.respawnTimeEnd = NetworkTime.time + smartMonster.respawnTime; // respawn in a while
            return "DEAD";
        }
        if (EventAggro(smartMonster))
        {
            // target in attack range. try to cast a first skill on it
            // (we may get a target while randomly wandering around)
            if (smartMonster.skills.skills.Count > 0) smartMonster.skills.currentSkill = ((MonsterSkills)smartMonster.skills).NextSkill();
            else Debug.LogError(name + " has no skills to attack with.");
            smartMonster.movement.Reset();
            return "IDLE";
        }
        if (EventDeathTimeElapsed(smartMonster)) { } // don't care
        if (EventRespawnTimeElapsed(smartMonster)) { } // don't care
        if (EventSkillFinished(smartMonster)) { } // don't care
        if (EventTargetDisappeared(smartMonster)) { } // don't care
        if (EventSkillRequest(smartMonster)) { } // don't care, finish movement first
        if (EventMoveRandomly(smartMonster)) { } // don't care

        return "MOVING"; // nothing interesting happened
    }

    string UpdateServer_CASTING(SmartMonster smartMonster)
    {
        // keep looking at the target for server & clients (only Y rotation)
        if (smartMonster.target)
            smartMonster.movement.LookAtY(smartMonster.target.transform.position);

        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventDied(smartMonster))
        {
            // we died.
            return "DEAD";
        }
        if (EventStunned(smartMonster))
        {
            smartMonster.skills.CancelCast();
            smartMonster.movement.Reset();
            return "STUNNED";
        }
        if (EventTargetDisappeared(smartMonster))
        {
            // cancel if the target matters for this skill
            if (smartMonster.skills.skills[smartMonster.skills.currentSkill].cancelCastIfTargetDied)
            {
                smartMonster.skills.CancelCast();
                smartMonster.target = null;
                return "IDLE";
            }
        }
        if (EventTargetDied(smartMonster))
        {
            // cancel if the target matters for this skill
            if (smartMonster.skills.skills[smartMonster.skills.currentSkill].cancelCastIfTargetDied)
            {
                smartMonster.skills.CancelCast();
                smartMonster.target = null;
                return "IDLE";
            }
        }
        if (EventTargetEnteredSafeZone(smartMonster))
        {
            // cancel if the target matters for this skill
            if (smartMonster.skills.skills[smartMonster.skills.currentSkill].cancelCastIfTargetDied)
            {
                // if our target entered the safe zone, we need to be really careful
                // to avoid kiting.
                // -> players could pull a smartMonster near a safe zone and then step in
                //    and out of it before/after attacks without ever getting hit by
                //    the smartMonster
                // -> running back to start won't help, can still kit while running
                // -> warping back to start won't help, we might accidentally placed
                //    a smartMonster in attack range of a safe zone
                // -> the 100% secure way is to die and hide it immediately. many
                //    popular MMOs do it the same way to avoid exploits.
                // => call Entity.OnDeath without rewards etc. and hide immediately
                smartMonster.OnDeath(); // no looting
                smartMonster.respawnTimeEnd = NetworkTime.time + smartMonster.respawnTime; // respawn in a while
                return "DEAD";
            }
        }
        if (EventSkillFinished(smartMonster))
        {
            // finished casting. apply the skill on the target.
            smartMonster.skills.FinishCast(smartMonster.skills.skills[smartMonster.skills.currentSkill]);

            // did the target die? then clear it so that the smartMonster doesn't
            // run towards it if the target respawned
            // (target might be null if disappeared or targetless skill)
            if (smartMonster.target != null && smartMonster.target.health.current == 0)
                smartMonster.target = null;

            // go back to IDLE, reset current skill
            ((MonsterSkills)smartMonster.skills).lastSkill = smartMonster.skills.currentSkill;
            smartMonster.skills.currentSkill = -1;
            return "IDLE";
        }
        if (EventDeathTimeElapsed(smartMonster)) { } // don't care
        if (EventRespawnTimeElapsed(smartMonster)) { } // don't care
        if (EventMoveEnd(smartMonster)) { } // don't care
        if (EventTargetTooFarToAttack(smartMonster)) { } // don't care, we were close enough when starting to cast
        if (EventTargetTooFarToFollow(smartMonster)) { } // don't care, we were close enough when starting to cast
        if (EventAggro(smartMonster)) { } // don't care, always have aggro while casting
        if (EventSkillRequest(smartMonster)) { } // don't care, that's why we are here
        if (EventMoveRandomly(smartMonster)) { } // don't care

        return "CASTING"; // nothing interesting happened
    }

    string UpdateServer_STUNNED(SmartMonster smartMonster)
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventDied(smartMonster))
        {
            // we died.
            return "DEAD";
        }
        if (EventStunned(smartMonster))
        {
            return "STUNNED";
        }

        // go back to idle if we aren't stunned anymore and process all new
        // events there too
        return "IDLE";
    }

    string UpdateServer_DEAD(SmartMonster smartMonster)
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventRespawnTimeElapsed(smartMonster))
        {
            // respawn at the start position with full health, visibility, no loot
            smartMonster.gold = 0;
            smartMonster.inventory.slots.Clear();
            smartMonster.Show();
            smartMonster.movement.Warp(smartMonster.startPosition); // recommended over transform.position
            smartMonster.Revive();
            return "IDLE";
        }
        if (EventDeathTimeElapsed(smartMonster))
        {
            // we were lying around dead for long enough now.
            // hide while respawning, or disappear forever
            if (smartMonster.respawn) smartMonster.Hide();
            else NetworkServer.Destroy(smartMonster.gameObject);
            return "DEAD";
        }
        if (EventSkillRequest(smartMonster)) { } // don't care
        if (EventSkillFinished(smartMonster)) { } // don't care
        if (EventMoveEnd(smartMonster)) { } // don't care
        if (EventTargetDisappeared(smartMonster)) { } // don't care
        if (EventTargetDied(smartMonster)) { } // don't care
        if (EventTargetTooFarToFollow(smartMonster)) { } // don't care
        if (EventTargetTooFarToAttack(smartMonster)) { } // don't care
        if (EventTargetEnteredSafeZone(smartMonster)) { } // don't care
        if (EventAggro(smartMonster)) { } // don't care
        if (EventMoveRandomly(smartMonster)) { } // don't care
        if (EventStunned(smartMonster)) { } // don't care
        if (EventDied(smartMonster)) { } // don't care, of course we are dead

        return "DEAD"; // nothing interesting happened
    }

    public override string UpdateServer(Entity entity)
    {
        SmartMonster smartMonster = (SmartMonster)entity;

        if (smartMonster.state == "IDLE") return UpdateServer_IDLE(smartMonster);
        if (smartMonster.state == "MOVING") return UpdateServer_MOVING(smartMonster);
        if (smartMonster.state == "CASTING") return UpdateServer_CASTING(smartMonster);
        if (smartMonster.state == "STUNNED") return UpdateServer_STUNNED(smartMonster);
        if (smartMonster.state == "DEAD") return UpdateServer_DEAD(smartMonster);

        Debug.LogError("invalid state:" + smartMonster.state);
        return "IDLE";
    }

    public override void UpdateClient(Entity entity)
    {
        SmartMonster smartMonster = (SmartMonster)entity;
        if (smartMonster.state == "CASTING")
        {
            // keep looking at the target for server & clients (only Y rotation)
            if (smartMonster.target)
                smartMonster.movement.LookAtY(smartMonster.target.transform.position);
        }
    }

    // DrawGizmos can be used for debug info
    public override void DrawGizmos(Entity entity)
    {
        Monster smartMonster = (Monster)entity;

        // draw the movement area (around 'start' if game running,
        // or around current position if still editing)
        Vector3 startHelp = Application.isPlaying ? smartMonster.startPosition : smartMonster.transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startHelp, moveDistance);

        // draw the follow dist
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(startHelp, followDistance);
    }
}
