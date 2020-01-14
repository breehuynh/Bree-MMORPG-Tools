// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// UCE ARROW PROJECTILE

[RequireComponent(typeof(NetworkIdentity))]
public partial class UCE_ArrowProjectile : UCE_Projectile
{
    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public override void Init()
    {
        base.Init();
    }

    // -----------------------------------------------------------------------------------
    // FixedUpdate
    // -----------------------------------------------------------------------------------
    public override void FixedUpdate()
    {
        if (target != null && caster != null)
        {
            checkWall();

            Vector3 goal = target.collider.bounds.center;
            transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.fixedDeltaTime);
            transform.LookAt(goal);

            if (isServer && transform.position == goal)
            {
                if (target.isAlive)
                    OnProjectileImpact();

                if (destroyDelay != 0)
                {
                    Invoke("OnDestroyDelayed", destroyDelay);
                }
                else
                {
                    OnDestroyDelayed();
                }
            }
        }
        else if (isServer) NetworkServer.Destroy(gameObject);
    }

    // -----------------------------------------------------------------------------------
    // checkWall
    // -----------------------------------------------------------------------------------
    protected void checkWall()
    {
        if (!isServer || wallTag == "") return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.25f))
        {
            if (hit.collider.tag == wallTag)
            {
                if (data.impactEffect != null)
                {
                    GameObject go = Instantiate(data.impactEffect.gameObject, transform.position, Quaternion.identity);
                    go.GetComponent<OneTimeTargetSkillEffect>().caster = caster;
                    go.GetComponent<OneTimeTargetSkillEffect>().target = target;
                    NetworkServer.Spawn(go);
                    NetworkServer.Destroy(gameObject);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
