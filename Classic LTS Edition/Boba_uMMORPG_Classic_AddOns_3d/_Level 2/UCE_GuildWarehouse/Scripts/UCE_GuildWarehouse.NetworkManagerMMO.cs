// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;

// NETWORK MANAGER

public partial class NetworkManagerMMO
{
    // -----------------------------------------------------------------------------------
    // OnServerDisconnect
    // @Server
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerDiconnect")]
    private void OnServerDisconnect_UCE_GuildUCE_warehouse(NetworkConnection conn)
    {
        if (conn.identity != null)
            Database.singleton.UCE_SaveGuildWarehouse(conn.identity.GetComponent<Player>());
    }

    // -----------------------------------------------------------------------------------
}
