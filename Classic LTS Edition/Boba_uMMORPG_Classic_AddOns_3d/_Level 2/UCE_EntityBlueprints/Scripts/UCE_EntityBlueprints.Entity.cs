// =======================================================================================
// Maintained by bobatea#9400 on Discord
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

// ENTITY

public partial class Entity
{

    [Header("BLUEPRINTS (Editor)")]
    public UCE_TemplateEntityBlueprint[] blueprints;

    // -----------------------------------------------------------------------------------
    // ApplyBlueprints
    // @Editor
    // -----------------------------------------------------------------------------------
    public void ApplyBlueprints()
    {
#if UNITY_EDITOR

        int baseValue = 0;
        int bonusPerLevel = 0;

        // -------- Health
        foreach (UCE_TemplateEntityBlueprint blueprint in blueprints)
        {
            if (!blueprint) continue;

            baseValue       += blueprint.healthMax.baseValue;
            bonusPerLevel   += blueprint.healthMax.bonusPerLevel;
        }

        if (baseValue != 0)
            _healthMax = new LinearInt { baseValue = baseValue, bonusPerLevel = bonusPerLevel };

        // -------- 


        // -------- 


        // -------- 


#endif
    }

    // -----------------------------------------------------------------------------------
}
