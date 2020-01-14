using UnityEngine;

public static class TransformHelpers
{
    public static void RemoveAllChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            Transform child = transform.GetChild(i);
            Object.Destroy(child.gameObject);
        }
    }

    public static void EnableComponentsInChildren<T>(this Transform transform, bool enabled) where T : Behaviour
    {
        var components = transform.GetComponentsInChildren<T>();
        for (int i = 0; i < components.Length; ++i)
        {
            var component = components[i];
            component.enabled = enabled;
        }
    }
}