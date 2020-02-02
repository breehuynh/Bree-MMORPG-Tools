// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System.Linq;

#if _iMMODOORS

// UCE DOORS

public partial class UCE_Doors : UCE_InteractableObject
{
    [Header("[-=-=-=- UCE DOORS -=-=-=-]")]
    public Animator animator;

    [Tooltip("Base duration (in seconds) it takes to access (open)")]
    public float accessDuration;

    [Tooltip("Base duration (in seconds) it takes for the door to auto close.")]
    public float closeDuration;

    [Header("[ANIMATION & SOUND]")]
    [Tooltip("[Optional] Name of player animation parameter that is played when accessing (leave empty to disable).")]
    public string playerAnimation = "";

    [Tooltip("[Optional] Name of animation parameter that is played when opening (leave empty to disable).")]
    public string doorAnimationOpen = "OPEN";

    [Tooltip("[Optional] Name of animation parameter that is played when closing (leave empty to disable).")]
    public string doorAnimationClose = "CLOSE";

    [Tooltip("[Optional] Sound played when opening.")]
    public AudioClip soundOpen;

    [Tooltip("[Optional] Sound played when closing.")]
    public AudioClip soundClose;

    [Header("[MESSAGES]")]
    [Tooltip("[Optional] Message shown to the player when the access requirements are not met.")]
    public string lockedMessage;

    [Tooltip("[Optional] Shown while opening the door.")]
    public string accessLabel;

    private float closeTimer = 0;
    [SyncVar] private bool lastOpen = false;
    [SyncVar, HideInInspector] public bool open = false;

    // -----------------------------------------------------------------------------------
    // OnInteractClient
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public override void OnInteractClient(Player player)
    {
        player.UCE_OnSelect_Door(this);
    }

    // -----------------------------------------------------------------------------------
    // OnInteractServer
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public override void OnInteractServer(Player player)
    {
        open = !open;
        lastOpen = !open;

        if (open)
            closeTimer = Time.time + closeDuration;
    }

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public override void Start()
    {
        base.Start();
    }

    // -----------------------------------------------------------------------------------
    // OnOpen
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public void OnOpen()
    {
        if (open != lastOpen)
        {
            if (!string.IsNullOrWhiteSpace(doorAnimationClose))
                animator.SetBool(doorAnimationClose, false);
            if (!string.IsNullOrWhiteSpace(doorAnimationOpen))
                animator.SetBool(doorAnimationOpen, true);
            PlaySound(soundClose);
            lastOpen = true;
        }
    }

    // -----------------------------------------------------------------------------------
    // OnClose
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public void OnClose()
    {
        if (open != lastOpen)
        {
            if (!string.IsNullOrWhiteSpace(doorAnimationOpen))
                animator.SetBool(doorAnimationOpen, false);
            if (!string.IsNullOrWhiteSpace(doorAnimationClose))
                animator.SetBool(doorAnimationClose, true);
            PlaySound(soundOpen);
            lastOpen = false;
        }
    }

    // -----------------------------------------------------------------------------------
    // OnClose
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public void OnLocked()
    {
        if (open != lastOpen)
        {
            if (!string.IsNullOrWhiteSpace(doorAnimationOpen))
                animator.SetBool(doorAnimationOpen, false);
            if (!string.IsNullOrWhiteSpace(doorAnimationClose))
                animator.SetBool(doorAnimationClose, false);
            PlaySound(soundOpen);
            lastOpen = false;
        }
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    public override void Update()
    {
        base.Update();

        if (IsWorthUpdating())
        {
            if (isClient)
                UpdateClient();

            if (isServer)
                UpdateServer();
        }
    }

    // -----------------------------------------------------------------------------------
    // UpdateClient
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    protected void UpdateClient()
    {
        if (!unlocked)
        {
            OnLocked();
            return;
        }

        if (open)
            OnOpen();
        else
            OnClose();
    }

    // -----------------------------------------------------------------------------------
    // UpdateServer
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    protected void UpdateServer()
    {
        RpcDoor();

        if (EventCloseTimeElapsed())
            open = false;
    }

    // -----------------------------------------------------------------------------------
    // ServerClientDoor
    // @Server
    // -----------------------------------------------------------------------------------
    [ClientRpc]
    public void RpcDoor()
    {
        if (!unlocked)
        {
            OnLocked();
            return;
        }

        if (open)
            OnOpen();
        else
            OnClose();
    }

    // -----------------------------------------------------------------------------------
    // EventEmptyTimeElapsed
    // -----------------------------------------------------------------------------------
    private bool EventCloseTimeElapsed()
    {
        return open && NetworkTime.time >= closeTimer;
    }
}

#endif
