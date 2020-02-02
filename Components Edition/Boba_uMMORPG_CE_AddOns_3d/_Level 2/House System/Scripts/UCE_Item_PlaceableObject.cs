// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// PLACEABLE OBJECT - ITEM

[CreateAssetMenu(menuName = "UCE Item/UCE Item Structure", order = 998)]
public class UCE_Item_PlaceableObject : UsableItem
{
    [Header("-=-=-=- UCE BUILD SYSTEM - STRUCTURE -=-=-=-")]
    [Tooltip("[Required] The structure(s) to spawn in front of the player when used")]
    public GameObject placementObject;

    public GameObject gridObject;

    [Tooltip("[Optional] GameObject used as a preview before building the structure")]
    public GameObject previewObject;

    [Tooltip("Are the structures permanently saved in the server database?")]
    public bool isPermanent;

    [Tooltip("The structures can only be spawned while the player is inside an area of ID (not 0)")]
    public int placementAreaId;

    [Tooltip("The structure can NOT be spawned while the player is inside an area of ID (not 0)")]
    public int restrictedAreaId;

    [Tooltip("Structure is only placeable while in an area bound to the player?")]
    public bool onlyPersonalArea;

    [Tooltip("Structure is only placeable while in an area bound to the own guild?")]
    public bool onlyGuildArea;

    [Tooltip("Select a Layer to prevent structures being spawned on that layer")]
    public LayerMask doNotSpawnAt;

#if _iMMOPVP

    [Tooltip("Structures only placeable while in an UCE PVPZone ?")]
    public bool onlyPVPZone;

#endif

    [Tooltip("Minimum required guild rank (only if guild rank required is checked)")]
    public bool guildRankRequired;

    public GuildRank guildRank;

    [Tooltip("Text that is sent to the chat when the object cannot be placed")]
    public string cannotBePlaced = "You cannot place that there!";

    [Tooltip("How long it takes the player to place the structure(s)")]
    public float placementTime;

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    // -----------------------------------------------------------------------------------
    // Use
    // @Server
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory.slots[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            // always call base function too
            base.Use(player, inventoryIndex);
            player.PrepareSpawnPlaceableObject(this);
        }
    }

    // -----------------------------------------------------------------------------------
    // ToolTip
    // @Client
    // -----------------------------------------------------------------------------------
    /*public override string ToolTip() {
        StringBuilder tip = new StringBuilder(base.ToolTip());
        //tip.Replace("{PLACEABLEOBJECT}", placementObject.name);
        return tip.ToString();
    }*/

    // -----------------------------------------------------------------------------------
}
