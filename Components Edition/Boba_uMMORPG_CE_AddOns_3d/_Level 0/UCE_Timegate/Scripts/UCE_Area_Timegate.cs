// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// ===================================================================================
// UCE TIMEGATE
// ===================================================================================
[RequireComponent(typeof(BoxCollider))]
public class UCE_Area_Timegate : NetworkBehaviour
{
    [Header("[-=-=-=- UCE TIMEGATE -=-=-=-]")]
    [Tooltip("One click deactivation")]
    public bool isActive = true;

    [Tooltip("[Required] Any on scene Transform or GameObject OR off scene coordinates (requires UCE Network Zones AddOn)")]
    public UCE_TeleportationTarget teleportationTarget;

    [Tooltip("Maximum number of visits while the gate is open")]
    public int maxVisits = 10;

    [Tooltip("Minimum number of hours that must pass between visits while open")]
    public int hoursBetweenVisits = 10;

    [Tooltip("The day this timegate will open (set 0 to disable)"), Range(0, 31)]
    public int dayStart = 1;

    [Tooltip("The day this timegate will close (set 0 to disable)"), Range(0, 31)]
    public int dayEnd = 1;

    [Tooltip("The month this timegate is open (set 0 to disable)"), Range(0, 12)]
    public int activeMonth = 1;

    protected UCE_UI_Timegate _UCE_UI_Timegate;

    // -------------------------------------------------------------------------------
    // OnTriggerEnter
    // -------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();
        if (player && isActive && teleportationTarget.Valid)
        {
            player.UCE_myTimegate = this;

            if (!_UCE_UI_Timegate)
                _UCE_UI_Timegate = FindObjectOfType<UCE_UI_Timegate>();

            _UCE_UI_Timegate.Show();
        }
    }

    // -------------------------------------------------------------------------------
    // OnTriggerExit
    // -------------------------------------------------------------------------------
    private void OnTriggerExit(Collider co)
    {
        Player player = co.GetComponentInParent<Player>();
        if (player && isActive && teleportationTarget.Valid)
        {
            player.UCE_myTimegate = null;
            if (!_UCE_UI_Timegate)
                _UCE_UI_Timegate = FindObjectOfType<UCE_UI_Timegate>();

            _UCE_UI_Timegate.Hide();
        }
    }

    // -------------------------------------------------------------------------------
}
