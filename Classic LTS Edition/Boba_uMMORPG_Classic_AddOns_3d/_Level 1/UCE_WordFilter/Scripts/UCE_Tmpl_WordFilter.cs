// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// DATABASE CLEANER

[CreateAssetMenu(fileName = "UCE WordFilter", menuName = "UCE Templates/New UCE WordFilter", order = 999)]
public class UCE_Tmpl_WordFilter : ScriptableObject
{
    [Tooltip("[Required] Enter all bad words here. If a chatext or player name contains one of them, it will be denied.")]
    public string[] badwords;
}
