using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MobileSwipeArea))]
[RequireComponent(typeof(MobilePinchArea))]
public class MobileSwipeAndPinchArea : MobileInputComponent, IPointerDownHandler, IPointerUpHandler
{
    private int touchId1 = -1;
    private int touchId2 = -1;

    private MobileSwipeArea cacheMobileSwipeArea;

    public MobileSwipeArea CacheMobileSwipeArea
    {
        get
        {
            if (cacheMobileSwipeArea == null)
                cacheMobileSwipeArea = GetComponent<MobileSwipeArea>();
            return cacheMobileSwipeArea;
        }
    }

    private MobilePinchArea cacheMobilePinchArea;

    public MobilePinchArea CacheMobilePinchArea
    {
        get
        {
            if (cacheMobilePinchArea == null)
                cacheMobilePinchArea = GetComponent<MobilePinchArea>();
            return cacheMobilePinchArea;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        int touchId = eventData.pointerId;
        if (ContainsTouchId(touchId))
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
        {
            CacheMobileSwipeArea.enabled = false;
            CacheMobilePinchArea.enabled = true;
            CacheMobilePinchArea.OnPointerDown(touchId1);
            CacheMobilePinchArea.OnPointerDown(touchId2);
        }
        else if (touchId1 != -1 || touchId2 != -1)
        {
            CacheMobileSwipeArea.enabled = true;
            CacheMobilePinchArea.enabled = false;
            CacheMobileSwipeArea.OnPointerDown(touchId1);
            CacheMobileSwipeArea.OnPointerDown(touchId2);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        int touchId = eventData.pointerId;
        if (touchId == touchId1)
        {
            RemoveTouchId(touchId);
            touchId1 = -1;
            CacheMobileSwipeArea.OnPointerUp(eventData);
            CacheMobilePinchArea.OnPointerUp(eventData);
        }
        if (touchId == touchId2)
        {
            RemoveTouchId(touchId);
            touchId2 = -1;
            CacheMobileSwipeArea.OnPointerUp(eventData);
            CacheMobilePinchArea.OnPointerUp(eventData);
        }
    }

    private void Update()
    {
        if (touchId1 == -1 && touchId2 == -1)
        {
            CacheMobileSwipeArea.enabled = false;
            CacheMobilePinchArea.enabled = false;
        }
    }
}