using UnityEngine;
using UnityEngine.EventSystems;

public class MobilePinchArea : MobileInputComponent, IPointerDownHandler, IPointerUpHandler
{
    public string axisName = "Mouse ScrollWheel";
    public float sensitivity = 1f;

    private bool isDragging = false;
    private int touchId1 = -1;
    private int touchId2 = -1;

    public void OnPointerDown(int touchId)
    {
        if (Application.isMobilePlatform && touchId < 0)
            return;
        if (touchId1 == -1)
        {
            touchId1 = touchId;
            AddTouchId(touchId);
        }
        else if (touchId2 == -1)
        {
            touchId2 = touchId;
            AddTouchId(touchId);
        }
        if (touchId1 != -1 && touchId2 != -1)
            isDragging = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDragging)
            return;

        int touchId = eventData.pointerId;
        if (ContainsTouchId(touchId))
            return;

        OnPointerDown(touchId);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        int touchId = eventData.pointerId;
        if (touchId == touchId1)
        {
            RemoveTouchId(touchId);
            isDragging = false;
            touchId1 = -1;
        }
        if (touchId == touchId2)
        {
            RemoveTouchId(touchId);
            isDragging = false;
            touchId2 = -1;
        }
    }

    private void Update()
    {
        if (!isDragging)
        {
            InputManager.SetAxis(axisName, 0f);
            return;
        }

        // Store both touches.
        Touch touch1 = Input.touches[touchId1];
        Touch touch2 = Input.touches[touchId2];

        // Find the position in the previous frame of each touch.
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
        Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
        float touchDeltaMag = (touch1.position - touch2.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        InputManager.SetAxis(axisName, deltaMagnitudeDiff * sensitivity);
    }
}