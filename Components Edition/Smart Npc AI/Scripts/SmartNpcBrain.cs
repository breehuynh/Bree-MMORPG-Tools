using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "uMMORPG Brain/Brains/Smart Npc", order = 999)]
public class SmartNpcBrain : CommonBrain
{
    [Header("Movement")]
    [Range(0, 1)] public float moveProbability = 0.1f; // chance per second
    public float moveDistance = 10;
    // smartNpcs should follow their targets even if they run out of the movement
    // radius. the follow dist should always be bigger than the biggest archer's
    // attack range, so that archers will always pull aggro, even when attacking
    // from far away.
    public float followDistance = 20;
    [Range(0.1f, 1)] public float attackToMoveRangeRatio = 0.8f; // move as close as 0.8 * attackRange to a target

    [Header("Smart AI")]
    public bool suspicion = true;
    public float suspicionTime = 3f;
    public bool patrolPath = true;
    public float waypointTolerance = 1f;
    public float waypointDwellTime = 3f;

    // events //////////////////////////////////////////////////////////////////
    public bool EventDeathTimeElapsed(SmartNpc smartNpc) =>
        smartNpc.state == "DEAD" && NetworkTime.time >= smartNpc.deathTimeEnd;

    public bool EventMoveRandomly(SmartNpc smartNpc) =>
        Random.value <= moveProbability * Time.deltaTime;

    public bool EventRespawnTimeElapsed(SmartNpc smartNpc) =>
        smartNpc.state == "DEAD" && smartNpc.respawn && NetworkTime.time >= smartNpc.respawnTimeEnd;

    public bool EventTargetTooFarToFollow(SmartNpc smartNpc) =>
        smartNpc.target != null &&
        Vector3.Distance(smartNpc.startPosition, Utils.ClosestPoint(smartNpc.target, smartNpc.transform.position)) > followDistance;

    public bool EventPatrolPath(SmartNpc smartNpc) =>
        patrolPath; //&& smartNpc.timeSinceArrivedAtWaypoint > waypointDwellTime;

