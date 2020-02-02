// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Linq;
using UnityEngine;

#if _iMMOCHEST

// UCE LOOTCRATE

public partial class UCE_Lootcrate : UCE_InteractableObject
{
    [Header("[-=-=-=- UCE LOOTCRATE [See Tooltips] -=-=-=-]")]
    public Animator animator;

    public ParticleSystem lootIndicator;

    [Tooltip("Base duration (in seconds) it takes to access (open)")]
    public float accessDuration;

    [Header("[LOOT]")]
    [Tooltip("[Optional] Minimum amount of gold available")]
    public int lootGoldMin = 0;

    [Tooltip("[Optional] Maximum amount of gold available")]
    public int lootGoldMax = 10;

    [Tooltip("[Optional] Minimum amount of coins (premium currency) available")]
    public int lootCoinsMin = 0;

    [Tooltip("[Optional] Maximum amount of coins (premium currency) available")]
    public int lootCoinsMax = 5;

    [Tooltip("[Optional] Loot Items available")]
    public ItemDropChance[] dropChances;

    [Header("[RESPAWN]")]
    [Tooltip("[Optional] How long the lootcrate stays around, when it has been plundered (in seconds)")]
    public float emptyTime = 10f;

    [Tooltip("[Optional] If the lootcrate is respawned once it got plundered. Otherwise its available only once.")]
    public bool respawn = true;

    [Tooltip("[Optional] How long it takes for a lootcrate to respawn (in seconds) with loot again (0 to disable).")]
    public float respawnTime = 30f;

    [Header("[ANIMATION & SOUND]")]
    [Tooltip("[Optional] Name of player animation parameter that is played when accessing (leave empty to disable).")]
    public string playerAnimation = "";

    [Tooltip("[Optional] Name of animation parameter that is played when opening (leave empty to disable).")]
    public string crateAnimationOpen = "OPEN";

    [Tooltip("[Optional] Name of animation parameter that is played when closing (leave empty to disable).")]
    public string crateAnimationClose = "CLOSE";

    [Tooltip("[Optional] Sound played when opening.")]
    public AudioClip soundOpen;

    [Tooltip("[Optional] Sound played when closing.")]
    public AudioClip soundClose;

    [Tooltip("[Optional] GameObject spawned as effect when successfully looted (see ReadMe).")]
    public GameObject lootEffect;

    [Tooltip("[Optional] Sound played when successfully looted.")]
    public AudioClip lootSound;

    [Header("[MESSAGES]")]
    [Tooltip("[Optional] Message shown to the player when opening the lootcrate.")]
    public string successMessage;

    [Tooltip("[Optional] Message shown to the player when the access requirements are not met.")]
    public string lockedMessage;

    [Tooltip("[Optional] Shown while opening the lootcrate.")]
    public string accessLabel;

    private bool lastOpen = false;
    private double emptyTimeEnd;
    private double respawnTimeEnd;

    [SyncVar, HideInInspector] public long coins;
    [SyncVar, HideInInspector] public long gold;
    [SyncVar, HideInInspector] public bool _open = false;

    public SyncListItemSlot inventory = new SyncListItemSlot();

    // -----------------------------------------------------------------------------------
    // open
    // -----------------------------------------------------------------------------------
    public bool open
    {
        get { return _open; }
        set
        {
            lastOpen = _open;
            _open = value;
        }
    }

    // -----------------------------------------------------------------------------------
    // OnInteractClient
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public override void OnInteractClient(Player player)
    {
        player.UCE_OnSelect_Lootcrate(this);
    }

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public override void Start()
    {
        base.Start();
        OnRefill();
    }

    // -----------------------------------------------------------------------------------
    // EventEmptyTimeElapsed
    // -----------------------------------------------------------------------------------
    private bool EventEmptyTimeElapsed()
    {
        return open && !HasLoot() && NetworkTime.time >= emptyTimeEnd;
    }

    // -----------------------------------------------------------------------------------
    // EventRespawnTimeElapsed
    // -----------------------------------------------------------------------------------
    private bool EventRespawnTimeElapsed()
    {
        return IsHidden() && respawn && !HasLoot() && NetworkTime.time >= respawnTimeEnd;
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
            if (!string.IsNullOrWhiteSpace(crateAnimationClose))
                animator.SetBool(crateAnimationClose, false);
            if (!string.IsNullOrWhiteSpace(crateAnimationOpen))
                animator.SetBool(crateAnimationOpen, true);
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
            if (!string.IsNullOrWhiteSpace(crateAnimationOpen))
                animator.SetBool(crateAnimationOpen, false);
            if (!string.IsNullOrWhiteSpace(crateAnimationClose))
                animator.SetBool(crateAnimationClose, true);
            PlaySound(soundOpen);
            lastOpen = false;
        }
    }

    // -----------------------------------------------------------------------------------
    // OnLoot
    // -----------------------------------------------------------------------------------
    public void OnLoot()
    {
        if (HasLoot())
            SpawnEffect(lootEffect, lootSound);
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
            {
                UpdateClient();
            }
            if (isServer)
            {
                UpdateServer();
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UpdateClient
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    protected void UpdateClient()
    {
        if (lootIndicator != null)
        {
            bool hasLoot = HasLoot();
            if (hasLoot && !lootIndicator.isPlaying)
                lootIndicator.Play();
            else if (!hasLoot && lootIndicator.isPlaying)
                lootIndicator.Stop();
        }

        if (open)
            OnOpen();
        else if (!unlocked)
            OnClose();
    }

    // -----------------------------------------------------------------------------------
    // UpdateServer
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    protected void UpdateServer()
    {
        if (EventEmptyTimeElapsed())
        {
            if (respawn)
            {
                open = false;
                Hide();
            }
            else
            {
                NetworkServer.Destroy(gameObject);
            }
        }

        if (EventRespawnTimeElapsed())
        {
            OnRefill();
            Show();
        }
    }

    // -----------------------------------------------------------------------------------
    // OnAccessed
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void OnAccessed()
    {
        emptyTimeEnd = NetworkTime.time + emptyTime;
        respawnTimeEnd = emptyTimeEnd + respawnTime;
        open = true;
    }

    // -----------------------------------------------------------------------------------
    // OnRefill
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    private void OnRefill()
    {
        inventory.Clear();

        gold = Random.Range(lootGoldMin, lootGoldMax);
        coins = Random.Range(lootCoinsMin, lootCoinsMax);

        foreach (ItemDropChance itemChance in dropChances)
            if (Random.value <= itemChance.probability)
                inventory.Add(new ItemSlot(new Item(itemChance.item)));
    }

    // -----------------------------------------------------------------------------------
    // HasLoot
    // -----------------------------------------------------------------------------------
    public bool HasLoot()
    {
        return gold > 0 || coins > 0 || inventory.Any(item => item.amount > 0);
    }

    // -----------------------------------------------------------------------------------
}

#endif
