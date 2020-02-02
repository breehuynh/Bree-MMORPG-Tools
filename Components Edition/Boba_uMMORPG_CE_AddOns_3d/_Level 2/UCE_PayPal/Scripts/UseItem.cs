// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UseItem

public class UseItem : MonoBehaviour
{
    public static void Use(UCE_Tmpl_PayPalProduct product)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (product != null)
        {
            player.Cmd_UCE_PayPal_PurchaseCoins(product.name.GetStableHashCode());
        }
    }
}
