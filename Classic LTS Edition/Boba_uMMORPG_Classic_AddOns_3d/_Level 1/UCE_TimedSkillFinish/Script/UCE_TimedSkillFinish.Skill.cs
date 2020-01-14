// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;

// SKILL

public partial struct Skill
{
    // -----------------------------------------------------------------------------------
    // UCE_CastTimePassed
    // -----------------------------------------------------------------------------------
    public double UCE_CastTimePassed() => (NetworkTime.time - castTimeEnd) + castTime;
}
