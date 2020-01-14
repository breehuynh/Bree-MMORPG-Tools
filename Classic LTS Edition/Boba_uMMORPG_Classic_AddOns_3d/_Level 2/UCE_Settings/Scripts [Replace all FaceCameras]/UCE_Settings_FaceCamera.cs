// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// Useful for Text Meshes that should face the camera.
//
// In some cases there seems to be a Unity bug where the text meshes end up in
// weird positions if it's not positioned at (0,0,0). In that case simply put it
// into an empty GameObject and use that empty GameObject for positioning.
using UnityEngine;

public partial class UCE_Settings_FaceCamera : MonoBehaviour
{
    private UCE_UI_SettingsVariables settingsVariables;
    private MeshRenderer mesh;
    private SpriteRenderer sprite;

    // Grab UCE_UI_SettingsVariables and the mesh for use later.
    private void Start()
    {
        settingsVariables = FindObjectOfType<UCE_UI_Settings>().GetComponent<UCE_UI_SettingsVariables>();

        mesh = GetComponent<MeshRenderer>();

        if (mesh != null) sprite = GetComponent<SpriteRenderer>();
    }

    // Check if our overhead mesh is allowed to show.
    private void Update()
    {
        if (!settingsVariables.isShowOverhead) if (mesh != null) mesh.enabled = false; else if (sprite != null) sprite.enabled = false; else return;
        if (settingsVariables.isShowOverhead) if (mesh != null) mesh.enabled = true; else if (sprite != null) sprite.enabled = true; else return;
    }

    // LateUpdate so that all camera updates are finished.
    private void LateUpdate()
    {
        if (settingsVariables.isShowOverhead)
            transform.forward = Camera.main.transform.forward;
    }
}
