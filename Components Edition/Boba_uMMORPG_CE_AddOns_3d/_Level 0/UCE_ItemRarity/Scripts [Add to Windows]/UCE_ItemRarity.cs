// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UCE_ItemRarity : MonoBehaviour
{
    public enum SlotType { Equipment, Inventory, Loot, PlayerTrade, NpcTrade, ItemMall, Crafting }

    public SlotType slotType;

    private int selfIndex, targetIndex;
    private Player player;
    private ItemSlot itemSlot;
    private ScriptableItem scriptItem;
    private UCE_RaritySlot raritySlot;
    private UIShowToolTip tooltip;

    // Assign our componenet based on the slot type.
    private void LateUpdate()
    {
        player = Player.localPlayer;

        if (player != null)
            switch (slotType)
            {
                case SlotType.Equipment:
                    // refresh all
                    int lastECount = 0;
                    if (lastECount != player.equipment.slots.Count)
                        for (int i = 0; i < player.equipment.slots.Count; ++i)
                        {
                            lastECount = player.equipment.slots.Count;
                            if (player.equipment.slots[i].amount > 0)
                            {
                                UIEquipment equipmentContents = gameObject.GetComponent<UIEquipment>();
                                UIUtils.BalancePrefabs(equipmentContents.slotPrefab.gameObject, player.equipment.slots.Count, equipmentContents.content);
                                if (equipmentContents.panel.activeSelf)
                                {
                                    UIEquipmentSlot slot = equipmentContents.content.transform.GetChild(i).GetComponent<UIEquipmentSlot>();
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    tooltip = slot.tooltip;
                                    slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                                    itemSlot = player.equipment.slots[i];
                                    SetRarityColor(itemSlot.item.data);
                                }
                            }
                            else
                            {
                                UIEquipment equipmentContents = gameObject.GetComponent<UIEquipment>();
                                if (equipmentContents.panel.activeSelf)
                                {
                                    UIEquipmentSlot slot = equipmentContents.content.transform.GetChild(i).GetComponent<UIEquipmentSlot>();
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    raritySlot.rarityOutline.color = Color.clear;
                                }
                            }
                        }
                    break;

                case SlotType.Inventory:
                    // refresh all
                    int lastICount = 0;
                    if (lastICount != player.inventory.slots.Count)
                        for (int i = 0; i < player.inventory.slots.Count; ++i)
                        {
                            lastICount = player.inventory.slots.Count;
                            if (player.inventory.slots[i].amount > 0)
                            {
                                UIInventory inventoryContents = GetComponent<UIInventory>();
                                UIUtils.BalancePrefabs(inventoryContents.slotPrefab.gameObject, player.inventory.slots.Count, inventoryContents.content);
                                if (inventoryContents.panel.activeSelf)
                                {
                                    UIInventorySlot slot = inventoryContents.content.transform.GetChild(i).GetComponent<UIInventorySlot>();
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    tooltip = slot.tooltip;
                                    slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                                    itemSlot = player.inventory.slots[i];
                                    SetRarityColor(itemSlot.item.data);
                                }
                            }
                            else
                            {
                                UIInventory inventoryContents = gameObject.GetComponent<UIInventory>();
                                if (inventoryContents.panel.activeSelf)
                                {
                                    UIInventorySlot slot = inventoryContents.content.transform.GetChild(i).GetComponent<UIInventorySlot>();
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    raritySlot.rarityOutline.color = Color.clear;
                                }
                            }
                        }
                    break;

                case SlotType.Loot:
                    if (player.target != null && player.target.health.current <= 0 && player.target is Player)
                    {
                        UILoot lootContent = GetComponent<UILoot>();
                        List<ItemSlot> items = ((Player) (player.target)).inventory.slots.Where(slot => slot.amount > 0).ToList();
                        UIUtils.BalancePrefabs(lootContent.itemSlotPrefab.gameObject, items.Count, lootContent.content);

                        // refresh all valid items
                        for (int i = 0; i < items.Count; ++i)
                        {
                            UILootSlot slot = lootContent.content.GetChild(i).GetComponent<UILootSlot>();
                            slot.dragAndDropable.name = i.ToString(); // drag and drop index
                            int itemIndex = ((Player)(player.target)).inventory.slots.FindIndex(
                                // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                                itemSlot => itemSlot.amount > 0 && itemSlot.item.Equals(items[i].item)
                            );

                            // refresh
                            raritySlot = slot.GetComponent<UCE_RaritySlot>();
                            tooltip = slot.tooltip;
                            slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                            itemSlot = items[i];
                            SetRarityColor(itemSlot.item.data);
                        }
                    }
                    break;

                case SlotType.PlayerTrade:
                    if (player.state == "TRADING")
                    {
                        Player other = (Player)player.target;
                        int lastPTYCount = 0;
                        if (lastPTYCount != player.trading.offerItems.Count)
                            for (int i = 0; i < player.trading.offerItems.Count; ++i)
                            {
                                lastPTYCount = player.trading.offerItems.Count;
                                UIPlayerTrading tradeContents = GetComponent<UIPlayerTrading>();
                                UIUtils.BalancePrefabs(tradeContents.slotPrefab.gameObject, player.trading.offerItems.Count, tradeContents.myContent);
                                if (tradeContents.panel.activeSelf)
                                {
                                    UIPlayerTradingSlot slot = tradeContents.myContent.transform.GetChild(i).GetComponent<UIPlayerTradingSlot>();
                                    if (slot.amountText.text != "0")
                                    {
                                        raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                        tooltip = slot.tooltip;
                                        slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                                        int inventoryIndex = player.trading.offerItems[i];
                                        itemSlot = player.inventory.slots[inventoryIndex];
                                        SetRarityColor(itemSlot.item.data);
                                    }
                                    else
                                    {
                                        raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                        raritySlot.rarityOutline.color = Color.clear;
                                    }
                                }
                            }

                        int lastPTOCount = 0;
                        if (lastPTOCount != other.trading.offerItems.Count)
                            for (int i = 0; i < other.trading.offerItems.Count; ++i)
                            {
                                lastPTOCount = other.trading.offerItems.Count;
                                UIPlayerTrading tradeContents = GetComponent<UIPlayerTrading>();
                                UIUtils.BalancePrefabs(tradeContents.slotPrefab.gameObject, other.trading.offerItems.Count, tradeContents.otherContent);
                                if (tradeContents.panel.activeSelf)
                                {
                                    UIPlayerTradingSlot slot = tradeContents.otherContent.transform.GetChild(i).GetComponent<UIPlayerTradingSlot>();
                                    if (slot.amountText.text != "0")
                                    {
                                        raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                        tooltip = slot.tooltip;
                                        slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                                        int inventoryIndex = other.trading.offerItems[i];
                                        itemSlot = other.inventory.slots[inventoryIndex];
                                        SetRarityColor(itemSlot.item.data);
                                    }
                                    else
                                    {
                                        raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                        raritySlot.rarityOutline.color = Color.clear;
                                    }
                                }
                            }
                    }
                    break;

                case SlotType.NpcTrade:
                    if (player.target is Npc)
                    {
                        Npc npc = (Npc)player.target;
#if _iMMONPCSHOP
                        UCE_UI_NpcShop shopContents = GetComponent<UCE_UI_NpcShop>();
                        if (shopContents.panel.activeSelf)
                        {
                            ScriptableItem[] items = npc.trading.saleItems.Where(x => x.itemCategory == shopContents.currentCategory || shopContents.currentCategory == "").ToArray();
                            UIUtils.BalancePrefabs(shopContents.itemSlotPrefab.gameObject, items.Length, shopContents.itemContent);

                            int lastIMCount = 0;
                            string currentPage = "";
                            if (lastIMCount != items.Length || currentPage != shopContents.currentCategory)
                                for (int i = 0; i < items.Length; ++i)
                                {
                                    lastIMCount = items.Length;
                                    currentPage = shopContents.currentCategory;

                                    UCE_UI_NpcShopSlot slot = shopContents.itemContent.GetChild(i).GetComponent<UCE_UI_NpcShopSlot>();
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    tooltip = slot.tooltip;
                                    scriptItem = items[i];
                                    SetRarityColor(scriptItem);
                                }
                        }
#else
                        int lastNTCount = 0;
                        if (lastNTCount != npc.trading.saleItems.Length)
                            for (int i = 0; i < npc.trading.saleItems.Length; ++i)
                            {
                                lastNTCount = npc.trading.saleItems.Length;
                                UINpcTrading npcContents = GetComponent<UINpcTrading>();
                                UIUtils.BalancePrefabs(npcContents.slotPrefab.gameObject, npc.trading.saleItems.Length, npcContents.content);
                                if (npcContents.panel.activeSelf)
                                {
                                    UINpcTradingSlot slot = npcContents.content.transform.GetChild(i).GetComponent<UINpcTradingSlot>();
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    tooltip = slot.tooltip;
                                    scriptItem = npc.trading.saleItems[i];
                                    SetRarityColor(scriptItem);
                                }
                            }
#endif
                    }
                    break;

                case SlotType.ItemMall:
                    UIItemMall mallContents = GetComponent<UIItemMall>();
                    if (mallContents.panel.activeSelf)
                    {
                        ScriptableItem[] items = player.itemMall.config.categories[mallContents.currentCategory].items;
                        UIUtils.BalancePrefabs(mallContents.itemSlotPrefab.gameObject, items.Length, mallContents.itemContent);

                        int lastIMCount = 0;
                        int currentPage = 0;
                        if (lastIMCount != items.Length || currentPage != mallContents.currentCategory)
                            for (int i = 0; i < items.Length; ++i)
                            {
                                lastIMCount = items.Length;
                                currentPage = mallContents.currentCategory;
                                UIItemMallSlot slot = mallContents.itemContent.GetChild(i).GetComponent<UIItemMallSlot>();
                                raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                tooltip = slot.tooltip;
                                scriptItem = items[i];
                                SetRarityColor(scriptItem);
                            }
                    }
                    break;

                case SlotType.Crafting:
                    UICrafting craftContents = GetComponent<UICrafting>();
                    UIUtils.BalancePrefabs(craftContents.ingredientSlotPrefab.gameObject, player.crafting.indices.Count, craftContents.ingredientContent);
                    if (craftContents.panel.activeSelf)
                    {
                        int lastCCount = 0;
                        if (lastCCount != player.crafting.indices.Count)
                            for (int i = 0; i < player.crafting.indices.Count; ++i)
                            {
                                lastCCount = player.crafting.indices.Count;
                                UICraftingIngredientSlot slot = craftContents.ingredientContent.GetChild(i).GetComponent<UICraftingIngredientSlot>();
                                if (player.crafting.indices[i] != -1)
                                {
                                    int itemIndex = player.crafting.indices[i];
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    tooltip = slot.tooltip;
                                    itemSlot = player.inventory.slots[itemIndex];
                                    SetRarityColor(itemSlot.item.data);
                                }
                                else
                                {
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    raritySlot.rarityOutline.color = Color.clear;
                                }
                            }
                    }
                    break;
            }
    }

    // Set our rarity outline color from scriptable item.
    private void SetRarityColor(ScriptableItem slot)
    {
        switch (slot.rarity)
        {
            case ScriptableItem.ItemRarity.Poor:
                raritySlot.rarityOutline.color = Color.grey;
                break;

            case ScriptableItem.ItemRarity.Common:
                raritySlot.rarityOutline.color = Color.white;
                break;

            case ScriptableItem.ItemRarity.Uncommon:
                raritySlot.rarityOutline.color = Color.green;
                break;

            case ScriptableItem.ItemRarity.Rare:
                raritySlot.rarityOutline.color = Color.blue;
                break;

            case ScriptableItem.ItemRarity.Epic:
                raritySlot.rarityOutline.color = Color.magenta;
                break;

            case ScriptableItem.ItemRarity.Legendary:
                raritySlot.rarityOutline.color = Color.yellow;
                break;
        }
    }
}

