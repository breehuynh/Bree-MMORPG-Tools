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

// -----------------------------------------------------------------------------------
// UCE_AttachmentChild
// -----------------------------------------------------------------------------------
[Serializable]
public partial class UCE_AttachmentChild
{
    [Tooltip("Only one object according to the level of the parent is spawned/unspawned")]
    public GameObject[] childGameObjects;

    public bool autoSpawn = true;
    public bool autoUnspawn = true;

    [Tooltip("[Optional] Makes each child object die instead of just removing it (only works with Entities)")]
    public bool killOnUnspawn = false;

#if _iMMOMINION
    public bool followMaster = true;
#endif
    public Transform mountPoint;
    public UCE_AttachmentChildCondition[] childConditions;
    [HideInInspector] public GameObject cacheGameObject;
}

// -----------------------------------------------------------------------------------
// UCE_AttachmentChildCondition
// -----------------------------------------------------------------------------------
[Serializable]
public partial class UCE_AttachmentChildCondition
{
    [Tooltip("Health of the parent must be 'below' or 'above' the Health threshold")]
    public Monster.ParentThreshold parentThreshold;

    [Tooltip("Health treshold of the parent in order to trigger condition")]
    [Range(0, 1)] public float parentHealth;

    [Tooltip("Parent must have this active Buff in order to trigger condition")]
    public BuffSkill activeBuff;

    [Tooltip("Parent must have this item equipped in order to trigger condition")]
    public ScriptableItem equippedItem;

    [Tooltip("Parent must have this item in inventory in order to trigger condition")]
    public ScriptableItem inventoryItem;

    [Tooltip("When triggered - is the child spawned or destroyed?")]
    public Monster.ChildAction childAction;
}
