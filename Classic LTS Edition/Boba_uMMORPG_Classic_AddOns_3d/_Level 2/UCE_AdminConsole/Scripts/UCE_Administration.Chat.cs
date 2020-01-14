// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// UCE ADMINISTRATION - CHAT

public partial class PlayerChat
{
    protected UCE_AdministrationConsole ac;

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnSubmit")]
    private void OnSubmit_UCE_Administration(string text)
    {
        Player player = Player.localPlayer;
        if (!player || player.UCE_adminLevel <= 0 || string.IsNullOrWhiteSpace(text)) return;

        if (ac == null)
            ac = player.GetComponent<UCE_AdministrationConsole>();

        if (ac != null)
            ac.ProcessCommand(text);
    }
}
