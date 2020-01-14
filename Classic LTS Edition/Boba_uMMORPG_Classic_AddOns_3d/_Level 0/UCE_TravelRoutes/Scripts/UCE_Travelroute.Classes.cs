// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;

// TRAVELROUTE - CLASS

[System.Serializable]
public struct UCE_TravelrouteClass
{
    public string name;

    public UCE_TravelrouteClass(string _name)
    {
        name = _name;
    }
}

public class SyncListUCE_TravelrouteClass : SyncList<UCE_TravelrouteClass> { }
