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

// UCE_Statistic

[Serializable]
public partial struct UCE_Statistic
{
    public string name;
    public long amount;
    public long total;
}

public class SyncListUCE_Statistic : SyncList<UCE_Statistic> { }
