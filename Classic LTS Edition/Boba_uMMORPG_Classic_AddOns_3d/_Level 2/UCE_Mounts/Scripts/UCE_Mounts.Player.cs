// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Linq;
using UnityEngine;

// PLAYER

public partial class Player
{
    [Header("-=-=-=- UCE MOUNT -=-=-=-")]
    [SyncVar] protected GameObject _UCE_unmountedMount;

    [SyncVar] protected GameObject _UCE_mountedMount;
    [SyncVar, HideInInspector] public bool UCE_mounted = false;

    // -----------------------------------------------------------------------------------
    // unmountedMount
    // -----------------------------------------------------------------------------------
    public UCE_Mount unmountedMount
    {
        get { return _UCE_unmountedMount != null ? _UCE_unmountedMount.GetComponent<UCE_Mount>() : null; }
        set { _UCE_unmountedMount = value != null ? value.gameObject : null; }
    }

    // -----------------------------------------------------------------------------------
    // mountedMount
    // -----------------------------------------------------------------------------------
    public UCE_MountedMount mountedMount
    {
        get { return _UCE_mountedMount != null ? _UCE_mountedMount.GetComponent<UCE_MountedMount>() : null; }
        set { _UCE_mountedMount = value != null ? value.gameObject : null; }
    }

    // -----------------------------------------------------------------------------------
    // UCE_mountDestination
    // -----------------------------------------------------------------------------------
    public Vector3 UCE_mountDestination
    {
        get
        {
            Bounds bounds = collider.bounds;
            return transform.position + transform.right * bounds.size.x;
        }
    }

    // -----------------------------------------------------------------------------------
    // OnDeath_UCE_Mount
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDeath")]
    private void OnDeath_UCE_Mount()
    {
        if (mountedMount)
            UCE_Unmount(mountedMount.autoRide);

        UCE_ClearMountedData();

        if (!UCE_mounted && _UCE_unmountedMount)
            UCE_UnsummonMount();
    }

