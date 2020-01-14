// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// MONSTER

public partial class Monster
{
    [Header("[-=-=-=- PvP Zone Settings -=-=-=-]")]
    [Tooltip("[Optional] Set to true and RealmId + AlliedRealmId will be set to the Attacker's after the Monster respawned")]
    public bool captureable;

    [Tooltip("[Required] Neutral: Only retaliates, Aggressive: Attacks & retaliates, Pacifistic: Never attacks or retaliates")]
    public AggroBehaviour aggroBehaviour;

    // -------------------------------------------------------------------------------
    // Awake_UCE_PvPZone
    // -------------------------------------------------------------------------------
    [DevExtMethods("Awake")]
    private void Awake_UCE_PvPZone()
    {
        if (Realm != null)
            hashRealm = Realm.name.GetDeterministicHashCode();

        if (Ally != null)
            hashAlly = Ally.name.GetDeterministicHashCode();

        _originalRealm = hashRealm;
        _originalAlly = hashAlly;
        _aggroBehaviour = aggroBehaviour;
    }

    // -------------------------------------------------------------------------------
    // OnDeath_UCE_PVPZone
    // -------------------------------------------------------------------------------
    [Server]
    [DevExtMethods("OnDeath")]
    private void OnDeath_UCE_PVPZone()
    {
        if (captureable && lastAggressor != null)
        {
            // -- set to realm of attacker
            UCE_setRealm(lastAggressor.hashRealm, lastAggressor.hashAlly);
            lastAggressor = null;
        }
        else if (!captureable)
        {
            // -- revert to original realm
            UCE_revertRealm();
            aggroBehaviour = _aggroBehaviour;
        }
    }

    // -------------------------------------------------------------------------------
    // UCE_revertRealm
    // -------------------------------------------------------------------------------
    public void UCE_revertRealm()
    {
        UCE_setRealm(_originalRealm, _originalAlly);
    }

    // -------------------------------------------------------------------------------
    // UCE_getAttackAllowance
    // -------------------------------------------------------------------------------
    public override bool UCE_getAttackAllowance(Entity target)
    {
        if (_cannotCast) return false;

        if (aggroBehaviour == AggroBehaviour.Pacifistic)
            return false;

        if (aggroBehaviour == AggroBehaviour.SuperAggressive)
            return true;

        // ---------- Non-Aggressive Monsters never attack by themselves
        if (aggroBehaviour != AggroBehaviour.Aggressive && (lastAggressor == null || lastAggressor != target))
            return false;

        // ---------- Retaliation (Neutral/Aggressive)
        if (lastAggressor == target)
            return true;

#if _iMMOFACTIONS
        // ---------- Aggressive Monsters: Only aggressive to Players if below faction threshold
        if (aggroBehaviour == AggroBehaviour.Aggressive &&
            target is Player &&
            myFaction != null &&
            UCE_checkFactionThreshold(target))
            return false;

        // ---------- Neutral Monsters: Are aggressive to Players if below faction threshold
        if (aggroBehaviour == AggroBehaviour.Neutral &&
            target is Player &&
            myFaction != null &&
            !UCE_checkFactionThreshold(target))
            return true;

#endif

#if _iMMOBUILDSYSTEM
        // ---------- Build System: Attack non-owner groups
        if (UCE_checkBuildSystem(target))
            return true;
#endif

        // ---------- Check Realm PVE
        // You can attack other realm/neutral, but not own realm or allied realm
        return UCE_getHostileRealms(target);
    }

    // -----------------------------------------------------------------------------------
}
