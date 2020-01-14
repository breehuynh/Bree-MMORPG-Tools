// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// PLAYER

public partial class Player
{
    [Header("-=-=-=- UCE TRAVELROUTES -=-=-=-")]
    public UCE_PopupClass travelroutePopup;

    [HideInInspector] public UCE_AreaBox_Travelroute UCE_myTravelrouteArea;
    [HideInInspector] public SyncListUCE_TravelrouteClass UCE_travelroutes = new SyncListUCE_TravelrouteClass();

    // -----------------------------------------------------------------------------------
    // UCE_UnlockTravelroutes
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void UCE_UnlockTravelroutes()
    {
        foreach (UCE_Unlockroute route in UCE_myTravelrouteArea.Unlockroutes)
            UCE_UnlockTravelroute(route);
    }

    // -----------------------------------------------------------------------------------
    // UCE_UnlockTravelroute
    // @Server
    // -----------------------------------------------------------------------------------
    public void UCE_UnlockTravelroute(UCE_Unlockroute unlockroute)
    {
        if (unlockroute == null) return;

        string name = "";
        bool pass = true;

        // ------------------------------------------------------------------ get Name

        if (unlockroute.teleportationTarget.Valid)
        {
            name = unlockroute.teleportationTarget.name;
        }

        // ------------------------------------------------------------------ Validate Name
        if (!UCE_travelroutes.Any(t => t.name == name))
        {
            // -- validate and unlock
            if (pass && !string.IsNullOrWhiteSpace(name))
            {
                UCE_TravelrouteClass tRoute = new UCE_TravelrouteClass(name);
                UCE_travelroutes.Add(tRoute);
                experience += unlockroute.ExpGain;
                string msg = travelroutePopup.message + name;
                UCE_ShowPopup(msg, travelroutePopup.iconId, travelroutePopup.soundId);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_WarpTravelroute
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_WarpTravelroute(int index)
    {
        if (UCE_myTravelrouteArea)
        {
            int price = 0;
            List<UCE_Travelroute> travelroutesAvailable = UCE_TravelroutesVisibleForPlayer();
            UCE_Travelroute targetTravelroute = travelroutesAvailable[index];
            string name = "";

            if (targetTravelroute.teleportationTarget.Valid)
                name = targetTravelroute.teleportationTarget.name;

            if (!string.IsNullOrWhiteSpace(name) && UCE_travelroutes.Any(t => t.name == name))
            {
#if _iMMOHONORSHOP
                if (!UCE_checkHonorCost(index)) return;
                UCE_payHonorCost(index);
#endif
                price = UCE_getTravelCost(index);

                if (gold >= price)
                {
                    gold -= price;

                    targetTravelroute.teleportationTarget.OnTeleport(this);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_getTravelCost
    // -----------------------------------------------------------------------------------
#if _iMMOHONORSHOP

    public bool UCE_checkHonorCost(int index)
    {
        bool valid = false;

        if (UCE_myTravelrouteArea)
        {
            List<UCE_Travelroute> travelroutesAvailable = UCE_TravelroutesVisibleForPlayer();
            UCE_Travelroute targetTravelroute = travelroutesAvailable[index];

            if (targetTravelroute != null)
            {
                if (targetTravelroute.currencyCost.Length == 0) return true;

                foreach (UCE_HonorShopCurrencyCost currency in targetTravelroute.currencyCost)
                {
                    int price = (int)(targetTravelroute.teleportationTarget.getDistance(UCE_myTravelrouteArea.transform) * currency.amount);

                    if (UCE_GetHonorCurrency(currency.honorCurrency) >= price)
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = false;
                        break;
                    }
                }
            }
        }

        return valid;
    }

#endif

    // -----------------------------------------------------------------------------------
    // UCE_getTravelCost
    // -----------------------------------------------------------------------------------
#if _iMMOHONORSHOP

    public void UCE_payHonorCost(int index)
    {
        if (UCE_myTravelrouteArea)
        {
            List<UCE_Travelroute> travelroutesAvailable = UCE_TravelroutesVisibleForPlayer();
            UCE_Travelroute targetTravelroute = travelroutesAvailable[index];

            if (targetTravelroute != null)
            {
                foreach (UCE_HonorShopCurrencyCost currency in targetTravelroute.currencyCost)
                {
                    int price = (int)(targetTravelroute.teleportationTarget.getDistance(UCE_myTravelrouteArea.transform) * currency.amount);

                    UCE_AddHonorCurrency(currency.honorCurrency, price * -1);
                }
            }
        }
    }

#endif

    // -----------------------------------------------------------------------------------
    // UCE_getTravelCost
    // -----------------------------------------------------------------------------------
    public int UCE_getTravelCost(int index)
    {
        int price = 0;

        if (UCE_myTravelrouteArea)
        {
            List<UCE_Travelroute> travelroutesAvailable = UCE_TravelroutesVisibleForPlayer();
            UCE_Travelroute targetTravelroute = travelroutesAvailable[index];

            if (targetTravelroute != null && targetTravelroute.GoldPricePerUnit > 0)
                price = (int)(targetTravelroute.teleportationTarget.getDistance(UCE_myTravelrouteArea.transform) * targetTravelroute.GoldPricePerUnit);
        }

        return price;
    }

    // -----------------------------------------------------------------------------------
    // UCE_TravelroutesVisibleForPlayer
    // -----------------------------------------------------------------------------------
    public List<UCE_Travelroute> UCE_TravelroutesVisibleForPlayer()
    {
        return (from travelroute in UCE_myTravelrouteArea.Travelroutes
                where UCE_travelroutes.Any(t => travelroute.teleportationTarget.Valid && t.name == travelroute.teleportationTarget.name) // || t.name == travelroute.routeName)
                select travelroute).ToList();
    }

    // -----------------------------------------------------------------------------------
}
