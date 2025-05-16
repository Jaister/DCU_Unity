using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class SafeAreaStars : MonoBehaviour
{
    // Offset from the edge in pixels
    [SerializeField] private Vector2 offset = new Vector2(20, 20);

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (Screen.safeArea.width > 1920 && Screen.safeArea.height > 1100)
            ApplySafeArea();
    }

    void ApplySafeArea()
    {
        // Get the safe area
        Rect safeArea = Screen.safeArea;

        // Get the top right position, but adjust Y position to be lower
        // This positions it correctly within the visible area
        Vector2 adjustedPosition = new Vector2(
            safeArea.x + safeArea.width *0.6f,  // Slightly inset from the right edge
            safeArea.y + (safeArea.height * 0.65f)   // Lower on the screen (~85% of height)
        );

        // Set the position directly
        rectTransform.position = adjustedPosition;

        Debug.Log($"Safe Area: {safeArea}");
        Debug.Log($"Adjusted Position: {adjustedPosition}");
    }
}