// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;

// PLAYER

public partial class Player
{
    protected UIMinimap minimap;

    // -----------------------------------------------------------------------------------
    // UCE_MinimapSceneText
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void UCE_MinimapSceneText(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        Target_UCE_MinimapSceneText(connectionToClient, name);
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_MinimapSceneText
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void Target_UCE_MinimapSceneText(NetworkConnection target, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        if (minimap == null)
            minimap = FindObjectOfType<UIMinimap>();

        if (minimap != null && minimap.sceneText != null)
            minimap.sceneText.text = name;
    }

    // -----------------------------------------------------------------------------------
}
