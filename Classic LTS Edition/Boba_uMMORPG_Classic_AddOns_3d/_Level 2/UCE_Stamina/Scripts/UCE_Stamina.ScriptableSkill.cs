// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

public abstract partial class ScriptableSkill
{

#if _iMMOSTAMINA
    [Tooltip("Stamina Special Rule: Can be cast with insufficient stamina as well!")]
    public LinearInt staminaCosts;
#endif

}
