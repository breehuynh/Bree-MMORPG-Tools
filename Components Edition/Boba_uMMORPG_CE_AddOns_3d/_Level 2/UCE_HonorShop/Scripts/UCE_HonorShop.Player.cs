// =======================================================================================
// Maintained by bobatea#9400 on Discord
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
    [HideInInspector] public SyncListUCE_HonorShopCurrency UCE_currencies = new SyncListUCE_HonorShopCurrency();

    // -----------------------------------------------------------------------------------
    // UCE_AddHonorCurrency
    // -----------------------------------------------------------------------------------
    public void UCE_AddHonorCurrency(UCE_Tmpl_HonorCurrency honorCurrency, long currencyAmount)
    {
        int idx = UCE_currencies.FindIndex(x => x.honorCurrency.name == honorCurrency.name);

        if (idx == -1)
        {
            UCE_HonorShopCurrency hsc = new UCE_HonorShopCurrency();
            hsc.honorCurrency = honorCurrency;
            hsc.amount = currencyAmount;
            hsc.total += currencyAmount;
            UCE_currencies.Add(hsc);
        }
        else
        {
            UCE_HonorShopCurrency hsc = UCE_currencies.FirstOrDefault(x => x.honorCurrency.name == honorCurrency.name);
            hsc.amount += currencyAmount;
            UCE_currencies[idx] = hsc;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetHonorCurrency
    // -----------------------------------------------------------------------------------
    public long UCE_GetHonorCurrency(UCE_Tmpl_HonorCurrency honorCurrency)
    {
        int idx = UCE_currencies.FindIndex(x => x.honorCurrency.name == honorCurrency.name);
        if (idx != -1)
            return UCE_currencies[idx].amount;
        return 0;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckHonorCurrencyCost
    // -----------------------------------------------------------------------------------
    public bool UCE_CheckHonorCurrencyCost(UCE_HonorShopCurrencyCost[] currencyCost)
    {
        bool valid = true;

        foreach (UCE_HonorShopCurrencyCost currency in currencyCost)
        {
            if (UCE_GetHonorCurrency(currency.honorCurrency) < currency.amount)
            {
                valid = false;
                break;
            }
        }

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckHonorCurrencyCost
    // -----------------------------------------------------------------------------------
    public bool UCE_CheckHonorCurrencyCost(UCE_HonorShopCurrencyDrop[] currencyCost)
    {
        bool valid = true;

        foreach (UCE_HonorShopCurrencyDrop currency in currencyCost)
        {
            if (UCE_GetHonorCurrency(currency.honorCurrency) < currency.amount)
            {
                valid = false;
                break;
            }
        }

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_PayHonorCurrencyCost
    // -----------------------------------------------------------------------------------
    public void UCE_PayHonorCurrencyCost(UCE_HonorShopCurrency[] currencyCost)
    {
        foreach (UCE_HonorShopCurrency currency in currencyCost)
            UCE_AddHonorCurrency(currency.honorCurrency, currency.amount * -1);
    }

    // -----------------------------------------------------------------------------------
    // UCE_PayHonorCurrencyCost
    // -----------------------------------------------------------------------------------
    public void UCE_PayHonorCurrencyCost(UCE_HonorShopCurrencyCost[] currencyCost)
    {
        foreach (UCE_HonorShopCurrencyCost currency in currencyCost)
            UCE_AddHonorCurrency(currency.honorCurrency, currency.amount * -1);
    }

    // -----------------------------------------------------------------------------------
    // UCE_PayHonorCurrencyCost
    // -----------------------------------------------------------------------------------
    public void UCE_PayHonorCurrencyCost(UCE_HonorShopCurrencyDrop[] currencyCost)
    {
        foreach (UCE_HonorShopCurrencyDrop currency in currencyCost)
            UCE_AddHonorCurrency(currency.honorCurrency, currency.amount * -1);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_HonorShop
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_HonorShop(int categoryIndex, int itemIndex)
    {
        if (state == "IDLE" &&
            target != null &&
            target.isAlive &&
            isAlive &&
            target is Npc &&
            Utils.ClosestDistance(this, target) <= interactionRange)
        {
            Npc npc = (Npc)target;

            UCE_Tmpl_HonorCurrency honorCurrency = npc.itemShopCategories[categoryIndex].honorCurrency;

            long amount = UCE_GetHonorCurrency(honorCurrency);
            if (amount == -1) amount = 0;

            if (0 <= categoryIndex && categoryIndex <= npc.itemShopCategories.Length &&
             0 <= itemIndex && itemIndex <= npc.itemShopCategories[categoryIndex].items.Length &&
             0 < amount)
            {
                Item item = new Item(npc.itemShopCategories[categoryIndex].items[itemIndex]);
                long currencyCost = item.UCE_GetHonorCurrency(honorCurrency);

                if (0 < item.UCE_GetHonorCurrency(honorCurrency) && currencyCost <= amount)
                {
                    if (inventory.Add(item, 1))
                    {
                        UCE_AddHonorCurrency(honorCurrency, currencyCost * -1);
                    }
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_HonorCurrency_ShareToParty
    // -----------------------------------------------------------------------------------
    public void UCE_HonorCurrency_ShareToParty(UCE_Tmpl_HonorCurrency honorCurrency, long amount)
    {
        List<Player> closeMembers = party.GetMembersInProximity();

        // calculate the share via ceil, so that uneven numbers
        // still result in at least total gold in the end.
        // e.g. 4/2=2 (good); 5/2=2 (1 gold got lost)
        long share = (long)Mathf.Ceil((float)amount / (float)closeMembers.Count);

        // now distribute
        foreach (Player member in closeMembers)
            member.UCE_AddHonorCurrency(honorCurrency, share);

        UCE_TargetAddMessage(MSG_GAINED + honorCurrency.name + " x" + share.ToString());
    }

    // -----------------------------------------------------------------------------------
    // UCE_HonorCurrency_ShareToGuild
    // -----------------------------------------------------------------------------------
    public void UCE_HonorCurrency_ShareToGuild(UCE_Tmpl_HonorCurrency honorCurrency, long amount)
    {
        List<Player> players = new List<Player>();

        foreach (GuildMember member in guild.guild.members)
        {
            if (onlinePlayers.ContainsKey(member.name))
            {
                players.Add(onlinePlayers[member.name]);
            }
        }

        if (players.Count > 0)
        {
            long share = (long)Mathf.Ceil((float)amount / (float)players.Count);

            foreach (Player player in players)
            {
                player.UCE_AddHonorCurrency(honorCurrency, share);
            }

            UCE_TargetAddMessage(MSG_GAINED + honorCurrency.name + " x" + share.ToString());
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_HonorCurrency_ShareToRealm
    // -----------------------------------------------------------------------------------
#if _iMMOPVP

    public void UCE_HonorCurrency_ShareToRealm(UCE_Tmpl_HonorCurrency honorCurrency, long amount)
    {
        List<Player> players = new List<Player>();

        foreach (Player player in onlinePlayers.Values)
        {
            if (UCE_getAlliedRealms(player))
            {
                players.Add(onlinePlayers[player.name]);
            }
        }

        if (players.Count > 0)
        {
            long share = (long)Mathf.Ceil((float)amount / (float)players.Count);

            foreach (Player player in players)
            {
                player.UCE_AddHonorCurrency(honorCurrency, share);
            }
        }
    }

#endif

    // -----------------------------------------------------------------------------------
}
