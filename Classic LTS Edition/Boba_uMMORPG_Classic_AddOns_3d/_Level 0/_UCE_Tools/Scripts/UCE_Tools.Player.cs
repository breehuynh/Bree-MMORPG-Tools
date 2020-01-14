// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// PLAYER

public partial class Player
{
    protected double UCE_timer;
    protected bool UCE_timerRunning;
    protected int UCE_activeTasks;
    protected UCE_InfoBox UCE_infobox;
    protected UCE_UI_CastBar UCE_castbar;
    protected UCE_UI_Prompt UCE_popup;

    // ================================== AOE RELATED ====================================

    // -----------------------------------------------------------------------------------
    // UCE_GetCorrectedTargetsInSphere
    // Returns a list of all legal targets within a sphere around the caster. Comes
    // with several targeting options.
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public override List<Entity> UCE_GetCorrectedTargetsInSphere(Transform origin, float fRadius, bool deadOnly = false, bool affectSelf = false, bool affectOwnParty = false, bool affectOwnGuild = false, bool affectOwnRealm = false, bool reverseTargets = false, bool affectPlayers = false, bool affectMonsters = false, bool affectPets = false)
    {
        List<Entity> correctedTargets = new List<Entity>();

        Collider[] colliders = Physics.OverlapSphere(origin.position, fRadius);
        foreach (Collider co in colliders)
        {
            Entity candidate = co.GetComponentInParent<Entity>();

            if (candidate != null && !correctedTargets.Any(x => x == candidate))
            {
                bool bValidTarget = false;

                if (candidate is Player)
                    bValidTarget = ((Player)this).UCE_SameCheck((Player)candidate, affectSelf, affectPlayers, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargets);
                else if (affectMonsters && candidate is Monster)
                    bValidTarget = true;
                else if (affectPets && candidate is Pet)
                    bValidTarget = true;

                if (bValidTarget)
                {
                    if ((deadOnly && !candidate.isAlive) || (!deadOnly && candidate.isAlive))
                        correctedTargets.Add(candidate);
                }
            }
        }

        return correctedTargets;
    }

    // ========================== UCE SELECTION HANDLING =================================
    // Custom selection handling script that is used in combination with interactable
    // objects. Allows to break from the default selection handling that only allows to
    // interact with Entities.
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_OnInteractServer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_OnInteractServer(GameObject go)
    {
        if (go == null || go.GetComponent<UCE_Interactable>() == null) return;

        go.GetComponent<UCE_Interactable>().interactionRequirements.payCosts(this);
        go.GetComponent<UCE_Interactable>().interactionRequirements.grantRewards(this);

        if (go.GetComponent<UCE_InteractableObject>() != null)
            go.GetComponent<UCE_InteractableObject>().OnUnlock();

        go.GetComponent<UCE_Interactable>().OnInteractServer(this);
    }

