// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// ===================================================================================
// LEVEL SALE ITEMS
// ===================================================================================
[System.Serializable]
public partial class LevelSaleItems
{
    public ScriptableItem[] saleItems;
}

// ===================================================================================
// NPC SHOP
// ===================================================================================
[System.Serializable]
public partial class UCE_NpcShopContent
{
    [Tooltip("[Required] Item categories to show, add at least one category (can be empty)")]
    public string[] shopCategories;

    [Tooltip("[Optional] Add categories if you want content to scale with level (optional)")]
    public LevelSaleItems[] levelSaleItems;

    [Tooltip("[Optional] Check to scale 'levelSaleItems' with player level, uncheck to scale with NPC level")]
    public bool scaleContentWithPlayer;

    [Tooltip("[Optional] Check to show all content up to the playes level, uncheck to show only content of current level")]
    public bool showAllValidContent;
}
