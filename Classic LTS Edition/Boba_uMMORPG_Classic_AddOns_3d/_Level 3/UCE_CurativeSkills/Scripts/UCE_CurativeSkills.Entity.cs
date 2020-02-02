// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/iMMOban
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

// =======================================================================================
// ENTITY
// =======================================================================================
public partial class Entity
{

	[Header("Healing Popup")]
    public GameObject healingPopupPrefab;
    
    // -----------------------------------------------------------------------------------
	// ShowHealingPopup
	// -----------------------------------------------------------------------------------

    [Client]
    void ShowHealingPopup(int amount)
    {
        // spawn the damage popup (if any) and set the text
        if (healingPopupPrefab != null)
        {
        
            // showing it above their head looks best, and we don't have to use
            // a custom shader to draw world space UI in front of the entity
            Bounds bounds = collider.bounds;
            Vector3 position = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

            GameObject popup = Instantiate(healingPopupPrefab, position, Quaternion.identity);
            
            popup.GetComponentInChildren<TextMeshPro>().text = amount.ToString();
            
            
        }
    }
	
	// -----------------------------------------------------------------------------------
	// RpcOnHealingReceived
	// -----------------------------------------------------------------------------------

    [ClientRpc]
    public void RpcOnHealingReceived(int amount)
    {
        // show popup above receiver's head in all observers via ClientRpc
        ShowHealingPopup(amount);

        // addon system hooks
        //this.InvokeInstanceDevExtMethods("OnHealingReceived", amount);
        Utils.InvokeMany(typeof(Entity), this, "OnHealingReceived_", amount);
    }
	
    // -----------------------------------------------------------------------------------
}

// =======================================================================================