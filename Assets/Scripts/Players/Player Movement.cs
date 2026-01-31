using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput playerInput;
    Vector2 moveInput;
    Rigidbody2D rb;
    [SerializeField] float movementSpeed = 200f;
    bool isMoving = false;
    bool isMovingRight = false;
    [SerializeField] float jumpForce = 5f;
    bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
        playerInput.actions["Jump"].performed += ctx => Jump();
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector2.right * moveInput.x * movementSpeed);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
