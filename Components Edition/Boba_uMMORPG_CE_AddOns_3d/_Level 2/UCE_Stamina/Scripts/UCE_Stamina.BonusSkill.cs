// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using UnityEngine;
using Mirror;

// BONUS SKILL

public abstract partial class BonusSkill : ScriptableSkill
{
    public bool blockStaminaRecovery;
    public LinearInt bonusStaminaMax;
    public LinearFloat bonusStaminaPercentPerSecond;
}
