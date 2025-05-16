using UnityEngine;

public class AspectRatioHandler : MonoBehaviour
{
    [Header("Target Aspect Ratio")]
    [Tooltip("The aspect ratio your game was designed for (width/height)")]
    public float targetAspect = 16.0f / 9.0f;

    [Header("Camera Settings")]
    public Camera mainCamera;
    [Tooltip("Content that should stay visible regardless of aspect ratio")]
    public float orthographicSizeForTargetAspect = 5f;

    [Header("Safe Area Settings")]
    [Tooltip("Root container that will be adjusted to fit safe area")]
    public RectTransform safeAreaRect;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Apply aspect ratio adjustments
        AdjustCamera();

        // Apply safe area adjustments
        if (safeAreaRect != null)
            ApplySafeArea();
    }

    private void AdjustCamera()
    {
        // Calculate the current aspect ratio
        float currentAspect = (float)Screen.width / Screen.height;

        // If the camera is orthographic (2D games)
        if (mainCamera.orthographic)
        {
            // Calculate the orthographic size based on the aspect ratio difference
            if (currentAspect < targetAspect)
            {
                // Tall screen - adjust orthographic size to fit width
                mainCamera.orthographicSize = orthographicSizeForTargetAspect * (targetAspect / currentAspect);
            }
            else
            {
                // Wide screen - maintain the original orthographic size
                mainCamera.orthographicSize = orthographicSizeForTargetAspect;
            }
        }
        else
        {
            // For perspective cameras (3D games)
            // You may need to adjust field of view or camera position
            // This simple implementation maintains a consistent vertical FOV
            float targetHeight = 2.0f * orthographicSizeForTargetAspect;
            float targetWidth = targetHeight * targetAspect;
            float currentWidth = targetHeight * currentAspect;

            if (currentAspect < targetAspect)
            {
                // Tall screen - move camera back to see the same width
                float distance = targetWidth / (2.0f * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad));
                mainCamera.transform.position = new Vector3(
                    mainCamera.transform.position.x,
                    mainCamera.transform.position.y,
                    -distance
                );
            }
        }
    }

    private void ApplySafeArea()
    {
        // Get the safe area
        Rect safeArea = Screen.safeArea;

        // Convert safe area to anchors (0-1 space)
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Apply the anchors to our safe area container
        safeAreaRect.anchorMin = anchorMin;
        safeAreaRect.anchorMax = anchorMax;
        safeAreaRect.offsetMin = Vector2.zero;
        safeAreaRect.offsetMax = Vector2.zero;
    }

    // Call this method when the screen orientation changes
    public void OnOrientationChanged()
    {
        AdjustCamera();
        if (safeAreaRect != null)
            ApplySafeArea();
    }
}