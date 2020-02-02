using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Adding this script to UI elements makes them draggable
/// </summary>
public class UIDraggable : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private CanvasGroup group;
    private RectTransform rectTransform;
    private float offsetY;
    private float offsetX;

    public bool keepInScreen = true;
    public bool setAsLastSiblingOnDrag = true;
    public bool setAsLastSiblingOnEnable = true;
    public bool changeOpacity = true;
    public float draggedOpacity = 0.7f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (setAsLastSiblingOnDrag)
            transform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offsetX = transform.position.x - Input.mousePosition.x;
        offsetY = transform.position.y - Input.mousePosition.y;

        if (group != null)
            group.alpha = draggedOpacity;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(offsetX + Input.mousePosition.x, offsetY + Input.mousePosition.y);
        if (setAsLastSiblingOnDrag)
            transform.SetAsLastSibling();
        UpdateKeepInScreen();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (group != null)
            group.alpha = 1f;
        UpdateKeepInScreen();
    }

    public void UpdateKeepInScreen()
    {
        if (!keepInScreen)
            return;

        Vector3 oldPosition = transform.position;
        // Keeping ui in screen
        Vector3 screenSize = new Vector3(Screen.width, Screen.height);
        Rect transformRect = rectTransform.rect;
        Vector3 worldSpaceRectMin = rectTransform.TransformPoint(transformRect.min);
        Vector3 worldSpaceRectMax = rectTransform.TransformPoint(transformRect.max);
        Vector3 moveableMax = screenSize - (worldSpaceRectMax - worldSpaceRectMin);

        float x = worldSpaceRectMin.x;
        float y = worldSpaceRectMin.y;

        if (x < 0)
            x = 0;
        else if (x > moveableMax.x)
            x = moveableMax.x;

        if (y < 0)
            y = 0;
        else if (y > moveableMax.y)
            y = moveableMax.y;

        transform.position = new Vector3(x, y, 0) + oldPosition - worldSpaceRectMin;
    }

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (setAsLastSiblingOnEnable)
            transform.SetAsLastSibling();
    }
}