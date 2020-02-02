// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;

// ITEM

public partial struct Item
{
    // -----------------------------------------------------------------------------------
    // UCE_GetHonorCurrency
    // -----------------------------------------------------------------------------------
    public long UCE_GetHonorCurrency(UCE_Tmpl_HonorCurrency honorCurrency)
    {
        return data.currencyCosts.FirstOrDefault(x => x.honorCurrency.name == honorCurrency.name).amount;
    }
}
