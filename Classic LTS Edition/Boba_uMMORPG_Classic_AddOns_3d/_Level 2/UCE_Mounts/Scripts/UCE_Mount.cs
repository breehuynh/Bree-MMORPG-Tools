// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Linq;
using TMPro;
using UnityEngine;

// UCE MOUNT (UNMOUNTED)

[RequireComponent(typeof(Animator))]
public partial class UCE_Mount : Summonable
{
    [Header("-=-=-=- UCE MOUNT -=-=-=-")]
    public GameObject mountedMount;

    public TextMeshPro nameOverlay;

    [Header("Movement")]
    public float returnDistance = 25; 			// return to player if dist > ...

    public float followDistance = 20;
    public float teleportDistance = 30;

    [Range(0.1f, 1)]
    public float attackToMoveRangeRatio = 0.8f; 		// move as close as 0.8 * attackRange to a target

    [Header("Death")]
    public float deathTime = 2; 			// enough for animation

    private float deathTimeEnd;

    [Header("Behaviour")]
    [SyncVar] public bool defendOwner = true; 		// attack what attacks the owner

    [SyncVar] public bool autoAttack = true;        // attack what the owner attacks

#if !_iMMOCONDITIONALSKILLS
    private int lastSkill = -1;
#endif

    [HideInInspector] public long experience;
    [HideInInspector] public bool autoRide;

    // ================================ NETWORK BEHAVIOUR ================================

    protected override void Awake()
    {
        base.Awake();

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("Awake");
        Utils.InvokeMany(typeof(UCE_Mount), this, "Awake_");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        foreach (var template in skillTemplates)
            skills.Add(new Skill(template));

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("OnStartServer");
        Utils.InvokeMany(typeof(UCE_Mount), this, "OnStartServer_");
    }

    protected override void Start()
    {
        base.Start();

        if (nameOverlay)
            nameOverlay.text = owner.name + "'s " + this.name;

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("Start");
        Utils.InvokeMany(typeof(UCE_Mount), this, "Start_");
    }

    // warp
    public override void Warp(Vector3 destination)
    {
        // just warp. no need for any Rpc because both client and server always
        // follow the owner anyway.
        agent.Warp(destination);
    }

    // reset movement
    public override void ResetMovement()
    {
        agent.ResetMovement();
    }

    // -----------------------------------------------------------------------------------
    // LateUpdate
    // -----------------------------------------------------------------------------------
    private void LateUpdate()
    {
        // only if worth updating right now (e.g. a player is around)
        if (!IsWorthUpdating()) return;

        if (isClient)
        {
            if (state != "CASTING")
                animator.speed = 1;

            if (animator.parameters.Any(x => x.name == "MOVING"))
                animator.SetBool("MOVING", state == "MOVING" && agent.velocity != Vector3.zero);

            if (animator.parameters.Any(x => x.name == "CASTING"))
                animator.SetBool("CASTING", state == "CASTING");

            if (animator.parameters.Any(x => x.name == "STUNNED"))
                animator.SetBool("STUNNED", state == "STUNNED");

            if (animator.parameters.Any(x => x.name == "DEAD"))
                animator.SetBool("DEAD", state == "DEAD");

            /*foreach (Skill skill in skills) {
            	if (skill.level > 0 && !(skill.data is PassiveSkill) && animator.parameters.Any(x => x.name == skill.name))
            	animator.SetBool(skill.name, skill.CastTimeRemaining() > 0);
        	}*/

            foreach (Skill skill in skills)
            {
                if (skill.level > 0 && !(skill.data is PassiveSkill) && animator.parameters.Any(x => x.name == skill.name))
                {
                    if (skill.CastTimeRemaining() > 0)
                        animator.speed = animator.GetCurrentAnimatorStateInfo(0).length / skill.castTime;
                    if (animator.parameters.Any(x => x.name == skill.name))
                        animator.SetBool(skill.name, skill.CastTimeRemaining() > 0);
                }
            }
        }

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("LateUpdate");
        Utils.InvokeMany(typeof(UCE_Mount), this, "LateUpdate_");
    }

