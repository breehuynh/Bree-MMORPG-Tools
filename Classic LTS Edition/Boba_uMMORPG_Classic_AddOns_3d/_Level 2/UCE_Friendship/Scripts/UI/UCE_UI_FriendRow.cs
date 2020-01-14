// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// FRIEND ROW

public class UCE_UI_FriendRow : MonoBehaviour
{
    public Text playerName;
    public Text levelClass;
    public Text guildName;
    public Image statusImage;

    public Sprite onlineImage;
    public Sprite offlineImage;

    public Button buttonPartyInvite;
    public Button buttonGuildInvite;
    public Button buttonGiftFriend;
    public Button buttonRemove;

    // -----------------------------------------------------------------------------------
    // ´SetData
    // -----------------------------------------------------------------------------------
    public void SetData(int friendNo)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        UCE_Friend frnd = player.UCE_Friends[friendNo];

        // -- Friend Data
        playerName.text = frnd.name;

        if (!string.IsNullOrWhiteSpace(frnd._class) && frnd.level > 0)
        {
            levelClass.text = frnd._class + " [" + frnd.level + "]";
        }
        else
        {
            levelClass.text = "";
        }

        // -- Friend Guild
        if (!string.IsNullOrWhiteSpace(frnd.guild))
        {
            guildName.text = frnd.guild;
            guildName.gameObject.SetActive(true);
        }
        else
        {
            guildName.text = "";
            guildName.gameObject.SetActive(false);
        }

        // -- Friend Online
        if (!frnd.online)
        {
            statusImage.sprite = offlineImage;
        }
        else
        {
            statusImage.sprite = onlineImage;
        }

        // -- Button: Party Invite
        if (frnd.online && (!player.InParty() || !player.party.IsFull()) && !frnd.inParty)
        {
            buttonPartyInvite.interactable = true;
            buttonPartyInvite.onClick.SetListener(() =>
            {
                player.CmdPartyInvite(frnd.name);
                buttonPartyInvite.interactable = false;
            }
            );
        }
        else
        {
            buttonPartyInvite.interactable = false;
        }

        // -- Button: Guild Invite
        if (frnd.online && player.InGuild() && string.IsNullOrWhiteSpace(frnd.guild) && player.guild.CanInvite(player.name, frnd.name))
        {
            buttonGuildInvite.interactable = true;
            buttonGuildInvite.onClick.SetListener(() =>
            {
                player.Cmd_UCE_GuildInvite(frnd.name);
                buttonGuildInvite.interactable = false;
            }
            );
        }
        else
        {
            buttonGuildInvite.interactable = false;
        }

        // -- Button: Gift Friend
#if _iMMOHONORSHOP
        if (player.friendshipTemplate.honorCurrency || player.friendshipTemplate.friendHonorCurrency)
        {
            buttonGiftFriend.gameObject.SetActive(true);

            if (player.UCE_CanHug(frnd.lastGifted))
            {
                buttonGiftFriend.interactable = true;
                buttonGiftFriend.onClick.SetListener(() =>
                {
                    player.Cmd_UCE_HugFriend(frnd.name);
                    buttonGiftFriend.interactable = false;
                }
                );
            }
            else
            {
                buttonGiftFriend.interactable = false;
            }
        }
        else
        {
            buttonGiftFriend.gameObject.SetActive(false);
        }

#else
		buttonGiftFriend.gameObject.SetActive(false);
#endif

        // -- Button: Friend Remove
        bool canRemove = true;
#if _iMMOHONORSHOP
        canRemove = player.UCE_CanHug(frnd.lastGifted);
#endif
        if (canRemove)
        {
            buttonRemove.interactable = true;
            buttonRemove.onClick.SetListener(() => player.Cmd_UCE_RemoveFriend(frnd.name));
        }
        else
        {
            buttonRemove.interactable = false;
        }
    }
}
