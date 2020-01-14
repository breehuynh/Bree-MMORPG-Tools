// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;

// ===================================================================================
// UCE Leaderboard Player
// ===================================================================================
[Serializable]
public partial struct UCE_LeaderboardPlayer
{
    public string name;
    public int rank;
    public int level;
    public long gold;

    // -------------------------------------------------------------------------------
    // UCE_Quest (Constructor)
    // -------------------------------------------------------------------------------
    public UCE_LeaderboardPlayer(string _name, int _level, long _gold)
    {
        name = _name;
        level = _level;
        gold = _gold;
        rank = 0;

        rank = calculateRank();
    }

    // -------------------------------------------------------------------------------
    //
    // -------------------------------------------------------------------------------
    public int calculateRank()
    {
        return 0;
    }

    // -------------------------------------------------------------------------------
}

public class SyncListUCE_LeaderboardPlayer : SyncList<UCE_LeaderboardPlayer> { }
