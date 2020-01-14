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

// UCE_HonorShopCurrency

[Serializable]
public partial struct UCE_HonorShopCurrency
{
    public int hash;
    public long amount;
    [HideInInspector] public long total;

    public UCE_Tmpl_HonorCurrency honorCurrency
    {
        get
        {
            if (hash == 0)

            {
                return null;
            }
            else
            {
                UCE_Tmpl_HonorCurrency currency;
                if (UCE_Tmpl_HonorCurrency.dict.TryGetValue(hash, out currency))
                    return currency;
                else
                    return null;
            }
        }

        set
        {
            if (value != null)
                hash = value.name.GetDeterministicHashCode();
            else
                hash = 0;
        }
    }
}

public class SyncListUCE_HonorShopCurrency : SyncList<UCE_HonorShopCurrency> { }
