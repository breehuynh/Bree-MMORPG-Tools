// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Linq;

#if _iMMOCRAFTING

// CRAFTING PROFESSION

public struct UCE_CraftingProfession
{
    public string templateName;
    public long experience;

    // -----------------------------------------------------------------------------------
    // UCE_CraftingProfession (Constructor)
    // -----------------------------------------------------------------------------------
    public UCE_CraftingProfession(string _templateName)
    {
        templateName = _templateName;
        experience = 0;
    }

    // -----------------------------------------------------------------------------------
    // level (Getter)
    // -----------------------------------------------------------------------------------
    public int level
    {
        get
        {
            long exp = this.experience;
            return 1 + template.levels.Count(l => l <= exp);
        }
    }

    // -----------------------------------------------------------------------------------
    // experiencePercent (Getter)
    // -----------------------------------------------------------------------------------
    public float experiencePercent
    {
        get
        {
            return (experience != 0 && experienceNext != 0) ? (float)(experience - experiencePrevious) / (float)(experienceNext - experiencePrevious) : 0;
        }
    }

    // -----------------------------------------------------------------------------------
    // experiencePrevious (Getter)
    // -----------------------------------------------------------------------------------
    public long experiencePrevious
    {
        get
        {
            if (level == 1)
                return 0;
            else
                return template.levels[level - 2];
        }
    }

    // -----------------------------------------------------------------------------------
    // maxlevel (Getter)
    // -----------------------------------------------------------------------------------
    public int maxlevel
    {
        get { return 1 + template.levels.Count(); }
    }

    // -----------------------------------------------------------------------------------
    // experienceNext (Getter)
    // -----------------------------------------------------------------------------------
    public long experienceNext
    {
        get
        {
            long exp = this.experience;

            if (level == maxlevel)
            {
                return exp;
            }
            else if (level > 1)
            {
                return template.levels[level - 1];
            }
            return template.levels[0];
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CraftingProfessionTemplate (Getter)
    // -----------------------------------------------------------------------------------
    public UCE_CraftingProfessionTemplate template
    {
        get { return UCE_CraftingProfessionTemplate.dict[templateName.GetStableHashCode()]; }
    }

    // -----------------------------------------------------------------------------------
}

public class SyncListUCE_CraftingProfession : SyncList<UCE_CraftingProfession> { }

#endif
