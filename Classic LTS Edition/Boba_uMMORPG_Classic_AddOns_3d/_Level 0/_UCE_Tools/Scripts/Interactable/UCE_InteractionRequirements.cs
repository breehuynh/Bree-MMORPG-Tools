// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Linq;
using UnityEngine;

// INTERACTION REQUIREMENTS CLASS
// THIS CLASS IS PRIMARILY FOR OBJECTS THE PLAYER CAN CHOOSE TO INTERACT WITH

[System.Serializable]
public partial class UCE_InteractionRequirements : UCE_Requirements
{
    [Header("[-=-=-=- UCE COSTS [Removed after checking Requirements] -=-=-=-]")]
    [Tooltip("[Optional] These items will be removed from players inventory")]
    public UCE_ItemRequirement[] removeItems;

    [Tooltip("[Optional] Gold cost to interact")]
    public long goldCost = 0;
    [Tooltip("[Optional] Coins cost to interact")]
    public long coinCost = 0;
    [Tooltip("[Optional] Health cost to interact")]
    public int healthCost = 0;
    [Tooltip("[Optional] Mana cost to interact")]
    public int manaCost = 0;

#if _iMMOHONORSHOP
    [Tooltip("[Optional] Honor Currency costs to interact")]
    public UCE_HonorShopCurrencyDrop[] honorCurrencyCost;
#endif

    [Header("[-=-=-=- UCE REWARDS [awarded after checks & costs (repetitive)] -=-=-=-]")]
    public int expRewardMin                     = 0;
    public int expRewardMax                     = 0;
    public int skillExpRewardMin                = 0;
    public int skillExpRewardMax                = 0;

    [Header("-=-=-=- Chat Messages (automatic activation only) -=-=-=-")]
    public string labelMinLevel                 = " Minimum Level: ";
    public string labelMaxLevel                 = " Maximum Level: ";
    public string labelMinHealth                = " Min. Health Percent: ";
    public string labelMinMana                  = " Min. Mana Percent: ";
    public string labelDayStart                 = " Start Day: ";
    public string labelDayEnd                   = " End Day: ";
    public string labelActiveMonth              = " Active Month: ";
    public string labelRequiredSkills           = " Skill(s): ";
    public string labelLevel                    = "LV";
    public string labelAllowedClasses           = " Allowed Class(es): ";
    public string labelRequiresGuild            = " Guild Membership";
    public string labelRequiresParty            = " Party Membership";
#if _iMMOPRESTIGECLASSES
    public string labelAllowedPrestigeClasses   = " Allowed Prestige Class(es): ";
#endif
#if _iMMOPVP
    public string labelRequiresRealm            = " Limited to Specific Realm";
#endif
    public string labelRequiresQuest            = " Quest: ";
    public string labelInProgressQuest          = "[Must be in progress]";
#if _iMMOFACTIONS
    public string labelFactionRequirements      = " Faction Ratings: ";
#endif
    public string labelRequiredEquipment        = " Equipment: ";
    public string labelRequiredItems            = " Item(s): ";
    public string labelDestroyItem              = "[Destroyed on use]";
#if _iMMOHARVESTING
    public string requiredHarvestProfessions    = " Harvesting Profession(s): ";
#endif
#if _iMMOCRAFTING
    public string requiredCraftProfessions      = " Craft Profession(s): ";
#endif
#if _iMMOMOUNTS
    public string labelMountedOnly              = " Accessible only while mounted.";
    public string labelUnmountedOnly            = " Accessible only while unmounted.";
#endif
#if _iMMOTRAVEL
    public string labelTravelroute              = " Travelroute: ";
#endif
#if _iMMOWORLDEVENTS
    public string labelWorldEvent               = " World Event: ";
#endif
#if _iMMOGUILDUPGRADES
    public string labelGuildUpgrades            = " Guild Level: ";
#endif
#if _iMMOACCOUNTUNLOCKABLES
    public string labelAccountUnlockable        = " Account Unlockable: ";
#endif
#if _iMMOPATREON
    public string labelPatreonSubscription      = " - Requires active Patreon subscription.";
#endif

    public ChannelInfo requires = new ChannelInfo("", "(Requires)", "(Requires)", null);

