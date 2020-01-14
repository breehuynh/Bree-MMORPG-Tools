// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// UCE BALLISTIC PROJECTILE

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody), typeof(NetworkIdentity))]
public partial class UCE_BallisticProjectile : UCE_Projectile
{
    [Header("-=-=-=- Visual Effects -=-=-=-")]
    [Tooltip("[Required] Should be 15-45 for best results")]
    [Range(10f, 80f)] public float angle = 15f;

    // -----------------------------------------------------------------------------------
    // Init
    // -----------------------------------------------------------------------------------
    public override void Init()
    {
        GetComponent<Rigidbody>().velocity = calcBallisticVelocityVector(caster.transform, target.collider.bounds.center, angle);
        base.Init();
    }

    // -----------------------------------------------------------------------------------
    // OnStartClient
    // -----------------------------------------------------------------------------------
    public override void OnStartClient()
    {
        GetComponent<Rigidbody>().velocity = calcBallisticVelocityVector(caster.transform, target.collider.bounds.center, angle);
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerEnter
    // -----------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (!isServer || bArrivedAtTarget) return;

        if (wallTag != "" && other.gameObject.tag == wallTag)
        {
            OnDestroyDelayed();
            return;
        }

        Entity candidate = other.gameObject.GetComponentInParent<Entity>();

        if (isServer && candidate != null && candidate != caster && caster.CanAttack(candidate) && candidate.isAlive)
        {
            Debug.Log(candidate);
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

    // -----------------------------------------------------------------------------------
}
