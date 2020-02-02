// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// PLAYER

public partial class Player
{
    [HideInInspector] public UCE_Item_PlaceableObject UCE_myPlaceableObjectItem;
    protected UCE_UI_PlaceableObjectSpawn _UCE_UI_PlaceableObjectSpawn;
    protected int UCE_myPlaceableObjectAreaId;
    protected bool UCE_myPlaceableObjectAreaOwner;
    protected bool UCE_myPlaceableObjectAreaGuild;
    [HideInInspector] public UCE_PlaceableObject UCE_myPlaceableObject;

    protected GameObject UCE_BuildSystem_PreviewObject;
    protected GameObject UCE_BuildSystem_GridObject;

    private const string UCE_MSG_PLACEABLEOBJECT_UPGRADE = "Upgrading...";

    // -----------------------------------------------------------------------------------
    // OnDestroy_UCE_RTSBuildSystem
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDestroy")]
    private void OnDestroy_UCE_RTSBuildSystem()
    {
        UCE_CancelSpawnPlaceableObject();
    }

    // ===================================== CANCEL ======================================

    // -----------------------------------------------------------------------------------
    // OnDamageDealt_UCE_cancelStartSpawnPlaceableObject
    // Custom Hook
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDamageDealtTo")]
    private void OnDamageDealtTo_UCE_cancelStartSpawnPlaceableObject()
    {
        UCE_myPlaceableObjectItem = null;
        UCE_removeTask();
    }

    // ===================================== LATE UPDATE =================================