// Setup our item rarity enum so they can be set from the scriptable item.
public partial class ScriptableItem
{
    public enum ItemRarity { Poor, Common, Uncommon, Rare, Epic, Legendary }

    public ItemRarity rarity = ItemRarity.Poor;
}

// Setup our item rarity tooltip so the item name is the same color as border.
public partial struct Item
{
    private void ToolTip_ItemRarity(StringBuilder tip)
    {
        string rarityName = "";
        switch (data.rarity)
        {
            case ScriptableItem.ItemRarity.Poor:
                rarityName = "<b><color=#" + ColorUtility.ToHtmlStringRGBA(Color.grey) + ">" + name + "</color></b>";
                break;

            case ScriptableItem.ItemRarity.Common:
                rarityName = "<b><color=#" + ColorUtility.ToHtmlStringRGBA(Color.white) + ">" + name + "</color></b>";
                break;

            case ScriptableItem.ItemRarity.Uncommon:
                rarityName = "<b><color=#" + ColorUtility.ToHtmlStringRGBA(Color.green) + ">" + name + "</color></b>";
                break;

            case ScriptableItem.ItemRarity.Rare:
                rarityName = "<b><color=#" + ColorUtility.ToHtmlStringRGBA(Color.blue) + ">" + name + "</color></b>";
                break;

            case ScriptableItem.ItemRarity.Epic:
                rarityName = "<b><color=#" + ColorUtility.ToHtmlStringRGBA(Color.magenta) + ">" + name + "</color></b>";
                break;

            case ScriptableItem.ItemRarity.Legendary:
                rarityName = "<b><color=#" + ColorUtility.ToHtmlStringRGBA(Color.yellow) + ">" + name + "</color></b>";
                break;
        }

        tip.Replace(name, rarityName);
    }
}
