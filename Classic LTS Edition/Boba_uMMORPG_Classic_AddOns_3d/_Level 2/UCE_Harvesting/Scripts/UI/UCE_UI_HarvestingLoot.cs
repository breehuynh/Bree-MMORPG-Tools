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

#if _iMMOHARVESTING

// 	UI HARVESTING

public partial class UCE_UI_HarvestingLoot : MonoBehaviour
{
    public static UCE_UI_HarvestingLoot singleton;
    public GameObject panel;
    public UILootSlot itemSlotPrefab;
    public Transform content;

    // -----------------------------------------------------------------------------------
    // UCE_UI_HarvestingLoot
    // -----------------------------------------------------------------------------------
    public UCE_UI_HarvestingLoot()
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
            player.UCE_ResourceNodeValidation()
            )
        {
            if (player.UCE_selectedResourceNode == null || !player.UCE_ResourceNodeValidation() || !player.UCE_selectedResourceNode.HasResources())
            {
                Hide();
                return;
            }

            List<ItemSlot> items = player.UCE_selectedResourceNode.inventory.Where(slot => slot.amount > 0).ToList();
            UIUtils.BalancePrefabs(itemSlotPrefab.gameObject, items.Count, content);

            for (int i = 0; i < items.Count; ++i)
            {
                UILootSlot slot = content.GetChild(i).GetComponent<UILootSlot>();
                slot.dragAndDropable.name = i.ToString(); // drag and drop index
                int itemIndex = player.UCE_selectedResourceNode.inventory.FindIndex(
                    itemSlot => itemSlot.amount > 0 && itemSlot.item.Equals(items[i].item)
                );

                slot.button.interactable = player.InventoryCanAdd(items[i].item, items[i].amount);
                slot.button.onClick.SetListener(() =>
                {
                    player.Cmd_UCE_TakeHarvestingResources(itemIndex);
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

        if (player.UCE_ResourceNodeValidation())
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
            player.UCE_cancelResourceNode();

        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
}

#endif
