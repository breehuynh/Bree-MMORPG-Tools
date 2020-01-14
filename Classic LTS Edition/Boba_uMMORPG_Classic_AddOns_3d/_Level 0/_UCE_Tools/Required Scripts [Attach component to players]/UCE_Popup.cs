using System;
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// POPUP
// ===================================================================================
[Serializable]
public partial class UCE_Popup : MonoBehaviour
{
    public bool forceUseChat;

    protected UCE_UI_Popup instance;

    [Tooltip("[Optional] Add sounds + icons here and use their Index numbers to use them when showing a popup")]
    public Sprite[] availableIcons;

    public AudioClip[] availableSounds;

    protected string popupLabel;
    protected Sprite popupIcon;
    protected AudioClip popupSoundEffect;
    protected Image popupBackground;

    // -----------------------------------------------------------------------------------
    // Awake
    // -----------------------------------------------------------------------------------
    private void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<UCE_UI_Popup>();
    }

    // -----------------------------------------------------------------------------------
    // Prepare
    // -----------------------------------------------------------------------------------
    public void Prepare(string message, byte iconId = 0, byte soundId = 0)
    {
        popupLabel = message;

        if (availableIcons.Length > 0 && availableIcons.Length >= iconId)
            popupIcon = availableIcons[iconId];

        if (availableSounds.Length > 0 && availableSounds.Length >= soundId)
            popupSoundEffect = availableSounds[soundId];
    }

    // -----------------------------------------------------------------------------------
    // ShowPopup
    // -----------------------------------------------------------------------------------
    public void Show()
    {
        if (instance != null && !forceUseChat)
        {
            if (popupLabel != null)
                instance.popupText.text = popupLabel;

            if (popupSoundEffect != null)
                instance.popupSoundEffect = popupSoundEffect;

            if (popupBackground != null)
                instance.popupBackground = popupBackground;

            if (popupIcon != null)
                instance.popupIcon.sprite = popupIcon;

            if (instance.popupText.text != "")
                instance.Show();
        }
        else if (forceUseChat)
        {
            GetComponent<Player>().chat.AddMsgInfo(popupLabel);
        }
    }

    // -----------------------------------------------------------------------------------
}