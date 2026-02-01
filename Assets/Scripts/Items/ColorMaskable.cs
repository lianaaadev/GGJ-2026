using UnityEngine;

public enum PlatformColorType
{
    Red,
    Blue,
    Green,
    Yellow,
    Cyan,
    Magenta
}

public class ColorMaskable : MonoBehaviour
{
    [Header("Color Mask Settings")]
    public PlatformColorType colorType = PlatformColorType.Red;

    private Renderer[] renderers;
    private Collider2D[] colliders;

    void Awake()
    {
        renderers = GetComponents<Renderer>();
        colliders = GetComponents<Collider2D>();
    }

    public void UpdateVisibility(PlatformColorType maskColorType)
    {
        bool shouldBeActive = colorType != maskColorType;

        foreach (var r in renderers)
            r.enabled = shouldBeActive;

        foreach (var c in colliders)
            c.enabled = shouldBeActive;
    }
}
