// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sets our new hotkeys for wasd movement.
// Creates our commands for passing information on gameplay settings.
public partial class Player : Entity
{
    private UCE_UI_SettingsVariables settingsVariables;
    private float horizontal;
    private float vertical;
    [HideInInspector] [SyncVar] public bool isBlockingTrade = false;
    [HideInInspector] [SyncVar] public bool isBlockingParty = false;
    [HideInInspector] [SyncVar] public bool isBlockingGuild = false;

    // Initialize to avoid warnings.
    private void Start_UCE_Settings()
    {
        settingsVariables = FindObjectOfType<UCE_UI_SettingsVariables>().GetComponent<UCE_UI_SettingsVariables>();
        horizontal = 0; vertical = 0;
        vertical = horizontal;
        horizontal = vertical;
    }

    // Assign our movement hotkeys then check our skillbar hotkeys.
    private void UpdateClient_DSM()
    {
        if (settingsVariables != null)
            if (!UIUtils.AnyInputActive())
            {
                if (Input.GetKey(settingsVariables.keybindings[0])) vertical = 1;
                else if (Input.GetKey(settingsVariables.keybindings[1])) vertical = -1;
                else vertical = 0;

                if (Input.GetKey(settingsVariables.keybindings[2])) horizontal = -1;
                else if (Input.GetKey(settingsVariables.keybindings[3])) horizontal = 1;
                else horizontal = 0;
            }

        if (settingsVariables != null)
            UpdateHotkeys();
    }

    // If a skillbar hotkey is updated then set its new hotkey.
    private void UpdateHotkeys()
    {
        if (settingsVariables.keybindUpdate[7] || settingsVariables.keybindUpdate[8] || settingsVariables.keybindUpdate[9] || settingsVariables.keybindUpdate[10] ||
            settingsVariables.keybindUpdate[11] || settingsVariables.keybindUpdate[12] || settingsVariables.keybindUpdate[13] || settingsVariables.keybindUpdate[14] ||
            settingsVariables.keybindUpdate[15] || settingsVariables.keybindUpdate[16])
        {
            for (int i = 0; i < 10; i++)
            {
                skillbar[i].hotKey = settingsVariables.keybindings[(i + 1) + 6];
                settingsVariables.keybindUpdate[(i + 1) + 6] = false;
            }
        }
    }

    #region Commands

    [Command]
    public void CmdBlockPartyInvite(bool block)
    {
        isBlockingParty = block;
    }

    [Command]
    public void CmdBlockGuildInvite(bool block)
    {
        isBlockingGuild = block;
    }

    [Command]
    public void CmdBlockTradeRequest(bool block)
    {
        isBlockingTrade = block;
    }

    #endregion Commands
}
