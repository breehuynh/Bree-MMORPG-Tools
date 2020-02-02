using UnityEngine;

[ExecuteInEditMode]
public class FollowCamera : MonoBehaviour
{
    private Transform cacheTransform;

    public Transform CacheTransform
    {
        get
        {
            if (cacheTransform == null)
                cacheTransform = GetComponent<Transform>();
            return cacheTransform;
        }
    }

    // The target we are following
    public Transform target;

    public Vector3 targetOffset;

    [Header("Follow")]
    public float damping = 10.0f;

    public bool dontSmoothFollow;

    [Header("Look at")]
    public float lookAtDamping = 2.0f;

    public bool dontSmoothLookAt;

    [Header("Rotation")]
    public float xRotation;

    public float yRotation;

    [Tooltip("If this is TRUE, will update Y-rotation follow target")]
    public bool useTargetYRotation;

    [Header("Zoom")]
    public float zoomDistance = 10.0f;

    [Header("Zoom by ratio (Currently work well with landscape screen)")]
    public bool zoomByAspectRatio;

    public float zoomByAspectRatioWidth;
    public float zoomByAspectRatioHeight;
    public float zoomByAspectRatioMin;

    private void Update()
    {
        Vector3 targetPosition = target == null ? Vector3.zero : target.position;
        float targetYRotation = target == null ? 0 : target.eulerAngles.y;
        Vector3 wantedPosition = targetPosition + targetOffset;
        float wantedYRotation = useTargetYRotation ? targetYRotation : yRotation;

        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(xRotation, wantedYRotation, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        wantedPosition -= currentRotation * Vector3.forward * zoomDistance;

        // Update position
        if (!dontSmoothFollow)
            CacheTransform.position = Vector3.Slerp(CacheTransform.position, wantedPosition, damping * Time.deltaTime);
        else
            CacheTransform.position = wantedPosition;

        // Always look at the target
        if (!dontSmoothLookAt)
        {
            Quaternion lookAtRotation = Quaternion.LookRotation(targetPosition + targetOffset - CacheTransform.position);
            CacheTransform.rotation = Quaternion.Slerp(CacheTransform.rotation, lookAtRotation, lookAtDamping * Time.deltaTime);
        }
        else
            CacheTransform.rotation = Quaternion.LookRotation(targetPosition + targetOffset - CacheTransform.position);

        if (zoomByAspectRatio)
        {
            float targetaspect = zoomByAspectRatioWidth / zoomByAspectRatioHeight;
            float windowaspect = (float)Screen.width / (float)Screen.height;
            float scaleheight = windowaspect / targetaspect;
            float diffScaleHeight = 1 - scaleheight;
            if (diffScaleHeight < zoomByAspectRatioMin)
                diffScaleHeight = zoomByAspectRatioMin;
            zoomDistance = diffScaleHeight * 20f;
        }
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        // Update camera when it's updating edit mode (not play mode)
        if (!Application.isPlaying && Application.isEditor)
            Update();
#endif
    }
}