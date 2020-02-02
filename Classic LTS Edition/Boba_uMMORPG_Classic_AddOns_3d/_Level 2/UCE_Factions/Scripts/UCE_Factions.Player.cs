// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;
using UnityEngine;

// PLAYER

public partial class Player
{
    [Header("-=-=-=- UCE FACTIONS -=-=-=-")]
    public UCE_FactionRating[] startingFactions;

    public string messageFactionModified = "' alignment shifted by ";

    [HideInInspector] public SyncListUCE_Faction UCE_Factions;

    // -----------------------------------------------------------------------------------
    // UCE_AddFactionRating
    // -----------------------------------------------------------------------------------
    public void UCE_AddFactionRating(UCE_Tmpl_Faction faction, int ratingAmount)
    {
        if (faction == null || ratingAmount == 0) return;

        int rating = UCE_GetFactionRating(faction);

        if (rating == -99999)
        {
            UCE_Faction f = new UCE_Faction();
            f.name = faction.name;
            f.rating = ratingAmount;

            UCE_Factions.Add(f);
        }
        else
        {
            int idx = UCE_Factions.FindIndex(x => x.name == faction.name);
            UCE_Faction f = UCE_Factions.FirstOrDefault(x => x.name == faction.name);
            f.rating += ratingAmount;
            UCE_Factions[idx] = f;
        }

        UCE_TargetAddMessage(faction.name + messageFactionModified + ratingAmount.ToString());
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetFactionRating
    // -----------------------------------------------------------------------------------
    public int UCE_GetFactionRating(UCE_Tmpl_Faction faction)
    {
        if (faction == null) return -99999;
        int idx = UCE_Factions.FindIndex(x => x.name == faction.name);
        if (idx != -1)
            return UCE_Factions[idx].rating;
        return -99999;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckFactionRatings
    // -----------------------------------------------------------------------------------
    public bool UCE_CheckFactionRatings(UCE_FactionRequirement[] factionRequirements, bool requiresAll = false)
    {
        if (factionRequirements.Length == 0) return true;

        bool valid = true;

        foreach (UCE_FactionRequirement factionRequirement in factionRequirements)
        {
            valid = UCE_CheckFactionRating(factionRequirement);
            if (valid && !requiresAll) return true;
        }

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckFactionRating
    // -----------------------------------------------------------------------------------
    public bool UCE_CheckFactionRating(UCE_FactionRequirement factionRequirement)
    {
        if (factionRequirement.faction == null) return true;

        int rating = UCE_GetFactionRating(factionRequirement.faction);

        if (rating >= factionRequirement.min && rating <= factionRequirement.max)
            return true;
        else
            return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckFactionRating
    // -----------------------------------------------------------------------------------
    public bool UCE_CheckFactionRating(UCE_FactionQuest factionRequirement)
    {
        if (factionRequirement.faction == null) return true;

        int rating = UCE_GetFactionRating(factionRequirement.faction);

        if (rating >= factionRequirement.min)
            return true;
        else
            return false;
    }

    // -----------------------------------------------------------------------------------
}
