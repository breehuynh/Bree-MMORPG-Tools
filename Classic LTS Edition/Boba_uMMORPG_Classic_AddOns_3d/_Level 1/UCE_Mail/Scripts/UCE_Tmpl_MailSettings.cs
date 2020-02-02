// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// PLAYER MAIL SETTINGS

[CreateAssetMenu(fileName = "UCE Mail Settings", menuName = "UCE Templates/New UCE Mail Settings", order = 999)]
public class UCE_Tmpl_MailSettings : ScriptableObject
{
    [Header("[EXPIRATION]")]
    [Range(1, 999)] public int expiresAmount = 30;

    public DateInterval expiresPart = DateInterval.Days;

    [Header("[SEND]")]
    public bool mailSendFromAnywhere = true;

    [Range(0, 99)] public int mailWaitSeconds = 3;
    public UCE_Cost costPerMail;

    [Header("[RECEIVE]")]
    [Range(1, 999)] public int mailCheckSeconds = 30;

    [Header("[LABELS]")]
    public string labelRecipient;

    public string labelSubject;
    public string labelBody;
    public string labelCost;
}
