// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// Travelroute

[System.Serializable]
public class UCE_Travelroute
{
    [Header("[-=-=-=- UCE TRAVELROUTE -=-=-=-]")]
    [Tooltip("[Required] Any on scene Transform or GameObject OR off scene coordinates (requires UCE Network Zones AddOn)")]
    public UCE_TeleportationTarget teleportationTarget;

    [Tooltip("[Optional] Price calculated based on distance of current position and destination (or fixed price on off scene)")]
    public float GoldPricePerUnit;

#if _iMMOHONORSHOP

    [Header("-=-=-=- UCE Honor Currency Cost -=-=-=-")]
    [Tooltip("[Optional] Total price is calculated based on distance of current position and destination")]
    public UCE_HonorShopCurrencyCost[] currencyCost;

#endif
}
