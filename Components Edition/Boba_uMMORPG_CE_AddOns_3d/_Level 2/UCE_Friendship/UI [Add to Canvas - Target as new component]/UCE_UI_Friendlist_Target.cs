// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// FRIENDLIST TARGET
// ===================================================================================
public partial class UCE_UI_Friendlist_Target : MonoBehaviour
{
    public Button friendAddButton;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

#if _iMMOPVP
        if (player.target && player.target is Player && player.target != player && player.UCE_SameRealm((Player)player.target))
        {
            friendAddButton.gameObject.SetActive(true);
            friendAddButton.interactable = player.UCE_Friends.FindIndex(x => x.name == ((Player)(player.target)).name) == -1 ? true : false;
            friendAddButton.onClick.SetListener(() =>
            {
                player.Cmd_UCE_AddFriend(((Player)(player.target)).name);
            });
        }
        else friendAddButton.gameObject.SetActive(false);
#else
 		if (player.target && player.target is Player && player.target != player) {
            friendAddButton.gameObject.SetActive(true);
            friendAddButton.interactable = player.UCE_Friends.FindIndex(x=> x.name == ((Player)(player.target)).name) == -1  ? true : false;
            friendAddButton.onClick.SetListener(() => {
                player.Cmd_UCE_AddFriend(((Player)(player.target)).name);
            });
        }
        else friendAddButton.gameObject.SetActive(false);
#endif
    }

    // -----------------------------------------------------------------------------------
}
