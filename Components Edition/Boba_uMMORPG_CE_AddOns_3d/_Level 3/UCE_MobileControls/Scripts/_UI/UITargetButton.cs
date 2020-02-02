// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UITargetButton

public class UITargetButton : MonoBehaviour
{
    private Button targetButton;

    private void Start()
    {
        targetButton = GetComponent<Button>();
        if (targetButton != null)
            targetButton.onClick.SetListener(() => SelectTarget());
    }

    private void SelectTarget()
    {
        Player player = Player.localPlayer;
        if (!player) return;
        player.TargetNearestButton();
    }
}
