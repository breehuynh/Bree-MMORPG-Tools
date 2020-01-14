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

#if _iMMOHARVESTING

// RESOURCE NODE

public partial class UCE_ResourceNode : UCE_InteractableObject
{
    [Header("[-=-=-=- UCE RESOURCE NODE [See Tooltips] -=-=-=-]")]
    public ParticleSystem lootIndicator;

    [Tooltip("[Required] Base duration (in seconds) it takes to harvest")]
    [Range(1, 999)] public float harvestDuration = 2;

    [Tooltip("[Optional] Profession experience per harvest attempt")]
    public int ProfessionExperienceRewardMin = 0;

    public int ProfessionExperienceRewardMax = 2;

    [Tooltip("[Optional] Mana cost per harvest attempt")]
    public int manaCost;

    [Header("[RESOURCES]")]
    [Tooltip("[Optional] Harvested items available")]
    public UCE_HarvestingHarvestItems[] harvestItems;

    [Tooltip("[Optional] Harvested items available on critical result")]
    public UCE_HarvestingHarvestItems[] harvestCriticalItems;

    [Tooltip("[Optional] Object automatically unspawns if the amount of resources has been collected from it (set to 0 to disable)")]
    public int totalResourcesMin = 10;

    public int totalResourcesMax = 50;

    [Header("[RESPAWN]")]
    [Tooltip("[Optional] How long the node stays empty in the scene when it has been completely harvested (in Seconds)")]
    public float emptyTime = 5f;

    [Tooltip("[Optional] Will the node respawn once it got harvested? Otherwise its available only once.")]
    public bool respawn = true;

    [Tooltip("[Optional] How long it takes for a empty object to respawn (in Seconds) with full resources again. Set higher than 0 !")]
    public float respawnTime = 10f;

    [Header("[ANIMATION & SOUND]")]
    [Tooltip("[Optional] GameObject spawned as effect when successfully harvested (see ReadMe).")]
    public GameObject harvestEffect;

    [Tooltip("[Optional] Sound played when successfully harvested.")]
    public AudioClip harvestSound;

    [Header("[MESSAGES]")]
    public string experienceMessage = " Profession Exp gained.";

    public string levelUpMessage = "Profession level up: ";
    public string harvestMessage = "You harvested: ";
    public string requirementsMessage = "Harvesting Requirements not met!";
    public string depletedMessage = "No resources left!";
    public string successMessage = "Harvest successful!";
    public string criticalSuccessMessage = "Critical harvest success!";
    public string failedMessage = "Harvest Failed!";
    public string breakMessage = "Your tool broke!";
    public string boosterMessage = "You used a booster!";

    [Tooltip("[Optional] Shown while harvesting the resource node.")]
    public string accessLabel;

    [HideInInspector] public int baseSuccessFactor = 10;

    private double emptyTimeEnd;
    private double respawnTimeEnd;

    [SyncVar] private int currentResources;

    public SyncListItemSlot inventory = new SyncListItemSlot();

    // -----------------------------------------------------------------------------------
    // OnInteractClient
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public override void OnInteractClient(Player player)
    {
        player.UCE_OnSelect_ResourceNode(this);
    }

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public override void Start()
    {
        base.Start();
        currentResources = UnityEngine.Random.Range(totalResourcesMin, totalResourcesMax);
        OnRefill();
    }

    // -----------------------------------------------------------------------------------
    // FiniteStateMachineEvents
    // -----------------------------------------------------------------------------------
    public bool EventDepleted()
    {
        return (totalResourcesMax > 0 && currentResources <= 0 && !HasResources());
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    private bool EventEmptyTimeElapsed()
    {
        return !IsHidden() && currentResources <= 0 && !HasResources() && NetworkTime.time >= emptyTimeEnd;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    private bool EventRespawnTimeElapsed()
    {
        return IsHidden() && respawn && NetworkTime.time >= respawnTimeEnd;
    }

    // -----------------------------------------------------------------------------------
    // OnHarvested
    // -----------------------------------------------------------------------------------
    public void OnHarvested()
    {
        if (HasResources())
            SpawnEffect(harvestEffect, harvestSound);
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
    // -----------------------------------------------------------------------------------
    [Client]
    protected void UpdateClient()
    {
        if (lootIndicator != null)
        {
            if ((HasResources() || !IsDepleted()) && !lootIndicator.isPlaying)
                lootIndicator.Play();
            else if ((!HasResources() && IsDepleted()) && lootIndicator.isPlaying)
                lootIndicator.Stop();
        }
    }

    // -----------------------------------------------------------------------------------
    // UpdateServer
    // -----------------------------------------------------------------------------------
    [Server]
    protected void UpdateServer()
    {
        if (EventEmptyTimeElapsed())
        {
            if (respawn)
            {
                Hide();
            }
            else
            {
                NetworkServer.Destroy(gameObject);
            }
        }

        if (EventRespawnTimeElapsed())
        {
            currentResources = UnityEngine.Random.Range(totalResourcesMin, totalResourcesMax);
            OnRefill();
            Show();
        }
    }

    // -----------------------------------------------------------------------------------
    // OnDepleted
    // -----------------------------------------------------------------------------------
    [Server]
    public void OnDepleted()
    {
        emptyTimeEnd = NetworkTime.time + emptyTime;
        respawnTimeEnd = emptyTimeEnd + respawnTime;
    }

    // -----------------------------------------------------------------------------------
    // canStartHarvest
    // -----------------------------------------------------------------------------------
    public bool canStartHarvest()
    {
        return ((totalResourcesMax <= 0 || currentResources > 0) && !inventory.Any(item => item.amount > 0));
    }

    // -----------------------------------------------------------------------------------
    // RefillResources
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void OnRefill()
    {
        inventory.Clear();

        int resCount = 0;

        foreach (UCE_HarvestingHarvestItems harvestItem in harvestItems)
        {
            if (UnityEngine.Random.value <= harvestItem.probability)
            {
                int amount = (UnityEngine.Random.Range(harvestItem.minAmount, harvestItem.maxAmount));
                for (int i = 1; i <= amount; i++)
                    inventory.Add(new ItemSlot(new Item(harvestItem.template)));
                resCount += amount;

                if (resCount >= currentResources) break;
            }
        }

        currentResources -= resCount;
    }

    // -----------------------------------------------------------------------------------
    // OnCritical
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void OnCritical()
    {
        int resCount = 0;

        foreach (UCE_HarvestingHarvestItems harvestItem in harvestCriticalItems)
        {
            if (UnityEngine.Random.value <= harvestItem.probability)
            {
                int amount = (UnityEngine.Random.Range(harvestItem.minAmount, harvestItem.maxAmount));
                for (int i = 1; i <= amount; i++)
                    inventory.Add(new ItemSlot(new Item(harvestItem.template)));
                resCount += amount;

                if (resCount >= currentResources) break;
            }
        }

        currentResources -= resCount;
    }

    // -----------------------------------------------------------------------------------
    // IsDepleted
    // -----------------------------------------------------------------------------------
    public bool IsDepleted()
    {
        return currentResources <= 0 && totalResourcesMax > 0;
    }

    // -----------------------------------------------------------------------------------
    // HasResources
    // -----------------------------------------------------------------------------------
    public bool HasResources()
    {
        return inventory.Any(item => item.amount > 0);
    }

    // -----------------------------------------------------------------------------------
}

#endif
