// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// ===================================================================================
// INVISIBLE HINT UI
// ===================================================================================
public partial class UCE_UI_InvisibleHint : MonoBehaviour
{
    public GameObject panel;
    public Transform content;
    public ScrollRect scrollRect;
    public GameObject textPrefab;

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show(string textToDisplay)
    {
        var player = Player.localPlayer;
        if (!player) return;

        for (int i = 0; i < content.childCount; ++i)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        if (textToDisplay != "")
        {
            AddMessage(textToDisplay, Color.white);
        }

        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // Hide
    // -----------------------------------------------------------------------------------
    public void Hide()
    {
        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
    // AutoScroll
    // -----------------------------------------------------------------------------------
    private void AutoScroll()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    // -----------------------------------------------------------------------------------
    // AddMessage
    // -----------------------------------------------------------------------------------
    private void AddMessage(string msg, Color color)
    {
        var go = Instantiate(textPrefab);
        go.transform.SetParent(content.transform, false);
        go.GetComponent<Text>().text = msg;
        go.GetComponent<Text>().color = color;
        AutoScroll();
    }
}
