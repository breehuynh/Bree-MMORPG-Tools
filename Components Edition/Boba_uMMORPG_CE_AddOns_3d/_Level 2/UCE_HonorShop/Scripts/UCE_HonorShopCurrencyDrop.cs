// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;

// UCE_HonorShopCurrencyDrop

[Serializable]
public partial struct UCE_HonorShopCurrencyDrop
{
    public UCE_Tmpl_HonorCurrency honorCurrency;
    public long amountMin;
    public long amountMax;

    public long amount
    {
        get
        {
            return (long)UnityEngine.Random.Range(amountMin, amountMax);
        }
    }
}
