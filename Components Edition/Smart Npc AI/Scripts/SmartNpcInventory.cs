using UnityEngine;
using Mirror;

[RequireComponent(typeof(SmartNpc))]
[DisallowMultipleComponent]
public class SmartNpcInventory : Inventory
{
    [Header("Components")]
    public SmartNpc smartNpc;

    [Header("Loot")]
    public int lootGoldMin = 0;
    public int lootGoldMax = 10;
    public ItemDropChance[] dropChances;
    public ParticleSystem lootIndicator;
    // note: Items have a .valid property that can be used to 'delete' an item.
    //       it's better than .RemoveAt() because we won't run into index-out-of
    //       range issues

    [ClientCallback]
    void Update()
    {
        // show loot indicator on clients while it still has items
        if (lootIndicator != null)
        {
            // only set active once. we don't want to reset the particle
            // system all the time.
            bool hasLoot = HasLoot();
            if (hasLoot && !lootIndicator.isPlaying)
                lootIndicator.Play();
            else if (!hasLoot && lootIndicator.isPlaying)
                lootIndicator.Stop();
        }
    }

    // other scripts need to know if it still has valid loot (to show UI etc.)
    public bool HasLoot()
    {
        // any gold or valid items?
        return smartNpc.gold > 0 || SlotsOccupied() > 0;
    }

    [Server]
    public void OnDeath()
    {
        // generate gold
        smartNpc.gold = Random.Range(lootGoldMin, lootGoldMax);

        // generate items (note: can't use Linq because of SyncList)
        foreach (ItemDropChance itemChance in dropChances)
            if (Random.value <= itemChance.probability)
                slots.Add(new ItemSlot(new Item(itemChance.item)));
    }
}
