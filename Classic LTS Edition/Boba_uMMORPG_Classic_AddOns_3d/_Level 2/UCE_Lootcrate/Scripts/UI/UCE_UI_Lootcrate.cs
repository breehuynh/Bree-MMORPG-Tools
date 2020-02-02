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

#if _iMMOCHEST

// 	UI LOOTCRATE

public partial class UCE_UI_Lootcrate : MonoBehaviour
{
    public static UCE_UI_Lootcrate singleton;
    public GameObject panel;
    public GameObject goldSlot;
    public Text goldText;
    public GameObject coinSlot;
    public Text coinText;
    public UILootSlot itemSlotPrefab;
    public Transform content;

    // -----------------------------------------------------------------------------------
    // UCE_UI_Lootcrate
    // -----------------------------------------------------------------------------------
    public UCE_UI_Lootcrate()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (panel.activeSelf &&
            player.UCE_LootcrateValidation()
             )
        {
            if (!player.UCE_selectedLootcrate.HasLoot())
            {
                Hide();
                return;
            }

            // -- gold slot
            if (player.UCE_selectedLootcrate.gold > 0)
            {
                goldSlot.SetActive(true);
                goldSlot.GetComponentInChildren<Button>().onClick.SetListener(() =>
                {
                    player.Cmd_UCE_TakeLootcrateGold();
                });
                goldText.text = player.UCE_selectedLootcrate.gold.ToString();
            }
            else goldSlot.SetActive(false);

            // -- coin slot
            if (player.UCE_selectedLootcrate.coins > 0)
            {
                coinSlot.SetActive(true);
                coinSlot.GetComponentInChildren<Button>().onClick.SetListener(() =>
                {
                    player.Cmd_UCE_TakeLootcrateCoins();
                });
                coinText.text = player.UCE_selectedLootcrate.coins.ToString();
            }
            else coinSlot.SetActive(false);

            // instantiate/destroy enough slots
            // (we only want to show the non-empty slots)
            List<ItemSlot> items = player.UCE_selectedLootcrate.inventory.Where(slot => slot.amount > 0).ToList();
            UIUtils.BalancePrefabs(itemSlotPrefab.gameObject, items.Count, content);

            // refresh all valid items
            for (int i = 0; i < items.Count; ++i)
            {
                UILootSlot slot = content.GetChild(i).GetComponent<UILootSlot>();
                slot.dragAndDropable.name = i.ToString(); // drag and drop index
                int itemIndex = player.UCE_selectedLootcrate.inventory.FindIndex(
                    // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                    itemSlot => itemSlot.amount > 0 && itemSlot.item.Equals(items[i].item)
                );

                // refresh
                slot.button.interactable = player.InventoryCanAdd(items[i].item, items[i].amount);
                slot.button.onClick.SetListener(() =>
                {
                    player.Cmd_UCE_TakeLootcrateItem(itemIndex);
                });
                slot.tooltip.text = items[i].ToolTip();
                slot.image.color = Color.white;
                slot.image.sprite = items[i].item.image;
                slot.nameText.text = items[i].item.name;
                slot.amountOverlay.SetActive(items[i].amount > 1);
                slot.amountText.text = items[i].amount.ToString();
            }
        }
        else
        {
            panel.SetActive(false);
        }
    }

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (player.UCE_LootcrateValidation() && player.UCE_selectedLootcrate.HasLoot())
            panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // Hide
    // -----------------------------------------------------------------------------------
    public void Hide(bool cancel = true)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (cancel)
            player.UCE_cancelLootcrate();

        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
}

#endif
