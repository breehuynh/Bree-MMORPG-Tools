using System.Collections.Generic;
using UnityEngine;

public class MobileInputComponent : MonoBehaviour
{
    private static readonly List<int> touchIds = new List<int>();

    public static void AddTouchId(int id)
    {
        if (id < 0 || touchIds.Contains(id))
            return;
        touchIds.Add(id);
    }

    public static bool RemoveTouchId(int id)
    {
        return touchIds.Remove(id);
    }

    public static bool ContainsTouchId(int id)
    {
        return touchIds.Contains(id);
    }

    public static void ClearTouchId()
    {
        touchIds.Clear();
    }

    public static Vector2 GetPointerPosition(int id)
    {
        return Application.isMobilePlatform ? Input.touches[id].position : new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
}