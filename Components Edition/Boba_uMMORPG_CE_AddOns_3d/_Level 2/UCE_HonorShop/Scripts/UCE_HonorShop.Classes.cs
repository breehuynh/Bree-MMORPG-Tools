// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;

// UCE_HonorShopCategory

[Serializable]
public partial struct UCE_HonorShopCategory
{
    public string categoryName;
    public UCE_Tmpl_HonorCurrency honorCurrency;
    public ScriptableItem[] items;
}
