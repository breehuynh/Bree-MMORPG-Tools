// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// DEFAULT UNLOCKED CLASSES

[CreateAssetMenu(fileName = "UCE Default Unlocked Classes", menuName = "UCE Templates/New UCE Default Unlocked Classes", order = 999)]
public class UCE_Tmpl_UnlockableClasses : ScriptableObject
{
    [Tooltip("[Required] Default classes available to all players")]
    public Player[] defaultClasses;
}
