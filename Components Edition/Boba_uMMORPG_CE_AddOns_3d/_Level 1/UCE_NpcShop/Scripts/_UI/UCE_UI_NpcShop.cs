// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// UCE UI NPC SHOP

public partial class UCE_UI_NpcShop : MonoBehaviour
{
    public UCE_UI_NpcShop singleton;
    public GameObject panel;
    public Button categorySlotPrefab;
    public Button buttonSwitchMode;
    public Transform categoryContent;
    public ScrollRect scrollRect;
    public UCE_UI_NpcShopSlot itemSlotPrefab;
    public Transform itemContent;
    public Text currencyAmountText;
    public GameObject inventoryPanel;

    public string emptyCategoryLabel = "(All)";
    public string sellText = "Sell";
    public string buyText = "Buy";
    public string buyAmountText = "How many do you want to buy?";
    public string sellAmountText = "How many do you want to sell?";

    [HideInInspector] public string currentCategory = "";

    private UCE_UI_InputCallback instance;

    private bool sellMode = false;
#pragma warning disable
    private int chosenAmount = 0;
#pragma warning restore

    public UCE_UI_NpcShop()
    {
        singleton = this;
    }

    protected ScriptableItem[] saleItems;

    // -----------------------------------------------------------------------------------
    // ScrollToBeginning
    // -----------------------------------------------------------------------------------
    private void ScrollToBeginning()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1;
    }

    // -----------------------------------------------------------------------------------
    // ClearContent
    // -----------------------------------------------------------------------------------
    private void ClearContent()
    {
        for (int i = 0; i < itemContent.childCount; ++i)
        {
            Destroy(itemContent.GetChild(i).gameObject);
        }
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (instance == null)
            instance = this.GetComponent<UCE_UI_InputCallback>();

        if (player.target != null && player.target is Npc &&
            Utils.ClosestDistance(player, player.target) <= player.interactionRange)
        {
            Npc npc = (Npc)player.target;

            // --------------------------------------------------------------------------- Check Confirmation

            if (instance != null && instance.confirmed && instance.chosenAmount > 0 && instance.selectedID > -1)
            {
                if (sellMode)
                {
                    ConfirmSell(instance.chosenAmount, instance.selectedID);
                }
                else
                {
                    ConfirmBuy(instance.chosenAmount, instance.selectedID);
                }

                instance.Reset();
            }

            // --------------------------------------------------------------------------- Refresh Sale Items

            List<ScriptableItem> saleItems = new List<ScriptableItem>();
            saleItems.AddRange(npc.trading.saleItems);

            if (npc.shopContent.scaleContentWithPlayer)
            {
                saleItems.Clear();

                int idx = Mathf.Max((Mathf.RoundToInt(player.level.current / (player.level.max / npc.shopContent.levelSaleItems.Length)) - 1), 0);

                if (npc.shopContent.showAllValidContent)
                {
                    for (int i = 0; i <= idx; ++i)
                        saleItems.AddRange(npc.shopContent.levelSaleItems[i].saleItems);
                }
                else
                {
                    if (npc.shopContent.levelSaleItems.Length > 0)
                        saleItems.AddRange(npc.shopContent.levelSaleItems[idx].saleItems);
                }
            }
            else
            {
                if (npc.shopContent.levelSaleItems.Length > 0 && npc.shopContent.levelSaleItems.Length >= npc.level.current)
                {
                    saleItems.Clear();
                    saleItems.AddRange(npc.shopContent.levelSaleItems[npc.level.current - 1].saleItems);
                }
            }

            // instantiate/destroy enough category slots
            UIUtils.BalancePrefabs(categorySlotPrefab.gameObject, npc.shopContent.shopCategories.Length, categoryContent);

            // --------------------------------------------------------------------------- Refresh Mode Button
            if (sellMode)
            {
                buttonSwitchMode.GetComponentInChildren<Text>().text = buyText;
                buttonSwitchMode.onClick.SetListener(() =>
                {
                    ClearContent();
                    sellMode = false;
                });
            }
            else
            {
                buttonSwitchMode.GetComponentInChildren<Text>().text = sellText;
                buttonSwitchMode.onClick.SetListener(() =>
                {
                    ClearContent();
                    sellMode = true;
                });
            }

            // --------------------------------------------------------------------------- Refresh Category Buttons
            for (int i = 0; i < npc.shopContent.shopCategories.Length; ++i)
            {
                Button button = categoryContent.GetChild(i).GetComponent<Button>();
                button.interactable = npc.shopContent.shopCategories[i] != currentCategory;

                if (npc.shopContent.shopCategories[i] == "")
                    button.GetComponentInChildren<Text>().text = emptyCategoryLabel;
                else
                    button.GetComponentInChildren<Text>().text = npc.shopContent.shopCategories[i];

                int icopy = i; // needed for lambdas, otherwise i is Count
                button.onClick.SetListener(() =>
                {
                    currentCategory = npc.shopContent.shopCategories[icopy];
                    ScrollToBeginning();
                });
            }

            // --------------------------------------------------------------------------- Update Content (Buy Mode)
            if (!sellMode && npc.shopContent.shopCategories.Length > 0)
            {
                // instantiate/destroy enough item slots for that category
                ScriptableItem[] items = saleItems.Where(x => x.itemCategory == currentCategory || currentCategory == "").ToArray();
                UIUtils.BalancePrefabs(itemSlotPrefab.gameObject, items.Length, itemContent);

                // refresh all items in that category
                for (int i = 0; i < items.Length; ++i)
                {
                    UCE_UI_NpcShopSlot slot = itemContent.GetChild(i).GetComponent<UCE_UI_NpcShopSlot>();
                    ScriptableItem itemData = items[i];

                    slot.tooltip.text = new Item(itemData).ToolTip();
                    slot.image.color = Color.white;
                    slot.image.sprite = itemData.image;
                    slot.nameText.text = itemData.name;
                    slot.priceText.text = itemData.buyPrice.ToString();
                    slot.buyText.text = buyText;
                    slot.buyButton.interactable = player.isAlive && player.gold >= itemData.buyPrice;
                    int icopy = i; // needed for lambdas, otherwise i is Count
                    int maxBuy = MaxBuyAmount(player, itemData);
                    slot.buyButton.onClick.SetListener(() =>
                    {
                        if (maxBuy == 1)
                            ConfirmBuy(1, icopy);
                        else
                            instance.Show(buyAmountText, 1, maxBuy, icopy);
                    });
                }
            }

            // --------------------------------------------------------------------------- Update Content (Sell Mode)
            if (sellMode && npc.shopContent.shopCategories.Length > 0)
            {
                int j = 0;
                int itemCount = player.inventory.slots.Count(x => x.amount > 0 && x.item.data.sellable && (x.item.data.itemCategory == currentCategory || currentCategory == ""));
                UIUtils.BalancePrefabs(itemSlotPrefab.gameObject, itemCount, itemContent);

                // refresh all items in that category
                for (int i = 0; i < player.inventory.slots.Count; ++i)
                {
                    if (player.inventory.slots[i].amount > 0 && (player.inventory.slots[i].item.data.sellable && (player.inventory.slots[i].item.data.itemCategory == currentCategory || currentCategory == "")))
                    {
                        UCE_UI_NpcShopSlot slot = itemContent.GetChild(j).GetComponent<UCE_UI_NpcShopSlot>();
                        ScriptableItem itemData = player.inventory.slots[i].item.data;

                        slot.tooltip.text = new Item(itemData).ToolTip();
                        slot.image.color = Color.white;
                        slot.image.sprite = itemData.image;
                        slot.nameText.text = itemData.name + " x" + player.inventory.slots[i].amount;
                        slot.priceText.text = itemData.sellPrice.ToString();
                        slot.buyText.text = sellText;
                        slot.buyButton.interactable = player.isAlive && player.inventory.slots[i].amount > 0;
                        int icopy = i; // needed for lambdas, otherwise i is Count
                        slot.buyButton.onClick.SetListener(() =>
                        {
                            if (player.inventory.slots[icopy].amount == 1)
                                ConfirmSell(1, icopy);
                            else
                                instance.Show(sellAmountText, 1, player.inventory.slots[icopy].amount, icopy);
                            ClearContent();
                            inventoryPanel.SetActive(true); // better feedback
                        });
                        j++;
                    }
                }
            }

            // --------------------------------------------------------------------------- Update Overview
            currencyAmountText.text = player.gold.ToString();
        }
        else
        {
            if (instance != null)
                instance.Reset();
            panel.SetActive(false);
        }
    }

    // -----------------------------------------------------------------------------------
    // ConfirmBuy
    // -----------------------------------------------------------------------------------
    private void ConfirmBuy(int amount, int id)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        player.Cmd_UCE_NpcBuyItem(id, currentCategory, amount);
        inventoryPanel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // ConfirmSell
    // -----------------------------------------------------------------------------------
    private void ConfirmSell(int amount, int id)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        player.npcTrading.CmdSellItem(id, amount);
    }

    // -----------------------------------------------------------------------------------
    // MaxBuyAmount
    // -----------------------------------------------------------------------------------
    private int MaxBuyAmount(Player player, ScriptableItem item)
    {
        if (item.maxStack == 1) return 1;

        return Mathf.Min((int)(player.gold / item.buyPrice), item.maxStack);
    }

    // -----------------------------------------------------------------------------------
}
