// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UCE UI PLAYER WAREHOUSE

public class UCE_UI_PlayerWarehouse : MonoBehaviour
{
    public static UCE_UI_PlayerWarehouse singleton;

    public GameObject panel;
    public UCE_UI_PlayerWarehouseUpgradePanel upgradePanel;
    public UCE_UI_WarehouseSlot slotPrefab;
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
    // Awake
    // -----------------------------------------------------------------------------------
    public void Awake()
    {
        if (singleton == null) singleton = this;
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;

        if (!player) return;

        if (panel.activeSelf)
        {
            UIUtils.BalancePrefabs(slotPrefab.gameObject, player.UCE_playerWarehouse.Count, content);

            for (int i = 0; i < player.UCE_playerWarehouse.Count; ++i)
            {
                UCE_UI_WarehouseSlot slot = content.GetChild(i).GetComponent<UCE_UI_WarehouseSlot>();
                slot.dragAndDropable.name = i.ToString(); // drag and drop index
                ItemSlot itemSlot = player.UCE_playerWarehouse[i];

                if (itemSlot.amount > 0)
                {
                    slot.tooltip.enabled = true;
                    slot.tooltip.text = itemSlot.ToolTip();
                    slot.dragAndDropable.dragable = true;
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
            levelText.text = levelLabel + (player.playerWarehouseLevel + 1).ToString();

            // ----- gold
            goldText.text = player.playerWarehouseGold.ToString() + "/" + player.playerWarehouseStorageGold;

            buttonUpgrade.interactable = player.UCE_CanUpgradePlayerWarehouse();
            buttonDeposit.interactable = player.isAlive && player.gold > 0 && player.UCE_HasEnoughPlayerWarehouseGoldSpace();
            buttonWithdrawal.interactable = player.isAlive && player.playerWarehouseGold > 0;
            goldInputPanel.interactable = player.isAlive && (player.gold > 0 || player.playerWarehouseGold > 0);

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
                var amountValue = goldInputPanel.text;

                if (!string.IsNullOrWhiteSpace(amountValue))
                {
                    int amount = int.Parse(amountValue);

                    if ((bdc != 1 || bwc != 1) && amount < 1) return;

                    if (bdc == 1)
                    {
                        player.CmdDepositGold(amount);
                    }

                    if (bwc == 1)
                    {
                        player.CmdWithdrawGold(amount);
                    }

                    goldText.text = player.playerWarehouseGold.ToString();
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

        if (!panel.activeSelf)
        {
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
            panel.SetActive(false);
        }
    }

    // -----------------------------------------------------------------------------------
}
