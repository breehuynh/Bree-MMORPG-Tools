// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// ===================================================================================
// UCE ITEM DROP CLASS
// ===================================================================================
public partial class UCE_ItemDrop : UCE_InteractableObject
{
    [Tooltip("[Required] How long it takes the drop to be deleted from server (drops are not saved in the database!)")]
    [Range(1, 9999)] public double decayTime = 30f;

    [Tooltip("[Optional] Effect spawned alongside the drop, when it appears (Also add a 'Destroy After' component to effect object)")]
    public GameObject spawnEffect;

    public string takeMessage = "You picked up: ";
    public string goldMessage = "You got gold: ";
    public string failMessage = "You can't take that right now!";

    [HideInInspector] public ScriptableItem itemData;
    [SyncVar, HideInInspector] public int amount = 1;
    [SyncVar, HideInInspector] public Item item;
    [SyncVar, HideInInspector] public long gold = 0;

    protected double decayTimeEnd;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public override void Start()
    {
        base.Start();
        decayTimeEnd = NetworkTime.time + decayTime;

        // ------ create spawn effect if any
        if (spawnEffect != null)
        {
            GameObject go = Instantiate(spawnEffect.gameObject, transform.position, transform.rotation);
            NetworkServer.Spawn(go);
        }
    }

    // -----------------------------------------------------------------------------------
    // OnUpdateServer
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public override void OnUpdateServer()
    {
        if (NetworkTime.time >= decayTimeEnd)
        {
            gold = 0;
            amount = 0;
            NetworkServer.Destroy(gameObject);
        }
    }

    // -----------------------------------------------------------------------------------
    // OnInteractServer
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public override void OnInteractServer(Player player)
    {
        if (
            (amount > 0 || gold > 0) &&
            player.InventoryCanAdd(item, amount)
#if _iMMOLOOTRULES && _iMMOITEMDROP
            && UCE_ValidateTaggedLooting(player)
#endif
            )
        {
            if (gold > 0 || player.InventoryAdd(item, amount))
            {
                player.gold += gold;

                if (amount > 0)
                    player.UCE_TargetAddMessage(takeMessage + item.name);

                if (gold > 0)
                    player.UCE_TargetAddMessage(goldMessage + gold);

                // clear drop's item slot too so it can't be looted again
                // before truly destroyed
                gold = 0;
                amount = 0;

                NetworkServer.Destroy(gameObject);
            }
        }
        else
        {
            player.UCE_TargetAddMessage(failMessage);
        }
    }

    // -----------------------------------------------------------------------------------
}
