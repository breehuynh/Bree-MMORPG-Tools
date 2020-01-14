using UnityEngine;
using UnityEngine.EventSystems;

public class MobileSwipeArea : MobileInputComponent, IPointerDownHandler, IPointerUpHandler
{
    public bool useAxisX = true;
    public bool useAxisY = true;
    public string axisXName = "Horizontal";
    public string axisYName = "Vertical";
    public float xSensitivity = 1f;
    public float ySensitivity = 1f;

    private bool isDragging = false;
    private Vector2 previousTouchPosition;
    private int touchId = -1;

    public void OnPointerDown(int touchId)
    {
        if (Application.isMobilePlatform && touchId < 0)
            return;
        AddTouchId(touchId);
        isDragging = true;
        this.touchId = touchId;
        previousTouchPosition = GetPointerPosition(touchId);
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

        if (eventData.pointerId != touchId)
            return;

        RemoveTouchId(touchId);

        isDragging = false;
        touchId = -1;
    }

    private void Update()
    {
        if (!isDragging)
        {
            UpdateVirtualAxes(Vector3.zero);
            return;
        }

        Vector2 currentPosition = GetPointerPosition(touchId);

        Vector2 pointerDelta = currentPosition - previousTouchPosition;
        // Set previous touch position to use next frame
        previousTouchPosition = currentPosition;
        // Update virtual axes
        UpdateVirtualAxes(new Vector2(pointerDelta.x * xSensitivity, pointerDelta.y * ySensitivity));
    }

    public void UpdateVirtualAxes(Vector2 value)
    {
        if (useAxisX)
            InputManager.SetAxis(axisXName, value.x);

        if (useAxisY)
            InputManager.SetAxis(axisYName, value.y);
    }
}