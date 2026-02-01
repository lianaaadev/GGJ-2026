using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CameraDetach cameraDetach;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Collider2D playerCollider;
    PlayerInput playerInput;
    Vector2 moveInput;
    [SerializeField] float movementSpeed = 300f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float groundCheckDistance = 0.05f;
    [SerializeField] LayerMask groundLayer;
    public bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
        playerInput.actions["Jump"].performed += ctx => Jump();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * movementSpeed * Time.fixedDeltaTime, rb.linearVelocityY);
        if (moveInput.x < 0)
            spriteRenderer.flipX = true;
        else if (moveInput.x > 0)
            spriteRenderer.flipX = false;

        isGrounded = CheckGrounded();
    }

    private bool CheckGrounded()
    {
        // Get the bottom center of the player's collider
        Bounds bounds = playerCollider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);

        // Use BoxCast for wider ground detection (prevents missed jumps at platform edges)
        Vector2 boxSize = new Vector2(bounds.size.x * 0.9f, 0.05f);
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, groundCheckDistance, groundLayer);

        // Debug visualization - draw the box shape
        Color debugColor = hit ? Color.green : Color.red;
        Vector2 boxBottom = origin + Vector2.down * groundCheckDistance;
        float halfWidth = boxSize.x / 2f;
        float halfHeight = boxSize.y / 2f;
        // Draw box at cast end position
        Debug.DrawLine(new Vector2(boxBottom.x - halfWidth, boxBottom.y - halfHeight), new Vector2(boxBottom.x + halfWidth, boxBottom.y - halfHeight), debugColor);
        Debug.DrawLine(new Vector2(boxBottom.x - halfWidth, boxBottom.y + halfHeight), new Vector2(boxBottom.x + halfWidth, boxBottom.y + halfHeight), debugColor);
        Debug.DrawLine(new Vector2(boxBottom.x - halfWidth, boxBottom.y - halfHeight), new Vector2(boxBottom.x - halfWidth, boxBottom.y + halfHeight), debugColor);
        Debug.DrawLine(new Vector2(boxBottom.x + halfWidth, boxBottom.y - halfHeight), new Vector2(boxBottom.x + halfWidth, boxBottom.y + halfHeight), debugColor);

        return hit.collider != null;
    }

    private void Jump()
    {
        // Check ground state fresh to avoid timing issues with FixedUpdate
        if (!CheckGrounded())
        {
            Debug.Log("NOT GROUNDED");
            return;
        }
        AudioManager.Instance.PlaySE(AudioManager.SE_JUMP);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("Player has died!");
            AudioManager.Instance.PlaySE(AudioManager.SE_FAIL);
            cameraDetach.OnPlayerDeath();
            GameManager.Instance.OnPlayerDeath();
        }
    }
}
