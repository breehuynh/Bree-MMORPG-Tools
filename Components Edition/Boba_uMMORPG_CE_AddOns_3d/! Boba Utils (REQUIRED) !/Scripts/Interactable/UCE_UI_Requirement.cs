// =======================================================================================
// Maintained by bobatea#9400 on Discord
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
// UCE UI ACCESS REQUIREMENT
// ===================================================================================
public partial class UCE_UI_Requirement : MonoBehaviour
{
    [Header("-=-=-=- Required Assignments -=-=-=-")]
    public GameObject panel;
    public Button interactButton;
    public Button cancelButton;
    public Transform content;
    public ScrollRect scrollRect;
    public GameObject textPrefab;

    [Header("-=-=-=- Configureable Colors -=-=-=-")]
    public Color headingColor;
    public Color textColor;
    public Color errorColor;

    [Header("-=-=-=- Configureable Labels -=-=-=-")]
    public string labelHeading = "Interaction requirements:";
    public string labelMinLevel = " - Required minimum Level: ";
    public string labelMaxLevel = " - Required maximum level: ";

    public string labelMinHealth = " - Min. Health Percent: ";
    public string labelMinMana = " - Min. Mana Percent: ";

    public string labelDayStart = " - Start Day: ";
    public string labelDayEnd = " - End Day: ";
    public string labelActiveMonth = " - Active Month: ";

    public string labelRequiredSkills = " - Required Skill(s): ";
    public string labelLevel = "LV";
    public string labelAllowedClasses = " - Allowed Class(es): ";
    public string labelRequiresGuild = " - Requires guild membership.";
    public string labelRequiresParty = " - Requires party membership.";
#if _iMMOPRESTIGECLASSES
    public string labelAllowedPrestigeClasses 		= " - Allowed Prestige Class(es): ";
#endif
#if _iMMOPVP
    public string labelRequiresRealm 				= " - Limited to specific Realm.";
#endif
    public string labelRequiresQuest = " - Requires Quest: ";
    public string labelInProgressQuest = "[Must be in progress]";
#if _iMMOFACTIONS
    public string labelFactionRequirements 			= " - Required faction ratings:";
#endif

    public string labelRequiredEquipment = " - Required equipment: ";
    public string labelRequiredItems = " - Required item(s): ";
    public string labelDestroyItem = "[Destroyed on use]";
#if _iMMOHARVESTING
    public string requiredHarvestProfessions 		= " - Requires Harvesting Profession(s):";
#endif
#if _iMMOCRAFTING
    public string requiredCraftProfessions 			= " - Requires Craft Profession(s):";
#endif
#if _iMMOMOUNTS
    public string labelMountedOnly 					= " - Accessible only while mounted.";
    public string labelUnmountedOnly 				= " - Accessible only while unmounted.";
#endif
#if _iMMOTRAVEL
	public string labelTravelroute					= " - Required Travelroute:";
#endif
#if _iMMOWORLDEVENTS
	public string labelWorldEvent 					= " - Required World Event:";
#endif
#if _iMMOGUILDUPGRADES
	public string labelGuildUpgrades 				= " - Required Guild Level:";
#endif
#if _iMMOACCOUNTUNLOCKABLES
	public string labelAccountUnlockable			= " - Required Account Unlockable:";
#endif
#if _iMMOPATREON
    public string labelPatreonSubscription          = " - Requires active Patreon subscription.";
#endif

