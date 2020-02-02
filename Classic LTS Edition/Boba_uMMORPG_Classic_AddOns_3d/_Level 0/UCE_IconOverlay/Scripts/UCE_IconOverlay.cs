// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// =======================================================================================

using UnityEngine;
using System.Collections;

// =======================================================================================
// UCE UCE_IconOverlay
// =======================================================================================
public class UCE_IconOverlay : MonoBehaviour
{
	public GameObject childObject;
	public float hideAfter = 0.5f;
	
	public Sprite[] icons;
	
	// -----------------------------------------------------------------------------------
	// Awake
	// -----------------------------------------------------------------------------------
	private void Awake()
    {
        if (childObject.GetComponent<SpriteRenderer>().sprite == null)
	        childObject.SetActive(false);
        else
            childObject.SetActive(true);
    }
	
	// -----------------------------------------------------------------------------------
	// Show
	// -----------------------------------------------------------------------------------
	public void Show(int index=0)
	{
		if (childObject == null || childObject.activeInHierarchy || index < 0) return;

		childObject.GetComponent<SpriteRenderer>().sprite = icons[index];
		childObject.SetActive(true);
		
		Invoke("Hide", hideAfter);
	}

    // -----------------------------------------------------------------------------------
    // ShowPermanent
    // -----------------------------------------------------------------------------------
    public void ShowPermanent(int index = 0)
    {
        if (childObject == null || index < 0) return;

        if (childObject.activeInHierarchy)
            Hide();

        childObject.GetComponent<SpriteRenderer>().sprite = icons[index];
        childObject.SetActive(true);

    }

    // -----------------------------------------------------------------------------------
    // Hide
    // -----------------------------------------------------------------------------------
    public void Hide()
	{
		CancelInvoke();
		
		if (childObject == null) return;
		
		childObject.SetActive(false);
	}
	
	// -----------------------------------------------------------------------------------	
}