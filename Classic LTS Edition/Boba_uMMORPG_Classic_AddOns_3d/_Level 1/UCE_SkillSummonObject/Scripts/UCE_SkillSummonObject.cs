// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using UnityEngine;

// SUMMON SKILL

[CreateAssetMenu(menuName = "uMMORPG Skill/UCE Skill Summon Object", order = 999)]
public class UCE_SkillSummonObject : ScriptableSkill
{
    [System.Serializable]
    public partial class SpawnableEntity
    {
        public GameObject gameObject;
        public GameObject effectObject;
        [Range(0, 1)] public float probability;

        [Tooltip("[Optional] The level of the summoned Minion (Monster only)")]
        public int level;
    }

    [Serializable]
    public struct SpawnInfo
    {
        public SpawnableEntity[] gameObjects;
        public int minAmount;
        public int maxAmount;
    }

    [Header("[-=-=-=- UCE Summon Skill -=-=-=-]")]
    public SpawnInfo[] summonableObjectsPerLevel;

    public float distanceMultiplier;
    public bool increaseRangeWithLevel;

#if _iMMOPVP

    [Tooltip("[Optional] Can only be activated while the caster is inside a PVP Zone?")]
    public bool useInPvPZoneOnly;

    [Tooltip("[Optional] Is the summoned Minion's Realm set to that of the Caster (Monster only)?")]
    public bool setToCasterRealm;

#endif

#if _iMMOMINION

    [Tooltip("[Optional] Is the summoned Minion following the Caster (Monster only)?")]
    public bool minionFollowCaster;

    [Tooltip("[Optional] Is the summoned Minion bound to the Caster (Monster only)?")]
    public bool minionBoundToCaster;

#endif

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
#if _iMMOPVP
        return (!useInPvPZoneOnly || (useInPvPZoneOnly && ((Player)caster).UCE_getInPvpRegion()));
#else
        return false;
#endif
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        destination = caster.transform.position;
        return true;
    }

    // -----------------------------------------------------------------------------------
    // getSpawnInfo
    // -----------------------------------------------------------------------------------
    protected SpawnInfo getSpawnInfo(int skillLevel)
    {
        if (summonableObjectsPerLevel.Length >= skillLevel - 1)
            return summonableObjectsPerLevel[skillLevel - 1];

        return summonableObjectsPerLevel[0];
    }

    // -----------------------------------------------------------------------------------
    // CanActivate
    // -----------------------------------------------------------------------------------
    public bool CanActivate(Entity caster, SpawnInfo spawn)
    {
#if _iMMOPVP
        return
                (
                (!useInPvPZoneOnly || (useInPvPZoneOnly && caster.UCE_getInPvpRegion())) &&
                spawn.gameObjects != null && spawn.gameObjects.Length > 0 && spawn.minAmount > 0 && spawn.maxAmount > 0
                );

#else
		return 	(spawn.gameObjects != null && spawn.gameObjects.Length > 0 && spawn.minAmount > 0 && spawn.maxAmount > 0);
#endif
    }

    // -----------------------------------------------------------------------------------
    // Apply
    // -----------------------------------------------------------------------------------
    public override void Apply(Entity caster, int skillLevel)
    {
        SpawnInfo spawn = getSpawnInfo(skillLevel);

        // -- General Activation check
        if (CanActivate(caster, spawn))
        {
            int amount = UnityEngine.Random.Range(spawn.minAmount, spawn.maxAmount);

            for (int i = 0; i < amount; ++i)
            {
                int idx = 0;

                foreach (SpawnableEntity spawnableEntity in spawn.gameObjects)
                {
                    if (spawnableEntity.gameObject != null)
                    {
                        if (UnityEngine.Random.value <= spawnableEntity.probability)
                        {
                            idx = System.Array.IndexOf(spawn.gameObjects, spawnableEntity);
                            break;
                        }
                    }
                }

                if (idx != -1)
                {
                    float distance = distanceMultiplier;

                    if (increaseRangeWithLevel)
                        distance *= castRange.Get(skillLevel);

                    Vector2 circle2D = UnityEngine.Random.insideUnitCircle * distance;
                    Vector3 position = caster.transform.position + new Vector3(circle2D.x, 0, circle2D.y);
                    GameObject go = Instantiate(spawn.gameObjects[idx].gameObject, position, Quaternion.identity);
                    go.name = spawn.gameObjects[idx].gameObject.name; // avoid "(Clone)"

                    if (go && spawn.gameObjects[idx].effectObject)
                    {
                        GameObject ef = Instantiate(spawn.gameObjects[idx].effectObject, position, Quaternion.identity);
                        NetworkServer.Spawn(ef);
                    }

                    Monster m = go.GetComponent<Monster>();

                    if (m)
                    {
                        if (spawn.gameObjects[idx].level > 0)
                            m.level = spawn.gameObjects[idx].level;
#if _iMMOPVP
                        if (setToCasterRealm)
                            m.UCE_setRealm(caster.Realm, caster.Ally);
#endif

#if _iMMOMINION
                        if (minionFollowCaster)
                            m.followMaster = true;

                        if (minionBoundToCaster)
                            m.boundToMaster = true;

                        if (minionFollowCaster || minionBoundToCaster)
                            m.myMaster = caster.gameObject;
#endif
                    }

                    NetworkServer.Spawn(go);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
