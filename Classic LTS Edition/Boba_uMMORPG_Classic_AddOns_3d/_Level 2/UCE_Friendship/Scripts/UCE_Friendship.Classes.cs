// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;

// FRIEND

public partial struct UCE_Friend
{
    public string name;
    public string lastGifted;

    public int level;
    public string _class;
    public string guild;
    public bool online;
    public bool inParty;

    // -----------------------------------------------------------------------------------
    // UCE_Friend (Constructor)
    // -----------------------------------------------------------------------------------
    public UCE_Friend(string _name, string _lastGifted)
    {
        name = _name;
        lastGifted = _lastGifted;

        level = 0;
        _class = "";
        guild = "";
        online = false;
        inParty = false;
    }

    // -----------------------------------------------------------------------------------
}

public class SyncListUCE_Friend : SyncList<UCE_Friend> { }