    // states //////////////////////////////////////////////////////////////////
    string UpdateServer_IDLE(SmartNpc smartNpc)
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventDied(smartNpc))
        {
            // we died.
            return "DEAD";
        }
        if (EventStunned(smartNpc))
        {
            smartNpc.movement.Reset();
            return "STUNNED";
        }
        if (EventTargetDied(smartNpc))
        {
            // we had a target before, but it died now. clear it.
            smartNpc.target = null;
            smartNpc.skills.CancelCast();
            return "IDLE";
        }
        if (EventTargetTooFarToFollow(smartNpc))
        {
            // we had a target before, but it's out of follow range now.
            // clear it and go back to start. don't stay here.
            smartNpc.target = null;
            smartNpc.skills.CancelCast();
            smartNpc.movement.Navigate(smartNpc.startPosition, 0);
            return "MOVING";
        }
        if (EventTargetTooFarToAttack(smartNpc))
        {
            // we had a target before, but it's out of attack range now.
            // follow it. (use collider point(s) to also work with big entities)
            float stoppingDistance = ((SmartNpcSkills)smartNpc.skills).CurrentCastRange() * attackToMoveRangeRatio;
            Vector3 destination = Utils.ClosestPoint(smartNpc.target, smartNpc.transform.position);
            smartNpc.movement.Navigate(destination, stoppingDistance);
            return "MOVING";
        }
        if (EventTargetEnteredSafeZone(smartNpc))
        {
            // if our target entered the safe zone, we need to be really careful
            // to avoid kiting.
            // -> players could pull a smartNpc near a safe zone and then step in
            //    and out of it before/after attacks without ever getting hit by
            //    the smartNpc
            // -> running back to start won't help, can still kit while running
            // -> warping back to start won't help, we might accidentally placed
            //    a smartNpc in attack range of a safe zone
            // -> the 100% secure way is to die and hide it immediately. many
            //    popular MMOs do it the same way to avoid exploits.
            // => call Entity.OnDeath without rewards etc. and hide immediately
            smartNpc.OnDeath(); // no looting
            smartNpc.respawnTimeEnd = NetworkTime.time + smartNpc.respawnTime; // respawn in a while
            return "DEAD";
        }
        if (EventSkillRequest(smartNpc))
        {
            // we had a target in attack range before and trying to cast a skill
            // on it. check self (alive, mana, weapon etc.) and target
            Skill skill = smartNpc.skills.skills[smartNpc.skills.currentSkill];
            if (smartNpc.skills.CastCheckSelf(skill))
            {
                if (smartNpc.skills.CastCheckTarget(skill))
                {
                    // start casting
                    smartNpc.skills.StartCast(skill);
                    return "CASTING";
                }
                else
                {
                    // invalid target. clear the attempted current skill.
                    smartNpc.target = null;
                    smartNpc.skills.currentSkill = -1;
                    return "IDLE";
                }
            }
            else
            {
                // we can't cast this skill at the moment (cooldown/low mana/...)
                // -> clear the attempted current skill, but keep the target to
                // continue later
                smartNpc.skills.currentSkill = -1;
                return "IDLE";
            }
        }
        if (EventAggro(smartNpc))
        {
            // target in attack range. try to cast a first skill on it
            if (smartNpc.skills.skills.Count > 0) smartNpc.skills.currentSkill = ((SmartNpcSkills)smartNpc.skills).NextSkill();
            else Debug.LogError(name + " has no skills to attack with.");
            return "IDLE";
        }
        if (EventMoveRandomly(smartNpc))
        {
            // walk to a random position in movement radius (from 'start')
            // note: circle y is 0 because we add it to start.y
            Vector2 circle2D = Random.insideUnitCircle * moveDistance;
            smartNpc.movement.Navigate(smartNpc.startPosition + new Vector3(circle2D.x, 0, circle2D.y), 0);
            return "MOVING";
        }
        if (EventPatrolPath(smartNpc))
        {
            Vector3 currentWaypoint = smartNpc.patrolPath.GetWaypoint(smartNpc.currentWaypointIndex);
            float distanceToWaypoint = Vector3.Distance(smartNpc.transform.position, currentWaypoint);
            bool atWaypoint = distanceToWaypoint < waypointTolerance;
            if (atWaypoint)
            {
                //smartNpc.SetTimeSinceArrivedAtWaypoint(0);
                smartNpc.CycleWaypoint();
                Vector3 nextPosition = smartNpc.patrolPath.GetWaypoint(smartNpc.currentWaypointIndex);
                smartNpc.movement.Navigate(nextPosition, 0);
                return "MOVING";
            }
            smartNpc.movement.Navigate(currentWaypoint, 0);
            return "MOVING";
        }
        if (EventDeathTimeElapsed(smartNpc)) { } // don't care
        if (EventRespawnTimeElapsed(smartNpc)) { } // don't care
        if (EventMoveEnd(smartNpc)) { } // don't care
        if (EventSkillFinished(smartNpc)) { } // don't care
        if (EventTargetDisappeared(smartNpc)) { } // don't care

        return "IDLE"; // nothing interesting happened
    }

    string UpdateServer_MOVING(SmartNpc smartNpc)
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventDied(smartNpc))
        {
            // we died.
            smartNpc.movement.Reset();
            return "DEAD";
        }
        if (EventStunned(smartNpc))
        {
            smartNpc.movement.Reset();
            return "STUNNED";
        }
        if (EventMoveEnd(smartNpc))
        {
            // we reached our destination.
            return "IDLE";
        }
        if (EventTargetDied(smartNpc))
        {
            // we had a target before, but it died now. clear it.
            smartNpc.target = null;
            smartNpc.skills.CancelCast();
            smartNpc.movement.Reset();
            return "IDLE";
        }
        if (EventTargetTooFarToFollow(smartNpc))
        {
            // we had a target before, but it's out of follow range now.
            // clear it and go back to start. don't stay here.
            smartNpc.target = null;
            smartNpc.skills.CancelCast();
            smartNpc.movement.Navigate(smartNpc.startPosition, 0);
            return "MOVING";
        }
        if (EventTargetTooFarToAttack(smartNpc))
        {
            // we had a target before, but it's out of attack range now.
            // follow it. (use collider point(s) to also work with big entities)
            float stoppingDistance = ((SmartNpcSkills)smartNpc.skills).CurrentCastRange() * attackToMoveRangeRatio;
            Vector3 destination = Utils.ClosestPoint(smartNpc.target, smartNpc.transform.position);
            smartNpc.movement.Navigate(destination, stoppingDistance);
            return "MOVING";
        }
        if (EventTargetEnteredSafeZone(smartNpc))
        {
            // if our target entered the safe zone, we need to be really careful
            // to avoid kiting.
            // -> players could pull a smartNpc near a safe zone and then step in
            //    and out of it before/after attacks without ever getting hit by
            //    the smartNpc
            // -> running back to start won't help, can still kit while running
            // -> warping back to start won't help, we might accidentally placed
            //    a smartNpc in attack range of a safe zone
            // -> the 100% secure way is to die and hide it immediately. many
            //    popular MMOs do it the same way to avoid exploits.
            // => call Entity.OnDeath without rewards etc. and hide immediately
            smartNpc.OnDeath(); // no looting
            smartNpc.respawnTimeEnd = NetworkTime.time + smartNpc.respawnTime; // respawn in a while
            return "DEAD";
        }
        if (EventAggro(smartNpc))
        {
            // target in attack range. try to cast a first skill on it
            // (we may get a target while randomly wandering around)
            if (smartNpc.skills.skills.Count > 0) smartNpc.skills.currentSkill = ((SmartNpcSkills)smartNpc.skills).NextSkill();
            else Debug.LogError(name + " has no skills to attack with.");
            smartNpc.movement.Reset();
            return "IDLE";
        }
        if (EventDeathTimeElapsed(smartNpc)) { } // don't care
        if (EventRespawnTimeElapsed(smartNpc)) { } // don't care
        if (EventSkillFinished(smartNpc)) { } // don't care
        if (EventTargetDisappeared(smartNpc)) { } // don't care
        if (EventSkillRequest(smartNpc)) { } // don't care, finish movement first
        if (EventMoveRandomly(smartNpc)) { } // don't care

        return "MOVING"; // nothing interesting happened
    }

    string UpdateServer_CASTING(SmartNpc smartNpc)
    {
        // keep looking at the target for server & clients (only Y rotation)
        if (smartNpc.target)
            smartNpc.movement.LookAtY(smartNpc.target.transform.position);

        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventDied(smartNpc))
        {
            // we died.
            return "DEAD";
        }
        if (EventStunned(smartNpc))
        {
            smartNpc.skills.CancelCast();
            smartNpc.movement.Reset();
            return "STUNNED";
        }
        if (EventTargetDisappeared(smartNpc))
        {
            // cancel if the target matters for this skill
            if (smartNpc.skills.skills[smartNpc.skills.currentSkill].cancelCastIfTargetDied)
            {
                smartNpc.skills.CancelCast();
                smartNpc.target = null;
                return "IDLE";
            }
        }
        if (EventTargetDied(smartNpc))
        {
            // cancel if the target matters for this skill
            if (smartNpc.skills.skills[smartNpc.skills.currentSkill].cancelCastIfTargetDied)
            {
                smartNpc.skills.CancelCast();
                smartNpc.target = null;
                return "IDLE";
            }
        }
        if (EventTargetEnteredSafeZone(smartNpc))
        {
            // cancel if the target matters for this skill
            if (smartNpc.skills.skills[smartNpc.skills.currentSkill].cancelCastIfTargetDied)
            {
                // if our target entered the safe zone, we need to be really careful
                // to avoid kiting.
                // -> players could pull a smartNpc near a safe zone and then step in
                //    and out of it before/after attacks without ever getting hit by
                //    the smartNpc
                // -> running back to start won't help, can still kit while running
                // -> warping back to start won't help, we might accidentally placed
                //    a smartNpc in attack range of a safe zone
                // -> the 100% secure way is to die and hide it immediately. many
                //    popular MMOs do it the same way to avoid exploits.
                // => call Entity.OnDeath without rewards etc. and hide immediately
                smartNpc.OnDeath(); // no looting
                smartNpc.respawnTimeEnd = NetworkTime.time + smartNpc.respawnTime; // respawn in a while
                return "DEAD";
            }
        }
        if (EventSkillFinished(smartNpc))
        {
            // finished casting. apply the skill on the target.
            smartNpc.skills.FinishCast(smartNpc.skills.skills[smartNpc.skills.currentSkill]);

            // did the target die? then clear it so that the smartNpc doesn't
            // run towards it if the target respawned
            // (target might be null if disappeared or targetless skill)
            if (smartNpc.target != null && smartNpc.target.health.current == 0)
                smartNpc.target = null;

            // go back to IDLE, reset current skill
            ((SmartNpcSkills)smartNpc.skills).lastSkill = smartNpc.skills.currentSkill;
            smartNpc.skills.currentSkill = -1;
            return "IDLE";
        }
        if (EventDeathTimeElapsed(smartNpc)) { } // don't care
        if (EventRespawnTimeElapsed(smartNpc)) { } // don't care
        if (EventMoveEnd(smartNpc)) { } // don't care
        if (EventTargetTooFarToAttack(smartNpc)) { } // don't care, we were close enough when starting to cast
        if (EventTargetTooFarToFollow(smartNpc)) { } // don't care, we were close enough when starting to cast
        if (EventAggro(smartNpc)) { } // don't care, always have aggro while casting
        if (EventSkillRequest(smartNpc)) { } // don't care, that's why we are here
        if (EventMoveRandomly(smartNpc)) { } // don't care

        return "CASTING"; // nothing interesting happened
    }

    string UpdateServer_STUNNED(SmartNpc smartNpc)
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventDied(smartNpc))
        {
            // we died.
            return "DEAD";
        }
        if (EventStunned(smartNpc))
        {
            return "STUNNED";
        }

        // go back to idle if we aren't stunned anymore and process all new
        // events there too
        return "IDLE";
    }

    string UpdateServer_DEAD(SmartNpc smartNpc)
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventRespawnTimeElapsed(smartNpc))
        {
            // respawn at the start position with full health, visibility, no loot
            smartNpc.gold = 0;
            smartNpc.inventory.slots.Clear();
            smartNpc.Show();
            smartNpc.movement.Warp(smartNpc.startPosition); // recommended over transform.position
            smartNpc.Revive();
            return "IDLE";
        }
        if (EventDeathTimeElapsed(smartNpc))
        {
            // we were lying around dead for long enough now.
            // hide while respawning, or disappear forever
            if (smartNpc.respawn) smartNpc.Hide();
            else NetworkServer.Destroy(smartNpc.gameObject);
            return "DEAD";
        }
        if (EventSkillRequest(smartNpc)) { } // don't care
        if (EventSkillFinished(smartNpc)) { } // don't care
        if (EventMoveEnd(smartNpc)) { } // don't care
        if (EventTargetDisappeared(smartNpc)) { } // don't care
        if (EventTargetDied(smartNpc)) { } // don't care
        if (EventTargetTooFarToFollow(smartNpc)) { } // don't care
        if (EventTargetTooFarToAttack(smartNpc)) { } // don't care
        if (EventTargetEnteredSafeZone(smartNpc)) { } // don't care
        if (EventAggro(smartNpc)) { } // don't care
        if (EventMoveRandomly(smartNpc)) { } // don't care
        if (EventStunned(smartNpc)) { } // don't care
        if (EventDied(smartNpc)) { } // don't care, of course we are dead

        return "DEAD"; // nothing interesting happened
    }

    public override string UpdateServer(Entity entity)
    {
        SmartNpc smartNpc = (SmartNpc)entity;

        if (smartNpc.state == "IDLE") return UpdateServer_IDLE(smartNpc);
        if (smartNpc.state == "MOVING") return UpdateServer_MOVING(smartNpc);
        if (smartNpc.state == "CASTING") return UpdateServer_CASTING(smartNpc);
        if (smartNpc.state == "STUNNED") return UpdateServer_STUNNED(smartNpc);
        if (smartNpc.state == "DEAD") return UpdateServer_DEAD(smartNpc);

        Debug.LogError("invalid state:" + smartNpc.state);
        return "IDLE";
    }

    public override void UpdateClient(Entity entity)
    {
        SmartNpc smartNpc = (SmartNpc)entity;
        if (smartNpc.state == "CASTING")
        {
            // keep looking at the target for server & clients (only Y rotation)
            if (smartNpc.target)
                smartNpc.movement.LookAtY(smartNpc.target.transform.position);
        }
    }

    // DrawGizmos can be used for debug info
    public override void DrawGizmos(Entity entity)
    {
        SmartNpc smartNpc = (SmartNpc)entity;

        // draw the movement area (around 'start' if game running,
        // or around current position if still editing)
        Vector3 startHelp = Application.isPlaying ? smartNpc.startPosition : smartNpc.transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startHelp, moveDistance);

        // draw the follow dist
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(startHelp, followDistance);
    }
}
