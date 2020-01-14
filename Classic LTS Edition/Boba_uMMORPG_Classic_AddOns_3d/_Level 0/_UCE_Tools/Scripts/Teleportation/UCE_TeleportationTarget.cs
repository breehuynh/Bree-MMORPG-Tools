// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE TELEPORTATION TARGET

[System.Serializable]
public partial class UCE_TeleportationTarget
{
    public enum TeleportationType { onScene, offScene }

#if _iMMOZONES
    public TeleportationType teleportationType = TeleportationType.onScene;

    [StringShowConditional(conditionFieldName: "teleportationType", conditionValue: "onScene")]
    public Transform targetPosition;

    [StringShowConditional(conditionFieldName: "teleportationType", conditionValue: "offScene")]
    public SceneLocation offSceneTarget;
#else
    public Transform targetPosition;
#endif

    // -----------------------------------------------------------------------------------
    // name
    // -----------------------------------------------------------------------------------
    public string name
    {
        get
        {
#if _iMMOZONES
            if (teleportationType == TeleportationType.onScene)
                return targetPosition.name;
            return offSceneTarget.mapScene.SceneName;
#else
            if (targetPosition != null)
                return targetPosition.name;
            else
                return "";
#endif
        }
    }

    // -----------------------------------------------------------------------------------
    // getDistance
    // Returns the distance of the stated transform to the target
    // -----------------------------------------------------------------------------------
    public float getDistance(Transform transform)
    {
#if _iMMOZONES
        if (teleportationType == TeleportationType.onScene)
            return Vector3.Distance(targetPosition.position, transform.position);
        return 1;
#else
        return Vector3.Distance(targetPosition.position, transform.position);
#endif
    }

    // -----------------------------------------------------------------------------------
    // Valid
    // -----------------------------------------------------------------------------------
    public bool Valid
    {
        get
        {
#if _iMMOZONES
            if (teleportationType == TeleportationType.onScene)
                return targetPosition != null;
            return offSceneTarget.Valid;
#else
            return targetPosition != null;
#endif
        }
    }

    // -----------------------------------------------------------------------------------
    // OnTeleport
    // @Server
    // -----------------------------------------------------------------------------------
    public void OnTeleport(Player player)
    {
        if (!player || !Valid) return;

#if _iMMOZONES
        if (teleportationType == TeleportationType.onScene)
            player.UCE_Warp(targetPosition.position);
        else
            player.UCE_OnPortal(offSceneTarget);
#else
        player.UCE_Warp(targetPosition.position);
#endif
    }

    // -----------------------------------------------------------------------------------
}
