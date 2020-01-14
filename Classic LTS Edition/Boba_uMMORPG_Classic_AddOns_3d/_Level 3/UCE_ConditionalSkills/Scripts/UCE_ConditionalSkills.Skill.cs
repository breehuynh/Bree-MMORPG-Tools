// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Mirror;

// SKILL

public partial struct Skill
{

    // -------------------------------------------------------------------------
    // UCE_CheckSkillConditions
    // -------------------------------------------------------------------------
    public bool UCE_CheckSkillConditions(Entity caster)
    {
       
       	bool bValid = CheckSelf(caster);
       
        if (UnityEngine.Random.value > data.skillConditions.activationChance)
			bValid = false;
			
        if (data.skillConditions.healthThreshold == Monster.ParentThreshold.Above && (caster.health < caster.healthMax * data.skillConditions.casterHealth))
            bValid = false;

        if (data.skillConditions.healthThreshold == Monster.ParentThreshold.Below && (caster.health > caster.healthMax * data.skillConditions.casterHealth))
            bValid = false;

#if _iMMOMORALE
        if (data.skillConditions.moraleThreshold == Monster.ParentThreshold.Above && (caster.morale < caster.moraleMax * data.skillConditions.casterMorale))
            bValid = false;

        if (data.skillConditions.moraleThreshold == Monster.ParentThreshold.Below && (caster.morale > caster.moraleMax * data.skillConditions.casterMorale))
            bValid = false;
#endif

        if (data.skillConditions.activeBuff != null && !caster.UCE_checkHasBuff(data.skillConditions.activeBuff))
            bValid = false;

        return bValid;
    }

}