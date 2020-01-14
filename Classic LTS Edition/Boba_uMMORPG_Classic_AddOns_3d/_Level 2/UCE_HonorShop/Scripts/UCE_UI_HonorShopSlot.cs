// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// Attach to the prefab for easier component access by the UI Scripts.
// Otherwise we would need slot.GetChild(0).GetComponentInChildren<Text> etc.
using UnityEngine;
using UnityEngine.UI;

public class UCE_UI_HonorShopSlot : MonoBehaviour
{
    public UIShowToolTip tooltip;
    public Image image;
    public UIDragAndDropable dragAndDropable;
    public Text nameText;
    public Text priceText;
    public Text currencyText;
    public Button buyButton;
}
