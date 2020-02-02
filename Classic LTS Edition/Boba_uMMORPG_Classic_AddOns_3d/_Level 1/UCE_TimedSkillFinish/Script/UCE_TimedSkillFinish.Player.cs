// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// PLAYER

public partial class Player
{

#if !_iMMOCONDITIONALSKILLS
    private int lastSkill = -1;
#endif

    // -----------------------------------------------------------------------------------
    // UCE_StateSkillFinished
    // -----------------------------------------------------------------------------------
    public bool UCE_StateSkillFinished()
    {
        // -- only triggers with time modifier
        if (UCE_EventSkillFinished())
        {
            if (lastSkill != currentSkill)
            {
                Skill skill = skills[currentSkill];
                UCE_FinishCastSkillEarly(skill);
                lastSkill = currentSkill;
            }
        }

        // -- triggers in any case when its finished
        if (EventSkillFinished())
        {
            Skill skill = skills[currentSkill];

            if (lastSkill != currentSkill)
            {
                FinishCastSkill(skill);
            }
            else
            {
                UCE_FinishCastSkillLate(skill);
            }

            lastSkill = -1;
            currentSkill = -1;

            UseNextTargetIfAny();

            return true;
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
}
