using System.Collections;
using UnityEngine;

public class ColorSwapItem : MonoBehaviour
{
    private BackgroundSprite backgroundSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Sprite Settings")]
    [Tooltip("Sprite to display when pressed (leave empty to keep current)")]
    public Sprite pressedSprite;

    [Header("Behavior Settings")]
    
    [Tooltip("Hide the item after anim")]
    public float hideSpriteDelay = 0.3f;

    [Tooltip("Destroy the item after collection")]
    public bool destroyAfterCollection = true;

    [Tooltip("Delay before destroying (time to show pressed sprite)")]
    public float destroyDelay = 0.3f;

    [Tooltip("Which color index the button will change it to (-1 if you want to cycle through colors instead)")]
    public int colorIndex = -1;



    void Start()
    {
        backgroundSprite = FindFirstObjectByType<BackgroundSprite>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (backgroundSprite == null)
        {
            Debug.LogWarning("ColorSwapItem: No BackgroundSprite found in scene!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (pressedSprite != null && spriteRenderer != null)
            {
                Sprite originalSprite = spriteRenderer.sprite;
                spriteRenderer.sprite = pressedSprite;
                if (destroyAfterCollection)
                {
                    StartCoroutine(HideSpriteAfterDelay());
                }
                else
                {
                    StartCoroutine(UnpressSpriteAfterDelay(originalSprite));
                }

            }

            if (backgroundSprite != null)
            {
                backgroundSprite.OnSwitch(colorIndex);
            }

            if (destroyAfterCollection)
            {
                Destroy(gameObject, destroyDelay);
            }
        }
    }

    IEnumerator HideSpriteAfterDelay()
    {
        yield return new WaitForSeconds(hideSpriteDelay);
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }
    IEnumerator UnpressSpriteAfterDelay(Sprite originalSprite)
    {
        yield return new WaitForSeconds(hideSpriteDelay);
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = originalSprite;
        }
    }    
}