    // -----------------------------------------------------------------------------------
    // UCE_ToggleMount
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_ToggleMount(int inventoryIndex)
    {
        ItemSlot slot = inventory[inventoryIndex];

        UCE_Tmpl_MountItem data = (UCE_Tmpl_MountItem)slot.item.data;

        // -- Mounted: Unmount & Summon Mount

        if (UCE_mounted)
        {
            UCE_Unmount(data.autoRide);

            // -- Unmounted & Mount available: Unsummon Mount
        }
        else if (!UCE_mounted && _UCE_unmountedMount && !data.autoRide)
        {
            UCE_UnsummonMount();

            // -- Unmounted: Summon Mount & Ride
        }
        else if (!UCE_mounted && data.autoRide)
        {
            UCE_SummonAndRideMount(slot);

            // -- Unmounted & no Mount available: Summon Mount
        }
        else if (!UCE_mounted && _UCE_unmountedMount == null && !data.autoRide)
        {
            UCE_SummonMount(inventoryIndex);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_ClearMountedData
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_ClearMountedData()
    {
        if (!UCE_mounted) return;

        UCE_mounted = false;

        // -- Buff
        if (mountedMount.applyBuffWhileMounted)
            buffs.RemoveAt(buffs.FindIndex(x => x.data == mountedMount.applyBuffWhileMounted));

        // -- Speed
        UCE_resetModifySpeed();

        // -- Visibility
        if (mountedMount.playerInvisibleWhileMounted)
        {
            UCE_ToggleVisibility(true);
        }

        // -- Activate Skills
        _cannotCast = false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_Unmount
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_Unmount(bool autoRide)
    {
        UCE_ClearMountedData();

        // -- Unmount & Summon Mount
        if (!autoRide)
            UCE_UnmountAndSummonMount();

        // -- Destroy mounted Mount
        NetworkServer.Destroy(_UCE_mountedMount);
        _UCE_mountedMount = null;

        Rpc_UCE_Unmount();
    }

    // -----------------------------------------------------------------------------------
    // UCE_UnsummonMount
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_UnsummonMount()
    {
        NetworkServer.Destroy(_UCE_unmountedMount);
        _UCE_unmountedMount = null;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SummonMount
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_UnmountAndSummonMount()
    {
        GameObject _mount = (GameObject)Instantiate(mountedMount.unmountedMount.gameObject, UCE_mountDestination, Quaternion.identity);

        _mount.name = mountedMount.name;
        _mount.GetComponent<UCE_Mount>().health = mountedMount.health;
        _mount.GetComponent<UCE_Mount>().level = mountedMount.level;
        _mount.GetComponent<UCE_Mount>().experience = mountedMount.experience;
        _mount.GetComponent<UCE_Mount>().autoRide = mountedMount.autoRide;
        _mount.GetComponent<UCE_Mount>().owner = this;

        NetworkServer.Spawn(_mount);
        _UCE_unmountedMount = _mount;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SummonMount
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_SummonMount(int inventoryIndex)
    {
        if (_UCE_unmountedMount == null && !UCE_mounted)
        {
            ItemSlot slot = inventory[inventoryIndex];

            UCE_Tmpl_MountItem data = (UCE_Tmpl_MountItem)slot.item.data;

            GameObject _mount = (GameObject)Instantiate(data.summonPrefab.gameObject, UCE_mountDestination, Quaternion.identity);

            _mount.name = data.summonPrefab.name;
            _mount.GetComponent<UCE_Mount>().health = slot.item.summonedHealth;
            _mount.GetComponent<UCE_Mount>().level = slot.item.summonedLevel;
            _mount.GetComponent<UCE_Mount>().experience = slot.item.summonedExperience;
            _mount.GetComponent<UCE_Mount>().autoRide = data.autoRide;
            _mount.GetComponent<UCE_Mount>().owner = this;

            NetworkServer.Spawn(_mount);
            _UCE_unmountedMount = _mount;

            slot.item.summoned = _mount;
            inventory[inventoryIndex] = slot;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_SummonAndRideMount
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    public void UCE_SummonAndRideMount(ItemSlot slot)
    {
        if (UCE_mounted) return;

        UCE_Tmpl_MountItem data = (UCE_Tmpl_MountItem)slot.item.data;

        GameObject go = Instantiate(((UCE_Mount)data.summonPrefab).mountedMount.gameObject, transform.position, Quaternion.identity);
        UCE_MountedMount mount = go.GetComponent<UCE_MountedMount>();

        mount.health = slot.item.summonedHealth;
        mount.level = slot.item.summonedLevel;
        mount.experience = slot.item.summonedExperience;
        mount.autoRide = data.autoRide;
        mount.owner = this;

        mount.transform.position = transform.position;
        mount.transform.rotation = transform.rotation;

        NetworkServer.Spawn(go);

        _UCE_mountedMount = go;

        // -- Buff
        if (mountedMount.applyBuffWhileMounted)
            AddOrRefreshBuff(new Buff(mountedMount.applyBuffWhileMounted, mountedMount.buffLevel));

        // -- Speed
        UCE_ModifySpeedPercentage(mountedMount.speedModiferPercent);

        // -- Visibility
        if (mountedMount.playerInvisibleWhileMounted)
        {
            UCE_ToggleVisibility(false);
        }

        // -- active Skills
        _cannotCast = go.GetComponent<UCE_MountedMount>().cannotCastWhileMounted;

        transform.position = go.GetComponent<UCE_MountedMount>().mountPoint.transform.position;

        UCE_mounted = true;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("LateUpdate")]
    private void LateUpdate_UCE_Mount()
    {
        if (Player.localPlayer == this)
        {
            if (Input.GetMouseButtonDown(0) && !Utils.IsCursorOverUserInterface() && Input.touchCount <= 1)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    UCE_MountedMount mountedMount = hit.transform.GetComponent<UCE_MountedMount>();
                    if (mountedMount != null)
                    {
                        if (mountedMount.owner == this)
                            Cmd_UCE_Unmount();
                    }

                    UCE_Mount mount = hit.transform.GetComponent<UCE_Mount>();
                    if (mount != null && mount.isAlive)
                    {
                        if (mount.owner == this)
                            UCE_TryMount(mount.gameObject);
                    }
                }
            }
        }

        if (isClient)
        {
            if (UCE_mounted && (state == "MOVING" || state == "IDLE"))
            {
                foreach (Animator anim in GetComponentsInChildren<Animator>())
                {
                    if (anim.parameters.Any(x => x.name == "MOUNTED"))
                        anim.SetBool("MOUNTED", UCE_mounted);
                    if (anim.parameters.Any(x => x.name == "MOVING"))
                        anim.SetBool("MOVING", false);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_TryMount
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    public void UCE_TryMount(GameObject _mount)
    {
        if (Utils.ClosestDistance(this, _mount.GetComponent<UCE_Mount>()) <= interactionRange / 4)
        {
            Cmd_UCE_Ride();
        }
        else
        {
            agent.destination = _mount.GetComponent<UCE_Mount>().collider.ClosestPointOnBounds(transform.position);
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Ride
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_Ride()
    {
        if (unmountedMount == null || UCE_mounted) return;

        GameObject go = Instantiate(unmountedMount.mountedMount.gameObject, transform.position, Quaternion.identity);
        UCE_MountedMount mount = go.GetComponent<UCE_MountedMount>();

        mount.health = unmountedMount.health;
        mount.level = unmountedMount.level;
        mount.experience = unmountedMount.experience;
        mount.autoRide = unmountedMount.autoRide;
        mount.owner = this;

        mount.transform.position = transform.position;
        mount.transform.rotation = transform.rotation;
        mount.GetComponent<UCE_MountedMount>().owner = this;

        NetworkServer.Spawn(go);

        _UCE_mountedMount = go;

        // -- Buff
        if (mountedMount.applyBuffWhileMounted)
            AddOrRefreshBuff(new Buff(mountedMount.applyBuffWhileMounted, mountedMount.buffLevel));

        // -- Speed
        UCE_ModifySpeedPercentage(mountedMount.speedModiferPercent);

        // -- Visibility
        if (mountedMount.playerInvisibleWhileMounted)
        {
            UCE_ToggleVisibility(false);
        }

        // -- active Skills
        _cannotCast = go.GetComponent<UCE_MountedMount>().cannotCastWhileMounted;

        transform.position = go.GetComponent<UCE_MountedMount>().mountPoint.transform.position;

        NetworkServer.Destroy(_UCE_unmountedMount.gameObject);

        UCE_mounted = true;
    }

    // -----------------------------------------------------------------------------------
    // UCE_UpdateMounted
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    public void UCE_UpdateMounted(GameObject _UCE_mountedMount)
    {
        if (_UCE_mountedMount)
            transform.position = _UCE_mountedMount.GetComponent<UCE_MountedMount>().mountPoint.position;
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Unmount
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_Unmount()
    {
        if (mountedMount)
            UCE_Unmount(mountedMount.autoRide);
    }

    // -----------------------------------------------------------------------------------
    // Rpc_UCE_Unmount
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [ClientRpc]
    public void Rpc_UCE_Unmount()
    {
        _UCE_mountedMount = null;
        UCE_mounted = false;

        foreach (Animator anim in GetComponentsInChildren<Animator>())
        {
            if (anim.parameters.Any(x => x.name == "MOUNTED"))
            {
                anim.SetBool("MOUNTED", UCE_mounted);

                if (anim.parameters.Any(x => x.name == "MOVING"))
                    anim.SetBool("MOVING", false);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // OnDamageDealt_UCE_Mounts
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    [DevExtMethods("OnDamageDealt")]
    private void OnDamageDealt_UCE_Mounts()
    {
        if (UCE_mounted && mountedMount)
        {
            if (UnityEngine.Random.value <= mountedMount.unmountWhenHit)
                Cmd_UCE_Unmount();
        }
    }

    // -----------------------------------------------------------------------------------
}
