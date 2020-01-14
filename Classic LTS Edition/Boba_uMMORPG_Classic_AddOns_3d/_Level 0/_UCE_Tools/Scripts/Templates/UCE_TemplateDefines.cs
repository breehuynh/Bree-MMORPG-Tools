// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

// TemplateDefines

[CreateAssetMenu(menuName = "UCE Other/UCE Defines", fileName = "UCE_Defines", order = 999)]
public partial class UCE_TemplateDefines : ScriptableObject
{

    static UCE_TemplateDefines _instance;
#if UNITY_EDITOR
    [SerializeField]
    [Header("(Change list size to force refresh)")]
    public List<UCE_AddOn> addons = new List<UCE_AddOn>();
#endif
    // -----------------------------------------------------------------------------------
    // OnValidate
    // -----------------------------------------------------------------------------------
    void OnValidate()
    {
#if UNITY_EDITOR

        if (UCE_DefinesManager.addons.Count() > 0 && addons.Count() != UCE_DefinesManager.addons.Count() - 1)
        {

            addons.Clear();

            for (int i = 0; i < UCE_DefinesManager.addons.Count(); ++i)
            {

                UCE_AddOn addon = new UCE_AddOn();
                addon.Copy(UCE_DefinesManager.addons[i]);

                if (addon.define != "_iMMOTOOLS" && addon.define != "_iMMOCORE")
                    addons.Add(addon);
                else
                    UCE_EditorTools.AddScriptingDefine(addon.define);
            }

        }

        UpdateDefines();

#endif
    }

    // -----------------------------------------------------------------------------------
    // UpdateDefines
    // -----------------------------------------------------------------------------------
    public void UpdateDefines()
    {
#if UNITY_EDITOR
        for (int i = 0; i < addons.Count(); ++i)
        {
            if (addons[i].define == "_iMMOTOOLS" && addons[i].define == "_iMMOCORE") continue;

            if (!addons[i].active)
                UCE_EditorTools.RemoveScriptingDefine(addons[i].define);
            else
                UCE_EditorTools.AddScriptingDefine(addons[i].define);
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // Singleton
    // -----------------------------------------------------------------------------------
    public static UCE_TemplateDefines singleton
    {
        get 
        {
            if (!_instance)
                _instance = Resources.FindObjectsOfTypeAll<UCE_TemplateDefines>().FirstOrDefault();
            return _instance;
        }
    }

    // -----------------------------------------------------------------------------------

}
