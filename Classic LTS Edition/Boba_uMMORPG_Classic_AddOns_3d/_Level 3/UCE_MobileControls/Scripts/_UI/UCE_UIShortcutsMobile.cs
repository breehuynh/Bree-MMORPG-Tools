// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UIShortcuts

public partial class UIShortcuts : MonoBehaviour
{
    [Header("Link the new show/close Buttons here:")]
    public GameObject showButtonObject;

    public Button closeButton;
    private Button showButton;
    private bool panelVisible;

    private void Start()
    {
        if (showButtonObject)
        {
            showButtonObject.SetActive(false);
            showButton = showButtonObject.GetComponent<Button>();
        }
    }

    private void Update_Mobile()
    {
        Player player = Player.localPlayer;

        if (!player || !showButtonObject || !closeButton)
            return;

        panel.SetActive(panelVisible);
        showButtonObject.SetActive(!panelVisible);

        // Stue Show Button
        showButton.onClick.SetListener(() =>
        {
            panel.SetActive(true);
            panelVisible = true;
        });

        // Stue Close Button
        closeButton.onClick.SetListener(() =>
        {
            panel.SetActive(false);
            panelVisible = false;
        });
    }
}
