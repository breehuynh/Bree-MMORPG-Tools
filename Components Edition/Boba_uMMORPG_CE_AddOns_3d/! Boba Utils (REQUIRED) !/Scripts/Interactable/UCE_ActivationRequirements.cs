// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// ACTIVATION REQUIREMENTS CLASS
// THIS CLASS IS FOR OBJECTS THAT ARE ACTIVATED AUTOMATICALLY IF CERTAIN CRITERIA ARE MET

[System.Serializable]
public partial class UCE_ActivationRequirements : UCE_Requirements
{
#if _iMMOBUILDSYSTEM

    [Header("[UCE BUILD SYTEM REQUIREMENTS]")]
    [Tooltip("[Optional] Build System - only the owner character of the structure can access it?")]
    public bool ownerCharacterOnly;

    [Tooltip("[Optional] Build System - only the owner guild of the structure can access it?")]
    public bool ownerGuildOnly;

    [Tooltip("[Optional] Build System - will reverse both checks from above and only activate when non owner / non guild members access")]
    public bool reverseCheck;
#endif

    protected GameObject parent;

    // -----------------------------------------------------------------------------------
    // setParent
    // -----------------------------------------------------------------------------------
    public void setParent(GameObject myParent)
    {
        parent = myParent;
    }

    // -----------------------------------------------------------------------------------
    // checkRequirements
    // -----------------------------------------------------------------------------------
    public override bool checkRequirements(Player player)
    {
        bool valid = true;

        valid = base.checkRequirements(player);

#if _iMMOBUILDSYSTEM
        valid = (checkBuildSystem(player) == true) ? valid : false;
#endif

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // checkBuildSystem
    // -----------------------------------------------------------------------------------
#if _iMMOBUILDSYSTEM

    public bool checkBuildSystem(Player player)
    {
        if (!parent || !parent.GetComponentInParent<UCE_PlaceableObject>()) return true;

        UCE_PlaceableObject po = parent.GetComponentInParent<UCE_PlaceableObject>();

        if (po == null || (!ownerCharacterOnly && !ownerGuildOnly)) return true;

        return
                (!ownerCharacterOnly || (!reverseCheck && ownerCharacterOnly && player.name == po.ownerCharacter || reverseCheck && ownerCharacterOnly && player.name != po.ownerCharacter)) &&
                (!ownerGuildOnly || (!reverseCheck && ownerGuildOnly && player.guild.name == po.ownerGuild || reverseCheck && ownerGuildOnly && player.guild.name != po.ownerGuild));
    }

#endif

    // -----------------------------------------------------------------------------------
}
