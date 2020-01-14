// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE CONDITIONAL SKILL - CLASS

[System.Serializable]
public partial class UCE_ConditionalSkill
{
    [Header("Random")]
    [Tooltip("Basic, random activation chance of this skill (0.1 = 10%)")]
    [Range(0, 1)] public float activationChance = 1f;

    [Header("Health")]
    [Tooltip("Health of the caster must be 'below' or 'above' the threshold")]
    public Monster.ParentThreshold healthThreshold;

    [Tooltip("Health treshold of the caster in order to trigger condition")]
    [Range(0, 1)] public float casterHealth;

#if _iMMOMORALE
    [Header("Morale")]
    [Tooltip("Morale of the caster must be 'below' or 'above' the threshold")]
    public Monster.ParentThreshold moraleThreshold;

    [Tooltip("Morale treshold of the caster in order to trigger condition")]
    [Range(0, 1)] public float casterMorale;
#endif

    [Header("Other")]
    [Tooltip("Caster must have this active Buff in order to trigger condition")]
    public BuffSkill activeBuff;

}
