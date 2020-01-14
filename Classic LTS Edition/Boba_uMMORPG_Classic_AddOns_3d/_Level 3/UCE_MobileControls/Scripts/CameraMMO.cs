// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================

using UnityEngine;

public partial class CameraMMO : MonoBehaviour
{
    [Tooltip("[Required] 1 = right button by default")]
    public int mouseButton = 1; // right button by default

    public float distance = 20;
    public float minDistance = 3;
    public float maxDistance = 20;

    public float zoomSpeedMouse = 1;
    public float zoomSpeedTouch = 0.2f;

    [Tooltip("[Optional] Manual rotation speed (via mouse)")]
    public float rotationSpeed = 2;

#if _iMMOMOBILECONTROLS
    [Tooltip("[Optional] Manual rotation speed (via touch input)")]
    public float rotationSpeedTouch = 0.2f;
#endif

    [Tooltip("[Optional] Automatic follow speed (0 to disable lerp)")]
    public float autoFollowSpeed = 0;

    public float xMinAngle = -40;
    public float xMaxAngle = 80;

    // the target position can be adjusted by an offset in order to foucs on a
    // target's head for example
    public Vector3 offset = Vector3.zero;

    // view blocking
    // note: only works against objects with colliders.
    //       uMMORPG has almost none by default for performance reasons
    // note: remember to disable the entity layers so the camera doesn't zoom in
    //       all the way when standing inside another entity
    public LayerMask viewBlockingLayers;

    // store rotation so that unity never modifies it, otherwise unity will put
    // it back to 360 as soon as it's <0, which makes a negative min angle
    // impossible
    private Transform _target;

    private Vector3 rotation, rotOld;   // (Mobile Controls)
    private bool camDrag;				// (Mobile Controls)
    private bool rotationInitialized;

    // -----------------------------------------------------------------------------------
    // target
    // -----------------------------------------------------------------------------------
    public Transform target
    {
        set
        {
            if (!_target)
            {
                Vector3 targetPos = value.position + offset;
                transform.position = targetPos - (transform.rotation * Vector3.forward * distance);
            }
            _target = value;
        }
        get { return _target; }
    }

    // -----------------------------------------------------------------------------------
    // LateUpdate
    // -----------------------------------------------------------------------------------
    private void LateUpdate()
    {
        if (!target) return;

        Vector3 targetPos = target.position + offset;

        // rotation and zoom should only happen if not in a UI right now
        if (!Utils.IsCursorOverUserInterface())
        {
            // right mouse rotation if we have a mouse
            if (Input.mousePresent)
            {
                //Down Event (MobileControls)
                if (!camDrag)                                       //Start (MobileControls)
                    camDrag = (Input.GetMouseButtonDown(mouseButton) && !Utils.IsCursorOverUserInterface());

                //Drag Event (Mobile Controls)
                if (Input.GetMouseButton(mouseButton) && camDrag)
                {
                    // initialize the base rotation if not initialized yet.
                    // (only after first mouse click and not in Awake because
                    //  we might rotate the camera inbetween, e.g. during
                    //  character selection. this would cause a sudden jump to
                    //  the original rotation from Awake otherwise.)
                    if (!rotationInitialized)
                    {
                        rotation = transform.eulerAngles;
                        rotationInitialized = true;
                    }

                    // note: mouse x is for y rotation and vice versa
                    rotation.y += Input.GetAxis("Mouse X") * rotationSpeed;
                    rotation.x -= Input.GetAxis("Mouse Y") * rotationSpeed;
                    rotation.x = Mathf.Clamp(rotation.x, xMinAngle, xMaxAngle);
                    transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
                }

                //Up Event (Mobile Controls)
                if (Input.GetMouseButtonUp(mouseButton))    // (MobileControls)
                    camDrag = false;						// (MobileControls)
            }
            else
            {
#if _iMMOMOBILECONTROLS
                rotation = MobileControls.Get1FingerRotation(transform, rotation, xMinAngle, xMaxAngle, rotationSpeedTouch);
#else
                // forced 45 degree if there is no mouse to rotate (for mobile)
                transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
#endif
            }

            if (rotation != rotOld)
                UCE_Tools.pointerDown = false;
            rotOld = rotation;

            // zoom
            float speed = Input.mousePresent ? zoomSpeedMouse : zoomSpeedTouch;
            float step = Utils.GetZoomUniversal() * speed;
            distance = Mathf.Clamp(distance - step, minDistance, maxDistance);
        }

        // -- target auto follow
        Vector3 newPos = targetPos - (transform.rotation * Vector3.forward * distance);

        if (autoFollowSpeed > 0)
            transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * autoFollowSpeed);
        else
            transform.position = newPos;

        // avoid view blocking (disabled, see comment at the top)
        if (Physics.Linecast(targetPos, transform.position, out RaycastHit hit, viewBlockingLayers))
        {
            // calculate a better distance (with some space between it)
            float d = Vector3.Distance(targetPos, hit.point) - 0.1f;

            // set the final cam position
            transform.position = targetPos - (transform.rotation * Vector3.forward * d);
        }
    }
}
