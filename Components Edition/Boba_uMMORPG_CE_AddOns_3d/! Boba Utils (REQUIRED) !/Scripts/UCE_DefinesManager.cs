// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// DEFINES MANAGER

[InitializeOnLoad]
public partial class UCE_DefinesManager
{

    public static List<UCE_AddOn> addons = new List<UCE_AddOn>();

    // -----------------------------------------------------------------------------------
    // UCE_DefinesManager
    // -----------------------------------------------------------------------------------
    static UCE_DefinesManager()
    {
        DevExtUtils.InvokeStaticDevExtMethods(typeof(UCE_DefinesManager), "Constructor");
    }
	
	// -----------------------------------------------------------------------------------
	
}

#endif