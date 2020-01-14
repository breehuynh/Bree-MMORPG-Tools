// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// ===================================================================================
// NPC RESTRICTIONS
// ===================================================================================
public partial class Npc
{
    [Header("[NPC RESTRICTIONS]")]
    public UCE_Requirements npcRestrictions;

    protected UCE_UI_NpcAccessRequirement instance;

    // -----------------------------------------------------------------------------------
    // UCE_ValidateNpcRestrictions
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Awake")]
    private void Awake_UCE_NpcRestrictions()
    {
        if (instance == null)
            instance = FindObjectOfType<UCE_UI_NpcAccessRequirement>();
    }

    // -----------------------------------------------------------------------------------
    // UCE_ValidateNpcRestrictions
    // -----------------------------------------------------------------------------------
    public bool UCE_ValidateNpcRestrictions(Player player)
    {
        bool bValid = npcRestrictions.checkRequirements(player);
        if (!bValid)
            instance.Show(gameObject);
        return bValid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_ValidateNpcRestrictions
    // -----------------------------------------------------------------------------------
    public void ConfirmAccess()
    {
        UINpcDialogue.singleton.Show();
    }

    // -----------------------------------------------------------------------------------
}
