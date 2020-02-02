// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
public partial struct Skill
{
    // wont show up in the skill window for learning/upgrade - only works with UCE skill window addon
    public bool unlearnable { get { return data.unlearnable; } }

    // is considered to be a negative status effect and can be removed by certain skills
    public bool disadvantageous { get { return data.disadvantageous; } }

}
