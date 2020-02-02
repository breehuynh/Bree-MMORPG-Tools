// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UCE UI HONOR SHOP

public partial class UCE_UI_HonorShop : MonoBehaviour
{
    public GameObject panel;
    public Button categorySlotPrefab;
    public Transform categoryContent;
    public ScrollRect scrollRect;
    public UCE_UI_HonorShopSlot itemSlotPrefab;
    public Transform itemContent;
    private int currentCategory = 0;
    public Text currencyNameText;
    public Text currencyAmountText;
    public GameObject inventoryPanel;

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    private void ScrollToBeginning()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        // use collider point(s) to also work with big entities
        if (player.target != null && player.target is Npc &&
            Utils.ClosestDistance(player, player.target) <= player.interactionRange &&
            panel.activeSelf)
        {
            Npc npc = (Npc)player.target;

            long amount = player.UCE_GetHonorCurrency(npc.itemShopCategories[currentCategory].honorCurrency);
            if (amount == -1) amount = 0;

            // instantiate/destroy enough category slots
            UIUtils.BalancePrefabs(categorySlotPrefab.gameObject, npc.itemShopCategories.Length, categoryContent);

            // -- Categories
            for (int i = 0; i < npc.itemShopCategories.Length; ++i)
            {
                Button button = categoryContent.GetChild(i).GetComponent<Button>();
                button.interactable = i != currentCategory;
                button.GetComponentInChildren<Text>().text = npc.itemShopCategories[i].categoryName;
                int icopy = i; // needed for lambdas, otherwise i is Count
                button.onClick.SetListener(() =>
                {
                    // set new category and then scroll to the top again
                    currentCategory = icopy;
                    ScrollToBeginning();
                });
            }

            // -- Items
            if (npc.itemShopCategories.Length > 0)
            {
                // instantiate/destroy enough item slots for that category
                ScriptableItem[] items = npc.itemShopCategories[currentCategory].items;
                UIUtils.BalancePrefabs(itemSlotPrefab.gameObject, items.Length, itemContent);

                // refresh all items in that category
                for (int i = 0; i < items.Length; ++i)
                {
                    UCE_UI_HonorShopSlot slot = itemContent.GetChild(i).GetComponent<UCE_UI_HonorShopSlot>();
                    ScriptableItem itemData = items[i];
                    Item itm = new Item(itemData);

                    // refresh item
                    slot.tooltip.text = itm.ToolTip();
                    slot.image.color = Color.white;
                    slot.image.sprite = itemData.image;
                    slot.nameText.text = itemData.name;
                    slot.priceText.text = itm.UCE_GetHonorCurrency(npc.itemShopCategories[currentCategory].honorCurrency).ToString();
                    slot.currencyText.text = npc.itemShopCategories[currentCategory].honorCurrency.name;
                    slot.buyButton.interactable = player.isAlive && amount >= itm.UCE_GetHonorCurrency(npc.itemShopCategories[currentCategory].honorCurrency);
                    int icopy = i; // needed for lambdas, otherwise i is Count
                    slot.buyButton.onClick.SetListener(() =>
                    {
                        player.Cmd_UCE_HonorShop(currentCategory, icopy);
                        inventoryPanel.SetActive(true); // better feedback
                    });
                }
            }

            // Currency
            currencyNameText.text = npc.itemShopCategories[currentCategory].honorCurrency.name;
            currencyAmountText.text = amount.ToString();
        }
    }

    // -----------------------------------------------------------------------------------
}
