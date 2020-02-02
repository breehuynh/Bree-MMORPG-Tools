// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// TIMEGATE UI
// ===================================================================================
public partial class UCE_UI_Timegate : MonoBehaviour
{
    public GameObject panel;
    public Button TimegateButton;
    public Transform content;
    public ScrollRect scrollRect;
    public GameObject textPrefab;

    private const string MSG_HEADING = "Timegate available:";
    private const string MSG_TIMEGATE = "Timegate to: ";
    private const string MSG_MAXVISITS = " - Max entrances per player: ";
    private const string MSG_MAXHOURS = " - Time between entrances: ";
    private const string MSG_OPENFROM = " - Opening Day(s): ";
    private const string MSG_OPENON = " - Opening Month: ";
    private const string MSG_HOURS = " hrs";

    private bool okTimegate = false;
    private bool okDayStart = false;
    private bool okDayEnd = false;
    private bool okMonth = false;
    private bool okVisits = false;
    private bool okHours = false;

    // -----------------------------------------------------------------------------------
    // validateTimegate
    // -----------------------------------------------------------------------------------
    private void validateTimegate()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        UCE_Area_Timegate Timegate = player.UCE_myTimegate;

        if (Timegate)
        {
            int idx = player.UCE_GetTimegateIndexByName(Timegate.name);

            if (idx > -1 && player.UCE_timegates[idx].valid)
            {
                okVisits = (Timegate.maxVisits == 0 || player.UCE_timegates[idx].count < Timegate.maxVisits);
                okHours = (Timegate.hoursBetweenVisits == 0 || player.UCE_validateTimegateTime(player.UCE_timegates[idx].hours, Timegate.hoursBetweenVisits));
            }
            else
            {
                okVisits = true;
                okHours = true;
            }

            okTimegate = (Timegate.teleportationTarget.Valid) ? true : false;
            okDayStart = (Timegate.dayStart == 0 || Timegate.dayStart <= DateTime.UtcNow.Day) ? true : false;
            okDayEnd = (Timegate.dayEnd == 0 || Timegate.dayEnd >= DateTime.UtcNow.Day) ? true : false;
            okMonth = (Timegate.activeMonth == 0 || Timegate.activeMonth == DateTime.UtcNow.Month) ? true : false;

            okTimegate = okDayStart && okDayEnd && okMonth && okHours && okVisits;
        }
        else
        {
            okTimegate = false;
        }
    }

    // -----------------------------------------------------------------------------------
    // updateTextbox
    // -----------------------------------------------------------------------------------
    public void updateTextbox()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        string endDay = "";
        UCE_Area_Timegate Timegate = player.UCE_myTimegate;

        if (Timegate)
        {
            int idx = player.UCE_GetTimegateIndexByName(Timegate.name);

            AddMessage(MSG_HEADING, Color.white);
            if (idx > -1 && Timegate.maxVisits != 0)
            {
                AddMessage(MSG_MAXVISITS + player.UCE_timegates[idx].count.ToString() + " / " + Timegate.maxVisits.ToString(), okVisits ? Color.green : Color.red);
            }
            else if (Timegate.maxVisits != 0)
            {
                AddMessage(MSG_MAXVISITS + "0 / " + Timegate.maxVisits.ToString(), okVisits ? Color.green : Color.red);
            }
            if (Timegate.hoursBetweenVisits != 0) AddMessage(MSG_MAXHOURS + Timegate.hoursBetweenVisits.ToString() + MSG_HOURS, okHours ? Color.green : Color.red);
            if (Timegate.dayEnd != 0) endDay = ". - " + Timegate.dayEnd.ToString() + ".";
            if (Timegate.dayStart != 0) AddMessage(MSG_OPENFROM + Timegate.dayStart.ToString() + endDay, okDayStart && okDayEnd ? Color.green : Color.red);
            if (Timegate.activeMonth != 0) AddMessage(MSG_OPENON + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Timegate.activeMonth), okMonth ? Color.green : Color.red);
        }
    }

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        for (int i = 0; i < content.childCount; ++i)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        UCE_Area_Timegate Timegate = player.UCE_myTimegate;

        if (Timegate)
        {
            validateTimegate();
            if (okTimegate) TimegateButton.GetComponentInChildren<Text>().text = MSG_TIMEGATE + Timegate.teleportationTarget.name;

            TimegateButton.interactable = okTimegate;

            TimegateButton.onClick.SetListener(() =>
            {
                UCE_UI_Tools.FadeOutScreen();
                player.Cmd_UCE_SimpleTimegate();
            });
        }

        updateTextbox();
        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // Hide
    // -----------------------------------------------------------------------------------
    public void Hide()
    {
        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;
        if (player.UCE_myTimegate == null)
        {
            Hide();
        }
        else
        {
            validateTimegate();
        }
    }

    // -----------------------------------------------------------------------------------
    // AutoScroll
    // -----------------------------------------------------------------------------------
    private void AutoScroll()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    // -----------------------------------------------------------------------------------
    // AddMessage
    // -----------------------------------------------------------------------------------
    public void AddMessage(string msg, Color color)
    {
        GameObject go = Instantiate(textPrefab);
        go.transform.SetParent(content.transform, false);
        go.GetComponent<Text>().text = msg;
        go.GetComponent<Text>().color = color;
        AutoScroll();
    }
}

// ===================================================================================
