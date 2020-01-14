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
    // -----------------------------------------------------------------------------------
    // Cmd_UCE_NpcBuyItem
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_NpcBuyItem(int index, string currentCategory, int amount)
    {
        Npc npc = (Npc)target;
        List<ScriptableItem> saleItems = new List<ScriptableItem>();
        saleItems.AddRange(npc.saleItems);

        if (npc.shopContent.scaleContentWithPlayer)
        {
            saleItems.Clear();

            int idx = Mathf.Max((Mathf.RoundToInt(level / (maxLevel / npc.shopContent.levelSaleItems.Length)) - 1), 0);

            if (npc.shopContent.showAllValidContent)
            {
                for (int i = 0; i <= idx; ++i)
                    saleItems.AddRange(npc.shopContent.levelSaleItems[i].saleItems);
            }
            else
            {
                if (npc.shopContent.levelSaleItems.Length > 0)
                    saleItems.AddRange(npc.shopContent.levelSaleItems[idx].saleItems);
            }
        }
        else
        {
            if (npc.shopContent.levelSaleItems.Length > 0 && npc.shopContent.levelSaleItems.Length >= npc.level)
            {
                saleItems.Clear();
                saleItems.AddRange(npc.shopContent.levelSaleItems[npc.level - 1].saleItems);
            }
        }

        ScriptableItem[] items = saleItems.Where(x => x.itemCategory == currentCategory || currentCategory == "").ToArray();

        // validate: close enough, npc alive and valid index?
        // use collider point(s) to also work with big entities
        if (state == "IDLE" &&
            isAlive &&
            target != null &&
            target.isAlive &&
            target is Npc &&
            Utils.ClosestDistance(this, target) <= interactionRange &&
            0 <= index && index < items.Length)
        {
            // valid amount?
            Item npcItem = new Item(items[index]);

            if (1 <= amount && amount <= npcItem.maxStack)
            {
                long price = npcItem.buyPrice * amount;

                // enough gold and enough space in inventory?
                if (gold >= price && InventoryCanAdd(npcItem, amount))
                {
                    // pay for it, add to inventory
                    gold -= price;
                    InventoryAdd(npcItem, amount);
                }
            }
        }
    }
}
