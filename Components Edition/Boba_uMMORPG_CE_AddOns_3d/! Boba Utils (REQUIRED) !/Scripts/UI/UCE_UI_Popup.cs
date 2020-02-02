// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// POPUP - UI

public class UCE_UI_Popup : MonoBehaviour
{
    public GameObject panel;
    public Text popupText;
    public Image popupBackground;
    public Image popupIcon;
    [Range(0, 30)] public float displayDuration;
    [HideInInspector] public AudioClip popupSoundEffect;

    protected AudioSource audioSource;

    // -----------------------------------------------------------------------------------
    // Show
    // @Client
    // -----------------------------------------------------------------------------------
    public void Show()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        panel.SetActive(true);

        FadeIn(displayDuration / 4);

        audioSource = GetComponent<AudioSource>();

        if (popupSoundEffect != null)
            audioSource.PlayOneShot(popupSoundEffect);

        if (displayDuration > 0)
        {
            Invoke("FadeOut", displayDuration / 2);
            Invoke("Hide", displayDuration);
        }
    }

    // -----------------------------------------------------------------------------------
    // FadeIn
    // @Client
    // -----------------------------------------------------------------------------------
    protected void FadeIn(float duration)
    {
        // set to transparent
        popupText.canvasRenderer.SetAlpha(0.01f);
        popupBackground.canvasRenderer.SetAlpha(0.01f);
        popupIcon.canvasRenderer.SetAlpha(0.01f);
        // now fade in
        popupText.CrossFadeAlpha(1, duration, true);
        popupBackground.CrossFadeAlpha(1, duration, true);
        popupIcon.CrossFadeAlpha(1, duration, true);
    }

    // -----------------------------------------------------------------------------------
    // FadeOut
    // @Client
    // -----------------------------------------------------------------------------------
    protected void FadeOut()
    {
        popupText.CrossFadeAlpha(0, displayDuration / 4, true);
        popupBackground.CrossFadeAlpha(0, displayDuration / 4, true);
        popupIcon.CrossFadeAlpha(0, displayDuration / 4, true);
    }

    // -----------------------------------------------------------------------------------
    // Hide
    // @Client
    // -----------------------------------------------------------------------------------
    protected void Hide()
    {
        popupSoundEffect = null;
        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
}
