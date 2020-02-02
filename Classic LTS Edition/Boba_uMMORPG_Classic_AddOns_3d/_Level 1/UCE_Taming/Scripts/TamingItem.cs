// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "uMMORPG Item/Taming Item", order = 999)]
public class TamingItem : UsableItem
{
    public float successIncrease = 0.0f;
    public bool deleteItemAfter = true;

    // Start is called before the first frame update
    public override void Use(Player player, int inventoryIndex)
    {
        if (player.isTaming) return;
        if (player.target == null) return;
        if (player.target.state == "DEAD") return;
        Monster monster = (Monster)player.target;
        if (monster != null)
        {
            if (monster.tamingReward.item != null)
            {
                player.CmdsetupTamingSuccessRate(successIncrease);
                player.StartCoroutine(player.tamingTask());
            }
        }
        if (!deleteItemAfter) return;
        // decrease amount
        ItemSlot slot = player.inventory[inventoryIndex];
        slot.DecreaseAmount(1);
        player.inventory[inventoryIndex] = slot;
    }
}
