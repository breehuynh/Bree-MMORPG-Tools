using UnityEngine;

public class SmartNpcSkills : Skills
{
    // the last skill that was casted, to decide which one to cast next
    [HideInInspector] public int lastSkill = -1;

    public override void OnStartServer()
    {
        // load skills based on skill templates
        foreach (ScriptableSkill skillData in skillTemplates)
            skills.Add(new Skill(skillData));
    }

    // helper function to get the current cast range (if casting anything)
    public float CurrentCastRange()
    {
        return 0 <= currentSkill && currentSkill < skills.Count
               ? skills[currentSkill].castRange
               : 0;
    }

    // helper function to decide which skill to cast
    // => we got through skills one after another, this is better than selecting
    //    a random skill because it allows for some planning like:
    //    'strong skeleton always starts with a stun' etc.
    public int NextSkill()
    {
        // find the next ready skill, starting at 'lastSkill+1' (= next one)
        // and looping at max once through them all (up to skill.Count)
        //  note: no skills.count == 0 check needed, this works with empty lists
        //  note: also works if lastSkill is still -1 from initialization
        for (int i = 0; i < skills.Count; ++i)
        {
            int index = (lastSkill + 1 + i) % skills.Count;
            // could we cast this skill right now? (enough mana, skill ready, etc.)
            if (CastCheckSelf(skills[index]))
                return index;
        }
        return -1;
    }
}
