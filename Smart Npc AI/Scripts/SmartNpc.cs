// The The Smart Npc class is rather smart.
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NavMeshMovement))]
[RequireComponent(typeof(NetworkNavMeshAgent))]
public class SmartNpc : Npc
{
    [Header("Components")]
    public SmartNpcInventory inventory;

    [Header("Experience Reward")]
    public long rewardExperience = 10;
    public long rewardSkillExperience = 2;

    [Header("Respawn")]
    public float deathTime = 30f; // enough for animation & looting
    [HideInInspector] public double deathTimeEnd; // double for long term precision
    public bool respawn = true;
    public float respawnTime = 10f;
    [HideInInspector] public double respawnTimeEnd; // double for long term precision

    // save the start position for random movement distance and respawning
    [HideInInspector] public Vector3 startPosition;

    // networkbehaviour ////////////////////////////////////////////////////////
    protected override void Start()
    {
        base.Start();

        // remember start position in case we need to respawn later
        startPosition = transform.position;
    }

    void LateUpdate()
    {
        // only if worth updating right now (e.g. a player is around)
        if (!IsWorthUpdating()) return;

        // pass parameters to animation state machine
        // => passing the states directly is the most reliable way to avoid all
        //    kinds of glitches like movement sliding, attack twitching, etc.
        // => make sure to import all looping animations like idle/run/attack
        //    with 'loop time' enabled, otherwise the client might only play it
        //    once
        // => only play moving animation while the actually moving (velocity).
        //    the MOVING state might be delayed to due latency or we might be in
        //    MOVING while a path is still pending, etc.
        // => skill names are assumed to be boolean parameters in animator
        //    so we don't need to worry about an animation number etc.
        if (isClient) // no need for animations on the server
        {
            animator.SetBool("MOVING", state == "MOVING" && movement.GetVelocity() != Vector3.zero);
            animator.SetBool("CASTING", state == "CASTING");
            animator.SetBool("STUNNED", state == "STUNNED");
            animator.SetBool("DEAD", state == "DEAD");
            foreach (Skill skill in skills.skills)
                animator.SetBool(skill.name, skill.CastTimeRemaining() > 0);
        }
    }

    // aggro ///////////////////////////////////////////////////////////////////
    // this function is called by entities that attack us and by AggroArea
    [ServerCallback]
    public override void OnAggro(Entity entity)
    {
        // call base function
        base.OnAggro(entity);

        // are we alive, and is the entity alive and of correct type?
        if (CanAttack(entity))
        {
            // no target yet(==self), or closer than current target?
            // => has to be at least 20% closer to be worth it, otherwise we
            //    may end up nervously switching between two targets
            // => we do NOT use Utils.ClosestDistance, because then we often
            //    also end up nervously switching between two animated targets,
            //    since their collides moves with the animation.
            //    => we don't even need closestdistance here because they are in
            //       the aggro area anyway. transform.position is perfectly fine
            if (target == null)
            {
                target = entity;
            }
            else if (entity != target) // no need to check dist for same target
            {
                float oldDistance = Vector3.Distance(transform.position, target.transform.position);
                float newDistance = Vector3.Distance(transform.position, entity.transform.position);
                if (newDistance < oldDistance * 0.8) target = entity;
            }
        }
    }

    // death ///////////////////////////////////////////////////////////////////
    [Server]
    public override void OnDeath()
    {
        // take care of entity stuff
        base.OnDeath();

        // set death and respawn end times. we set both of them now to make sure
        // that everything works fine even if a monster isn't updated for a
        // while. so as soon as it's updated again, the death/respawn will
        // happen immediately if current time > end time.
        deathTimeEnd = NetworkTime.time + deathTime;
        respawnTimeEnd = deathTimeEnd + respawnTime; // after death time ended
    }


    // attack //////////////////////////////////////////////////////////////////
    // CanAttack check
    // we use 'is' instead of 'GetType' so that it works for inherited types too
    public override bool CanAttack(Entity entity)
    {
        bool baseCanAttack = health.current > 0 &&
                             entity.health.current > 0 &&
                             entity != this &&
                             !inSafeZone && !entity.inSafeZone;
        return baseCanAttack && entity is Monster;
        // return base.CanAttack(entity) &&
        //        (entity is Player ||
        //         entity is Pet ||
        //         entity is Mount);
    }

    // interaction /////////////////////////////////////////////////////////////
    protected override void OnInteract()
    {
        Player player = Player.localPlayer;

        // attackable and has skills? => attack
        if (player.CanAttack(this) && player.skills.skills.Count > 0)
        {
            // then try to use that one
            ((PlayerSkills)player.skills).TryUse(0);
        }
        else if (health.current > 0 &&
        Utils.ClosestDistance(player, this) <= player.interactionRange)
        {
            UINpcDialogue.singleton.Show();
        }
        // dead, has loot, close enough?
        // use collider point(s) to also work with big entities
        else if (health.current == 0 &&
                 Utils.ClosestDistance(player, this) <= player.interactionRange &&
                 inventory.HasLoot())
        {
            UILoot.singleton.Show();
        }
        // otherwise just walk there
        // (e.g. if clicking on it in a safe zone where we can't attack)
        else
        {
            // use collider point(s) to also work with big entities
            Vector3 destination = Utils.ClosestPoint(this, player.transform.position);
            player.movement.Navigate(destination, player.interactionRange);
        }
    }
}
