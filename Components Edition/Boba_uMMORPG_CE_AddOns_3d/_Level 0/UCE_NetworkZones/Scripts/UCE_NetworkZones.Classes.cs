// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Mirror;

// SceneLocation

[System.Serializable]
public partial class SceneLocation
{
    public UnityScene mapScene;
    public Vector3 position;

    public bool Valid
    {
        get
        {
            return mapScene.IsSet();
        }
    }
}