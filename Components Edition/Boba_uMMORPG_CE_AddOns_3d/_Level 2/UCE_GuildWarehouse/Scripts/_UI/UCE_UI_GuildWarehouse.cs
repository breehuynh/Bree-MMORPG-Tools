// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// =======================================================================================

using UnityEngine;
using UnityEngine.UI;

// =======================================================================================
// UCE UI GUILD WAREHOUSE
// =======================================================================================
public class UCE_UI_GuildWarehouse : MonoBehaviour
{
    public GameObject panel;
    public UCE_UI_GuildWarehouseUpgradePanel upgradePanel;
    public UCE_UI_GuildWarehouseSlot slotPrefab;
    public Transform content;
    public GameObject goldInOutPanel;
    public Button buttonUpgrade;
    public Button buttonDeposit;
    public Button buttonWithdrawal;
    public InputField goldInputPanel;
    public Text goldTextPlaceholder;
    public Button buttonAction;
    public Text levelText;
    public Text goldText;
    public Text goldInventoryText;

    [Header("[LABELS]")]
    public string depositLabel = "Deposit gold:";
    public string withdrawLabel = "Withdraw gold:";
    public string levelLabel = "L";

    private ColorBlock btnDColor;
    private ColorBlock btnWColor;

    private int bdc = 0;
    private int bwc = 0;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (!player.guild.InGuild())
            Hide();

        // only update the panel if it's active
        if (panel.activeSelf)
        {
            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(slotPrefab.gameObject, player.UCE_guildWarehouse.Count, content);

            // refresh all items
            for (int i = 0; i < player.UCE_guildWarehouse.Count; ++i)
            {
                UCE_UI_GuildWarehouseSlot slot = content.GetChild(i).GetComponent<UCE_UI_GuildWarehouseSlot>();
                slot.dragAndDropable.name = i.ToString(); // drag and drop index
                ItemSlot itemSlot = player.UCE_guildWarehouse[i];

                if (itemSlot.amount > 0)
                {
                    slot.tooltip.enabled = true;
                    slot.tooltip.text = itemSlot.ToolTip();
                    slot.dragAndDropable.dragable = player.UCE_CanAccessGuildWarehouse();
                    slot.image.color = Color.white;
                    slot.image.sprite = itemSlot.item.image;
                    slot.amountOverlay.SetActive(itemSlot.amount > 1);
                    slot.amountText.text = itemSlot.amount.ToString();
                }
                else
                {
                    slot.button.onClick.RemoveAllListeners();
                    slot.tooltip.enabled = false;
                    slot.dragAndDropable.dragable = false;
                    slot.image.color = Color.clear;
                    slot.image.sprite = null;
                    slot.amountOverlay.SetActive(false);
                }
            }

            // ----- level
            levelText.text = levelLabel + (player.guildWarehouseLevel + 1).ToString();

            // ----- gold
            goldText.text = player.guildWarehouseGold.ToString() + "/" + player.guildWarehouseStorageGold;

            buttonUpgrade.interactable = player.UCE_CanUpgradeGuildWarehouse();
            buttonDeposit.interactable = player.UCE_CanAccessGuildWarehouse() && player.gold > 0 && player.UCE_HasEnoughGoldSpace();
            buttonWithdrawal.interactable = player.UCE_CanAccessGuildWarehouse() && player.guildWarehouseGold > 0;
            goldInputPanel.interactable = player.UCE_CanAccessGuildWarehouse() && (player.gold > 0 || player.guildWarehouseGold > 0);

            buttonUpgrade.onClick.SetListener(() =>
            {
                upgradePanel.Show();
            });

            buttonDeposit.onClick.SetListener(() =>
            {
                if (goldInOutPanel.activeInHierarchy)
                {
                    goldInOutPanel.SetActive(false);
                }

                bdc = (bdc > 0 ? 0 : 1);
                bwc = 0;

                if (bdc == 1)
                {
                    goldTextPlaceholder.text = depositLabel;
                    goldInOutPanel.SetActive(true);
                    goldInputPanel.ActivateInputField();
                }
            });

            buttonWithdrawal.onClick.SetListener(() =>
            {
                if (goldInOutPanel.activeInHierarchy)
                {
                    goldInOutPanel.SetActive(false);
                }

                bwc = (bwc > 0 ? 0 : 1);
                bdc = 0;

                if (bwc == 1)
                {
                    goldTextPlaceholder.text = withdrawLabel;
                    goldInOutPanel.SetActive(true);
                    goldInputPanel.ActivateInputField();
                }
            });

            buttonAction.onClick.SetListener(() =>
            {
                string amountValue = goldInputPanel.text;

                if (!string.IsNullOrWhiteSpace(amountValue))
                {
                    int amount = int.Parse(amountValue);

                    if ((bdc != 1 || bwc != 1) && amount < 1) return;

                    if (bdc == 1)
                    {
                        player.Cmd_UCE_DepositGold(amount);
                    }

                    if (bwc == 1)
                    {
                        player.Cmd_UCE_WithdrawGold(amount);
                    }

                    goldText.text = player.guildWarehouseGold.ToString();
                    goldInventoryText.text = player.gold.ToString();

                    bdc = 0;
                    bwc = 0;
                }

                goldInputPanel.text = "";

                goldInOutPanel.SetActive(false);
            });
        }
        else
        {
            Hide();
        }
    }

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (!panel.activeSelf)
        {
            player.Cmd_UCE_LoadGuildWarehouse();
            panel.SetActive(true);
        }
    }

    // -----------------------------------------------------------------------------------
    // Hide
    // -----------------------------------------------------------------------------------
    public void Hide()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (panel.activeSelf)
        {
            if (player.guild.InGuild())
                player.Cmd_UCE_UnaccessGuildWarehouse(player.guild.name);
            panel.SetActive(false);
        }
    }

    // -----------------------------------------------------------------------------------
}

// =======================================================================================