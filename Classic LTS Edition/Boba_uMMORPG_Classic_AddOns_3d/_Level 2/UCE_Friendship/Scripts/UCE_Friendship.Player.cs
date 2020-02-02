// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using UnityEngine;

// PLAYER

public partial class Player
{
    [Header("-=-=-=- UCE FRIENDSHIP -=-=-=-")]
    public UCE_Tmpl_Friendship friendshipTemplate;

    [HideInInspector] public SyncListUCE_Friend UCE_Friends = new SyncListUCE_Friend();

    protected UCE_UI_FriendList _instance;
    protected float _cacheFriendship;

    // -----------------------------------------------------------------------------------
    // Start_UCE_Friendship
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Start")]
    private void Start_UCE_Friendship()
    {
        UCE_Friends.Callback += OnUCE_FriendListChanged;

        if (!_instance)
            _instance = FindObjectOfType<UCE_UI_FriendList>();

        if (_instance)
            _instance.Prepare();
    }

    // -----------------------------------------------------------------------------------
    // Update_UCE_Friendship
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    [DevExtMethods("Update")]
    private void Update_UCE_Friendship()
    {
        if (Time.time < _cacheFriendship) return;

        for (int i = 0; i < UCE_Friends.Count; i++)
        {
            UCE_Friend frnd = UCE_Friends[i];
            Player plyr = UCE_Tools.FindOnlinePlayerByName(frnd.name);

            if (plyr != null)
            {
                frnd.online = true;
                frnd.inParty = plyr.InParty();
                frnd.level = plyr.level;
                frnd._class = plyr.className;
                frnd.guild = plyr.guild.name;
            }
            else
            {
                frnd.online = false;
            }

            UCE_Friends[i] = frnd;
        }

        _cacheFriendship = Time.time + 4.0f;
    }

    // -----------------------------------------------------------------------------------
    // OnUCE_FriendListChanged
    // -----------------------------------------------------------------------------------
    private void OnUCE_FriendListChanged(SyncListUCE_Friend.Operation op, int itemIndex, UCE_Friend oldSlot, UCE_Friend newSlot)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (!_instance)
            _instance = FindObjectOfType<UCE_UI_FriendList>();

        _instance.Prepare();
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_AddFriend
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_AddFriend(string name)
    {
        UCE_AddFriend(name);
    }

    // -----------------------------------------------------------------------------------
    // UCE_AddFriend
    // @Server
    // -----------------------------------------------------------------------------------
    public void UCE_AddFriend(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        if (UCE_Friends.Count >= friendshipTemplate.maxFriends)
        {
            UCE_TargetAddMessage(friendshipTemplate.msgMaxFriends);
            return;
        }

        if (name != this.name)
        {
            if (Database.singleton.CharacterExists(name))
            {
                if (onlinePlayers.ContainsKey(name))
                {
                    if (UCE_Friends.FindIndex(x => x.name == name) == -1)
                    {
                        Player pl = UCE_Tools.FindOnlinePlayerByName(name);

                        DateTime time = DateTime.UtcNow;
#if _iMMOHONORSHOP
                        int hours = friendshipTemplate.timespanHours * -1;
                        time = time.AddHours(hours);
#endif
                        UCE_Friend frnd = new UCE_Friend(pl.name, time.ToString("s"));

                        UCE_Friends.Add(frnd);

                        UCE_TargetAddMessage(string.Format(friendshipTemplate.msgIsNowFriend, name));
                    }
                    else
                    {
                        UCE_TargetAddMessage(string.Format(friendshipTemplate.msgIsAlreadyFriend, name));
                    }
                }
                else
                {
                    UCE_TargetAddMessage(string.Format(friendshipTemplate.msgTargetOffline, name));
                }
            }
            else
            {
                UCE_TargetAddMessage(string.Format(friendshipTemplate.msgTargetNotExist, name));
            }
        }
        else
        {
            UCE_TargetAddMessage(friendshipTemplate.msgCannotFriendSelf);
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_RemoveFriend
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_RemoveFriend(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        if (UCE_Friends.FindIndex(x => x.name == name) != -1)
        {
            UCE_Friends.Remove(UCE_Friends[UCE_Friends.FindIndex(x => x.name == name)]);
            UCE_TargetAddMessage(string.Format(friendshipTemplate.msgNoLongerFriend, name));
        }
        else
        {
            UCE_TargetAddMessage(string.Format(friendshipTemplate.msgTargetNotFriend, name));
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_GuildInvite
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_GuildInvite(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || !InGuild()) return;

        Player plyr = Player.onlinePlayers[name];

        if (plyr && !plyr.InGuild() && guild.CanInvite(this.name, name) && NetworkTime.time >= nextRiskyActionTime)
        {
            plyr.guildInviteFrom = this.name;
            nextRiskyActionTime = Time.time + guildInviteWaitSeconds;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanHug
    // -----------------------------------------------------------------------------------
#if _iMMOHONORSHOP

    public bool UCE_CanHug(string lastGifted)
    {
        if (string.IsNullOrWhiteSpace(lastGifted)) return true;
        DateTime time = DateTime.Parse(lastGifted);
        return ((DateTime.UtcNow - time).TotalHours >= friendshipTemplate.timespanHours);
    }

#endif

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_HugFriend
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
#if _iMMOHONORSHOP

    [Command]
    public void Cmd_UCE_HugFriend(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        int idx = UCE_Friends.FindIndex(x => x.name == name);

        if (idx != -1)
        {
            Player plyr = UCE_Tools.FindOnlinePlayerByName(name);
            UCE_Friend frnd = UCE_Friends[idx];

            if (UCE_CanHug(frnd.lastGifted))
            {
                // -- Other player gains Honor Currency when online
                if (plyr && friendshipTemplate.friendHonorCurrency && friendshipTemplate.friendAmount > 0)
                    plyr.UCE_AddHonorCurrency(friendshipTemplate.friendHonorCurrency, friendshipTemplate.friendAmount);

                frnd.lastGifted = DateTime.UtcNow.ToString("s");

                UCE_Friends[idx] = frnd;

                // -- Hugging Player gains Honor Currency
                if (friendshipTemplate.honorCurrency && friendshipTemplate.amount > 0)
                    UCE_AddHonorCurrency(friendshipTemplate.honorCurrency, friendshipTemplate.amount);

                UCE_TargetAddMessage(string.Format(friendshipTemplate.msgTargetHugged, name, friendshipTemplate.amount.ToString(), friendshipTemplate.honorCurrency.name));
            }
        }
    }

#endif

    // -----------------------------------------------------------------------------------
}
