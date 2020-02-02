// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;

// ENTITY

public partial class Entity
{

    protected int lastSkill = -1;

    // -------------------------------------------------------------------------
    // NextSkill
    // -------------------------------------------------------------------------
    protected int NextSkill()
    {

        for (int i = 0; i < skills.skills.Count; ++i)
        {
            int index = (lastSkill + 1 + i) % skills.skills.Count;
            if (skills.skills[index].UCE_CheckSkillConditions(this))
                return index;

        }
        return -1;
    }

}
