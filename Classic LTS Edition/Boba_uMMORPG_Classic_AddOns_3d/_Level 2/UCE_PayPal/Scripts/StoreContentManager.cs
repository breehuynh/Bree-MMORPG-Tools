// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

public class StoreContentManager : MonoBehaviour
{
    public static StoreContentManager INSTANCE;

    public void Awake()
    {
        INSTANCE = this;
    }

    public GameObject scrollbar;

    public RectTransform scrollArea;

    [HideInInspector] // Hides var below
    public StoreItemContent[] storeItems;

    // Use this for initialization
    private void Start()
    {
        ArrangeItems();
    }

    public void ArrangeItems()
    {
        storeItems = GetComponentsInChildren<StoreItemContent>();

        for (int i = 0; i < storeItems.Length; i++)
        {
            StoreItemContent nextStoreItem = storeItems[i];

            GameObject nextStoreItemGO = nextStoreItem.gameObject;
            RectTransform nextStoreItemRect = nextStoreItemGO.GetComponent<RectTransform>();

            nextStoreItemRect.localPosition = new Vector3(nextStoreItemRect.localPosition.x, 0f, 0f);
            nextStoreItemRect.localPosition += new Vector3(0f, i * -100f, 0f);
        }

        //set the height of the scrollable area to the number of store items multiplied by 100 pixels
        GetComponent<RectTransform>().sizeDelta = new Vector2(350f, 100f * storeItems.Length);

        //hide scrollbar if store has less than 3 items
        if (storeItems.Length < 3)
        {
            scrollbar.SetActive(false);
        }
        else
        {
            scrollbar.SetActive(true);
        }

        scrollbar.GetComponent<Scrollbar>().value = 1f;
    }

    public Sprite GetSprite(string spriteName)
    {
        for (int i = 0; i < storeItems.Length; i++)
        {
            StoreItemContent nextStoreItem = storeItems[i];

            if (spriteName == nextStoreItem.itemName)
            {
                return nextStoreItem.itemImage;
            }
        }

        return Resources.Load<Sprite>("ItemSprites/DefaultImage");
    }

    public string GetDescription(string itemName)
    {
        for (int i = 0; i < storeItems.Length; i++)
        {
            StoreItemContent nextStoreItem = storeItems[i];

            if (itemName == nextStoreItem.itemName)
            {
                return nextStoreItem.itemDesc;
            }
        }

        // no description available so return empty string
        return "";
    }
}
