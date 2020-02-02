// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// PLAYER

public partial class Entity
{
    public enum AggroBehaviour { Neutral, Aggressive, Pacifistic, SuperAggressive }

    [Header("[-=-=-=- UCE REALM -=-=-=-]")]
    public UCE_Tmpl_Realm Realm;

    public UCE_Tmpl_Realm Ally;

    [SyncVar, HideInInspector] public int hashRealm;
    [SyncVar, HideInInspector] public int hashAlly;

    protected int _originalRealm;
    protected int _originalAlly;
    protected AggroBehaviour _aggroBehaviour;

#if _iMMOBUILDSYSTEM

    [Header("[UCE BUILD SYTEM]")]
    public bool dontAttackOwner;

    public bool dontAttackOwnerParty;
    public bool dontAttackOwnerGuild;
#endif

    // -------------------------------------------------------------------------------
    // UCE_getAttackAllowance
    // -------------------------------------------------------------------------------
    public virtual bool UCE_getAttackAllowance(Entity entity)
    {
        return true;
    }

    // -------------------------------------------------------------------------------
    // Awake_UCE_PvPZone
    // -------------------------------------------------------------------------------
    [DevExtMethods("Awake")]
    private void Awake_UCE_PvPZone()
    {
        _originalRealm = hashRealm;
        _originalAlly = hashAlly;
    }

    // -----------------------------------------------------------------------------------
    // UCE_getInPvpRegion
    // -----------------------------------------------------------------------------------
    public virtual bool UCE_getInPvpRegion()
    {
        return true;
    }

    // -------------------------------------------------------------------------------
    // UCE_setRealm
    // -------------------------------------------------------------------------------
    public void UCE_setRealm(UCE_Tmpl_Realm newRealm, UCE_Tmpl_Realm newAlly)
    {
        int newRealmHash = (newRealm != null) ? newRealm.name.GetStableHashCode() : 0;
        int newAllyHash = (newAlly != null) ? newAlly.name.GetStableHashCode() : 0;
        UCE_setRealm(newRealmHash, newAllyHash);
    }

    // -------------------------------------------------------------------------------
    // UCE_setRealm
    // -------------------------------------------------------------------------------
    public void UCE_setRealm(int newRealmHash, int newAllyHash)
    {
        UCE_Tmpl_Realm realmData;
        if (newRealmHash != 0 && UCE_Tmpl_Realm.dict.TryGetValue(newRealmHash, out realmData))
        {
            Realm = realmData;
            hashRealm = realmData.name.GetStableHashCode();
        }

        UCE_Tmpl_Realm allyData;
        if (newAllyHash != 0 && UCE_Tmpl_Realm.dict.TryGetValue(newAllyHash, out allyData))
        {
            Ally = allyData;
            hashAlly = allyData.name.GetStableHashCode();
        }
    }

    // -------------------------------------------------------------------------------
    // UCE_getAlliedRealms
    // -------------------------------------------------------------------------------
    public bool UCE_getAlliedRealms(Entity target)
    {
        return UCE_getAlliedRealms(target.hashRealm, target.hashAlly);
    }

    // -------------------------------------------------------------------------------
    // UCE_getHostileRealms
    // -------------------------------------------------------------------------------
    public bool UCE_getHostileRealms(Entity target)
    {
        return UCE_getHostileRealms(target.hashRealm, target.hashAlly);
    }

    // -----------------------------------------------------------------------------------
    // UCE_getHostileRealms
    // -----------------------------------------------------------------------------------
    public bool UCE_getHostileRealms(int targetRealm, int targetAlliedRealm)
    {
        if ((Realm == null && Ally == null) || (targetRealm == 0 && targetAlliedRealm == 0))
            return true;

        if (hashRealm == 0 && targetRealm == 0 || hashAlly == 0 && targetAlliedRealm == 0)
            return true;

        if (hashRealm == targetRealm || hashAlly == targetAlliedRealm || hashRealm == targetAlliedRealm || hashAlly == targetRealm)
            return false;

        return true;
    }

    // -----------------------------------------------------------------------------------
    // UCE_getAlliedRealms
    // -----------------------------------------------------------------------------------
    public bool UCE_getAlliedRealms(UCE_Tmpl_Realm requiredRealm, UCE_Tmpl_Realm requiredAlly)
    {
        if ((requiredRealm == null && requiredAlly == null) || (Realm == null && Ally == null))
            return true;

        if (requiredRealm == Realm || requiredAlly == Ally || requiredRealm == Ally || requiredAlly == Realm)
            return true;

        return false;
    }

    // -------------------------------------------------------------------------------
    // UCE_getAlliedRealms
    // You can attack other realm/neutral, but not own realm or allied realm
    // -------------------------------------------------------------------------------
    public bool UCE_getAlliedRealms(int targetRealm, int targetAlliedRealm)
    {
        if ((Realm == null && Ally == null) || (targetRealm == 0 && targetAlliedRealm == 0))
            return false;

        if (hashRealm == 0 && targetRealm == 0 || hashAlly == 0 && targetAlliedRealm == 0)
            return true;

        if (hashRealm == targetRealm || hashAlly == targetAlliedRealm || hashRealm == targetAlliedRealm || hashAlly == targetRealm)
            return true;

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkBuildSystem
    // -----------------------------------------------------------------------------------
#if _iMMOBUILDSYSTEM

    public bool UCE_checkBuildSystem(Entity target)
    {
        UCE_PlaceableObject po = GetComponentInParent<UCE_PlaceableObject>();

        if (po == null) return false;

        if (!(target is Player) || (!dontAttackOwner && !dontAttackOwnerParty && !dontAttackOwnerGuild)) return true;

        Player player = (Player)target;

        return
                (!dontAttackOwner || (dontAttackOwner && player.name == po.ownerCharacter)) &&
                (!dontAttackOwnerParty || (dontAttackOwnerParty && player.InParty() && player.party.Contains(po.ownerCharacter))) &&
                (!dontAttackOwnerGuild || (dontAttackOwnerGuild && player.guild.name == po.ownerGuild));
    }

#endif

    // -----------------------------------------------------------------------------------
}
