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
                    if (lastECount != player.equipment.Count)
                        for (int i = 0; i < player.equipment.Count; ++i)
                        {
                            lastECount = player.equipment.Count;
                            if (player.equipment[i].amount > 0)
                            {
                                UIEquipment equipmentContents = gameObject.GetComponent<UIEquipment>();
                                UIUtils.BalancePrefabs(equipmentContents.slotPrefab.gameObject, player.equipment.Count, equipmentContents.content);
                                if (equipmentContents.panel.activeSelf)
                                {
                                    UIEquipmentSlot slot = equipmentContents.content.transform.GetChild(i).GetComponent<UIEquipmentSlot>();
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    tooltip = slot.tooltip;
                                    slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                                    itemSlot = player.equipment[i];
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
                    if (lastICount != player.inventory.Count)
                        for (int i = 0; i < player.inventory.Count; ++i)
                        {
                            lastICount = player.inventory.Count;
                            if (player.inventory[i].amount > 0)
                            {
                                UIInventory inventoryContents = GetComponent<UIInventory>();
                                UIUtils.BalancePrefabs(inventoryContents.slotPrefab.gameObject, player.inventory.Count, inventoryContents.content);
                                if (inventoryContents.panel.activeSelf)
                                {
                                    UIInventorySlot slot = inventoryContents.content.transform.GetChild(i).GetComponent<UIInventorySlot>();
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    tooltip = slot.tooltip;
                                    slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                                    itemSlot = player.inventory[i];
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
                    if (player.target != null && player.target.health <= 0)
                    {
                        UILoot lootContent = GetComponent<UILoot>();
                        List<ItemSlot> items = player.target.inventory.Where(slot => slot.amount > 0).ToList();
                        UIUtils.BalancePrefabs(lootContent.itemSlotPrefab.gameObject, items.Count, lootContent.content);

                        // refresh all valid items
                        for (int i = 0; i < items.Count; ++i)
                        {
                            UILootSlot slot = lootContent.content.GetChild(i).GetComponent<UILootSlot>();
                            slot.dragAndDropable.name = i.ToString(); // drag and drop index
                            int itemIndex = player.target.inventory.FindIndex(
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
                        if (lastPTYCount != player.tradeOfferItems.Count)
                            for (int i = 0; i < player.tradeOfferItems.Count; ++i)
                            {
                                lastPTYCount = player.tradeOfferItems.Count;
                                UIPlayerTrading tradeContents = GetComponent<UIPlayerTrading>();
                                UIUtils.BalancePrefabs(tradeContents.slotPrefab.gameObject, player.tradeOfferItems.Count, tradeContents.myContent);
                                if (tradeContents.panel.activeSelf)
                                {
                                    UIPlayerTradingSlot slot = tradeContents.myContent.transform.GetChild(i).GetComponent<UIPlayerTradingSlot>();
                                    if (slot.amountText.text != "0")
                                    {
                                        raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                        tooltip = slot.tooltip;
                                        slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                                        int inventoryIndex = player.tradeOfferItems[i];
                                        itemSlot = player.inventory[inventoryIndex];
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
                        if (lastPTOCount != other.tradeOfferItems.Count)
                            for (int i = 0; i < other.tradeOfferItems.Count; ++i)
                            {
                                lastPTOCount = other.tradeOfferItems.Count;
                                UIPlayerTrading tradeContents = GetComponent<UIPlayerTrading>();
                                UIUtils.BalancePrefabs(tradeContents.slotPrefab.gameObject, other.tradeOfferItems.Count, tradeContents.otherContent);
                                if (tradeContents.panel.activeSelf)
                                {
                                    UIPlayerTradingSlot slot = tradeContents.otherContent.transform.GetChild(i).GetComponent<UIPlayerTradingSlot>();
                                    if (slot.amountText.text != "0")
                                    {
                                        raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                        tooltip = slot.tooltip;
                                        slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                                        int inventoryIndex = other.tradeOfferItems[i];
                                        itemSlot = other.inventory[inventoryIndex];
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
                            ScriptableItem[] items = npc.saleItems.Where(x => x.itemCategory == shopContents.currentCategory || shopContents.currentCategory == "").ToArray();
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
                        if (lastNTCount != npc.saleItems.Length)
                            for (int i = 0; i < npc.saleItems.Length; ++i)
                            {
                                lastNTCount = npc.saleItems.Length;
                                UINpcTrading npcContents = GetComponent<UINpcTrading>();
                                UIUtils.BalancePrefabs(npcContents.slotPrefab.gameObject, npc.saleItems.Length, npcContents.content);
                                if (npcContents.panel.activeSelf)
                                {
                                    UINpcTradingSlot slot = npcContents.content.transform.GetChild(i).GetComponent<UINpcTradingSlot>();
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    tooltip = slot.tooltip;
                                    scriptItem = npc.saleItems[i];
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
                        ScriptableItem[] items = player.itemMallCategories[mallContents.currentCategory].items;
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
                    UIUtils.BalancePrefabs(craftContents.ingredientSlotPrefab.gameObject, player.craftingIndices.Count, craftContents.ingredientContent);
                    if (craftContents.panel.activeSelf)
                    {
                        int lastCCount = 0;
                        if (lastCCount != player.craftingIndices.Count)
                            for (int i = 0; i < player.craftingIndices.Count; ++i)
                            {
                                lastCCount = player.craftingIndices.Count;
                                UICraftingIngredientSlot slot = craftContents.ingredientContent.GetChild(i).GetComponent<UICraftingIngredientSlot>();
                                if (player.craftingIndices[i] != -1)
                                {
                                    int itemIndex = player.craftingIndices[i];
                                    raritySlot = slot.GetComponent<UCE_RaritySlot>();
                                    tooltip = slot.tooltip;
                                    itemSlot = player.inventory[itemIndex];
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
