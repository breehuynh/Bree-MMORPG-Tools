// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

// UCE ADDON

[Serializable]
public partial class UCE_AddOn
{
    [HideInInspector] public string name;
    [HideInInspector] public string define;
    [ReadOnly] public string basis;
    [ReadOnly] public string author;
    [ReadOnly] public string version;
    [ReadOnly] public string dependencies;
    [ReadOnly] [TextArea(1, 30)]public string comments;
    public bool active;

    public void Copy(UCE_AddOn addon)
    {
        name            = addon.name;
        define          = addon.define;
        basis           = addon.basis;
        author          = addon.author;
        version         = addon.version;
        dependencies    = addon.dependencies;
        comments        = addon.comments;
        active          = addon.active;
    }

}

// UCE EDITOR TOOLS

[InitializeOnLoad]
public static partial class UCE_EditorTools
{
    // -------------------------------------------------------------------------------
    // AddScriptingDefine
    // -------------------------------------------------------------------------------
    public static void AddScriptingDefine(string define)
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string definestring = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        string[] defines = definestring.Split(';');

        if (UCE_Tools.ArrayContains(defines, define))
            return;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (definestring + ";" + define));
    }

    // -------------------------------------------------------------------------------
    // RemoveScriptingDefine
    // -------------------------------------------------------------------------------
    public static void RemoveScriptingDefine(string define)
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string definestring = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        string[] defines = definestring.Split(';');

        defines = UCE_Tools.RemoveFromArray(defines, define);

        definestring = string.Join(";", defines);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (definestring));
    }

    // -----------------------------------------------------------------------------------
}

#endif
