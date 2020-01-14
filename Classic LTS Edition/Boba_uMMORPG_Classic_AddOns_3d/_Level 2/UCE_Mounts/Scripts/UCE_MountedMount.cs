// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;
using UnityEngine.AI;

// UCE MOUNT (MOUNTED)

[RequireComponent(typeof(Animator))]
public partial class UCE_MountedMount : NetworkBehaviour
{
    [Header("-=-=-=- UCE MOUNTED MOUNT -=-=-=-")]
    [Header("Components")]
    public NavMeshAgent agent;

    [Tooltip("Speed bonus in percent (0.01 = +1%, 0.1 = +10%, 1.0 = +100%) - total can exceed max speed of entity")]
    public float speedModiferPercent = 0;

    public bool cannotCastWhileMounted;
    public bool playerInvisibleWhileMounted;

    [Range(0, 1)]
    public float unmountWhenHit;

    public BuffSkill applyBuffWhileMounted;
    public int buffLevel;

    public Transform mountPoint;
    public GameObject _unmountedMount;

    [HideInInspector] public int health;
    [HideInInspector] public int level;
    [HideInInspector] public long experience;
    [HideInInspector] public bool autoRide;

    [SyncVar] private GameObject _owner;

    // -----------------------------------------------------------------------------------
    // unmountedMount
    // -----------------------------------------------------------------------------------
    private void CopyOwnerPositionAndRotation()
    {
        if (owner != null)
        {
            agent.Warp(owner.transform.position);
            transform.rotation = owner.transform.rotation;
        }
    }

    // -----------------------------------------------------------------------------------
    // unmountedMount
    // -----------------------------------------------------------------------------------
    public UCE_Mount unmountedMount
    {
        get { return _unmountedMount != null ? _unmountedMount.GetComponent<UCE_Mount>() : null; }
        set { _unmountedMount = value != null ? value.gameObject : null; }
    }

    // -----------------------------------------------------------------------------------
    // owner
    // -----------------------------------------------------------------------------------
    public Player owner
    {
        get { return _owner != null ? _owner.GetComponent<Player>() : null; }
        set { _owner = value != null ? value.gameObject : null; }
    }

    // -----------------------------------------------------------------------------------
    // LateUpdate
    // @Client OR @Server
    // -----------------------------------------------------------------------------------
    private void LateUpdate()
    {
        // destroy server side when owner is gone
        if (owner == null && isServer)
        {
            NetworkServer.Destroy(gameObject);
            return;
        }

        // copy owner's position and rotation. no need for NetworkTransform.
        CopyOwnerPositionAndRotation();

        // stop when not on server
        if (!isClient || !owner) return;

        foreach (Animator anim in GetComponentsInChildren<Animator>())
        {
            anim.SetBool("MOVING", owner.IsMoving());
        }

        owner.UCE_UpdateMounted(this.gameObject);
    }

    // -----------------------------------------------------------------------------------
}
