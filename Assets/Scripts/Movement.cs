using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float maxSpeed = 8f;
    public float brakeDeceleration = 15f;
    public float naturalDeceleration = 2f;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public float jumpHoldForce = 8f;
    public float jumpHoldDuration = 0.3f;
    public float jumpCutMultiplier = 0.5f;
    public LayerMask groundLayerMask = 1;

    [Header("Ground Check")]
    public Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private bool isJumping;
    private bool isJumpButtonHeld;
    private float jumpHoldTime;
    private bool isBraking;
    private bool isGrounded;
    private bool wasGrounded;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction brakeAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        rb.gravityScale = 2f;
        rb.linearDamping = 0f;
        rb.angularDamping = 5f;
        rb.freezeRotation = true;

        SetupInputActions();
    }

    private void SetupInputActions()
    {
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        brakeAction = playerInput.actions["Brake"];

        jumpAction.performed += OnJumpPerformed;
        jumpAction.canceled += OnJumpCanceled;
        brakeAction.performed += OnBrakePressed;
        brakeAction.canceled += OnBrakeReleased;
    }

    private void OnDestroy()
    {
        if (jumpAction != null)
        {
            jumpAction.performed -= OnJumpPerformed;
            jumpAction.canceled -= OnJumpCanceled;
        }

        if (brakeAction != null)
        {
            brakeAction.performed -= OnBrakePressed;
            brakeAction.canceled -= OnBrakeReleased;
        }
    }

    private void Update()
    {
        CheckGrounded();
        ReadMovementInput();
        HandleMovement();
        HandleJumpHold();
    }

    private void ReadMovementInput()
    {
        if (moveAction != null)
        {
            movementInput = moveAction.ReadValue<Vector2>();
        }
    }

    private void HandleMovement()
    {
        float targetVelocityX = rb.linearVelocity.x;

        if (isBraking && isGrounded)
        {
            targetVelocityX = Mathf.MoveTowards(targetVelocityX, 0f, brakeDeceleration * Time.deltaTime);
        }
        else if (movementInput.x != 0)
        {
            targetVelocityX += movementInput.x * acceleration * Time.deltaTime;
            targetVelocityX = Mathf.Clamp(targetVelocityX, -maxSpeed, maxSpeed);
        }
        else if (isGrounded)
        {
            targetVelocityX = Mathf.MoveTowards(targetVelocityX, 0f, naturalDeceleration * Time.deltaTime);
        }

        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    private void HandleJumpHold()
    {
        if (isJumpButtonHeld && isJumping && jumpHoldTime < jumpHoldDuration)
        {
            jumpHoldTime += Time.deltaTime;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + (jumpHoldForce * Time.deltaTime));
        }
    }

    private void CheckGrounded()
    {
        wasGrounded = isGrounded;

        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        Collider2D groundCollider = Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayerMask);
        isGrounded = groundCollider != null;

        if (isGrounded && !wasGrounded && rb.linearVelocity.y <= 0)
        {
            isJumping = false;
            jumpHoldTime = 0f;
        }
    }

    private void PerformJump()
    {
        if (isGrounded && !isJumping)
        {
            Vector2 jumpVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            rb.linearVelocity = jumpVelocity;
            isJumping = true;
            isJumpButtonHeld = true;
            jumpHoldTime = 0f;
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        PerformJump();
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumpButtonHeld = false;

        if (isJumping && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
    }

    private void OnBrakePressed(InputAction.CallbackContext context)
    {
        isBraking = true;
    }

    private void OnBrakeReleased(InputAction.CallbackContext context)
    {
        isBraking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }
}
