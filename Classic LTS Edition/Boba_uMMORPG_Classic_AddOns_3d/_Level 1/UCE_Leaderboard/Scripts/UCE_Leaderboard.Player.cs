// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;

// ===================================================================================
// Player
// ===================================================================================
public partial class Player
{
    public SyncListUCE_LeaderboardPlayer currentOnlinePlayers = new SyncListUCE_LeaderboardPlayer();

    protected int maxPlayers = 50;

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_AllPlayersOnline
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_AllPlayersOnline()
    {
        // we wrap it in another function, because we want to call it only server-side
        UCE_UpdatePlayersOnline();
    }

    // -----------------------------------------------------------------------------------
    // UCE_UpdatePlayersOnline
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void UCE_UpdatePlayersOnline()
    {
        currentOnlinePlayers.Clear();

        int i = 0;

        foreach (Player plyr in onlinePlayers.Values)
        {
            UCE_LeaderboardPlayer ldplyr = new UCE_LeaderboardPlayer(plyr.name, plyr.level, plyr.gold);

            currentOnlinePlayers.Add(ldplyr);

            i++;

            if (i == maxPlayers) break;
        }
    }

    // -----------------------------------------------------------------------------------
}

// ===================================================================================
