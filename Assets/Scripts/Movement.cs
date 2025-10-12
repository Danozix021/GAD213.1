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
    public LayerMask groundLayerMask = 1;
    public float groundCheckDistance = 0.1f;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private bool isJumping;
    private bool isBraking;
    private bool isGrounded;
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

    private void CheckGrounded()
    {
        Vector2 raycastOrigin = (Vector2)transform.position + Vector2.down * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, groundCheckDistance, groundLayerMask);
        isGrounded = hit.collider != null;
    }

    private void PerformJump()
    {
        if (isGrounded && !isJumping)
        {
            Vector2 jumpVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            rb.linearVelocity = jumpVelocity;
            isJumping = true;
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        PerformJump();
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumping = false;
    }

    private void OnBrakePressed(InputAction.CallbackContext context)
    {
        isBraking = true;
    }

    private void OnBrakeReleased(InputAction.CallbackContext context)
    {
        isBraking = false;
    }
}
