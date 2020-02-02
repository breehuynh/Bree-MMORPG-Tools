// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if _iMMOASSETBUNDLEMANAGER
using Jacovone.AssetBundleMagic;
#endif

// PAYPAL PRODUCT - TEMPLATE

[CreateAssetMenu(menuName = "UCE Templates/New UCE PayPalProduct", order = 998)]
public class UCE_Tmpl_PayPalProduct : ScriptableObject
{

    [Header("PayPal Product")]
    [Tooltip("One click deactivation")]
    public bool _isActive = true;

    public Sprite image;

    [Tooltip("Cost of this product in real currency (depends on your currency setting)")]
    public float _cost;

    [Tooltip("Description will be displayed next to the product")]
    public string description;

    [Tooltip("How often the product can be purchased by a character (0 = infinite)")]
    public int purchaseLimit;

    [Tooltip("[Optional] Only the stated classes can buy the product (leave empty for all)")]
    public GameObject[] allowedClasses;

    public int coins;
    public int gold;
    public int experience;
#if _HonorShop
	[Header("-=-=-=- Honor Rewards -=-=-=-")]
	public UCE_HonorShopCurrency[] honorCurrency;
#endif
    public productItem[] items;

    [Header("-=-=-=- Special Offers -=-=-=-")]
    [Range(0, 31)] public int startDay;

    [Range(0, 12)] public int startMonth;
    [Range(0, 31)] public int endDay;
    [Range(0, 12)] public int endMonth;
    public bool onlyActiveDuringOffer;
    public float offerCost;

    // -----------------------------------------------------------------------------------
    // isActive
    // -----------------------------------------------------------------------------------
    public bool isActive
    {
        get
        {
            if (onlyActiveDuringOffer)
            {
                return checkOffer && _isActive;
            }
            else
            {
                return _isActive;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // cost
    // -----------------------------------------------------------------------------------
    public float cost
    {
        get
        {
            if (checkOffer)
            {
                return offerCost;
            }
            else
            {
                return _cost;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // hasOffer
    // -----------------------------------------------------------------------------------
    public bool hasOffer
    {
        get
        {
            return startDay != 0 &&
                    endDay != 0 &&
                    startMonth != 0 &&
                    endMonth != 0;
        }
    }

    // -----------------------------------------------------------------------------------
    // checkOffer
    // -----------------------------------------------------------------------------------
    public bool checkOffer
    {
        get
        {
            if (startDay == 0 || endDay == 0 || startMonth == 0 || endMonth == 0) return false;
            return
                    startDay <= DateTime.UtcNow.Day &&
                    endDay >= DateTime.UtcNow.Day &&
                    startMonth <= DateTime.UtcNow.Month &&
                    endMonth >= DateTime.UtcNow.Month;
        }
    }

    // -----------------------------------------------------------------------------------
    // Caching
    // -----------------------------------------------------------------------------------
    private static Dictionary<int, UCE_Tmpl_PayPalProduct> _cache;

    public static Dictionary<int, UCE_Tmpl_PayPalProduct> dict
    {
        get
        {
            if (_cache == null)
            {
                UCE_ScripableObjectEntry entry = UCE_TemplateConfiguration.singleton.GetEntry(typeof(UCE_Tmpl_PayPalProduct));
                string folderName = entry != null ? entry.folderName : "";
#if _iMMOASSETBUNDLEMANAGER
                if (entry != null && entry.loadFromAssetBundle)
                    _cache = AssetBundleMagic.LoadBundle(entry.bundleName).LoadAllAssets<UCE_Tmpl_PayPalProduct>().ToDictionary(x => x.name.GetStableHashCode(), x => x);
                else
                    _cache = Resources.LoadAll<UCE_Tmpl_PayPalProduct>(folderName).ToDictionary(x => x.name.GetStableHashCode(), x => x);
#else
                _cache = Resources.LoadAll<UCE_Tmpl_PayPalProduct>(UCE_TemplateConfiguration.singleton.GetTemplatePath(typeof(UCE_Tmpl_PayPalProduct))).ToDictionary(x => x.name.GetStableHashCode(), x => x);
#endif
            }

            return _cache;

        }
    }

    // -----------------------------------------------------------------------------------

}

[Serializable]
public class productItem
{
    public ScriptableItem item;
    public int amount;
}
