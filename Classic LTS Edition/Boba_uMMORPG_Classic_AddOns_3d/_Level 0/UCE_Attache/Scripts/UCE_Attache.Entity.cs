// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// ENTITY

public partial class Entity
{
    public enum ParentThreshold { None, Below, Above }

    public enum ChildAction { None, Spawn, Destroy }

    [Header("[-=-=-=- UCE ATTACHE -=-=-=-]")]
    public UCE_AttachmentChild[] attachedChilds;

    protected bool attacheSpawned;
    protected float cacheHealth;

    // -----------------------------------------------------------------------------------
    // Update
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    [DevExtMethods("Update")]
    private void Update_UCE_Attache()
    {
        if (NetworkServer.active && attachedChilds.Length > 0 && isAlive && cacheHealth != health)
        {
            cacheHealth = health;
            UCE_AttacheSpawn();
        }
    }

    // -----------------------------------------------------------------------------------
    // OnDeath_UCE_Attache
    // @Server
    // -----------------------------------------------------------------------------------
    [Server]
    [DevExtMethods("OnDeath")]
    private void OnDeath_UCE_Attache()
    {
        UCE_AttacheUnspawn();
    }

    // -----------------------------------------------------------------------------------
    // UCE_getAttache
    // @Server
    // -----------------------------------------------------------------------------------
    protected GameObject UCE_getAttache(int index)
    {
        if (attachedChilds.Length > 0 && attachedChilds.Length >= index && attachedChilds[index].childGameObjects.Length > 0 && attachedChilds[index].childGameObjects.Length >= level)
            return attachedChilds[index].childGameObjects[level - 1];

        if (attachedChilds.Length > 0 && attachedChilds.Length >= index)
            return attachedChilds[index].childGameObjects[0];

        return attachedChilds[0].childGameObjects[0];
    }

    // -----------------------------------------------------------------------------------
    // UCE_AttacheReset
    // @Server
    // -----------------------------------------------------------------------------------
    public void UCE_AttacheReset()
    {
        UCE_AttacheUnspawn();
        UCE_AttacheSpawn();
    }

    // -----------------------------------------------------------------------------------
    // UCE_AttacheUnspawn
    // @Server
    // -----------------------------------------------------------------------------------
    protected void UCE_AttacheUnspawn()
    {
        if (attachedChilds.Length > 0)
        {
            for (int i = 0; i < attachedChilds.Length; ++i)
            {
                if (attachedChilds[i].cacheGameObject != null && attachedChilds[i].autoUnspawn)
                {
                    if (attachedChilds[i].cacheGameObject.GetComponent<Entity>() != null && attachedChilds[i].killOnUnspawn)
                        attachedChilds[i].cacheGameObject.GetComponent<Entity>().health = 0;
                    else
                        NetworkServer.Destroy(attachedChilds[i].cacheGameObject);
                    attachedChilds[i].cacheGameObject = null;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_AttacheSpawn
    // @Server
    // -----------------------------------------------------------------------------------
    protected void UCE_AttacheSpawn()
    {
        if (attachedChilds.Length > 0)
        {
            for (int i = 0; i < attachedChilds.Length; ++i)
            {
                if (attachedChilds[i].autoSpawn && UCE_checkAttachmentConditions(i, ChildAction.Spawn) && attachedChilds[i].cacheGameObject == null)
                {
                    var go = (GameObject)Instantiate(UCE_getAttache(i), attachedChilds[i].mountPoint.position, Quaternion.identity);
                    if (go)
                    {
                        NetworkServer.Spawn(go);
#if _iMMOMINION
                        if (go.GetComponent<Monster>() != null)
                        {
                            go.GetComponent<Monster>().myMaster = this.gameObject;
                            go.GetComponent<Monster>().followMaster = attachedChilds[i].followMaster;
                        }
#endif
                        attachedChilds[i].cacheGameObject = go;
                    }
                }
                else if (UCE_checkAttachmentConditions(i, ChildAction.Destroy) && attachedChilds[i].cacheGameObject != null)
                {
                    NetworkServer.Destroy(attachedChilds[i].cacheGameObject);
                    attachedChilds[i].cacheGameObject = null;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_checkAttachmentConditions
    // @Server
    // -----------------------------------------------------------------------------------
    protected bool UCE_checkAttachmentConditions(int index, ChildAction action)
    {
        foreach (UCE_AttachmentChildCondition condi in attachedChilds[index].childConditions)
        {
            if (condi.childAction == action)
            {
                if (
                    (condi.activeBuff == null || UCE_checkHasBuff(condi.activeBuff)) &&
                    (condi.equippedItem == null || UCE_checkHasEquipment(condi.equippedItem)) &&
                    (condi.inventoryItem == null || UCE_checkHasItem(condi.inventoryItem)) &&
                    (
                    (condi.parentThreshold == ParentThreshold.Above && (health >= healthMax * condi.parentHealth)) ||
                    (condi.parentThreshold == ParentThreshold.Below && (health < healthMax * condi.parentHealth)))
                    )
                {
                    return true;
                }
            }
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
}
