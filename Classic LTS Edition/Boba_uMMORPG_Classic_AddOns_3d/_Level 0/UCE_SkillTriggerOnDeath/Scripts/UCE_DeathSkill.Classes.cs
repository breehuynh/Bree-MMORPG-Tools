// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE DEATH SKILL

[System.Serializable]
public partial class UCE_DeathSkill
{
    [SerializeField] public ScriptableSkill[] deathSkill;
    public int deathSkillMinLevel = 1;
    public int deathSkillMaxLevel = 2;
    [SerializeField] [Range(0, 1)] public float deathSkillChance = 1.0f;
}
