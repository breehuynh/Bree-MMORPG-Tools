// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;

// UCE TIMEGATE

[System.Serializable]
public struct UCE_Timegate
{
    public string name;
    public int count;
    public string hours;
    public bool valid;
}

public class SyncListUCE_Timegate : SyncList<UCE_Timegate> { }
