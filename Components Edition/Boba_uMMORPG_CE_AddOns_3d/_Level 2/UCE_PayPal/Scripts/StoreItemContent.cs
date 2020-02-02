// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// STORE ITEM CONTENT - UI

public class StoreItemContent : MonoBehaviour
{
    [HideInInspector] public UCE_Tmpl_PayPalProduct product;

    [HideInInspector] public Sprite itemImage;
    [HideInInspector] public string itemName;
    [HideInInspector] public string itemCost;
    [HideInInspector] public string itemDesc;
    [HideInInspector] public string itemOffer;

    public Image itemImageField;
    public Text itemNameTextField;
    public Text itemCostTextField;
    public Text itemCurCodeTextField;
    public Text itemDescTextField;
    public Text itemOfferTextField;

    public string validFrom;
    public string validUntil;
    public bool showCurCode;

    // -----------------------------------------------------------------------------------
    // Init
    // -----------------------------------------------------------------------------------
    public void Init(UCE_Tmpl_PayPalProduct myProduct)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        product = myProduct;

        if (product.isActive &&
        player.UCE_checkHasClass(product.allowedClasses) &&
        player.UCE_PayPal_CanPurchase(product)
        )
        {
            itemImage = product.image;
            itemName = product.name;
            itemDesc = product.description;
            itemCost = product.cost.ToString();
            itemOffer = "";

            if (product.hasOffer)
            {
                string offerStart = product.startMonth.ToString() + "/" + product.startDay.ToString();
                string offerEnd = product.endMonth.ToString() + "/" + product.endDay.ToString();
                itemOffer = validFrom + offerStart + validUntil + offerEnd;
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    private void Start()
    {
        if (product == null || (product != null && !product.isActive))
        {
            gameObject.SetActive(false);
            return;
        }

        if (itemImage == null)
            itemImage = Resources.Load<Sprite>("ItemSprites/DefaultImage");

        itemImageField.sprite = itemImage;

        if (itemName.Length > 100)
        {
            itemName = itemName.Substring(0, 99);
        }

        itemNameTextField.text = itemName;
        itemOfferTextField.text = itemOffer;

        itemCostTextField.text = string.Format("{0:N}", itemCost);
        itemCostTextField.text = CurrencyCodeMapper.GetCurrencySymbol(StoreProperties.INSTANCE.currencyCode) + itemCostTextField.text;
        itemCurCodeTextField.text = "";

        if (showCurCode)
            itemCurCodeTextField.text = "(" + StoreProperties.INSTANCE.currencyCode + ")";

        itemDescTextField.text = itemDesc;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void BuyItemAction()
    {
        StoreActions.INSTANCE.OpenPurchaseItemScreen(this);
    }

    // -----------------------------------------------------------------------------------
}
