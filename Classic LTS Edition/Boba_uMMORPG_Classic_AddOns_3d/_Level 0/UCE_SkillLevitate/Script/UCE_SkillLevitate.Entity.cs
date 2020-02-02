// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public partial class Entity
{
    protected float _levitateHeight = 0;

    // -----------------------------------------------------------------------------------
    // Update_UCE_SkillLevitate
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Update")]
    private void Update_UCE_SkillLevitate()
    {
        float levitateHeight = buffs.Sum(buff => buff.levitateHeight);

        if (levitateHeight > 0)
        {
            _levitateHeight = levitateHeight;

            if (GetComponent<NavMeshAgent>().baseOffset < levitateHeight)
                GetComponent<NavMeshAgent>().baseOffset += levitateHeight * Time.deltaTime;
        }
        else
        {
            if (GetComponent<NavMeshAgent>().baseOffset > 0)
                GetComponent<NavMeshAgent>().baseOffset -= _levitateHeight * Time.deltaTime;
        }
    }

    // -----------------------------------------------------------------------------------
}
