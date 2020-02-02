// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE_BindPoint

[System.Serializable]
public struct UCE_BindPoint
{
    public string name;
    public string SceneName;
    public Vector3 position;

    public UnityScene mapScene
    {
        set { SceneName = value.SceneName; }
    }

    public bool Valid
    {
        get
        {
            return !string.IsNullOrEmpty(SceneName);
        }
    }
}
