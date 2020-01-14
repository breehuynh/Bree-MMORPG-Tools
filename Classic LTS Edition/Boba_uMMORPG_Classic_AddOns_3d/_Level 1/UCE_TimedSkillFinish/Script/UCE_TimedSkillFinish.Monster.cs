// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// MONSTER

public partial class Monster
{
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
                UCE_FinishCastSkillEarly(skills[currentSkill]);
                if (target.health == 0) target = null;
                lastSkill = currentSkill;
            }
        }

        // -- triggers in any case when its finished
        if (EventSkillFinished())
        {
            if (lastSkill != currentSkill)
            {
                FinishCastSkill(skills[currentSkill]);
            }
            else
            {
                UCE_FinishCastSkillLate(skills[currentSkill]);
            }

            if (target.health == 0) target = null;
            lastSkill = -1;
            currentSkill = -1;

            return true;
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
}