    // -----------------------------------------------------------------------------------
    // checkRequirements
    // -----------------------------------------------------------------------------------
    public override bool checkRequirements(Player player)
    {
        bool valid = true;

        valid = base.checkRequirements(player);

        valid = checkCosts(player, valid);

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // checkCosts
    // -----------------------------------------------------------------------------------
    public bool checkCosts(Player player, bool valid)
    {
        valid = (removeItems.Length == 0 || player.UCE_checkHasItems(removeItems)) ? valid : false;
        valid = (goldCost == 0 || player.gold >= goldCost) ? valid : false;
        valid = (coinCost == 0 || player.coins >= coinCost) ? valid : false;
        valid = (healthCost == 0 || player.health >= healthCost) ? valid : false;
        valid = (manaCost == 0 || player.mana >= manaCost) ? valid : false;
#if _iMMOHONORSHOP
        valid = (player.UCE_CheckHonorCurrencyCost(honorCurrencyCost)) ? valid : false;
#endif

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // hasCosts
    // -----------------------------------------------------------------------------------
    public bool hasCosts()
    {
        return removeItems.Length > 0 ||
                goldCost > 0 ||
                coinCost > 0 ||
                healthCost > 0 ||
                manaCost > 0
#if _iMMOHONORSHOP
                || honorCurrencyCost.Any(x => x.amount > 0)
#endif
                ;
    }

    // -----------------------------------------------------------------------------------
    // payCosts
    // -----------------------------------------------------------------------------------
    public void payCosts(Player player)
    {
        if (checkRequirements(player))
        {
            player.UCE_removeItems(removeItems, true);

            player.gold -= goldCost;
            player.coins -= coinCost;

            player.mana -= manaCost;

            if (player.health > healthCost)
                player.health -= healthCost;
            else
                player.health = 1;

#if _iMMOHONORSHOP
            player.UCE_PayHonorCurrencyCost(honorCurrencyCost);
#endif
        }
    }

    // -----------------------------------------------------------------------------------
    // grantRewards
    // -----------------------------------------------------------------------------------
    public void grantRewards(Player player)
    {
        if (checkRequirements(player))
        {
            player.experience += Random.Range(expRewardMin, expRewardMax);
            player.skillExperience += Random.Range(skillExpRewardMin, skillExpRewardMax);
        }
    }


    // -----------------------------------------------------------------------------------
    // Update Required Chat (Auto Activation)
    // @Client
    // -----------------------------------------------------------------------------------
    public virtual void UpdateRequirementChat()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        // -- Requirements

        if (minLevel > 0)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelMinLevel + minLevel.ToString(), "", requires.textPrefab));

        if (maxLevel > 0)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelMaxLevel + maxLevel.ToString(), "", requires.textPrefab));

        if (minHealth > 0)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelMinHealth + minHealth.ToString(), "", requires.textPrefab));

        if (minMana > 0)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelMinMana + minMana.ToString(), "", requires.textPrefab));

        // TIME
        if (dayStart > 0)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelDayStart + dayStart.ToString(), "", requires.textPrefab));

        if (dayEnd > 0)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelDayEnd + dayEnd.ToString(), "", requires.textPrefab));

        if (activeMonth > 0)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelActiveMonth + activeMonth.ToString(), "", requires.textPrefab));

        if (requiredSkills.Length > 0)
        {
            foreach (UCE_SkillRequirement skill in requiredSkills)
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelRequiredSkills + skill.skill.name + labelLevel + skill.level.ToString(), "", requires.textPrefab));
        }

        // CLASS

        if (allowedClasses.Length > 0)
        {
            string temp_classes = "";
            foreach (GameObject classes in allowedClasses)
                temp_classes += " " + classes.name;
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelAllowedClasses + temp_classes, "", requires.textPrefab));
        }

        // PARTY & GUILD

        if (requiresParty)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelRequiresParty, "", requires.textPrefab));

        if (requiresGuild)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelRequiresGuild, "", requires.textPrefab));

#if _iMMOPRESTIGECLASSES
        if (allowedPrestigeClasses.Length > 0)
        {
            string temp_classes = "";
            foreach (UCE_PrestigeClassTemplate classes in allowedPrestigeClasses)
                temp_classes += " " + classes.name;
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelAllowedPrestigeClasses + temp_classes, "", requires.textPrefab));
        }
#endif

#if _iMMOPVP
        if (requiredRealm != null && requiredAlly != null)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelRequiresRealm, "", requires.textPrefab));
#endif

#if _iMMOQUESTS
        if (requiredQuest != null)
        {
            if (!questMustBeInProgress)
            {
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelRequiresQuest + requiredQuest.name, "", requires.textPrefab));
            }
            else
            {
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelRequiresQuest + requiredQuest.name + labelInProgressQuest, "", requires.textPrefab));
            }
        }