    // -----------------------------------------------------------------------------------
    // LateUpdate_UCE_SpawnPlaceableObject
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    [DevExtMethods("LateUpdate")]
    private void LateUpdate_UCE_SpawnPlaceableObject()
    {
        if (UCE_SpawnPlaceableObjectValidation())
        {
            if ((UCE_myPlaceableObject || UCE_myPlaceableObjectItem) && UCE_checkTimer())
            {
                if (UCE_myPlaceableObject != null)
                {
                    Cmd_UCE_FinishUpgradePlaceableObject();
                }
                else if (UCE_myPlaceableObjectItem != null)
                {
                    Cmd_UCE_FinishSpawnPlaceableObject();
                }

                UCE_CancelSpawnPlaceableObject();
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_SpawnPlaceableObjectValidation
    // -----------------------------------------------------------------------------------
    public bool UCE_SpawnPlaceableObjectValidation()
    {
        if (
            (UCE_myPlaceableObject != null || UCE_myPlaceableObjectItem != null) && canInteract
            )
        {
            return true;
        }

        if ((UCE_myPlaceableObject || UCE_myPlaceableObjectItem))
        {
            UCE_CancelSpawnPlaceableObject();
        }
        return false;
    }

    // ===================================== UNSPAWN =====================================

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_UnspawnPlaceableObject
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_UnspawnPlaceableObject(NetworkIdentity ni)
    {
        if (ni != null)
        {
            UCE_PlaceableObject po = ni.GetComponent<UCE_PlaceableObject>();
            Entity e = po.GetComponent<Entity>();

            if (po &&
                po.pickupable &&
                po.ownerCharacter == this.name
            )
            {
                movement.LookAtY(po.gameObject.transform.position);

                ScriptableItem item;

                if (ScriptableItem.dict.TryGetValue(po.itemName.GetStableHashCode(), out item))
                {
                    // -- enough inventory space?

                    Item result = new Item(item);

                    if (inventory.CanAdd(result, ((UCE_Item_PlaceableObject)item).decreaseAmount))
                    {
                        // -- delete object from database
                        Database.singleton.UCE_DeletePlaceableObject(this.name, this.guild.name, e.level.current, po.itemName, po.id);

                        // -- add it back to the inventory again
                        inventory.CanAdd(new Item(item), ((UCE_Item_PlaceableObject)item).decreaseAmount);

                        // -- remove the object
                        NetworkServer.Destroy(po.gameObject);
                    }
                }
            }
        }
    }

    // ===================================== UPGRADE =====================================

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_StartUpgradePlaceableObject
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_StartUpgradePlaceableObject(NetworkIdentity ni)
    {
        if (ni != null)
        {
            UCE_PlaceableObject po = ni.GetComponent<UCE_PlaceableObject>();

            if (UCE_CanUpgradePlaceableObject(po))
            {
                Entity e = po.GetComponent<Entity>();

                if (e != null)
                {
                    UCE_addTask();
                    UCE_myPlaceableObject = po;
                    UCE_setTimer(po.getUpgradeCost(e.level.current).duration);
                    Target_UCE_StartUpgradePlaceableObjectClient(connectionToClient, ni);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_StartUpgradePlaceableObjectClient
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void Target_UCE_StartUpgradePlaceableObjectClient(NetworkConnection target, NetworkIdentity ni)
    {
        if (ni != null)
        {
            UCE_PlaceableObject po = ni.GetComponent<UCE_PlaceableObject>();
            Entity e = po.GetComponent<Entity>();

            if (e != null)
            {
                movement.LookAtY(po.gameObject.transform.position);
                UCE_myPlaceableObject = po;
                UCE_setTimer(po.getUpgradeCost(e.level.current).duration);
                UCE_CastbarShow(UCE_MSG_PLACEABLEOBJECT_UPGRADE, po.getUpgradeCost(e.level.current).duration);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_FinishUpgradePlaceableObject
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_FinishUpgradePlaceableObject()
    {
        // -- Revalidate in case of anything changed while upgrading
        if (UCE_myPlaceableObject)
        {
            if (UCE_CanUpgradePlaceableObject(UCE_myPlaceableObject))
            {
                Entity e = UCE_myPlaceableObject.GetComponent<Entity>();

                if (e != null)
                {
                    UCE_removeTask();

                    // -- remove Resources & Items
                    UCE_PlaceableObjectUpgradeCost cost = UCE_myPlaceableObject.getUpgradeCost(e.level.current);

                    gold -= cost.gold;
                    itemMall.coins -= cost.coins;

                    foreach (UCE_ItemRequirement reqitem in cost.requiredItems)
                    {
                        if (reqitem.item == null || inventory.Count(new Item(reqitem.item)) >= reqitem.amount)
                            inventory.Remove(new Item(reqitem.item), reqitem.amount);
                    }

                    // -- delete the old object
                    if (UCE_myPlaceableObject.permanent)
                    {
                        Database.singleton.UCE_DeletePlaceableObject(this.name, this.guild.name, e.level.current, UCE_myPlaceableObject.itemName, UCE_myPlaceableObject.id);
                    }

                    // -- upgrade the Object
                    e.level.current++;

                    // -- save Object to database immediately
                    if (UCE_myPlaceableObject.permanent)
                    {
                        Database.singleton.UCE_DeletePlaceableObject(this.name, this.guild.name, e.level.current, UCE_myPlaceableObject.itemName, UCE_myPlaceableObject.id);
                        Database.singleton.UCE_SavePlaceableObject(this.name, this.guild.name, UCE_myPlaceableObject.gameObject, e.level.current, UCE_myPlaceableObject.itemName, UCE_myPlaceableObject.id);
                    }

                    // -- reset the Attache (if any)
#if _iMMOATTACHE
                    Monster m = UCE_myPlaceableObject.GetComponent<Monster>();
                    if (m != null)
                        m.UCE_AttacheReset();
#endif
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanUpgradePlaceableObject
    // -----------------------------------------------------------------------------------
    public bool UCE_CanUpgradePlaceableObject(UCE_PlaceableObject po)
    {
        if (po)
        {
            Entity e = po.GetComponent<Entity>();

            if (e != null)
            {
                UCE_PlaceableObjectUpgradeCost c = po.getUpgradeCost(e.level.current);

                if (c != null)
                {
                    if (level.current >= c.minLevel &&
                        gold >= c.gold &&
                        itemMall.coins >= c.coins &&
                        UCE_checkHasAllItems(c)
                        )
                        return true;
                }
            }
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkHasAllItems
    // -----------------------------------------------------------------------------------
    public bool UCE_checkHasAllItems(UCE_PlaceableObjectUpgradeCost cost)
    {
        foreach (UCE_ItemRequirement reqitem in cost.requiredItems)
        {
            if (reqitem.item == null || inventory.Count(new Item(reqitem.item)) < reqitem.amount)
                return false;
        }
        return true;
    }

    // ====================================== SPAWN ======================================

    // -----------------------------------------------------------------------------------
    // PrepareSpawnPlaceableObject
    // @Server
    // -----------------------------------------------------------------------------------
    public void PrepareSpawnPlaceableObject(UCE_Item_PlaceableObject item)
    {
        UCE_myPlaceableObjectItem = item;
        Target_UCE_PrepareSpawnPlaceableObjectClient(connectionToClient, item.name.GetStableHashCode());
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_StartSpawnPlaceableObjectClient
    // @Server -> @Client
    // -----------------------------------------------------------------------------------

    [TargetRpc]
    public void Target_UCE_PrepareSpawnPlaceableObjectClient(NetworkConnection target, int itemHash)
    {
        ScriptableItem item;

        if (ScriptableItem.dict.TryGetValue(itemHash, out item))
        {
            UCE_myPlaceableObjectItem = (UCE_Item_PlaceableObject)item;

            if (UCE_CanSpawnPlaceableObject(UCE_myPlaceableObjectItem))
            {
                Vector3 vBuildPosition = UCE_getSpawnDestination();

                indicator.SetViaPosition(vBuildPosition);

                if (UCE_myPlaceableObjectItem.previewObject != null)
                    UCE_BuildSystem_PreviewObject = (GameObject)Instantiate(UCE_myPlaceableObjectItem.previewObject, vBuildPosition, Quaternion.identity);

                if (UCE_myPlaceableObjectItem.gridObject != null)
                    UCE_BuildSystem_GridObject = (GameObject)Instantiate(UCE_myPlaceableObjectItem.gridObject, vBuildPosition, Quaternion.identity);

                if (!_UCE_UI_PlaceableObjectSpawn)
                    _UCE_UI_PlaceableObjectSpawn = FindObjectOfType<UCE_UI_PlaceableObjectSpawn>();

                _UCE_UI_PlaceableObjectSpawn.Show();
                if (UCE_myPlaceableObjectItem.placementObject.name == "Ownership Placement Area Structure")
                    _UCE_UI_PlaceableObjectSpawn.GetComponentInChildren<Text>().text = "Place ownership over this area?";
                else
                    _UCE_UI_PlaceableObjectSpawn.GetComponentInChildren<Text>().text = "Place object here?";
            }   
            else
            {
                UCE_PopupShow(UCE_myPlaceableObjectItem.cannotBePlaced);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_CancelSpawnPlaceableObject
    // @Client
    // -----------------------------------------------------------------------------------
    public void UCE_CancelSpawnPlaceableObject()
    {
        if (UCE_BuildSystem_PreviewObject != null)
            Destroy(UCE_BuildSystem_PreviewObject);

        if (UCE_BuildSystem_GridObject != null)
            Destroy(UCE_BuildSystem_GridObject);

        UCE_BuildSystem_PreviewObject = null;
        UCE_myPlaceableObject = null;
        UCE_myPlaceableObjectItem = null;
        UCE_removeTask();
        UCE_stopTimer();
        UCE_CastbarHide();

        //Destroy(indicator);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_StartSpawnPlaceableObject
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_StartSpawnPlaceableObject()
    {
        if (UCE_CanSpawnPlaceableObject())
        {
            UCE_addTask();
            Target_UCE_StartSpawnPlaceableObjectClient(connectionToClient, UCE_myPlaceableObjectItem.name.GetStableHashCode());
            indicator.SetViaPosition(UCE_getSpawnDestination());
            UCE_setTimer(UCE_myPlaceableObjectItem.placementTime);
        }
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_StartSpawnPlaceableObjectClient
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void Target_UCE_StartSpawnPlaceableObjectClient(NetworkConnection target, int itemHash)
    {
        ScriptableItem item;

        if (ScriptableItem.dict.TryGetValue(itemHash, out item))
        {
            UCE_myPlaceableObjectItem = (UCE_Item_PlaceableObject)item;
            UCE_setTimer(UCE_myPlaceableObjectItem.placementTime);
            UCE_CastbarShow(UCE_myPlaceableObjectItem.name, UCE_myPlaceableObjectItem.placementTime);
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_FinishSpawnPlaceableObject
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_FinishSpawnPlaceableObject()
    {
        UCE_removeTask();
        StartCoroutine("SpawnPlaceableObject");
    }

    // -----------------------------------------------------------------------------------
    // SpawnPlaceableObject
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    protected IEnumerator SpawnPlaceableObject()
    {
        UCE_PlaceableObject o = null;
        int level = 0;

        Vector3 vBuildPosition = UCE_getSpawnDestination();

        if (UCE_myPlaceableObjectItem.placementObject)
        {
            GameObject SimpleObject = (GameObject)Instantiate(UCE_myPlaceableObjectItem.placementObject, vBuildPosition, Quaternion.identity);

            NetworkServer.Spawn(SimpleObject);

            o = SimpleObject.GetComponent<UCE_PlaceableObject>();

            if (o)
            {
                o.permanent = UCE_myPlaceableObjectItem.isPermanent;
                o.ownerCharacter = this.name;
                o.ownerGuild = this.guild.name;
                o.itemName = UCE_myPlaceableObjectItem.name;

                Entity e = SimpleObject.GetComponent<Entity>();

                if (e)
                    level = e.level.current;
            }
        }

        if (o != null && UCE_myPlaceableObjectItem.isPermanent)
            Database.singleton.UCE_SavePlaceableObject(this.name, this.guild.name, o.gameObject, level, UCE_myPlaceableObjectItem.name, o.id);

        inventory.Remove(new Item(UCE_myPlaceableObjectItem), UCE_myPlaceableObjectItem.decreaseAmount);
        UCE_myPlaceableObjectItem = null;

        yield return new WaitForEndOfFrame();
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanSpawnPlaceableObject
    // -----------------------------------------------------------------------------------
    public bool UCE_CanSpawnPlaceableObject(UCE_Item_PlaceableObject item)
    {
        return (item != null && canInteract && isAlive
            && UCE_checkSpawnDestination(item.doNotSpawnAt)
            && (item.placementAreaId == 0 || (UCE_myPlaceableObjectAreaId == item.placementAreaId))
            && (item.restrictedAreaId == 0 || (UCE_myPlaceableObjectAreaId != item.restrictedAreaId))
            && (!item.onlyPersonalArea || (item.onlyPersonalArea && UCE_myPlaceableObjectAreaOwner))
            && (!item.onlyGuildArea || (item.onlyGuildArea && UCE_myPlaceableObjectAreaGuild))
            && (!item.guildRankRequired || (item.guildRankRequired && UCE_CheckGuildRank(item.guildRank)))
#if _iMMOPVP
            && (!item.onlyPVPZone || item.onlyPVPZone && UCE_getInPvpRegion())
#endif
            );
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanSpawnPlaceableObject
    // -----------------------------------------------------------------------------------
    public bool UCE_CanSpawnPlaceableObject()
    {
        return UCE_CanSpawnPlaceableObject(UCE_myPlaceableObjectItem);
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckGuildRank
    // -----------------------------------------------------------------------------------
    protected bool UCE_CheckGuildRank(GuildRank guildRank)
    {
        return guild.guild.members != null && Array.FindIndex(guild.guild.members, (m) => m.name == name && m.rank >= guildRank) != -1;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SetPlaceableObjectArea
    // -----------------------------------------------------------------------------------
    public void UCE_SetPlaceableObjectArea(int id, string areaOwnerName, string areaGuildName, bool condition)
    {
        if (condition)
        {
            UCE_myPlaceableObjectAreaId = id;
            if (areaOwnerName == name)
            {
                UCE_myPlaceableObjectAreaOwner = true;
            }
            else
            {
                UCE_myPlaceableObjectAreaOwner = false;
            }
            if (areaGuildName == guild.name)
            {
                UCE_myPlaceableObjectAreaGuild = true;
            }
            else
            {
                UCE_myPlaceableObjectAreaGuild = false;
            }
        }
        else
        {
            UCE_myPlaceableObjectAreaId = 0;
            UCE_myPlaceableObjectAreaOwner = false;
            UCE_myPlaceableObjectAreaGuild = false;
        }
    }

    // =================================== HELPERS =======================================

    // -----------------------------------------------------------------------------------
    // UCE_getSpawnDestination
    // -----------------------------------------------------------------------------------
    public Vector3 UCE_getSpawnDestination()
    {
        Vector3 spawnPosition = Vector3.zero;

        Bounds bounds = collider.bounds;
        spawnPosition = transform.position + transform.forward * (bounds.size.x + 1);

        bool pass = false;
        int i = 0;

        while (!pass)
        {
            i++;

            UnityEngine.AI.NavMeshHit hit;

            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPosition, out hit, 1f, UnityEngine.AI.NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
            }

            if (!Physics.Raycast(spawnPosition, Vector3.down, 0))
            {
                return spawnPosition;
            }

            if (i > 100)
            {
                break;          //emergency break in case of nothing found after 100 passes
            }
        }

        return spawnPosition;
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkSpawnDestination
    // -----------------------------------------------------------------------------------
    public bool UCE_checkSpawnDestination(LayerMask doNotSpawnAt)
    {
        Vector3 spawnPosition = Vector3.zero;
        Bounds bounds = collider.bounds;
        spawnPosition = transform.position + transform.forward * (bounds.size.x + 2f);

        int i = 0;
        bool pass = false;

        while (!pass)
        {
            i++;

            UnityEngine.AI.NavMeshHit hit;

            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPosition, out hit, 1f, UnityEngine.AI.NavMesh.AllAreas))
                spawnPosition = hit.position;

            if (!Physics.Raycast(spawnPosition, Vector3.down, 0, doNotSpawnAt))
                return true;

            if (i > 100)
                return false;           //emergency break in case of nothing found after 100 passes
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
}
