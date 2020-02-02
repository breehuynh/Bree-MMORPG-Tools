// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// ===================================================================================
// SPAWNABLE ENTITY - WAVE
// ===================================================================================
[System.Serializable]
public partial class UCE_WaveSpawnableEntity
{
    public GameObject entity;
    [Range(0.01f, 1)] public float probability;
    [Range(1, 999)] public int maxAmount;
    [HideInInspector] public int amount;
}

// ===================================================================================
// SPAWNABLE ENTITY - FIXED
// ===================================================================================
[System.Serializable]
public partial class UCE_FixedSpawnableGameObject : UCE_SpawnableGameObject
{
    public Transform transform;
}

// ===================================================================================
// SPAWNABLE ENTITY - MOB
// ===================================================================================
[System.Serializable]
public partial class UCE_MobSpawnableGameObject : UCE_SpawnableGameObject
{
    [Tooltip("[Optional] Target transform for this object (leave empty to randomize within area)")]
    public Transform transform;

    [Range(0.01f, 1)] public float probability;
    public UCE_Area_Spawner.ScaleLevel scaleLevel;
    public int levelAdjustment = 0;
}

// ===================================================================================
// SPAWNABLE ENTITY - BASE
// ===================================================================================
[System.Serializable]
public partial class UCE_StartSpawnableGameObject : UCE_SpawnableGameObject
{
    [Range(0.01f, 1)] public float probability;
}

// ===================================================================================
// SPAWNABLE ENTITY - BASE
// ===================================================================================
[System.Serializable]
public abstract partial class UCE_SpawnableGameObject
{
    public GameObject gameobject;
}

// ===================================================================================
// SPAWN DESTINATION
// ===================================================================================
[System.Serializable]
public partial class UCE_SpawnDestination
{
    public Transform transform;

    [Tooltip("[Required] Probabilities of all elements must sum up to 1.0 (= 100%)")]
    [Range(0, 1)] public float probability;
}
