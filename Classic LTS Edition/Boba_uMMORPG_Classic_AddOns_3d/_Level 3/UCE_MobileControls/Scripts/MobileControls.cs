// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.EventSystems;

// MobileControls

public static partial class MobileControls
{
    public static Vector2 joyVirtualAxis = Vector2.zero;
    public static bool camDrag;

    // -----------------------------------------------------------------------------------
    // Get1FingerRotation
    // -----------------------------------------------------------------------------------
    public static Vector3 Get1FingerRotation(Transform cam, Vector3 rot, float xMinAngle = 0, float xMaxAngle = 360, float rotationSpeedTouch = 0.2f)
    {
        if ((Input.touchCount == 1 && UCE_Tools.joy_tIdx == -1) || (Input.touchCount == 2 && UCE_Tools.joy_tIdx != -1))
        {
            int cam_tID = (UCE_Tools.joy_tIdx == 0) ? 1 : 0;
            Touch camTouch = Input.GetTouch(cam_tID);

            // Down Event
            if (!camDrag)
                camDrag = (camTouch.phase == TouchPhase.Began && !IsTouchOverUserInterface(camTouch));

            // Drag Event
            if (camDrag && camTouch.phase == TouchPhase.Moved)
            {
                Vector2 delta = Get1FingerPositionChange(camTouch);
                rot.y += delta.y * rotationSpeedTouch;
                rot.x -= delta.x * rotationSpeedTouch;
                rot.x = Mathf.Clamp(rot.x, xMinAngle, xMaxAngle);
                cam.rotation = Quaternion.Euler(rot.x, rot.y, 0);
            }
            // Up Event
            if (camTouch.phase == TouchPhase.Ended || camTouch.phase == TouchPhase.Canceled)
                camDrag = false;
        }
        return rot;
    }

    // -----------------------------------------------------------------------------------
    // SetJoystickAxis
    // -----------------------------------------------------------------------------------
    public static void SetJoystickAxis(Vector2 axis)
    {
        joyVirtualAxis = axis;
    }

    // -----------------------------------------------------------------------------------
    // IsTouchOverUserInterface
    // -----------------------------------------------------------------------------------
    public static bool IsTouchOverUserInterface(Touch touch)
    {
        return (EventSystem.current.IsPointerOverGameObject(touch.fingerId));
    }

    // -----------------------------------------------------------------------------------
    // Get1FingerPositionChange
    // -----------------------------------------------------------------------------------
    public static Vector2 Get1FingerPositionChange(Touch touch)
    {
        return new Vector2(touch.deltaPosition.y, touch.deltaPosition.x);
    }

    // -----------------------------------------------------------------------------------
}
