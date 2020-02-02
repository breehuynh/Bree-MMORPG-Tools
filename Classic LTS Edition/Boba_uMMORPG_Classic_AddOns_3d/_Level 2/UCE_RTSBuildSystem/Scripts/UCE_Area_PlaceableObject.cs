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
// PLACEABLE OBJECT AREA
// ===================================================================================
[RequireComponent(typeof(SphereCollider))]
public class UCE_Area_PlaceableObject : NetworkBehaviour
{
    [Tooltip("One click deactivation")]
    public bool isActive = true;

    [Tooltip("Only objects with the same ID can be placed in this area")]
    public int areaId;

    // -------------------------------------------------------------------------------
    // OnTriggerEnter
    // -------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider co)
    {
        string areaOwnerName = "";
        string areaGuildName = "";

        Player e = co.GetComponentInParent<Player>();

        if (e != null && isActive)
        {
            UCE_PlaceableObject o = GetComponent<UCE_PlaceableObject>();

            if (o && o.ownerCharacter != "")
                areaOwnerName = o.ownerCharacter;

            if (o && o.ownerGuild != "")
                areaGuildName = o.ownerGuild;

            e.UCE_SetPlaceableObjectArea(areaId, areaOwnerName, areaGuildName, true);
        }
    }

    // -------------------------------------------------------------------------------
    // OnTriggerExit
    // -------------------------------------------------------------------------------
    private void OnTriggerExit(Collider co)
    {
        string areaOwnerName = "";
        string areaGuildName = "";

        Player e = co.GetComponentInParent<Player>();

        if (e != null && isActive)
            e.UCE_SetPlaceableObjectArea(areaId, areaOwnerName, areaGuildName, false);
    }
}
