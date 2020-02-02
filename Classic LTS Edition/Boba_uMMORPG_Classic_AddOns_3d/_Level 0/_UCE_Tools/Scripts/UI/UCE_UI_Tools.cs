// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE UI TOOLS

public static partial class UCE_UI_Tools
{
    private static UCE_UI_CanvasOverlay canvasOverlayInstance;

    // -----------------------------------------------------------------------------------
    // FadeOutScreen
    // @Client
    // -----------------------------------------------------------------------------------
    public static void FadeOutScreen(bool automatic = true, float fDuration = 0f)
    {
        if (canvasOverlayInstance == null)
            canvasOverlayInstance = GameObject.FindObjectOfType<UCE_UI_CanvasOverlay>();

        if (canvasOverlayInstance != null)
            if (automatic)
                canvasOverlayInstance.AutoFadeOut(fDuration);
            else
                canvasOverlayInstance.FadeOut(fDuration);
    }

    // -----------------------------------------------------------------------------------
    // FadeInScreen
    // @Client
    // -----------------------------------------------------------------------------------
    public static void FadeInScreen(float fDelay = 0f)
    {
        if (canvasOverlayInstance == null)
            canvasOverlayInstance = GameObject.FindObjectOfType<UCE_UI_CanvasOverlay>();

        if (canvasOverlayInstance != null)
            if (fDelay != 0)
                canvasOverlayInstance.FadeInDelayed(fDelay);
            else
                canvasOverlayInstance.FadeIn();
    }

    // -----------------------------------------------------------------------------------
}