    // ============================ SELECTION HANDLING ===================================
    // Replaces the selection handling check of the core asset with custom functions
    // because it is possible to add more checks here later, but not to the core asset
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // UCE_SelectionHandling_Npc
    // Replaces the built-in selection handling for NPCs
    // -----------------------------------------------------------------------------------
    public bool UCE_SelectionHandling_Npc(Entity entity)
    {
        bool valid = true;

        valid = entity is Npc && entity.isAlive;

#if _iMMONPCRESTRICTIONS
        if (entity is Npc)
            valid = ((Npc)entity).UCE_ValidateNpcRestrictions(this) ? valid : false;
#endif

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SelectionHandling_DeadMonster
    // Replaces the built-in selection handling for Dead Monsters
    // -----------------------------------------------------------------------------------
    public bool UCE_SelectionHandling_DeadMonster(Entity entity)
    {
        bool valid = true;

        valid = entity is Monster && !entity.isAlive;

#if _iMMOLOOTRULES
        if (entity is Monster)
            valid = ((Monster)entity).UCE_ValidateTaggedLooting(this) ? valid : false;
#endif

        return valid;
    }

    // ================================ GROUP CHECKS =====================================
    // Group targeting check that allows to detect if the target is of the same Party,
    // Guild or Realm as the caster. Can also reverse the targeting process to only target
    // members that are NOT of those groups.
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // UCE_SameCheck
    // Checks if the target is considered to be of the same group as the caster
    // -----------------------------------------------------------------------------------
    public bool UCE_SameCheck(Player player, bool bSelf, bool bPlayers, bool bParty, bool bGuild, bool bRealm, bool bReverse = false)
    {
        if (!bReverse)
        {
            // -- we want to include all stated target groups

            if (bSelf && (player == this)) return true;
            if (bPlayers && player is Player) return true;
            if (bParty && UCE_SameParty(player)) return true;
            if (bGuild && UCE_SameGuild(player)) return true;
            if (bRealm && UCE_SameRealm(player)) return true;
            return false;
        }
        else
        {
            // -- we want to exclude all stated target groups

            if (bSelf && (player == this)) return false;
            if (bPlayers && player is Player) return false;
            if (bParty && UCE_SameParty(player)) return false;
            if (bGuild && UCE_SameGuild(player)) return false;
            if (bRealm && UCE_SameRealm(player)) return false;
            return true;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_SameParty
    // Checks if the target is in the same party as the caster
    // -----------------------------------------------------------------------------------
    public bool UCE_SameParty(Player player)
    {
        return (InParty() &&
                player.InParty() &&
                party.Contains(player.name) &&
                player != this
                );
    }

    // -----------------------------------------------------------------------------------
    // UCE_SameGuild
    // Checks if the target is in the same guild as the caster
    // -----------------------------------------------------------------------------------
    public bool UCE_SameGuild(Player player)
    {
        return (InGuild() &&
                player.InGuild() &&
                guild.name == player.guild.name &&
                player != this
                );
    }

    // -----------------------------------------------------------------------------------
    // UCE_SameRealm
    // Checks if the target is the same realmID/alliedRealmID as the caster
    // -----------------------------------------------------------------------------------
    public bool UCE_SameRealm(Player player)
    {
#if _iMMOPVP
        return UCE_getAlliedRealms(player);
#else
        return false;
#endif
    }

    // ================================ TELEPORTATION ====================================
    // Custom warp function because I plan to add visual effects to the client side a
    // bit later on. This will require a TargetRPC etc.
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // Cmd_NpcWarp
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
#if _iMMOTELEPORTER
	[Command]
	public void Cmd_NpcWarp(int index)
	{
		if (
            target != null &&
            target is Npc &&
            Utils.ClosestDistance(this, target) <= interactionRange &&
            ((Npc)target).teleportationDestinations.Length > 0 && ((Npc)target).teleportationDestinations.Length >= index
            )
        {
    		((Npc)target).teleportationDestinations[index].teleportationTarget.OnTeleport(this);
    	}
	}
#endif

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_Warp
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_Warp(Vector3 pos)
    {
        agent.Warp(pos);
        target = null;
    }

    // -----------------------------------------------------------------------------------
    // UCE_Warp
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void UCE_Warp(Vector3 pos)
    {
        agent.Warp(pos);
        target = null;
    }

    // ================================== COMMON UI ======================================
    // Some common UI functions (all Client side) that include a Popup, a universal
    // CastBar and a small player log (InfoBox). Just make sure to call the functions
    // only from the Client. Some functions also have a server variant that allows you
    // to call the function from the server and it is then executed at the connected
    // client.
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // UCE_PopupShow
    // @Client
    // -----------------------------------------------------------------------------------
    public void UCE_PopupShow(string message)
    {
        if (message == "") return;

        Player player = Player.localPlayer;
        if (!player) return;

        if (UCE_popup == null)
            UCE_popup = FindObjectOfType<UCE_UI_Prompt>();

        if (UCE_popup != null && !UCE_popup.forceUseChat)
            UCE_popup.Show(message);

        UCE_AddMessage(message, 0, false);                                      // todo: add editable color
    }

    // -----------------------------------------------------------------------------------
    // UCE_CastbarShow
    // @Client
    // -----------------------------------------------------------------------------------
    public void UCE_CastbarShow(string message, float duration)
    {
        if (duration <= 0) return;

        Player player = Player.localPlayer;
        if (!player) return;

        if (UCE_castbar == null)
            UCE_castbar = FindObjectOfType<UCE_UI_CastBar>();

        if (UCE_castbar != null)
            UCE_castbar.Show(message, duration);
    }

    // -----------------------------------------------------------------------------------
    // UCE_CastbarHide
    // @Client
    // -----------------------------------------------------------------------------------
    public void UCE_CastbarHide()
    {
        if (UCE_castbar == null)
            UCE_castbar = FindObjectOfType<UCE_UI_CastBar>();

        if (UCE_castbar != null)
            UCE_castbar.Hide();
    }

    // -----------------------------------------------------------------------------------
    // UCE_TargetAddMessage
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public override void UCE_TargetAddMessage(string message, byte color = 0, bool show = true)
    {
        if (message == "") return;

        if (this == Player.localPlayer)
        {
            UCE_AddMessage(message, color, show);
            return;
        }

        if (UCE_infobox == null)
            UCE_infobox = GetComponent<UCE_InfoBox>();

        if (UCE_infobox)
        {
            UCE_infobox.TargetAddMessage(connectionToClient, message, color, show);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_AddMessage
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public void UCE_AddMessage(string message, byte color = 0, bool show = true)
    {
        if (message == "") return;

        if (UCE_infobox == null)
            UCE_infobox = GetComponent<UCE_InfoBox>();

        if (UCE_infobox)
        {
            UCE_infobox.AddMessage(message, color, show);
        }
    }

    // ================================= BUFF RELATED ====================================
    // All buff related functions should only be called server side as only the server
    // can manipulate the syncList. The "get" functions are useable on client as well.
    //
    // Buff = a status effect that has a positive effect on the player (e.g. Blessing)
    // Nerf = a status effect thats has a negative effect on the player (e.g. Poison)
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // UCE_CleanupBuffs
    // Removes one or more buffs from the target. Comes with options.
    // @Server
    // -----------------------------------------------------------------------------------
    public override void UCE_CleanupStatusBuffs(float successChance = 1f, float modifier = 0f, int limit = 0)
    {
        int limited = 0;
        if (limit > 0)
            limited = limit;

        for (int i = 0; i < buffs.Count; ++i)
        {
            if (!buffs[i].data.disadvantageous && buffs[i].data != offenderBuff && buffs[i].data != murdererBuff && UCE_CheckChance(successChance, modifier))
            {
                buffs.RemoveAt(i);
                i--;

                if (limited > 0)
                {
                    limit--;
                    if (limit <= 0) return;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CleanupStatusNerfs
    // Removes one or more nerfs from the target. Comes with options.
    // @Server
    // -----------------------------------------------------------------------------------
    public override void UCE_CleanupStatusNerfs(float successChance = 1f, float modifier = 0f, int limit = 0)
    {
        int limited = 0;
        if (limit > 0)
            limited = limit;

        for (int i = 0; i < buffs.Count; ++i)
        {
            if (buffs[i].data.disadvantageous && buffs[i].data != offenderBuff && buffs[i].data != murdererBuff && UCE_CheckChance(successChance, modifier))
            {
                buffs.RemoveAt(i);
                i--;

                if (limited > 0)
                {
                    limit--;
                    if (limit <= 0) return;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_getBuffCount
    // Returns the number of buffs (positive status effect) currently active on the player
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public int UCE_getBuffCount()
    {
        int count = 0;
        for (int i = 0; i < buffs.Count; ++i)
        {
            if (!buffs[i].data.disadvantageous && buffs[i].BuffTimeRemaining() > 0 && buffs[i].data != offenderBuff && buffs[i].data != murdererBuff)
                count++;
        }
        return count;
    }

    // -----------------------------------------------------------------------------------
    // UCE_getNerfCount
    // Returns the number of nerfs (negative status effect) currently active on the player
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public int UCE_getNerfCount()
    {
        int count = 0;
        for (int i = 0; i < buffs.Count; ++i)
        {
            if (buffs[i].data.disadvantageous && buffs[i].BuffTimeRemaining() > 0 && buffs[i].data != offenderBuff && buffs[i].data != murdererBuff)
                count++;
        }
        return count;
    }

    // ================================= MISC FUNCS ======================================
    // A bunch of very common utility functions that are missing on the core asset
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // UCE_CanAttack
    // Replaces the built-in CanAttack check. This one can be expanded, the built-in one not.
    // -----------------------------------------------------------------------------------
    public override bool UCE_CanAttack(Entity entity)
    {
        return
            base.UCE_CanAttack(entity) &&
            (
                ((entity is Pet && entity != activePet) || (entity is Mount && entity != activeMount))
#if _iMMOMOUNTS
                || (entity is UCE_Mount && entity != unmountedMount)
#endif
                || (entity is Player && entity.isAlive) || (entity is Monster && entity.isAlive)
            )
#if _iMMOUSAGEREQUIREMENTS
            && ( UCE_GetWeapon() == null || (UCE_GetWeapon() != null && UCE_GetWeapon().UCE_CanUse(this)) )
#endif
#if _iMMOPVP
            && UCE_getAttackAllowance(entity)
#endif
            ;
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkHasClass
    // Checks if the player prefab is of one of the provided array of classes.
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public bool UCE_checkHasClass(GameObject[] classes)
    {
        if (classes == null || classes.Length == 0) return true;
        foreach (GameObject _class in classes)
            if (this.classIcon == _class.GetComponent<Player>().classIcon) return true;
        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkHasPrestigeClass
    // Checks if the player has one or more prestige classes
    // [Requires: UCE PRESTIGE CLASSES AddOn]
    // @Client or @Server
    // -----------------------------------------------------------------------------------
#if _iMMOPRESTIGECLASSES

    public bool UCE_checkHasPrestigeClass(UCE_PrestigeClassTemplate[] prestigeClasses)
    {
        if (prestigeClasses == null || prestigeClasses.Length == 0) return true;
        return (prestigeClasses.Any(x => x == UCE_prestigeClass)) ? true : false;
    }

#endif

    // -----------------------------------------------------------------------------------
    // UCE_getSkillLevel
    // Simple wrapper to return the current level of a skill on the player
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public int UCE_getSkillLevel(ScriptableSkill skill)
    {
        return skills.FirstOrDefault(s => s.name == skill.name).level;
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkHasSkill
    // Checks the existence of one skill and its level
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public bool UCE_checkHasSkill(ScriptableSkill skill, int level)
    {
        if (skill == null || level <= 0) return true;
        return skills.Any(s => s.name == skill.name && s.level >= level);
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkHasSkills
    // Checks the existence of one or more skills and their skill level
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public bool UCE_checkHasSkills(UCE_SkillRequirement[] skills, bool requiresAll = false)
    {
        if (skills == null || skills.Length == 0) return true;

        bool valid = false;

        foreach (UCE_SkillRequirement skill in skills)
        {
            if (UCE_checkHasSkill(skill.skill, skill.level))
            {
                valid = true;
                if (!requiresAll) return valid;
            }
            else
            {
                valid = false;
            }
        }

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // UCE_removeItems
    // Removes items from the inventory without checking. Removes either the first item
    // provided as a array or all items in that array of items.
    // @Server
    // -----------------------------------------------------------------------------------
    public void UCE_removeItems(UCE_ItemRequirement[] items, bool removeAll = false)
    {
        if (items.Length == 0) return;

        foreach (UCE_ItemRequirement item in items)
        {
            InventoryRemove(new Item(item.item), item.amount);
            if (!removeAll) return;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetWeapon
    // Returns the WeaponItem template that the player currently has equipped.
    // -----------------------------------------------------------------------------------
    public WeaponItem UCE_GetWeapon()
    {
        int idx = equipment.FindIndex(slot => slot.amount > 0 &&
            slot.item.data is WeaponItem &&
            ((WeaponItem)slot.item.data).category.StartsWith("Weapon"));

        if (idx != -1)
            return (WeaponItem)equipment[idx].item.data;
        else
            return null;
    }

    // -----------------------------------------------------------------------------------
    // UCE_inventorySlotCount
    // Returns the amount of inventory slots that have an item on them.
    // -----------------------------------------------------------------------------------
    public int UCE_inventorySlotCount()
    {
        int amnt = 0;

        for (int i = 0; i < inventory.Count; ++i)
        {
            if (inventory[i].amount > 0)
                amnt++;
        }

        return amnt;
    }

    // ==================================== TIMER ========================================
    // A very simple timer that keeps track of the duration that passed since it was
    // started. Each player can only have one active timer. The timer can be set, checked
    // and stopped if required. It's not synched and can be used either on the server or
    // on the client for low security things.
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // UCE_setTimer
    // Sets or resets the timer
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public void UCE_setTimer(float duration)
    {
        if (duration > 0)
        {
            UCE_timer = NetworkTime.time + duration;
            UCE_timerRunning = true;
        }
        else
        {
            UCE_stopTimer();
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkTimer
    // Checks if the timer is finished
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public bool UCE_checkTimer()
    {
        return (UCE_timerRunning && NetworkTime.time > UCE_timer);
    }

    // -----------------------------------------------------------------------------------
    // UCE_stopTimer
    // Simply stops the timer
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public void UCE_stopTimer()
    {
        UCE_timer = 0;
        UCE_timerRunning = false;
    }

    // ==================================== TASKS ========================================
    // Very simple task system that uses a counter to keep track of the things a player
    // is currently doing. Typically a player can have just one task but the system
    // supports any number. Tasks control if the castBar is shown and you can check if
    // a user is busy or not.
    //
    // Used mostly by Lootcrate, Crafting, Harvesting and a few others as all their
    // activities are considered to be a task.
    //
    // Tasks are kept track client side (they work on server side as well) without a
    // syncVar because they are considered a very low security risk. Nothing depends
    // on them.
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // UCE_isBusy
    // Simply checks if the task counter is greater than 0.
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public bool UCE_isBusy()
    {
        return UCE_activeTasks > 0;
    }

    // -----------------------------------------------------------------------------------
    // UCE_addTask
    // Simply increases the task counter
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public void UCE_addTask()
    {
        UCE_activeTasks++;
    }

    // -----------------------------------------------------------------------------------
    // UCE_removeTask
    // Reduces the task counter by one for this player, hides the castbar if this was the
    // last task removed.
    // @Client or @Server
    // -----------------------------------------------------------------------------------
    public void UCE_removeTask()
    {
        if (UCE_activeTasks > 0)
            UCE_activeTasks--;
        if (UCE_activeTasks < 1)
        {
            UCE_timer = 0;
            if (UCE_castbar == null)
                UCE_castbar = FindObjectOfType<UCE_UI_CastBar>();
            if (UCE_castbar != null)
                UCE_castbar.Hide();
        }
    }

    // ==================================== EVENTS =======================================

    // -----------------------------------------------------------------------------------
    // OnDeath_UCE
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDeath")]
    protected override void OnDeath_UCE()
    {
        base.OnDeath_UCE();
    }

    // ==================================== ADDON SPECIFIC ===============================
    // We need a few AddOn specific functions right here in the Tools, as several AddOns
    // depend on it. This way we don't have to create multiple, redundant functions in
    // each one of the individual AddOns.
    // ===================================================================================

    // -----------------------------------------------------------------------------------
    // UCE_UnequipCursedEquipment
    // @Server
    // -----------------------------------------------------------------------------------
#if _iMMOCURSEDEQUIPMENT && _iMMOTOOLS

    public void UCE_UnequipCursedEquipment(int maxCurseLevel)
    {
        for (int i = 0; i < equipment.Count; ++i)
        {
            int index = i;
            ItemSlot slot = equipment[index];

            if (
                slot.amount > 0 &&
                InventoryCanAdd(new Item(slot.item.data), slot.amount) &&
                ((EquipmentItem)slot.item.data).cursedLevel > 0 &&
                ((EquipmentItem)slot.item.data).cursedLevel <= maxCurseLevel
#if _iMMOEQUIPABLEBAG
                && ((EquipmentItem)slot.item.data).UCE_canUnequipBag(this)
#endif
                )
            {
                InventoryAdd(slot.item, 1);
                slot.DecreaseAmount(1);
                equipment[index] = slot;
            }
        }
    }

#endif

    // -----------------------------------------------------------------------------------
}
