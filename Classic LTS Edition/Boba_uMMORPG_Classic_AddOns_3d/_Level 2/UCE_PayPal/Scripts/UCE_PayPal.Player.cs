// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using UnityEngine;

// PLAYER

public partial class Player
{
    // -----------------------------------------------------------------------------------
    // UCE_PayPal_CanPurchase
    // -----------------------------------------------------------------------------------
    public bool UCE_PayPal_CanPurchase(UCE_Tmpl_PayPalProduct product)
    {
        if (product.purchaseLimit <= 0) return true;

        return (Database.singleton.UCE_loadCharacterPurchase(name, product.name) < product.purchaseLimit);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_PayPal_PurchaseCoins
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_PayPal_PurchaseCoins(int productHash)
    {
        UCE_Tmpl_PayPalProduct product;

        if (UCE_Tmpl_PayPalProduct.dict.TryGetValue(productHash, out product))
        {
            if (UCE_PayPal_CanPurchase(product))
            {
                coins += product.coins;
                gold += product.gold;
                experience += product.experience;

#if _HonorShop
				foreach (UCE_HonorShopCurrency currency in product.honorCurrency)
					UCE_AddHonorCurrency(currency.honorCurrency, currency.amount);
#endif

                foreach (productItem item in product.items)
                {
                    if (item.amount > 0)
                        InventoryAdd(new Item(item.item), item.amount);
                }

                Database.singleton.UCE_saveCharacterPurchase(name, product, DateTime.Now.ToShortDateString());
            }
        }
    }
}
