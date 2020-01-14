// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UCE UI INFO BOX

public partial class UCE_UI_InfoBox : MonoBehaviour
{
    public KeyCode hotKey = KeyCode.B;
    public GameObject panel;
    public Transform content;
    public ScrollRect scrollRect;
    public GameObject textPrefab;
    [Range(0, 300)] public int keepHistory = 100;
    [Tooltip("Time (in seconds) the box will be visible when adding a message, set to 0 to make it visible permanently")]
    [Range(0, 30)]
    public float displayTime = 3f;
    public Color[] textColors;
    public string messagePrefix;

    // -----------------------------------------------------------------------------------
    // Update
    // @Client
    // -----------------------------------------------------------------------------------
    void Update()
    {
        Player player = Player.localPlayer;
        if (!player && displayTime > 0) panel.SetActive(false);

        if (!player)
            return;
        else if (displayTime == 0)
            panel.SetActive(true);

        if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
            panel.SetActive(!panel.activeSelf);
    }

    // -----------------------------------------------------------------------------------
    // AutoScroll
    // @Client
    // -----------------------------------------------------------------------------------
    void AutoScroll()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    // -----------------------------------------------------------------------------------
    // AddMessage
    // @Client
    // -----------------------------------------------------------------------------------
    public void AddMsg(InfoText info, bool show = true)
    {
        if (content.childCount >= keepHistory)
            Destroy(content.GetChild(0).gameObject);

        GameObject go = Instantiate(textPrefab);
        go.transform.SetParent(content.transform, false);
        go.GetComponent<Text>().text = messagePrefix + info.content;
        go.GetComponent<Text>().color = textColors[info.color];

        AutoScroll();

        if (show)
        {
            if (displayTime > 0)
            {
                Invoke("FadeHide", displayTime);
                LeanTween.alpha(panel.GetComponent<RectTransform>(), 1.0f, 0f).setEase(LeanTweenType.easeInCirc);
            }

            panel.SetActive(true);
        }
    }

    // -----------------------------------------------------------------------------------
    // FadeHide
    // @Client
    // -----------------------------------------------------------------------------------
    private void FadeHide()
    {
        LeanTween.alpha(panel.GetComponent<RectTransform>(), 0f, displayTime / 4).setEase(LeanTweenType.easeInCirc);
        Invoke("Hide", displayTime / 4 + 0.25f);
    }

    // -----------------------------------------------------------------------------------
    // Hide
    // @Client
    // -----------------------------------------------------------------------------------
    public void Hide()
    {
        panel.SetActive(false);
        LeanTween.alpha(panel.GetComponent<RectTransform>(), 1.0f, 0f).setEase(LeanTweenType.easeInCirc);
    }
}
