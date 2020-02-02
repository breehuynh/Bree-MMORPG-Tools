// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// PLAYER

public partial class Player
{
    protected UCE_Popup popup;

    // -----------------------------------------------------------------------------------
    // UCE_ShowPrompt
    // Shows a popup, triggered on the server and sent to the client
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void UCE_ShowPrompt(string message)
    {
        Target_UCE_ShowPrompt(connectionToClient, message);
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_ShowPrompt
    // @Client
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    protected void Target_UCE_ShowPrompt(NetworkConnection target, string message)
    {
        UCE_PopupShow(message);
    }

    // -----------------------------------------------------------------------------------
    // UCE_ShowPopup
    // Shows a popup, triggered on the server and sent to the client
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void UCE_ShowPopup(string message, byte iconId = 0, byte soundId = 0)
    {
        Target_UCE_ShowPopup(connectionToClient, message, iconId, soundId);
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_ShowPopup
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void Target_UCE_ShowPopup(NetworkConnection target, string message, byte iconId, byte soundId)
    {
        if (popup == null)
            popup = GetComponent<UCE_Popup>();

        if (popup != null)
        {
            popup.Prepare(message, iconId, soundId);
            popup.Show();
            if (!popup.forceUseChat)
                UCE_AddMessage(message, 0, false);                                      // todo: add editable color
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_ClientShowPopup
    // Shows a popup, triggered on the client, shown on the client
    //  @Client
    // -----------------------------------------------------------------------------------
    public void UCE_ClientShowPopup(string message, byte iconId, byte soundId)
    {
        if (popup == null)
            popup = GetComponent<UCE_Popup>();

        if (popup != null)
        {
            popup.Prepare(message, iconId, soundId);
            popup.Show();
            if (!popup.forceUseChat)
                UCE_AddMessage(message, 0, false);                                      // todo: add editable color
        }
    }

    // -----------------------------------------------------------------------------------
}
