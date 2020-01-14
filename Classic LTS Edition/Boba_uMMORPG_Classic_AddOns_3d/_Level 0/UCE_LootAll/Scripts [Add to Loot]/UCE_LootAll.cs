// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

public partial class UCE_LootAll : MonoBehaviour
{
    public Button lootAllBtn;
    private UILoot loot;
    private int invFull;

    private void Start()
    {
        loot = GetComponent<UILoot>();
    }

    public void LootAll()
    {
        Player player = Player.localPlayer;

        var items = player.target.inventory.Where(item => item.amount > 0).ToList();
        invFull = 0;
        // refresh all valid items
        for (int i = 0; i < items.Count; ++i)
        {
            if (loot.content.childCount > 0)
            {
                UILootSlot slot = loot.content.GetChild(i).GetComponent<UILootSlot>();
                slot.dragAndDropable.name = i.ToString(); // drag and drop index
                                                          // int itemIndex = player.target.inventory.FindIndex(item => item.amount > 0 && item.item.name == items[i].item.name);
                                                          // add a check for each item (we cannot loot all if we dont have enough space in our inventory
                if (player.InventoryCanAdd(items[i].item, items[i].amount)) { invFull++; }
                else { invFull--; }

                if (invFull == items.Count) { lootAllBtn.interactable = true; }
                else { lootAllBtn.interactable = false; }

                lootAllBtn.onClick.RemoveAllListeners();
                lootAllBtn.onClick.SetListener(() => { player.TakeAllLootItem(); });
            }
        }
    }
}
