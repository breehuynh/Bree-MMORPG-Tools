// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

// ===================================================================================
// UCE UI NPC ACCESS REQUIREMENT
// ===================================================================================
public partial class UCE_UI_NpcAccessRequirement : UCE_UI_Requirement
{
    protected Npc npc;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    protected override void Update()
    {
        if (!panel.activeSelf) return;

        Player player = Player.localPlayer;
        if (!player) return;

        if (!npc || !UCE_Tools.UCE_CheckSelectionHandling(npc.gameObject))
        {
            npc = null;
            Hide();
        }
    }

#if _iMMONPCRESTRICTIONS

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(Npc _npc)
    {
        Player player = Player.localPlayer;
        if (!player) return;

		npc = _npc;
        requirements = npc.npcRestrictions;

        for (int i = 0; i < content.childCount; ++i)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        updateTextbox();

        interactButton.interactable = requirements.checkRequirements(player);

        interactButton.onClick.SetListener(() =>
        {
            npc.ConfirmAccess();
            Hide();
        });

        panel.SetActive(true);
    }

#endif

    // -----------------------------------------------------------------------------------
}
