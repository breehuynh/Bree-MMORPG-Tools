// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// Player

partial class Player
{
    [HideInInspector] public bool targetButtonPressed = false;

    // simple tab targeting for buttons
    [Client]
    public void TargetNearestButton()
    {
        targetButtonPressed = true;
    }

    public void UpdateClient_MobileControls()
    {
        if ((Input.GetMouseButtonDown(0) || UCE_Tools.GetTouchDown) && !Utils.IsCursorOverUserInterface())
            UCE_Tools.pointerDown = true;
    }
}
