// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

public abstract partial class ScriptableSkill
{
    [Tooltip("This skill cannot be learned via the Skill Window, only via other means")]
    public bool unlearnable;

    [Tooltip("Checked = negative skill, Unchecked = positive skill. Certain skills can debuff disadvantageous skills only")]
    public bool disadvantageous;

}
