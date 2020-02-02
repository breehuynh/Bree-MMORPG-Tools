// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// UCE COLLIDER PROJECTILE

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody), typeof(NetworkIdentity))]
public partial class UCE_ColliderProjectile : UCE_Projectile
{
    protected float fDistance = 0;
    protected Vector3 targetPosition;
    protected Vector3 startPosition;

    // -----------------------------------------------------------------------------------
    // Init
    // -----------------------------------------------------------------------------------
    public override void Init()
    {
        base.Init();
        startPosition = transform.position;
        targetPosition = target.collider.bounds.center;
    }

    // -----------------------------------------------------------------------------------
    // OnStartClient
    // -----------------------------------------------------------------------------------
    public override void OnStartClient()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerEnter
    // -----------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        // -- Impact Check only Server Side
        if (!isServer || bArrivedAtTarget) return;

        if (wallTag != "" && other.gameObject.tag == wallTag)
        {
            OnDestroyDelayed();
            return;
        }

        Entity candidate = other.gameObject.GetComponentInParent<Entity>();

        if (candidate != null && candidate != caster && caster.CanAttack(candidate) && candidate.isAlive)
            OnProjectileImpact(candidate);
    }

    // -----------------------------------------------------------------------------------
    // FixedUpdate
    // -----------------------------------------------------------------------------------
    public override void FixedUpdate()
    {
        if (bArrivedAtTarget) return;

        GetComponent<Rigidbody>().velocity = transform.forward * speed;

        fDistance = Vector3.Distance(transform.position, startPosition);

        // -- Impact Check only Server Side
        if (isServer && fDistance >= data.distance)
        {
            bArrivedAtTarget = true;

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
