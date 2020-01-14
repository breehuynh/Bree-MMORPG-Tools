// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCE_UI_UIVisibility : MonoBehaviour
{
    [Header("=-=-= UCE UI Visibility =-=-=")]
    public KeyCode hotkey = KeyCode.Z;

    public KeyCode modifierKey = KeyCode.LeftAlt;
    public GameObject[] parentObjects;
    private List<GameObject> hiddenPanels = new List<GameObject>();
    private bool modifierPressed = false;
    private float updateTime = 0;

    private void Update()
    {
        // Check if the modifier key was pressed and the hotkey.
        modifierPressed = Input.GetKey(modifierKey) ? true : false;
        if (modifierPressed && Input.GetKey(hotkey) && updateTime <= Time.time)
        {
            updateTime = Time.time + 0.5f;  // Create a wait time before allowing this action again.
                                            // If we don't have hidden parent objects then hide the user interface.
            if (hiddenPanels.Count == 0)
            {
                for (int i = 0; i < parentObjects.Length; i++)
                    if (parentObjects[i].activeSelf)
                    {
                        hiddenPanels.Add(parentObjects[i]);
                        parentObjects[i].SetActive(false);
                    }
            }
            // If we do have hidden parent objects then show them.
            else if (hiddenPanels.Count > 0)
            {
                for (int x = 0; x < hiddenPanels.Count; x++)
                    hiddenPanels[x].SetActive(true);

                hiddenPanels.Clear();
            }
        }
    }
}
