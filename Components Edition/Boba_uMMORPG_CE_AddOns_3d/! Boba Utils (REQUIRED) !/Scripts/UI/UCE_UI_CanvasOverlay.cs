// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// CANVAS OVERLAY - UI

public class UCE_UI_CanvasOverlay : MonoBehaviour
{
    // -----------------------------------------------------------------------------------
    // Awake
    // @Client
    // -----------------------------------------------------------------------------------
    private void Awake()
    {
        LeanTween.init();
        LeanTween.alpha(this.gameObject.GetComponent<RectTransform>(), 0f, 0f);
    }

    // -----------------------------------------------------------------------------------
    // FadeOut
    // @Client
    // -----------------------------------------------------------------------------------
    public void FadeOut(float fDuration = 0f)
    {
        LeanTween.alpha(this.gameObject.GetComponent<RectTransform>(), 1f, fDuration);
    }

    // -----------------------------------------------------------------------------------
    // AutoFadeOut
    // @Client
    // -----------------------------------------------------------------------------------
    public void AutoFadeOut(float fDuration = 0f)
    {
        LeanTween.alpha(this.gameObject.GetComponent<RectTransform>(), 1f, fDuration);
        Invoke("FadeIn", 0.5f);
    }

    // -----------------------------------------------------------------------------------
    // FadeIn
    // @Client
    // -----------------------------------------------------------------------------------
    public void FadeIn(float fDuration = 0.5f)
    {
        LeanTween.alpha(this.gameObject.GetComponent<RectTransform>(), 0f, fDuration);
    }

    // -----------------------------------------------------------------------------------
    // FadeIn
    // Same method without parameters for Invoke
    // @Client
    // -----------------------------------------------------------------------------------
    public void FadeIn()
    {
        LeanTween.alpha(this.gameObject.GetComponent<RectTransform>(), 0f, 0.5f);
    }

    // -----------------------------------------------------------------------------------
    // FadeInDelayed
    // @Client
    // -----------------------------------------------------------------------------------
    public void FadeInDelayed(float fDelay = 0f)
    {
        Invoke("FadeIn", fDelay);
    }

    // -----------------------------------------------------------------------------------
}
