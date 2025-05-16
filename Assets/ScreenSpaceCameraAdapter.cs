using UnityEngine;

public class ScreenSpaceCameraAdapter : MonoBehaviour
{
    [Header("References")]
    public Camera uiCamera;
    public Canvas canvas;

    [Header("Settings")]
    [Tooltip("The aspect ratio your game was designed for (width/height)")]
    public float targetAspect = 16.0f / 9.0f;

    [Tooltip("Maintain this plane distance from camera to canvas")]
    public float planeDistance = 10f;

    [Tooltip("Reference resolution width")]
    public float referenceWidth = 1920f;

    [Tooltip("Reference resolution height")]
    public float referenceHeight = 1080f;

    [Tooltip("Reference orthographic size (if using orthographic camera)")]
    public float referenceOrthographicSize = 5f;

    private void Awake()
    {
        if (uiCamera == null)
            Debug.LogError("UI Camera reference missing on ScreenSpaceCameraAdapter!");

        if (canvas == null)
            canvas = GetComponent<Canvas>();

        if (canvas == null)
            Debug.LogError("Canvas component missing or not assigned!");

        // Ensure Canvas is set to Screen Space - Camera
        if (canvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            Debug.LogWarning("Canvas is not set to Screen Space - Camera mode. Setting it now.");
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
        }

        // Assign the UI camera to the canvas
        canvas.worldCamera = uiCamera;
        canvas.planeDistance = planeDistance;

        // Apply the initial adaptation
        AdaptToScreenSize();
    }

    private void OnRectTransformDimensionsChange()
    {
        // This is called when the screen dimensions change
        AdaptToScreenSize();
    }

    public void AdaptToScreenSize()
    {
        if (uiCamera == null || canvas == null)
            return;

        // Calculate current aspect ratio
        float currentAspect = (float)Screen.width / Screen.height;

        // Keep a reference to original camera settings
        Vector3 originalPosition = uiCamera.transform.position;

        if (uiCamera.orthographic)
        {
            // For orthographic cameras
            AdaptOrthographicCamera(currentAspect);
        }
        else
        {
            // For perspective cameras
            AdaptPerspectiveCamera(currentAspect);
        }

        // Keep the camera at the same position (only adjust size/FOV not position)
        uiCamera.transform.position = originalPosition;

        // Make sure the canvas is still properly connected to the camera
        canvas.worldCamera = uiCamera;
        canvas.planeDistance = planeDistance;
    }

    private void AdaptOrthographicCamera(float currentAspect)
    {
        // Calculate what the orthographic size should be to maintain the same view width
        float targetSize = referenceOrthographicSize;

        if (currentAspect < targetAspect)
        {
            // If the screen is taller than the target aspect, increase the orthographic size
            // This ensures the width stays consistent while the height expands
            float adjustment = targetAspect / currentAspect;
            uiCamera.orthographicSize = targetSize * adjustment;
        }
        else
        {
            // If the screen is wider than the target aspect, keep the orthographic size the same
            // This ensures the height stays consistent while the width expands
            uiCamera.orthographicSize = targetSize;
        }
    }

    private void AdaptPerspectiveCamera(float currentAspect)
    {
        // For perspective cameras, we need to adjust the field of view
        // to maintain the same visible area at the canvas plane distance

        // Calculate the desired vertical FOV based on reference resolution and plane distance
        float refAspect = referenceWidth / referenceHeight;

        // Calculate the reference vertical field of view
        float refHeightAt10 = 2.0f * Mathf.Tan(uiCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * planeDistance;

        if (currentAspect < targetAspect)
        {
            // Taller screen - adjust FOV to maintain width
            float heightAdjustment = targetAspect / currentAspect;
            float newHeightAt10 = refHeightAt10 * heightAdjustment;
            float newFOV = 2.0f * Mathf.Atan(newHeightAt10 * 0.5f / planeDistance) * Mathf.Rad2Deg;
            uiCamera.fieldOfView = newFOV;
        }
        else if (currentAspect > targetAspect)
        {
            // Wider screen - keep same FOV and let the width expand
            // We don't need to adjust anything
        }
    }
}