    private void OnDestroy()
    {
        // Unity bug: isServer is false when called in host mode. only true when
        // called in dedicated mode. so we need a workaround:
        if (NetworkServer.active) // isServer
        {
            // keep player's pet item up to date
            SyncToOwnerItem();
        }

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("OnDestroy");
        Utils.InvokeMany(typeof(UCE_Mount), this, "OnDestroy_");
    }

    private void OnDrawGizmos()
    {
        Vector3 startHelp = Application.isPlaying ? transform.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startHelp, returnDistance);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(startHelp, followDistance);
    }

    public override bool IsWorthUpdating()
    {
        return true;
    }

    // ==================================== FSM EVENTS ===================================

    private bool EventOwnerDisappeared()
    {
        return owner == null;
    }

    private bool EventDied()
    {
        return !isAlive;
    }

    private bool EventDeathTimeElapsed()
    {
        return state == "DEAD" && Time.time >= deathTimeEnd;
    }

    private bool EventTargetDisappeared()
    {
        return target == null;
    }

    private bool EventTargetDied()
    {
        return target != null && target.health == 0;
    }

    private bool EventTargetTooFarToAttack()
    {
        Vector3 destination;
        return target != null &&
               0 <= currentSkill && currentSkill < skills.Count &&
               !CastCheckDistance(skills[currentSkill], out destination);
    }

    private bool EventTargetTooFarToFollow()
    {
        return target != null &&
               Vector3.Distance(owner.transform.position, target.collider.ClosestPointOnBounds(owner.transform.position)) > followDistance;
    }

    private bool EventNeedReturnToOwner()
    {
        return Vector3.Distance(owner.UCE_mountDestination, transform.position) > returnDistance;
    }

    private bool EventNeedTeleportToOwner()
    {
        return Vector3.Distance(owner.UCE_mountDestination, transform.position) > teleportDistance;
    }

    private bool EventAggro()
    {
        return target != null && target.isAlive;
    }

    private bool EventSkillRequest()
    {
        return 0 <= currentSkill && currentSkill < skills.Count;
    }

    private bool EventSkillFinished()
    {
        return 0 <= currentSkill && currentSkill < skills.Count &&
               skills[currentSkill].CastTimeRemaining() == 0;
    }

    private bool EventMoveEnd()
    {
        return state == "MOVING" && !IsMoving();
    }

    // ==================================== FSM SERVER ===================================

    [Server]
    [DevExtMethods("UpdateServer")]
    private string UpdateServer_IDLE()
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventOwnerDisappeared())
        {
            // owner might disconnect or get destroyed for some reason
            NetworkServer.Destroy(gameObject);
            return "IDLE";
        }
        if (EventDied())
        {
            // we died.
            OnDeath();
            currentSkill = -1; // in case we died while trying to cast
            return "DEAD";
        }
        if (EventTargetDied())
        {
            // we had a target before, but it died now. clear it.
            target = null;
            currentSkill = -1;
            return "IDLE";
        }
        if (EventNeedTeleportToOwner())
        {
            agent.Warp(owner.UCE_mountDestination);
            return "IDLE";
        }
        if (EventNeedReturnToOwner())
        {
            // return to owner only while IDLE
            LookAtY(owner.transform.position);
            target = null;
            currentSkill = -1;
            agent.stoppingDistance = 0;
            agent.destination = owner.UCE_mountDestination;
            return "MOVING";
        }
        if (EventTargetTooFarToFollow())
        {
            // we had a target before, but it's out of follow range now.
            // clear it and go back to start. don't stay here.
            LookAtY(owner.transform.position);
            target = null;
            currentSkill = -1;
            agent.stoppingDistance = 0;
            agent.destination = owner.UCE_mountDestination;
            return "MOVING";
        }
        if (EventTargetTooFarToAttack())
        {
            // we had a target before, but it's out of attack range now.
            // follow it. (use collider point(s) to also work with big entities)
            agent.stoppingDistance = CurrentCastRange() * attackToMoveRangeRatio;
            agent.destination = target.collider.ClosestPointOnBounds(transform.position);
            return "MOVING";
        }
        if (EventSkillRequest())
        {
            // we had a target in attack range before and trying to cast a skill
            // on it. check self (alive, mana, weapon etc.) and target
            Skill skill = skills[currentSkill];
            if (CastCheckSelf(skill) && CastCheckTarget(skill))
            {
                // start casting
                StartCastSkill(skill);
                return "CASTING";
            }
            else
            {
                // invalid target. stop trying to cast.
                target = null;
                currentSkill = -1;
                return "IDLE";
            }
        }
        if (EventAggro())
        {
            // target in attack range. try to cast a first skill on it
            if (skills.Count > 0) currentSkill = NextSkill();
            return "IDLE";
        }

        return "IDLE"; // nothing interesting happened
    }

    [Server]
    [DevExtMethods("UpdateServer")]
    private string UpdateServer_MOVING()
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventOwnerDisappeared())
        {
            // owner might disconnect or get destroyed for some reason
            NetworkServer.Destroy(gameObject);
            return "IDLE";
        }
        if (EventDied())
        {
            // we died.
            OnDeath();
            currentSkill = -1; // in case we died while trying to cast
            agent.ResetPath();
            return "DEAD";
        }
        if (EventMoveEnd())
        {
            // we reached our destination.
            return "IDLE";
        }
        if (EventTargetDied())
        {
            // we had a target before, but it died now. clear it.
            target = null;
            currentSkill = -1;
            agent.ResetPath();
            return "IDLE";
        }
        if (EventNeedTeleportToOwner())
        {
            agent.Warp(owner.UCE_mountDestination);
            return "IDLE";
        }
        if (EventTargetTooFarToFollow())
        {
            // we had a target before, but it's out of follow range now.
            // clear it and go back to start. don't stay here.
            LookAtY(owner.transform.position);
            target = null;
            currentSkill = -1;
            agent.stoppingDistance = 0;
            agent.destination = owner.UCE_mountDestination;
            return "MOVING";
        }
        if (EventTargetTooFarToAttack())
        {
            // we had a target before, but it's out of attack range now.
            // follow it. (use collider point(s) to also work with big entities)
            agent.stoppingDistance = CurrentCastRange() * attackToMoveRangeRatio;
            agent.destination = target.collider.ClosestPointOnBounds(transform.position);
            return "MOVING";
        }
        if (EventAggro())
        {
            // target in attack range. try to cast a first skill on it
            // (we may get a target while randomly wandering around)
            if (skills.Count > 0) currentSkill = NextSkill();
            agent.ResetPath();
            return "IDLE";
        }

        return "MOVING"; // nothing interesting happened
    }

    [Server]
    [DevExtMethods("UpdateServer")]
    private string UpdateServer_CASTING()
    {
        // keep looking at the target for server & clients (only Y rotation)
        if (target) LookAtY(target.transform.position);

        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventOwnerDisappeared())
        {
            // owner might disconnect or get destroyed for some reason
            NetworkServer.Destroy(gameObject);
            return "IDLE";
        }
        if (EventDied())
        {
            // we died.
            OnDeath();
            currentSkill = -1; // in case we died while trying to cast
            return "DEAD";
        }
        if (EventTargetDisappeared())
        {
            // cancel if the target matters for this skill
            if (skills[currentSkill].cancelCastIfTargetDied)
            {
                currentSkill = -1;
                target = null;
                return "IDLE";
            }
        }
        if (EventTargetDied())
        {
            // cancel if the target matters for this skill
            if (skills[currentSkill].cancelCastIfTargetDied)
            {
                currentSkill = -1;
                target = null;
                return "IDLE";
            }
        }
        if (EventSkillFinished())
        {
            // finished casting. apply the skill on the target.
            FinishCastSkill(skills[currentSkill]);

            // did the target die? then clear it so that the monster doesn't
            // run towards it if the target respawned
            if (target.health == 0) target = null;

            // go back to IDLE
            lastSkill = currentSkill;
            currentSkill = -1;
            return "IDLE";
        }

        return "CASTING"; // nothing interesting happened
    }

    [Server]
    [DevExtMethods("UpdateServer")]
    private string UpdateServer_DEAD()
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventOwnerDisappeared())
        {
            // owner might disconnect or get destroyed for some reason
            NetworkServer.Destroy(gameObject);
            return "DEAD";
        }
        if (EventDeathTimeElapsed())
        {
            // we were lying around dead for long enough now.
            // hide while respawning, or disappear forever
            NetworkServer.Destroy(gameObject);
            return "DEAD";
        }

        return "DEAD"; // nothing interesting happened
    }

    [Server]
    protected override string UpdateServer()
    {
        if (state == "IDLE") return UpdateServer_IDLE();
        if (state == "MOVING") return UpdateServer_MOVING();
        if (state == "CASTING") return UpdateServer_CASTING();
        if (state == "DEAD") return UpdateServer_DEAD();
        return "IDLE";
    }

    // ==================================== FSM CLIENT ===================================

    [Client]
    protected override void UpdateClient()
    {
        if (state == "CASTING")
        {
            if (target) LookAtY(target.transform.position);
        }

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("UpdateClient");
        Utils.InvokeMany(typeof(UCE_Mount), this, "UpdateClient_");
    }

    // ======================================= AGGRO =====================================

    [ServerCallback]
    public override void OnAggro(Entity entity)
    {
        if (entity != null && CanAttack(entity))
        {
            if (target == null)
            {
                target = entity;
            }
            else
            {
                float oldDistance = Vector3.Distance(transform.position, target.transform.position);
                float newDistance = Vector3.Distance(transform.position, entity.transform.position);
                if (newDistance < oldDistance * 0.8) target = entity;
            }
        }
    }

    [Server]
    protected override void OnDeath()
    {
        base.OnDeath();
        deathTimeEnd = Time.time + deathTime;

        // keep player's pet item up to date
        SyncToOwnerItem();

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("OnDeath");
        Utils.InvokeMany(typeof(UCE_Mount), this, "OnDeath_");
    }

    // ======================================= DAMAGE ====================================

    [Server]
    public override void DealDamageAt(Entity entity, int amount, float stunChance = 0, float stunTime = 0)
    {
        // deal damage with the default function
        base.DealDamageAt(entity, amount, stunChance, stunTime);

        // a monster?
        if (entity is Monster)
        {
            // forward to owner to share rewards with everyone
            owner.OnDamageDealtToMonster((Monster)entity);
        }
        // a player?
        // (see murder code section comments to understand the system)
        else if (entity is Player)
        {
            // forward to owner for murderer detection etc.
            owner.OnDamageDealtToPlayer((Player)entity);
        }
        // a pet?
        // (see murder code section comments to understand the system)
        else if (entity is Pet)
        {
            owner.OnDamageDealtToPet((Pet)entity);
        }

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("DealDamageAt", entity, amount, stunChance, stunTime);
        Utils.InvokeMany(typeof(UCE_Mount), this, "DealDamageAt_", entity, amount, stunChance, stunTime);
    }

    // ====================================== SKILLS =====================================

    public override bool CanAttack(Entity entity)
    {
        return isAlive &&
               entity.isAlive &&
               entity != this &&
               (entity is Monster ||
                (entity is Player && entity != owner) ||
                (entity is UCE_Mount && entity != owner) ||
                (entity is Pet && ((Pet)entity).owner != owner));
    }

    public float CurrentCastRange()
    {
        return 0 <= currentSkill && currentSkill < skills.Count ? skills[currentSkill].castRange : 0;
    }

#if !_iMMOCONDITIONALSKILLS
    protected int NextSkill()
    {
        for (int i = 0; i < skills.Count; ++i)
        {
            int index = (lastSkill + 1 + i) % skills.Count;
            if (CastCheckSelf(skills[index]))
                return index;
        }
        return -1;
    }
#endif

    // -----------------------------------------------------------------------------------
}