    protected UCE_Requirements requirements;

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public virtual void Show(GameObject go)
    {
        go.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // updateTextbox
    // -----------------------------------------------------------------------------------
    protected virtual void updateTextbox()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        AddMessage(labelHeading, headingColor);

        // -- Requirements

        if (requirements.minLevel > 0)
            AddMessage(labelMinLevel + requirements.minLevel.ToString(), player.level.current >= requirements.minLevel ? textColor : errorColor);

        if (requirements.maxLevel > 0)
            AddMessage(labelMaxLevel + requirements.maxLevel.ToString(), player.level.current <= requirements.maxLevel ? textColor : errorColor);

        if (requirements.minHealth > 0)
            AddMessage(labelMinHealth + requirements.minHealth.ToString(), player.health.Percent() >= requirements.minHealth ? textColor : errorColor);

        if (requirements.minMana > 0)
            AddMessage(labelMinMana + requirements.minMana.ToString(), player.mana.Percent() >= requirements.minMana ? textColor : errorColor);

        //TIME
        if (requirements.dayStart > 0)
            AddMessage(labelDayStart + requirements.dayStart.ToString(), requirements.dayStart <= DateTime.UtcNow.Day ? textColor : errorColor);
        if (requirements.dayEnd > 0)
            AddMessage(labelDayEnd + requirements.dayEnd.ToString(), requirements.dayEnd >= DateTime.UtcNow.Day ? textColor : errorColor);
        if (requirements.activeMonth > 0)
            AddMessage(labelActiveMonth + requirements.activeMonth.ToString(), requirements.activeMonth == DateTime.UtcNow.Month ? textColor : errorColor);

        if (requirements.requiredSkills.Length > 0)
        {
            AddMessage(labelRequiredSkills, textColor);
            foreach (UCE_SkillRequirement skill in requirements.requiredSkills)
                AddMessage(skill.skill.name + labelLevel + skill.level.ToString(), player.UCE_checkHasSkill(skill.skill, skill.level) ? textColor : errorColor);
        }

        if (requirements.allowedClasses.Length > 0)
        {
            AddMessage(labelAllowedClasses, textColor);
            string temp_classes = "";
            foreach (GameObject classes in requirements.allowedClasses)
                temp_classes += " " + classes.name;
            AddMessage(temp_classes, player.UCE_checkHasClass(requirements.allowedClasses) ? textColor : errorColor);
        }

        if (requirements.requiresParty)
            AddMessage(labelRequiresParty, player.party.InParty() ? textColor : errorColor);

        if (requirements.requiresGuild)
            AddMessage(labelRequiresGuild, player.guild.InGuild() ? textColor : errorColor);

#if _iMMOPRESTIGECLASSES
        if (requirements.allowedPrestigeClasses.Length > 0)
        {
            AddMessage(labelAllowedPrestigeClasses, textColor);
            string temp_classes = "";
            foreach (UCE_PrestigeClassTemplate classes in requirements.allowedPrestigeClasses)
                temp_classes += " " + classes.name;
            AddMessage(temp_classes, player.UCE_CheckPrestigeClass(requirements.allowedPrestigeClasses) ? textColor : errorColor);
        }
#endif

#if _iMMOPVP
        if (requirements.requiredRealm != null && requirements.requiredAlly != null)
            AddMessage(labelRequiresRealm, requirements.checkRealm(player) ? textColor : errorColor);
#endif

#if _iMMOQUESTS
        if (requirements.requiredQuest != null)
        {
            if (!requirements.questMustBeInProgress)
            {
                AddMessage(labelRequiresQuest + requirements.requiredQuest.name, player.UCE_HasCompletedQuest(requirements.requiredQuest.name) ? textColor : errorColor);
            }
            else
            {
                AddMessage(labelRequiresQuest + requirements.requiredQuest.name + labelInProgressQuest, player.UCE_HasActiveQuest(requirements.requiredQuest.name) ? textColor : errorColor);
            }
        }
#else
        if (requirements.requiredQuest != null)
            AddMessage(labelRequiresQuest + requirements.requiredQuest.name, player.quests.HasCompleted(requirements.requiredQuest.name) ? textColor : errorColor);
#endif

#if _iMMOFACTIONS
        if (requirements.factionRequirements.Length > 0)
        {
            AddMessage(labelFactionRequirements, textColor);
            foreach (UCE_FactionRequirement factionRequirement in requirements.factionRequirements)
                AddMessage(factionRequirement.faction.name, player.UCE_CheckFactionRating(factionRequirement) ? textColor : errorColor);
        }
#endif

        if (requirements.requiredEquipment.Length > 0)
        {
            AddMessage(labelRequiredEquipment, textColor);

            foreach (EquipmentItem item in requirements.requiredEquipment)
            {
                AddMessage(item.name, player.UCE_checkHasEquipment(item) ? textColor : errorColor);
            }
        }

        if (requirements.requiredItems.Length > 0)
        {
            AddMessage(labelRequiredItems, textColor);

            foreach (UCE_ItemRequirement item in requirements.requiredItems)
            {
                AddMessage(item.item.name + " x" + item.amount.ToString(), player.inventory.Count(new Item(item.item)) >= item.amount ? textColor : errorColor);
            }
        }

#if _iMMOHARVESTING
        if (requirements.harvestProfessionRequirements.Length > 0)
        {
            AddMessage(requiredHarvestProfessions, textColor);
            foreach (UCE_HarvestingProfessionRequirement prof in requirements.harvestProfessionRequirements)
            {
                AddMessage(prof.template.name + " " + labelLevel + prof.level, player.HasHarvestingProfessionLevel(prof.template, prof.level) ? textColor : errorColor);
            }
        }
#endif

#if _iMMOCRAFTING
        if (requirements.craftProfessionRequirements.Length > 0)
        {
            AddMessage(requiredCraftProfessions, textColor);
            foreach (UCE_CraftingProfessionRequirement prof in requirements.craftProfessionRequirements)
            {
                AddMessage(prof.template.name + " " + labelLevel + prof.level, player.UCE_HasCraftingProfessionLevel(prof.template, prof.level) ? textColor : errorColor);
            }
        }
#endif

#if _iMMOMOUNTS
        if (requirements.mountType == UCE_Requirements.MountType.Mounted)
        {
            AddMessage(labelMountedOnly, (player.UCE_mounted) ? textColor : errorColor);
        }
        else if (requirements.mountType == UCE_Requirements.MountType.Unmounted)
        {
            AddMessage(labelUnmountedOnly, (!player.UCE_mounted) ? textColor : errorColor);
        }
#endif

#if _iMMOTRAVEL
		if (!string.IsNullOrWhiteSpace(requirements.requiredTravelrouteName))
		{
			AddMessage(labelTravelroute + requirements.requiredTravelrouteName, player.UCE_travelroutes.Any(t => t.name == requirements.requiredTravelrouteName) ? textColor : errorColor);
		}
#endif

#if _iMMOWORLDEVENTS
		if (requirements.worldEvent != null)
		{
			AddMessage(labelWorldEvent, textColor);
			if (player.UCE_CheckWorldEvent(requirements.worldEvent, requirements.minEventCount, requirements.maxEventCount))
			{
				if (requirements.maxEventCount == 0)
					AddMessage(requirements.worldEvent.name + " (" + player.UCE_GetWorldEventCount(requirements.worldEvent) + "/" + requirements.minEventCount.ToString() + ")", textColor);
				else
            		AddMessage(requirements.worldEvent.name + " (" + requirements.minEventCount.ToString() + "-" + requirements.maxEventCount.ToString() + ") [" + player.UCE_GetWorldEventCount(requirements.worldEvent) + "]", textColor);
            }
            else
            {
            	if (requirements.maxEventCount == 0)
					AddMessage(requirements.worldEvent.name + " (" + player.UCE_GetWorldEventCount(requirements.worldEvent) + "/" + requirements.minEventCount.ToString() + ")", errorColor);
				else
            		AddMessage(requirements.worldEvent.name + " (" + requirements.minEventCount.ToString() + "-" + requirements.maxEventCount.ToString() + ") [" + player.UCE_GetWorldEventCount(requirements.worldEvent) + "]", errorColor);
            }
		}
#endif

#if _iMMOGUILDUPGRADES
		if (requirements.minGuildLevel > 0)
		{
			if (player.guild.InGuild())
				AddMessage(labelGuildUpgrades + player.guildLevel.ToString() + "/" + requirements.minGuildLevel.ToString(), textColor);
			else
				AddMessage(labelGuildUpgrades + "0/" + requirements.minGuildLevel.ToString(), errorColor);
		}
#endif

#if _iMMOACCOUNTUNLOCKABLES
		if (!string.IsNullOrWhiteSpace(requirements.accountUnlockable))
		{
			if (player.UCE_HasAccountUnlock(requirements.accountUnlockable))
				AddMessage(labelAccountUnlockable + requirements.accountUnlockable, textColor);
			else
				AddMessage(labelAccountUnlockable + requirements.accountUnlockable, errorColor);
		}
#endif

#if _iMMOPATREON
        if (requirements.activeMinPatreon > 0)
        {
            if (player.UCE_HasActivePatreonSubscription(requirements.activeMinPatreon))
                AddMessage(labelPatreonSubscription, textColor);
            else
                AddMessage(labelPatreonSubscription, errorColor);
        }
#endif

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
    protected virtual void Update()
    {
        if (!panel.activeSelf) return;

        Player player = Player.localPlayer;
        if (!player) return;
    }

    // -----------------------------------------------------------------------------------
    // AutoScroll
    // -----------------------------------------------------------------------------------
    protected void AutoScroll()
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

    // -----------------------------------------------------------------------------------
}
