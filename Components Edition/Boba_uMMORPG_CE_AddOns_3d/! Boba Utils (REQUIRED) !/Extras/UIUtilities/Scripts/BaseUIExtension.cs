using UnityEngine;

public abstract class BaseUIExtension : MonoBehaviour
{
    [System.NonSerialized]
    public UIBase ui;

    public abstract void Show();

    public abstract void Hide();

    public abstract bool IsVisible();
}