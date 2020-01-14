// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RTS BUILD SYSTEM NETWORK MANAGER MMO

public partial class NetworkManagerMMO
{
    // -----------------------------------------------------------------------------------
    // OnStartServer_UCE_RTSBuildSystem
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartServer")]
    private void OnStartServer_UCE_RTSBuildSystem()
    {
        Invoke("StartSpawnRTSStructures", 1);
    }

    // -----------------------------------------------------------------------------------
    // StartSpawnRTSStructures
    // Started with a small delay to prevent a sync issue because server is not ready yet
    // -----------------------------------------------------------------------------------
    protected void StartSpawnRTSStructures()
    {
#if _SERVER
        if (NetworkServer.active)
            StartCoroutine("SpawnRTSStructures");
#endif
    }

    // -----------------------------------------------------------------------------------
    // SpawnRTSStructures
    // -----------------------------------------------------------------------------------
    protected IEnumerator SpawnRTSStructures()
    {
#if _SERVER
        var table = Database.singleton.UCE_LoadPlaceableObjects();

        foreach (var row in table)
        {
#if _SQLITE
            string itemName = row.item;

            if (((UCE_Item_PlaceableObject)ScriptableItem.dict[itemName.GetDeterministicHashCode()]).placementObject)
            {
                Vector3 v = new Vector3(row.x, row.y, row.z);
                Quaternion rotation = Quaternion.Euler(row.xRot, row.yRot, row.zRot);

#elif _MYSQL
            string itemName = (string)row[9];

            if (((UCE_Item_PlaceableObject)ScriptableItem.dict[itemName.GetDeterministicHashCode()]).placementObject)
            {
                Vector3 v = new Vector3(
                                (float)row[2],
                                (float)row[3],
                                (float)row[4]);

                Quaternion rotation = Quaternion.Euler((float)row[5], (float)row[6], (float)row[7]);
#endif
                GameObject go = (GameObject)Instantiate(((UCE_Item_PlaceableObject)ScriptableItem.dict[itemName.GetDeterministicHashCode()]).placementObject, v, rotation);
                UCE_PlaceableObject po = go.GetComponent<UCE_PlaceableObject>();

                if (po)
                {
                    po.permanent = true;
                    po.itemName = itemName; // row 9
#if _SQLITE
                    po.ownerCharacter = row.character;
                    po.ownerGuild = row.guild;
                    po.id = row.id;
#elif _MYSQL
                    po.ownerCharacter = (string)row[0];
                    po.ownerGuild = (string)row[1];
					po.id				= (int)row[10];
#endif

                    Entity e = po.GetComponent<Entity>();

                    if (e)
                    {
#if _SQLITE
                        e.level = row.level;
#elif _MYSQL
						e.level = (int)row[8];
#endif
                    }
                }

                NetworkServer.Spawn(go);
            }
        }
#endif
        yield return new WaitForEndOfFrame();
    }

    // -----------------------------------------------------------------------------------
}
