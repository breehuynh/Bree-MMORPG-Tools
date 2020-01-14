using UnityEngine;

[ExecuteInEditMode]
public class FollowCameraControls : MonoBehaviour
{
    public Camera targetCamera;

    [Header("Controls")]
    public bool updateRotation = true;

    public bool updateZoom = true;

    [Header("General Settings")]
    public Transform target;

    public Vector3 targetOffset;

    [Header("Follow")]
    public float damping = 10.0f;

    public bool dontSmoothFollow;

    [Header("Look at")]
    public float lookAtDamping = 2.0f;

    public bool dontSmoothLookAt;

    [Header("X Rotation")]
    public bool limitXRotation;

    public float minXRotation = 0;
    public float maxXRotation = 0;

    [Header("Y Rotation")]
    public bool limitYRotation;

    public float minYRotation = 0;
    public float maxYRotation = 0;

    [Header("General Rotation Settings")]
    public float startXRotation;

    public float startYRotation;
    public float rotationSpeed = 5;

    [Header("Zoom")]
    public bool limitZoomDistance;

    public float minZoomDistance;
    public float maxZoomDistance;

    [Header("General Zoom Settings")]
    public float startZoomDistance;

    public float zoomSpeed = 5;

    private FollowCamera targetFollowCamera;

    public FollowCamera TargetFollowCamera
    {
        get
        {
            if (targetFollowCamera == null)
                targetFollowCamera = targetCamera.gameObject.GetComponent<FollowCamera>();
            if (targetFollowCamera == null)
                targetFollowCamera = targetCamera.gameObject.AddComponent<FollowCamera>();
            return targetFollowCamera;
        }
    }

    // Use this for initialization
    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        TargetFollowCamera.xRotation = startXRotation;
        TargetFollowCamera.yRotation = startYRotation;
        TargetFollowCamera.zoomDistance = startZoomDistance;
    }

    // Update is called once per frame
    private void Update()
    {
        TargetFollowCamera.target = target;
        TargetFollowCamera.targetOffset = targetOffset;
        TargetFollowCamera.damping = damping;
        TargetFollowCamera.dontSmoothFollow = dontSmoothFollow;
        TargetFollowCamera.lookAtDamping = lookAtDamping;
        TargetFollowCamera.dontSmoothLookAt = dontSmoothLookAt;

        if (updateRotation)
        {
            float mX = InputManager.GetAxis("Mouse X", false);
            float mY = InputManager.GetAxis("Mouse Y", false);
            TargetFollowCamera.xRotation -= mY * rotationSpeed;
            if (limitXRotation)
                TargetFollowCamera.xRotation = Mathf.Clamp(TargetFollowCamera.xRotation, minXRotation, maxXRotation);
            TargetFollowCamera.yRotation += mX * rotationSpeed;
            if (limitYRotation)
                TargetFollowCamera.yRotation = Mathf.Clamp(TargetFollowCamera.yRotation, minYRotation, maxYRotation);
        }

        if (updateZoom)
        {
            float mZ = InputManager.GetAxis("Mouse ScrollWheel", false);
            TargetFollowCamera.zoomDistance += mZ * zoomSpeed;
            if (limitZoomDistance)
                TargetFollowCamera.zoomDistance = Mathf.Clamp(TargetFollowCamera.zoomDistance, minZoomDistance, maxZoomDistance);
        }
    }
}