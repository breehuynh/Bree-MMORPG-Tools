// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEditor;
using System;

// NETWORK MANAGER

public partial class NetworkManagerMMO
{

    [Header("Spawnable Categories")]
    public string[] spawnableCategories;

    // -----------------------------------------------------------------------------------
    // AutoRegisterSpawnables
    // @Editor
    // -----------------------------------------------------------------------------------
    public void AutoRegisterSpawnables()
    {
#if UNITY_EDITOR

        var guids = AssetDatabase.FindAssets("t:Prefab");
        List<GameObject> toSelect = new List<GameObject>();
        spawnPrefabs.Clear();

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Object[] toCheck = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (UnityEngine.Object obj in toCheck)
            {
                var go = obj as GameObject;
                if (go == null)
                {
                    continue;
                }

                NetworkIdentity comp = go.GetComponent<NetworkIdentity>();
                if (comp != null && !comp.serverOnly)
                {

                    UCE_NetworkSpawnable sp = go.GetComponent<UCE_NetworkSpawnable>();

                    if (sp == null)
                    {
                        toSelect.Add(go);
                    }
                    else if (UCE_Tools.ArrayContains(spawnableCategories, sp.networkSpawnTag))
                    {
                        toSelect.Add(go);
                    }
                }

            }
        }

        spawnPrefabs.AddRange(toSelect.ToArray());

        Debug.Log("Added [" + toSelect.Count + "] network prefabs to spawnables list");

#endif
    }

    // -----------------------------------------------------------------------------------
}
