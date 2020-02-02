// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// VirtualJoystick

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Set alpha value for unused virtual joystick")]
    [Range(0, 1)] public float backgroundAlphaUnUsed = 0.3f;

    [Range(0, 1)] public float joystickAlphaUnUsed = 0.6f;

    [Header("Set alpha value for in use virtual joystick")]
    [Range(0, 1)] public float backgroundAlphaInUse = 0.75f;

    [Range(0, 1)] public float joystickAlphaInUse = 1f;

    private Image bgImg;
    private Image joystickImg;

    private Vector2 startPos;

    private float movementRange;
    private int joy_fID = -1;
    private bool joyDrag;

    private void Start()
    {
        bgImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();

        bgImg.CrossFadeAlpha(backgroundAlphaUnUsed, 0f, false);
        joystickImg.CrossFadeAlpha(joystickAlphaUnUsed, 0f, false);

        movementRange = (bgImg.rectTransform.sizeDelta.x - joystickImg.rectTransform.sizeDelta.x) * 0.75f;
        startPos = joystickImg.transform.position;
    }

    private void Update()
    {
        if (joyDrag)
        {
            Vector2 pos = Vector2.zero;
            if (joy_fID != -1)
            {
                int joy_tIdx = -1;
                foreach (Touch touch in Input.touches)
                {
                    joy_tIdx++;
                    if (touch.fingerId == joy_fID)
                    {
                        pos = touch.position - startPos;
                        break;
                    }
                }
                UCE_Tools.joy_tIdx = joy_tIdx;
            }
            else if (Input.mousePresent)
            {
                Vector2 mousePos = Input.mousePosition;
                pos = mousePos - startPos;
            }

            Vector2 virtualAxis = (pos * 0.5f) / movementRange;

            if (virtualAxis.magnitude > 1)
                virtualAxis.Normalize();

            MobileControls.SetJoystickAxis(virtualAxis);
            joystickImg.rectTransform.anchoredPosition = virtualAxis * movementRange;
        }
    }

    public void OnPointerDown(PointerEventData ped)
    {
        if (!joyDrag)
        {
            bgImg.CrossFadeAlpha(backgroundAlphaInUse, 0.2f, false);
            joystickImg.CrossFadeAlpha(joystickAlphaInUse, 0.2f, false);

            joyDrag = true; joy_fID = ped.pointerId;//Input.GetTouch(ped.pointerId).fingerId;
        }
    }

    public void OnPointerUp(PointerEventData ped)
    {
        if (ped.pointerId == joy_fID)
        {
            MobileControls.SetJoystickAxis(Vector2.zero);

            bgImg.CrossFadeAlpha(backgroundAlphaUnUsed, 0.2f, false);
            joystickImg.CrossFadeAlpha(joystickAlphaUnUsed, 0.2f, false);
            joystickImg.rectTransform.anchoredPosition = Vector2.zero;

            joyDrag = false; joy_fID = -1; UCE_Tools.joy_tIdx = -1;
        }
    }
}
