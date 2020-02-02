// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// CAST BAR UI
// ===================================================================================
public partial class UCE_UI_CastBar : MonoBehaviour
{
    public GameObject panel;
    public Slider slider;
    public Text nameText;
    public Text progressText;

    private double duration;
    private double durationRemaining;

    // -----------------------------------------------------------------------------------
    // Update
    // @Client
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (panel.activeSelf)
        {
            if (NetworkTime.time <= durationRemaining)
            {
                float ratio = Convert.ToSingle((durationRemaining - NetworkTime.time) / duration);
                double remain = durationRemaining - NetworkTime.time;
                slider.value = ratio;
                progressText.text = remain.ToString("F1") + "s";
            }
            else
            {
                Hide();
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Show
    // @Client
    // -----------------------------------------------------------------------------------
    public void Show(string labelName, float dura)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        duration = dura;
        durationRemaining = NetworkTime.time + duration;
        nameText.text = labelName;

        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // Hide
    // @Client
    // -----------------------------------------------------------------------------------
    public void Hide()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
}