#else
        if (requiredQuest != null)
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelRequiresQuest + requiredQuest.name, "", requires.textPrefab));
#endif

#if _iMMOFACTIONS
        if (factionRequirements.Length > 0)
        {
            foreach (UCE_FactionRequirement factionRequirement in factionRequirements)
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelFactionRequirements + factionRequirement.faction.name, "", requires.textPrefab));
        }
#endif

        if (requiredEquipment.Length > 0)
        {
            foreach (EquipmentItem item in requiredEquipment)
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelRequiredEquipment + item.name, "", requires.textPrefab));
        }

        if (requiredItems.Length > 0)
        {
            foreach (UCE_ItemRequirement item in requiredItems)
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelRequiredItems + item.item.name, "", requires.textPrefab));
        }

#if _iMMOHARVESTING
        if (harvestProfessionRequirements.Length > 0)
        {
            foreach (UCE_HarvestingProfessionRequirement prof in harvestProfessionRequirements)
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, requiredHarvestProfessions + prof.template.name + " " + labelLevel + prof.level, "", requires.textPrefab));
        }
#endif

#if _iMMOCRAFTING
        if (craftProfessionRequirements.Length > 0)
        {
            foreach (UCE_CraftingProfessionRequirement prof in craftProfessionRequirements)
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, requiredCraftProfessions + prof.template.name + " " + labelLevel + prof.level, "", requires.textPrefab));
        }
#endif

#if _iMMOMOUNTS
        if (mountType == UCE_Requirements.MountType.Mounted)
        {
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelMountedOnly, "", requires.textPrefab));
        }
        else if (mountType == UCE_Requirements.MountType.Unmounted)
        {
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelUnmountedOnly, "", requires.textPrefab));
        }
#endif

#if _iMMOTRAVEL
        if (!string.IsNullOrWhiteSpace(requiredTravelrouteName))
        {
            UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelTravelroute + requiredTravelrouteName, "", requires.textPrefab));
        }
#endif

#if _iMMOWORLDEVENTS
        if (worldEvent != null)
        {
            if (player.UCE_CheckWorldEvent(worldEvent, minEventCount, maxEventCount))
            {
                if (maxEventCount == 0)
                    UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelWorldEvent + worldEvent.name + " (" + player.UCE_GetWorldEventCount(worldEvent) + "/" + minEventCount.ToString() + ")", "", requires.textPrefab));
                else
                    UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelWorldEvent + worldEvent.name + " (" + minEventCount.ToString() + "-" + maxEventCount.ToString() + ") [" + player.UCE_GetWorldEventCount(worldEvent) + "]", "", requires.textPrefab));
            }
            else
            {
                if (maxEventCount == 0)
                    UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelWorldEvent + worldEvent.name + " (" + player.UCE_GetWorldEventCount(worldEvent) + "/" + minEventCount.ToString() + ")", "", requires.textPrefab));
                else
                    UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelWorldEvent + worldEvent.name + " (" + minEventCount.ToString() + "-" + maxEventCount.ToString() + ") [" + player.UCE_GetWorldEventCount(worldEvent) + "]", "", requires.textPrefab));
            }
        }
#endif

#if _iMMOGUILDUPGRADES
        if (minGuildLevel > 0)
        {
            if (player.InGuild())
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelGuildUpgrades + player.guildLevel.ToString() + "/" + minGuildLevel.ToString(), "", requires.textPrefab));
            else
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelGuildUpgrades + "0/" + minGuildLevel.ToString(), "", requires.textPrefab));
        }
#endif

#if _iMMOACCOUNTUNLOCKABLES
        if (!string.IsNullOrWhiteSpace(accountUnlockable))
        {
            if (player.UCE_HasAccountUnlock(accountUnlockable))
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelAccountUnlockable + accountUnlockable, "", requires.textPrefab));
            else
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelAccountUnlockable + accountUnlockable, "", requires.textPrefab));
        }
#endif

#if _iMMOPATREON
        if (activeMinPatreon > 0)
        {
            if (player.UCE_HasActivePatreonSubscription(activeMinPatreon))
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelPatreonSubscription, "", requires.textPrefab));
            else
                UIChat.singleton.AddMessage(new ChatMessage("", requires.identifierIn, labelPatreonSubscription, "", requires.textPrefab));
        }
#endif

    }

    // -----------------------------------------------------------------------------------
}
