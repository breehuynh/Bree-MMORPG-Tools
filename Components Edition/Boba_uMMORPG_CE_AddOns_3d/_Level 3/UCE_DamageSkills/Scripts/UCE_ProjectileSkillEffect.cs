using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class UCE_ProjectileSkillEffect : SkillEffect
{

    [Header("Properties")]
    public float speed = 25;
    public float rotationSpeed = 0;

    [HideInInspector] public UCE_BaseDamageSkill parentSkill; // set by skill
    [HideInInspector] public int skillLevel = 1; // set by skill

    // effects like a trail or particles need to have their initial positions
    // corrected too. simply connect their .Clear() functions to the event.
    public UnityEvent onSetInitialPosition;

    public override void OnStartClient()
    {
        SetInitialPosition();
    }

    void SetInitialPosition()
    {
        // the projectile should always start at the effectMount position.
        // -> server doesn't run animations, so it will never spawn it exactly
        //    where the effectMount is on the client by the time the packet
        //    reaches the client.
        // -> the best solution is to correct it here once
        if (target != null && caster != null)
        {
            transform.position = caster.skills.effectMount.position;

            //if (rotationSpeed > 0)
                //transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

            transform.LookAt(target.collider.bounds.center);

            onSetInitialPosition.Invoke();
        }
    }

    // fixedupdate on client and server to simulate the same effect without
    // using a NetworkTransform
    void FixedUpdate()
    {
        // target and caster still around?
        // note: we keep flying towards it even if it died already, because
        //       it looks weird if fireballs would be canceled inbetween.
        if (target != null && caster != null)
        {
            // move closer and look at the target
            Vector3 goal = target.collider.bounds.center;
            transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.fixedDeltaTime);
            transform.LookAt(goal);

            // server: reached it? apply skill and destroy self
            if (isServer && transform.position == goal)
            {
                if (target.isAlive)
                {
                    Apply();
                }
                NetworkServer.Destroy(gameObject);
            }
        }
        else if (isServer) NetworkServer.Destroy(gameObject);
    }

    // animation (if any)
    [ClientCallback]
    void Update()
    {
        if (rotationSpeed > 0)
            transform.RotateAround(transform.position, Vector3.forward, Time.deltaTime * rotationSpeed);
    }

    // -----------------------------------------------------------------------------------
    // Apply
    // -----------------------------------------------------------------------------------
    public void Apply()
    {
        List<Entity> targets = new List<Entity>();

        if (parentSkill.SpawnEffectOnMainTargetOnly)
            SpawnEffect(caster, caster.target);

        if (caster.target is Player && caster is Player && ((Player)caster).UCE_SameCheck((Player)caster.target, parentSkill.affectSelf, parentSkill.affectPlayers, parentSkill.affectOwnParty, parentSkill.affectOwnGuild, parentSkill.affectOwnRealm, parentSkill.reverseTargeting) ||
			(caster.target is Monster && parentSkill.affectEnemies ) ||
			(caster is Monster && caster.target is Monster && parentSkill.affectEnemies) ||
			(caster is Monster && caster.target is Player && parentSkill.affectPlayers)
		)
        	if (caster.target.isAlive)
                targets.Add(caster.target);

            if (parentSkill.castRadius.Get(skillLevel) > 0)
        {

            if (caster is Player)
                targets.AddRange(((Player)caster).UCE_GetCorrectedTargetsInSphere(target.transform, parentSkill.castRadius.Get(skillLevel), false, parentSkill.affectSelf, parentSkill.affectOwnParty, parentSkill.affectOwnGuild, parentSkill.affectOwnRealm, parentSkill.reverseTargeting, parentSkill.affectPlayers, parentSkill.affectEnemies));
            else
                targets.AddRange(caster.UCE_GetCorrectedTargetsInSphere(target.transform, parentSkill.castRadius.Get(skillLevel), false, parentSkill.affectSelf, parentSkill.affectOwnParty, parentSkill.affectOwnGuild, parentSkill.affectOwnRealm, parentSkill.reverseTargeting, parentSkill.affectPlayers, parentSkill.affectEnemies));

        }

        parentSkill.ApplyToTargets(targets, caster, skillLevel);

        targets.Clear();
    }

    // -----------------------------------------------------------------------------------
    // SpawnEffect
    // -----------------------------------------------------------------------------------
    public void SpawnEffect(Entity caster, Entity spawnTarget)
    {
        if (parentSkill.effect != null)
        {
            GameObject go = Instantiate(parentSkill.effect.gameObject, spawnTarget.transform.position, Quaternion.identity);
            go.GetComponent<OneTimeTargetSkillEffect>().caster = caster;
            go.GetComponent<OneTimeTargetSkillEffect>().target = spawnTarget;
            NetworkServer.Spawn(go);
        }
    }

}
