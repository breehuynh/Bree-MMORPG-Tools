// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// HAZARD BUFF

[System.Serializable]
public class UCE_HazardBuff
{
    public TargetBuffSkill buff;
    public int minBuffLevel;
    public int maxBuffLevel;
    public float chance = 1f;
    public string protectiveMessage = "You are protected against the Hazard Floor effects!";
    public UCE_ActivationRequirements protectiveRequirements;
}
