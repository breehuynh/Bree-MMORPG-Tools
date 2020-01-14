// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// PLACEABLE OBJECT

public partial class UCE_PlaceableObject : NetworkBehaviour
{
    [Tooltip("[Optional] Does destruction/death remove this object from the database?")]
    public bool destroyable;

    [Tooltip("[Optional] Can the object get picked-up again by its owner?")]
    public bool pickupable;

    [Tooltip("[Optional, Entity only] Each entry represents a upgrade that will increase this objects level")]
    public UCE_PlaceableObjectUpgradeCost[] upgradeable;

    [HideInInspector, SyncVar] public bool permanent;
    [HideInInspector, SyncVar] public string ownerCharacter;
    [HideInInspector, SyncVar] public string ownerGuild;
    [HideInInspector, SyncVar] public string itemName;

    protected UCE_UI_PlaceableObject _UCE_UI_PlaceableObject;
    protected int _id;

    // -----------------------------------------------------------------------------------
    // id
    // -----------------------------------------------------------------------------------
    public int id
    {
        get
        {
            if (_id == 0)
            {
                string sId = ownerCharacter + ownerGuild + itemName + transform.position.x.ToString() + transform.position.y.ToString() + transform.position.z.ToString();
                _id = sId.GetDeterministicHashCode();
            }
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    // -----------------------------------------------------------------------------------
    // Update
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        // -- check for click
        if (UCE_Tools.UCE_SelectionHandling(this.gameObject))
        {
            UCE_PlaceableObject po = this.gameObject.GetComponent<UCE_PlaceableObject>();

            if (po != null &&
                po.ownerCharacter == player.name
                )
            {
                player.UCE_myPlaceableObject = po;

                if (!_UCE_UI_PlaceableObject)
                    _UCE_UI_PlaceableObject = FindObjectOfType<UCE_UI_PlaceableObject>();

                _UCE_UI_PlaceableObject.Show();
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // OnDestroy
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    private void OnDestroy()
    {
        if ((!destroyable && !pickupable) || (this.GetComponent<Entity>() != null && this.GetComponent<Entity>().isAlive))
            return;

        // -- gather object level if it is an entity

        int level = 0;

        Entity m = this.GetComponent<Entity>();

        if (m)
            level = m.level;

        int id = 0;

        UCE_PlaceableObject po = this.gameObject.GetComponent<UCE_PlaceableObject>();

        if (po)
            id = po.id;

        // -- delete from database

        Database.singleton.UCE_DeletePlaceableObject(ownerCharacter, ownerGuild, level, itemName, id);
    }

    // -----------------------------------------------------------------------------------
    // getUpgradeCost
    // -----------------------------------------------------------------------------------
    public UCE_PlaceableObjectUpgradeCost getUpgradeCost(int level)
    {
        if (upgradeable.Length > level - 1)
        {
            return upgradeable[level - 1];
        }

        return null;
    }

    // -----------------------------------------------------------------------------------
}
