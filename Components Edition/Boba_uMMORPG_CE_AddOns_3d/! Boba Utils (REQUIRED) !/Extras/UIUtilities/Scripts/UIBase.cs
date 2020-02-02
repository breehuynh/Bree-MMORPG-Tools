using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    public GameObject root;
    public bool hideOnAwake;
    public UnityEvent eventShow;
    public UnityEvent eventHide;
    private bool isAwaken;
    private BaseUIExtension uiExtension;

    protected virtual void Awake()
    {
        if (isAwaken)
            return;
        isAwaken = true;
        ValidateRoot();
        if (hideOnAwake)
            Hide();
        else
            Show();
    }

    public void ValidateRoot()
    {
        uiExtension = GetComponent<BaseUIExtension>();
        if (uiExtension != null)
            uiExtension.ui = this;
        if (root == null)
            root = gameObject;
    }

    public virtual void Show()
    {
        isAwaken = true;
        ValidateRoot();
        if (uiExtension != null)
            uiExtension.Show();
        else
            root.SetActive(true);
        eventShow.Invoke();
    }

    public virtual void Hide()
    {
        isAwaken = true;
        ValidateRoot();
        if (uiExtension != null)
            uiExtension.Hide();
        else
            root.SetActive(false);
        eventHide.Invoke();
    }

    public virtual bool IsVisible()
    {
        ValidateRoot();
        if (uiExtension != null)
            return uiExtension.IsVisible();
        else
            return root.activeSelf;
    }

    public void SetEnableGraphics(bool isEnable)
    {
        Graphic[] graphics = GetComponentsInChildren<Graphic>();
        foreach (Graphic graphic in graphics)
        {
            graphic.enabled = isEnable;
        }
    }

    public void SetGraphicsAlpha(float alpha)
    {
        Graphic[] graphics = GetComponentsInChildren<Graphic>();
        foreach (Graphic graphic in graphics)
        {
            Color color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
            graphic.color = color;
        }
    }
}