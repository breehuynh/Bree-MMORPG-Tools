// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// TemplateEntityBlueprint

[CreateAssetMenu(menuName = "UCE Other/UCE Entity Blueprint", fileName = "New UCE Entity Blueprint", order = 999)]
public partial class UCE_TemplateEntityBlueprint : ScriptableObject
{

    [Header("Health")]
    public LinearInt healthMax = new LinearInt { baseValue = 100, bonusPerLevel = 0 };

    /*
     * TODO: add all other stats
     * like mana, crit rate, etc.
     * */    

    // -----------------------------------------------------------------------------------
    // OnValidate
    // -----------------------------------------------------------------------------------
    public void OnValidate()
    {
#if UNITY_EDITOR
        
#endif
    }

    // -----------------------------------------------------------------------------------
}
