using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform player;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Sprite smokePuffSprite;
    [SerializeField] float respawnDelay = 1f;

    Vector3 playerStartPos;
    Vector3 cameraStartPos;
    Transform cameraOriginalParent;
    SpriteRenderer playerSprite;
    Rigidbody2D playerRb;

    public bool IsDead { get; private set; } = false;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int maxLevel = 3;
    [SerializeField] Sprite lvl1BG;
    [SerializeField] Vector2 lvl1BGScale;
    [SerializeField] Sprite lvl2BG;
    [SerializeField] Vector2 lvl2BGScale;
    [SerializeField] Sprite lvl3BG;
    [SerializeField] Vector2 lvl3BGScale;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        cameraOriginalParent = cameraTransform.parent;
        playerSprite = player.GetComponent<SpriteRenderer>();
        playerRb = player.GetComponent<Rigidbody2D>();

        ChangeLevel(currentLevel);
    }

    // todo show game complete screen
    public void OnLevelComplete()
    {
        if (currentLevel >= maxLevel - 1)
        {
            Debug.Log("All levels completed! Restarting from level 0...");
            ChangeLevel(0);
            return;
        }
        Debug.Log("Loading next level...");
        ChangeLevel(currentLevel + 1);
    }

    // call when changing levels
    public void ChangeLevel(int lvl)
    {
        currentLevel = lvl;
        SpriteRenderer sr = BackgroundSprite.Instance.GetComponent<SpriteRenderer>();
        switch (currentLevel)
        {
            case 0:
                AudioManager.Instance.PlayBGM(AudioManager.BGM_FIRSTLEVEL);
                break;
            case 1:
                AudioManager.Instance.PlayBGM(AudioManager.BGM_CITY);
                break;
            case 2:
                AudioManager.Instance.PlayBGM(AudioManager.BGM_FINALLEVEL);
                break;
            default:
                AudioManager.Instance.PlayBGM(AudioManager.BGM_FIRSTLEVEL);
                break;
        }
        sr.sprite =
            currentLevel == 0 ? lvl1BG :
            currentLevel == 1 ? lvl2BG :
            currentLevel == 2 ? lvl3BG : lvl1BG;
        sr.size =
            currentLevel == 0 ? lvl1BGScale :
            currentLevel == 1 ? lvl2BGScale :
            currentLevel == 2 ? lvl3BGScale : lvl1BGScale;
        MapLoader.Instance.InitLevel(currentLevel);
    }

    public void StoreInitialState(Vector2 pos)
    {
        playerStartPos = pos;
        // cameraStartPos = cameraTransform.localPosition;
    }

    public void OnPlayerDeath()
    {
        if (IsDead) return;
        IsDead = true;

        // Freeze player immediately
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.simulated = false;
        }

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // Spawn smoke at player position
        if (smokePuffSprite != null)
        {
            GameObject smoke = new GameObject("SmokePuff");
            smoke.transform.position = player.position;
            SpriteRenderer sr = smoke.AddComponent<SpriteRenderer>();
            sr.sprite = smokePuffSprite;
            sr.sortingOrder = 100;
            Destroy(smoke, respawnDelay);
        }

        // Hide player sprite
        if (playerSprite != null)
            playerSprite.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Reset and show player
        ResetToInitialState();
        if (playerSprite != null)
            playerSprite.enabled = true;

        // Re-enable physics
        if (playerRb != null)
            playerRb.simulated = true;

        // Reattach camera
        CameraDetach camDetach = cameraTransform.GetComponent<CameraDetach>();
        if (camDetach != null)
            camDetach.AttachCameraToPlayer();

        IsDead = false;
    }

    public void ResetToInitialState()
    {
        // Reset player
        player.position = playerStartPos;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Reset camera
        cameraTransform.parent = cameraOriginalParent;
        cameraTransform.localPosition = cameraStartPos;
    }
}
