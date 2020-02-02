// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// PLAYER STATISTICS

[RequireComponent(typeof(Player))]
public partial class UCE_PlayerStatistics : NetworkBehaviour
{
    [HideInInspector] public SyncListUCE_Statistic UCE_statistics = new SyncListUCE_Statistic();

    // -----------------------------------------------------------------------------------
    // AddStatistic
    // -----------------------------------------------------------------------------------
    public void AddStatistic(string _name, long _amount, long _total=0)
    {
        int idx = UCE_statistics.FindIndex(x => x.name == _name);

        if (idx == -1)
        {
            UCE_Statistic statistic = new UCE_Statistic();
            statistic.name      = _name;
            statistic.amount    = _amount;
            statistic.total     = _total;
            UCE_statistics.Add(statistic);
        }
        else
        {
            UCE_Statistic statistic = UCE_statistics.FirstOrDefault(x => x.name == _name);
            statistic.amount    += _amount;
            if (_total != 0)
                statistic.total += _total;
            else
                statistic.total += _amount;
            UCE_statistics[idx] = statistic;
        }
    }

    // -----------------------------------------------------------------------------------
    // GetAmount
    // -----------------------------------------------------------------------------------
    public long GetAmount(string _name)
    {
        int idx = UCE_statistics.FindIndex(x => x.name == _name);
        if (idx != -1)
            return UCE_statistics[idx].amount;
        return 0;
    }

    // -----------------------------------------------------------------------------------
    // GetTotal
    // -----------------------------------------------------------------------------------
    public long GetTotal(string _name)
    {
        int idx = UCE_statistics.FindIndex(x => x.name == _name);
        if (idx != -1)
            return UCE_statistics[idx].total;
        return 0;
    }

    // -----------------------------------------------------------------------------------
}
