// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// ENTITY

public partial class Entity
{
    [SyncVar] protected GameObject _lastAggressor;
    [SyncVar, HideInInspector] public bool _cannotCast;

    protected float cacheTimerInterval = 1.0f;
    protected float _cacheTimer;
    [SyncVar] protected float _UCE_modifierSpeed;
    protected const float minBuffChance = 0.01f;
    protected const float maxBuffChance = 0.99f;

    // ================================== AGGRESSOR RELATED ==============================

    // -----------------------------------------------------------------------------------
    // lastAggressor
    // -----------------------------------------------------------------------------------
    public Entity lastAggressor
    {
        get { return _lastAggressor != null ? _lastAggressor.GetComponent<Entity>() : null; }
        set { _lastAggressor = value != null ? value.gameObject : null; }
    }

    // ================================== ANIMATION RELATED ==============================

    // -----------------------------------------------------------------------------------
    // StartAnimation
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public void StartAnimation(string animationName, AudioClip soundEffect = null)
    {
        if (string.IsNullOrWhiteSpace(animationName)) return;

        foreach (Animator anim in GetComponentsInChildren<Animator>())
        {
            if (anim.parameters.Any(x => x.name == (animationName)))
                anim.SetBool(animationName, true);
        }

        if (soundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
    }

    // -----------------------------------------------------------------------------------
    // StopAnimation
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public void StopAnimation(string animationName, AudioClip soundEffect = null)
    {
        if (string.IsNullOrWhiteSpace(animationName)) return;

        foreach (Animator anim in GetComponentsInChildren<Animator>())
        {
            if (anim.parameters.Any(x => x.name == (animationName)))
                anim.SetBool(animationName, false);
        }

        if (soundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
    }

    // ================================= FUNCTIONS =======================================

    // -----------------------------------------------------------------------------------
    // isAlive
    // -----------------------------------------------------------------------------------
    public bool isAlive
    {
        get
        {
            return health > 0;
        }
    }

    // -----------------------------------------------------------------------------------
    // canInteract
    // -----------------------------------------------------------------------------------
    public bool canInteract
    {
        get
        {
            return
                    isAlive &&
                    (
                    state == "IDLE" ||
                       state == "MOVING" ||
                       state == "CASTING" ||
                       state == "TRADING"
                       );
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanAttack
    // Replaces the built-in CanAttack check. This one can be expanded, the built-in one not.
    // -----------------------------------------------------------------------------------
    public virtual bool UCE_CanAttack(Entity entity)
    {
        return
            !_cannotCast &&
            isAlive &&
            entity != null &&
            entity.isAlive &&
            entity != this &&
            !inSafeZone &&
            !entity.inSafeZone &&
            !NavMesh.Raycast(transform.position, entity.transform.position, out NavMeshHit hit, NavMesh.AllAreas) &&
           (entity is Player ||
            entity is Monster ||
            entity is Pet ||
#if _iMMOMOUNTS
            entity is UCE_Mount ||
#endif
            entity is Mount
            )
            ;
    }

    // -----------------------------------------------------------------------------------
    // UCE_ToggleVisibility
    // -----------------------------------------------------------------------------------
    public void UCE_ToggleVisibility(bool visible)
    {
        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderer)
            rend.enabled = visible;
    }

    // -----------------------------------------------------------------------------------
    // UCE_OverrideState
    // Force overrides the Entity state, as the variable is protected its not possible
    // otherwise. State overwriting is required in certain places because otherwise the
    // state automatically switches back to IDLE or DEAD (for example) and we would not
    // be able to do anything about it.
    // -----------------------------------------------------------------------------------
    public void UCE_OverrideState(string newState)
    {
        if (newState != "")
            _state = newState;
    }

    // -----------------------------------------------------------------------------------
    // DealDamageAt_UCE
    // Custom hook and splits up the DealDamageAt hook into class based ones. I could not
    // get the built-in DealDamageAt hook to work with "Player" for example, it only and
    // ever triggered at the "Entity". Same for Monster and Pet. So it always triggers on
    // the base class, not on the derived classes.
    // -----------------------------------------------------------------------------------
    [DevExtMethods("DealDamageAt")]
    private void DealDamageAt_UCE(Entity entity, int amount, int damageDealt, DamageType damageType)
    {
        if (entity == null || amount <= 0 || !entity.isAlive) return;

        entity.lastAggressor = this;

        if (entity is Player)
        {
            //this.InvokeInstanceDevExtMethods("OnDamageDealt", amount);
            Utils.InvokeMany(typeof(Player), entity, "OnDamageDealt_");
        }

        if (entity is Monster)
        {
            //this.InvokeInstanceDevExtMethods("OnDamageDealt", amount);
            Utils.InvokeMany(typeof(Monster), entity, "OnDamageDealt_", amount); //only monster has amount parameter!
        }

        if (entity is Pet)
        {
            //this.InvokeInstanceDevExtMethods("OnDamageDealt", amount);
            Utils.InvokeMany(typeof(Pet), entity, "OnDamageDealt_");
        }

    }

    // -----------------------------------------------------------------------------------
    // Update_UCE
    // Workaround to achieve throttled Update
    // As with DealDamageAt, the hook in the core asset only triggers at the parent class,
    // but never on one of its child classes. At least this happened to me several times
    // during development.
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Update")]
    private void Update_UCE()
    {
        UCE_OverwriteSpeed();

        // -- Delayed Update (once per x seconds instead of once per frame)

        if (Time.time > _cacheTimer)
        {
            if (this is Player)
            {
                //this.InvokeInstanceDevExtMethods("Update");
                Utils.InvokeMany(typeof(Player), this, "Update_");
            }

            if (this is Monster)
            {
                //this.InvokeInstanceDevExtMethods("Update");
                Utils.InvokeMany(typeof(Monster), this, "Update_");
            }

            if (this is Pet)
            {
                //this.InvokeInstanceDevExtMethods("Update");
                Utils.InvokeMany(typeof(Pet), this, "Update_");
            }

            _cacheTimer = Time.time + cacheTimerInterval;
        }
    }

    // -----------------------------------------------------------------------------------
    // Awake_UCE
    // Messy workaround because Awake cannot be called from any class derived from Entity.
    // Same as in the two functions above, it happened during development that the "Awake"
    // hook in the core asset was only called on the base class, but not on derived classes.
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Awake")]
    private void Awake_UCE()
    {
        _UCE_modifierSpeed = 0;
        _lastAggressor = null;

        if (this is Player)
        {
            //this.InvokeInstanceDevExtMethods("Update");
            Utils.InvokeMany(typeof(Player), this, "Awake_");
        }

        if (this is Monster)
        {
            //this.InvokeInstanceDevExtMethods("Update");
            Utils.InvokeMany(typeof(Monster), this, "Awake_");
        }

        if (this is Pet) {
            //this.InvokeInstanceDevExtMethods("Update");
            Utils.InvokeMany(typeof(Pet), this, "Awake_");
        }

        if (this is Npc)
        {
            //this.InvokeInstanceDevExtMethods("Update");
            Utils.InvokeMany(typeof(Npc), this, "Awake_");
        }

    }

    // -----------------------------------------------------------------------------------
    // OnDeath_UCE
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDeath")]
    protected virtual void OnDeath_UCE()
    {
        target = null;
        _cannotCast = false;
        //_lastAggressor = null; // we cannot reset that here, would have to go into "on respawn" instead
        UCE_resetModifySpeed();
    }

    // -----------------------------------------------------------------------------------
    // OnAggro
    // Custom onAggro function that has a % chance to trigger (instead of automatically)
    // -----------------------------------------------------------------------------------
    public void UCE_OnAggro(Entity source, float fChance = 1f)
    {
        if (fChance <= 0 || skills.Count <= 0 || !isAlive || !UCE_CanAttack(source)) return;

        if (fChance > 0 && UnityEngine.Random.value <= fChance)
        {
            target = source;
            OnAggro(target);
        }
    }

    // =================================== HELPERS =======================================

    // -----------------------------------------------------------------------------------
    // UCE_SpawnEffect
    // Same as SpawnEffect that is found in skill effects of the core asset. It has been
    // put here because its required for almost every skill. Prevents duplicate code.
    // -----------------------------------------------------------------------------------
    public void UCE_SpawnEffect(Entity caster, BuffSkill buff)
    {
        if (buff.effect != null)
        {
            GameObject go = Instantiate(buff.effect.gameObject, transform.position, Quaternion.identity);
            go.GetComponent<BuffSkillEffect>().caster = caster;
            go.GetComponent<BuffSkillEffect>().target = this;
            go.GetComponent<BuffSkillEffect>().buffName = buff.name;
            NetworkServer.Spawn(go);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_Recoil
    // Pushes the target X unity units away from the caster. In case "distance" is negative
    // it instead pulls the target X unity units closer to the caster. The push itself it
    // not smoothed (due to complicated networking) but uses a Warp instead.
    // -----------------------------------------------------------------------------------
    public void UCE_Recoil(Entity caster, float distance)
    {
        // If the distance is positive we want to push the target away.
        if (distance > 0)
        {
            Vector3 newPosition = transform.position - transform.forward * distance;
            agent.Warp(newPosition);
        }
        // If the distance is negative we want to pull the target, but not if they're too close.
        else if (distance < 0 && Vector3.Distance(transform.position, caster.transform.position) > 5)
        {
            Vector3 newPosition = transform.position + transform.forward * distance;
            agent.Warp(newPosition);
        }
    }

    // ================================== AOE RELATED ====================================

    // -----------------------------------------------------------------------------------
    // UCE_GetCorrectedTargetsInSphere
    // Retrieves all legal targets within a sphere around the caster. This version is for
    // Entities only (Monsters, Pets etc.), players have a unique one with more options.
    // -----------------------------------------------------------------------------------
    public virtual List<Entity> UCE_GetCorrectedTargetsInSphere(Transform origin, float fRadius, bool deadOnly = false, bool affectSelf = false, bool affectOwnParty = false, bool affectOwnGuild = false, bool affectOwnRealm = false, bool reverseTargets = false, bool affectPlayers = false, bool affectMonsters = false, bool affectPets = false)
    {
        List<Entity> correctedTargets = new List<Entity>();

        int layerMask = ~(1 << 2); //2= ignore raycast
        Collider[] colliders = Physics.OverlapSphere(origin.position, fRadius, layerMask);

        foreach (Collider co in colliders)
        {
            Entity candidate = co.GetComponentInParent<Entity>();
            if (candidate != null && !correctedTargets.Any(x => x == candidate))
            {
                if ((deadOnly && !candidate.isAlive) || (!deadOnly && candidate.isAlive))
                {
                    if (UCE_SameCheck(candidate, affectSelf, affectPlayers, affectMonsters, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargets))
                    {
                        correctedTargets.Add(candidate);
                    }
                }
            }
        }

        return correctedTargets;
    }

    // ================================ GROUP CHECKS =====================================

    // -----------------------------------------------------------------------------------
    // UCE_SameCheck
    // Checks if the target is considered to be of the same group as the caster, a group
    // can be a party, guild or realm
    // -----------------------------------------------------------------------------------
    public bool UCE_SameCheck(Entity entity, bool bSelf, bool bPlayers, bool bMonsters, bool bPets, bool bParty, bool bGuild, bool bRealm, bool bReverse = false)
    {
        // -- we want to include all stated target groups

        if (!bReverse)
        {
            if (bSelf && (entity == this)) return true;
            if (bPlayers && entity is Player) return true;
            if (bMonsters && entity is Monster) return true;
            if (bPets && entity is Pet) return true;
            if (bRealm && UCE_SameRealm(entity)) return true;
            return false;
        }
        else
        {
            // -- we want to exclude all stated target groups

            if (bSelf && (entity == this)) return false;
            if (bPlayers && entity is Player) return false;
            if (bMonsters && entity is Monster) return false;
            if (bPets && entity is Pet) return false;
            if (bRealm && UCE_SameRealm(entity)) return false;
            return true;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_SameRealm
    // Checks if the target is the same realmID/alliedRealmID as the caster
    // -----------------------------------------------------------------------------------
    public bool UCE_SameRealm(Entity entity)
    {
#if _iMMOPVP
        return UCE_getAlliedRealms(entity);
#else
        return false;
#endif
    }

    // ================================= BUFF RELATED ====================================

    // -----------------------------------------------------------------------------------
    // UCE_ApplyBuff
    // -----------------------------------------------------------------------------------
    public virtual void UCE_ApplyBuff(BuffSkill buff, int level = 1, float successChance = 1f, float modifier = 0f)
    {
        if (buff == null || successChance <= 0) return;

        // -- check for buff/nerf blocking

        if (
            (buff.disadvantageous && buffs.Any(x => x.blockNerfs)) ||
            (!buff.disadvantageous && buffs.Any(x => x.blockBuffs))
            )
            return;

        // -- check apply chance and apply

        if (UCE_CheckChance(successChance, modifier))
        {
            AddOrRefreshBuff(new Buff(buff, level));
            UCE_SpawnEffect(this, buff);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_ApplyBuffs
    // -----------------------------------------------------------------------------------
    public virtual void UCE_ApplyBuffs(BuffSkill[] buffs, int level = 1, float successChance = 1f, float modifier = 0f, int limit = 0)
    {
        if (buffs.Length == 0 || successChance <= 0) return;

        foreach (BuffSkill buff in buffs)
        {
            if (buff == null || successChance <= 0) continue;

            // -- sanity check on the level

            level = Mathf.Clamp(level, 1, buff.maxLevel);

            // -- check for buff/nerf blocking

            if (
                (buff.disadvantageous && buffs.Any(x => x.blockNerfs)) ||
                (!buff.disadvantageous && buffs.Any(x => x.blockBuffs))
                )
                continue;

            // -- check apply chance and apply

            if (UCE_CheckChance(successChance, modifier))
            {
                AddOrRefreshBuff(new Buff(buff, level));

                UCE_SpawnEffect(this, buff);

                if (limit > 0)
                {
                    limit--;
                    if (limit <= 0) return;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_RemoveBuff
    // -----------------------------------------------------------------------------------
    public virtual void UCE_RemoveBuff(BuffSkill buff)
    {
        for (int i = 0; i < buffs.Count; ++i)
        {
            if (buffs[i].data == buff)
            {
                if (!buffs[i].data.cannotRemove)
                {
                    buffs.RemoveAt(i);
                    return;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_RemoveBuff
    // -----------------------------------------------------------------------------------
    public virtual void UCE_RemoveBuff(BuffSkill buff, float successChance = 1f, float modifier = 0f)
    {
        if (buff == null || successChance <= 0) return;

        for (int i = 0; i < buffs.Count; ++i)
        {
            if (buffs[i].data == buff && UCE_CheckChance(successChance, modifier))
            {
                if (!buffs[i].data.cannotRemove)
                {
                    buffs.RemoveAt(i);
                    return;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CleanupStatusBuffs
    // -----------------------------------------------------------------------------------
    public virtual void UCE_CleanupStatusBuffs(float successChance = 1f, float modifier = 0f, int limit = 0)
    {
        int limited = 0;
        if (limit > 0)
            limited = limit;

        for (int i = 0; i < buffs.Count; ++i)
        {
            if (!buffs[i].data.disadvantageous && UCE_CheckChance(successChance, modifier))
            {
                if (!buffs[i].data.cannotRemove)
                {
                    buffs.RemoveAt(i);
                    --i;
                    if (limit > 0)
                    {
                        limited--;
                        if (limited <= 0) return;
                    }
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CleanupStatusNerfs
    // -----------------------------------------------------------------------------------
    public virtual void UCE_CleanupStatusNerfs(float successChance = 1f, float modifier = 0f, int limit = 0)
    {
        int limited = 0;
        if (limit > 0)
            limited = limit;

        for (int i = 0; i < buffs.Count; ++i)
        {
            if (buffs[i].data.disadvantageous && UCE_CheckChance(successChance, modifier))
            {
                if (!buffs[i].data.cannotRemove)
                {
                    buffs.RemoveAt(i);
                    --i;
                    if (limit > 0)
                    {
                        limited--;
                        if (limited <= 0) return;
                    }
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CleanupStatusAny
    // -----------------------------------------------------------------------------------
    public virtual void UCE_CleanupStatusAny(float successChance = 1f, float modifier = 0f, int limit = 0)
    {
        int limited = 0;
        if (limit > 0)
            limited = limit;

        for (int i = 0; i < buffs.Count; ++i)
        {
            if (UCE_CheckChance(successChance, modifier))
            {
                if (!buffs[i].data.cannotRemove)
                {
                    buffs.RemoveAt(i);
                    --i;
                    if (limit > 0)
                    {
                        limited--;
                        if (limited <= 0) return;
                    }
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CleanupStatusAll
    // -----------------------------------------------------------------------------------
    public void UCE_CleanupStatusAll()
    {
        for (int i = 0; i < buffs.Count; ++i)
        {
            if (!buffs[i].data.cannotRemove)
            {
                buffs.RemoveAt(i);
                --i;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkHasBuff
    // -----------------------------------------------------------------------------------
    public bool UCE_checkHasBuff(BuffSkill buff)
    {
        return buffs.Any(x => x.data == buff);
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckChance
    // -----------------------------------------------------------------------------------
    protected bool UCE_CheckChance(float baseChance, float modifier = 0f)
    {
        if (baseChance >= 1 && modifier == 0) return true;

#if _iMMOATTRIBUTES
        baseChance += modifier;
        baseChance -= resistance;
        baseChance = Mathf.Clamp(baseChance, minBuffChance, maxBuffChance);
#endif

        return UnityEngine.Random.value <= baseChance;
    }

    // -----------------------------------------------------------------------------------
    // UCE_HarmonizeChance
    // -----------------------------------------------------------------------------------
    public float UCE_HarmonizeChance(float baseChance, float modifier)
    {
        baseChance += modifier;

#if _iMMOATTRIBUTES
        baseChance -= resistance;
        baseChance = Mathf.Clamp(baseChance, minBuffChance, maxBuffChance);
#endif

        return baseChance;
    }

    // ================================= ITEM RELATED ====================================

    // -----------------------------------------------------------------------------------
    // UCE_checkHasEquipment
    // -----------------------------------------------------------------------------------
    public bool UCE_checkHasEquipment(ScriptableItem item, int amount = 1)
    {
        if (item == null) return true;

        foreach (ItemSlot slot in equipment)
            if (slot.amount > 0 && slot.amount >= amount && slot.item.data == item) return true;

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkDepletableEquipment
    // -----------------------------------------------------------------------------------
    public bool UCE_checkDepletableEquipment(ScriptableItem item, int amount = 1)
    {
        if (item == null) return true;

        foreach (ItemSlot slot in equipment)
            if (slot.amount > 0 && slot.item.data.maxStack > 1 && slot.amount >= amount && slot.item.data == item) return true;

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_removeEquipment
    // Delete a equipped item, just like a inventory item can be deleted by default
    // -----------------------------------------------------------------------------------
    public void UCE_removeEquipment(ScriptableItem item)
    {
        for (int i = 0; i < equipment.Count; ++i)
        {
            ItemSlot slot = equipment[i];

            if (slot.amount > 0 && slot.item.data == item)
            {
                slot.amount--;
                equipment[i] = slot;
                return;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkHasEquipment
    // -----------------------------------------------------------------------------------
    public bool UCE_checkHasEquipment(ScriptableItem[] items, bool requiresAll = false)
    {
        if (items == null || items.Length <= 0) return true;

        bool valid = false;

        foreach (ScriptableItem item in items)
        {
            if (UCE_checkHasEquipment(item))
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
    // UCE_checkHasItem
    // -----------------------------------------------------------------------------------
    public bool UCE_checkHasItem(ScriptableItem item)
    {
        if (item == null) return true;
        return InventoryCount(new Item(item)) >= 1;
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkHasItems
    // -----------------------------------------------------------------------------------
    public bool UCE_checkHasItems(UCE_ItemRequirement[] items, bool requiresAll = false)
    {
        if (items == null || items.Length == 0) return true;

        bool valid = false;

        foreach (UCE_ItemRequirement item in items)
        {
            if (InventoryCount(new Item(item.item)) >= item.amount)
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
    // UCE_getTotalItemCount
    // -----------------------------------------------------------------------------------
    public int UCE_getTotalItemCount(ScriptableItem itemT)
    {
        int totalCount = 0;
        for (int i = 0; i < inventory.Count; ++i)
        {
            if (inventory[i].amount > 0 && inventory[i].item.data == itemT)
            {
                totalCount += inventory[i].amount;
            }
        }
        return totalCount;
    }

    // ================================= SPEED RELATED ===================================

    // -----------------------------------------------------------------------------------
    // UCE_speed
    // -----------------------------------------------------------------------------------
    public virtual float UCE_speed
    {
        get
        {
            return speed + _UCE_modifierSpeed;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_OverwriteSpeed
    // -----------------------------------------------------------------------------------
    public void UCE_OverwriteSpeed()
    {
        agent.speed = UCE_speed;
    }

    // -----------------------------------------------------------------------------------
    // UCE_resetSpeed
    // -----------------------------------------------------------------------------------
    public void UCE_resetModifySpeed()
    {
        _UCE_modifierSpeed = 0;
    }

    // -----------------------------------------------------------------------------------
    // UCE_setSpeed
    // -----------------------------------------------------------------------------------
    public void UCE_setSpeedModifier(float newSpeedModifier)
    {
        _UCE_modifierSpeed = newSpeedModifier;
    }

    // -----------------------------------------------------------------------------------
    // UCE_ModifySpeedPercentage
    // -----------------------------------------------------------------------------------
    public void UCE_ModifySpeedPercentage(float newSpeed)
    {
        UCE_setSpeedModifier(UCE_speed + (UCE_speed * newSpeed));
    }

    // ============================== STATE MACHINE RELATED ==============================

    // -----------------------------------------------------------------------------------
    // UCE_Stunned
    // -----------------------------------------------------------------------------------
    public bool UCE_Stunned()
    {
        return NetworkTime.time <= stunTimeEnd;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SetStun
    // -----------------------------------------------------------------------------------
    public void UCE_SetStun(float stunModifier)
    {
        stunTimeEnd += stunModifier;
    }

    // ================================= COMMON UI =======================================

    // -----------------------------------------------------------------------------------
    // UCE_TargetAddMessage
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public virtual void UCE_TargetAddMessage(string message, byte color = 0, bool show = true) { }

    // -----------------------------------------------------------------------------------
}
