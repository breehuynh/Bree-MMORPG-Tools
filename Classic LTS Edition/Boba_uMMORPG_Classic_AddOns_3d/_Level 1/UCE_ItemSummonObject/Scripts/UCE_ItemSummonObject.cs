// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using UnityEngine;

// SUMMON ITEM

[CreateAssetMenu(menuName = "uMMORPG Item/UCE Item Summon Object", order = 999)]
public class UCE_ItemSummonObject : UsableItem
{
    [System.Serializable]
    public partial class SpawnableEntity
    {
        public GameObject gameObject;
        public GameObject effectObject;
        [Range(0, 1)] public float probability;
        public int level;
    }

    [Serializable]
    public struct SpawnInfo
    {
        public SpawnableEntity[] gameObjects;
        public int minAmount;
        public int maxAmount;
    }

    [Header("[-=-=-=- UCE Summon Item -=-=-=-]")]
    public SpawnInfo[] summonableObjectsPerLevel;

    public float distanceMultiplier;
    public bool increaseRangeWithLevel;

    [Tooltip("[Optional] The level of the summoned Minion (Monster only)")]
    public int level;

    public float rangeMultiplierPerLevel;

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

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    public string errorMessage = "You can't use that right now!";

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public bool CheckTarget(Entity caster)
    {
        return true;
    }

    // -----------------------------------------------------------------------------------
    // getSpawnInfo
    // -----------------------------------------------------------------------------------
    protected SpawnInfo getSpawnInfo()
    {
        if (summonableObjectsPerLevel.Length >= level - 1)
            return summonableObjectsPerLevel[level - 1];

        return summonableObjectsPerLevel[0];
    }

    // -----------------------------------------------------------------------------------
    // CanActivate
    // -----------------------------------------------------------------------------------
    public bool CanActivate(Player player, ItemSlot slot, SpawnInfo spawn)
    {
#if _iMMOPVP
        return (!useInPvPZoneOnly || (useInPvPZoneOnly && player.UCE_getInPvpRegion())) &&
                (spawn.gameObjects != null && spawn.gameObjects.Length > 0 && spawn.minAmount > 0 && spawn.maxAmount > 0) &&
                (decreaseAmount == 0 || slot.amount >= decreaseAmount);
#else
		return 	(spawn.gameObjects != null && spawn.gameObjects.Length > 0 && spawn.minAmount > 0 && spawn.maxAmount > 0) &&
				(decreaseAmount == 0 || slot.amount >= decreaseAmount);
#endif
    }

    // -----------------------------------------------------------------------------------
    // Use
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory[inventoryIndex];
        SpawnInfo spawn = getSpawnInfo();

        // -- General Activation check
        if (CanActivate(player, slot, spawn))
        {
            // always call base function too
            base.Use(player, inventoryIndex);

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
                        distance *= rangeMultiplierPerLevel;

                    Vector2 circle2D = UnityEngine.Random.insideUnitCircle * distance;
                    Vector3 position = player.transform.position + new Vector3(circle2D.x, 0, circle2D.y);
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
                            m.UCE_setRealm(player.Realm, player.Ally);
#endif
#if _iMMOMINION
                        if (minionFollowCaster)
                            m.followMaster = true;

                        if (minionBoundToCaster)
                            m.boundToMaster = true;

                        if (minionFollowCaster || minionBoundToCaster)
                            m.myMaster = player.gameObject;
#endif
                    }

                    NetworkServer.Spawn(go);
                }
            }

            // decrease amount
            slot.DecreaseAmount(decreaseAmount);
            player.inventory[inventoryIndex] = slot;
        }
        else
        {
            player.UCE_TargetAddMessage(errorMessage);
        }
    }

    // -----------------------------------------------------------------------------------
}
