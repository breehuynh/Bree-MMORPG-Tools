using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// SHARED INSTANCE UI
// ===================================================================================
public partial class UCE_UI_LimitedZones : MonoBehaviour
{
	
    public GameObject panel;
    public UCE_Slot_LimitedZones slotPrefab;
    public Transform content;
	
	public UCE_LimitedZonesManager sharedInstanceManager;
	
	protected int instanceCategory;
	
    // -----------------------------------------------------------------------------------
    // Show
    // @Client
    // -----------------------------------------------------------------------------------
    public void Show(int _instanceCategory)
    {
    
        Player player = Player.localPlayer;
        if (!player) return;
        
        instanceCategory = _instanceCategory;
        
        if (!sharedInstanceManager)
			sharedInstanceManager = FindObjectOfType<UCE_LimitedZonesManager>();
        
        List<UCE_LimitedZonesEntry> instancesAvailable = sharedInstanceManager.getAvailableSharedInstances(player, instanceCategory);
        
        if (instancesAvailable.Count == 0)
        	return;
        
        UIUtils.BalancePrefabs(slotPrefab.gameObject, instancesAvailable.Count, content);
		
        for (int i = 0; i < instancesAvailable.Count; ++i)
        {
            
            UCE_Slot_LimitedZones slot = content.GetChild(i).GetComponent<UCE_Slot_LimitedZones>();
            
            int index = i;
            
            slot.image.sprite				= instancesAvailable[i].image;
            slot.titleText.text 			= instancesAvailable[i].title;
            slot.descriptionText.text 		= instancesAvailable[i].description;
			slot.groupText.text				= instancesAvailable[i].getGroupType();

            if (instancesAvailable[i].entranceCost.honorCurrency != null)
            { 
            slot.costImage.sprite			= instancesAvailable[i].entranceCost.honorCurrency.image;
            slot.costText.text				= instancesAvailable[i].entranceCost.amount.ToString();
            }

            slot.groupCountText.text 		= instancesAvailable[i].getGroupCountText();
			slot.countText.text				= instancesAvailable[i].getPlayerCountText();
			
			slot.actionButton.interactable 	= instancesAvailable[i].canPlayerEnter(player);
			
            slot.actionButton.onClick.SetListener(() =>
            {
                instancesAvailable[index].teleportPlayerToInstance(player, instanceCategory, index);
                panel.SetActive(false);
            });
            
            
        }

        panel.SetActive(true);
    }
	
    // -----------------------------------------------------------------------------------
    // Hide
    // @Client
    // -----------------------------------------------------------------------------------
    public void Hide()
    {
        panel.SetActive(false);
    }
    
   	// -----------------------------------------------------------------------------------
    // Update
    // @Client
    // -----------------------------------------------------------------------------------
    void Update()
    {
    	
    	Player player = Player.localPlayer;
    	
        if (!player)
        	Hide();
        
        if (panel.activeSelf)
        	Show(instanceCategory);
        
    }

    // -----------------------------------------------------------------------------------
}

// =======================================================================================