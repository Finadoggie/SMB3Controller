using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    enum PlayerState
    {
        Normal
    }

    public InputActionAsset actionAsset;
    private InputAction movement, jump, run;
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D groundCheck;
    public BoxCollider2D playerCollider;

    public Vector2 gravityVector, movementVector, walkVector, runVector, moveLimiter, jumpVector, maxFallVector;
    public float lowGravityMultiplier, jumpSpeedMultipier;
    private Vector2 gravityVectorNormal, gravityVectorLow;

    private PlayerState currentState;
    private bool isGrounded, canJump, justJumped, jumpHeld, jumpPressedLastFrame, runHeld, turningAround;
    [SerializeField] private int lowGravityFrames, runDelay, coyoteFrames;
    private int jumpTime, runTime, lastGrounded;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        movement = actionAsset["Movement"];
        movement.Enable();
        jump = actionAsset["Jump"];
        jump.Enable();
        run = actionAsset["Run"];
        run.Enable();
        
        currentState = PlayerState.Normal;

        CalculateGravity();

        jumpHeld = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // This exists solely for testing purposes
        CalculateGravity();

        switch (currentState)
        {
            case PlayerState.Normal:
                PreliminaryNormalStateChecks();
                RunGravity();
                RunMovement();
                RunJump();
                break;
        }
        UpdateAnimatorVariables();
    }
    void CalculateGravity()
    {
        gravityVectorNormal = gravityVector;
        gravityVectorLow = new Vector2(gravityVector.x, gravityVector.y * lowGravityMultiplier);
    }

    void PreliminaryNormalStateChecks()
    {
        // Check if jump is held and if player just jumped
        jumpPressedLastFrame = jumpHeld;
        jumpHeld = actionAsset["Jump"].IsPressed();
        justJumped = justJumped && jumpPressedLastFrame;

        // Check if grounded
        isGrounded = groundCheck.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (isGrounded) lastGrounded = coyoteFrames;
        canJump = isGrounded || lastGrounded-- > 0;

        // Check if running
        runHeld = actionAsset["Run"].IsPressed();
    }

    void RunGravity()
    {
        // Only run gravity if not on ground
        if (isGrounded) return;

        // Apply weaker gravity if jump is held (only for a certain # of frames)
        if (jumpTime-- > 0 && justJumped && rb.velocity.y > 0.1) rb.velocity += gravityVectorLow;
        else rb.velocity += gravityVectorNormal;

        // Hard cap for terminal downward velocity
        if (rb.velocity.y < maxFallVector.y) rb.velocity = maxFallVector + rb.velocity * Vector2.right;
    }

    void RunMovement()
    {
        // Accelerate in direction held
        rb.velocity += movement.ReadValue<Vector2>() * movementVector;
        
        // Calc current speed for use in capping speed
        speed = Mathf.Abs(rb.velocity.x);

        // Revert acceleration if above walk/run speeds
        if (speed > walkVector.x && !runHeld) rb.velocity -= movement.ReadValue<Vector2>() * movementVector;
        else if (speed > runVector.x) rb.velocity -= movement.ReadValue<Vector2>() * movementVector;
        
        // Recalc speed to account for changes
        speed = Mathf.Abs(rb.velocity.x);

        // Check if user is actively inputting in the direction opposite to what they're facing
        turningAround = (Mathf.Sign(rb.velocity.x) != Mathf.Sign(movement.ReadValue<Vector2>().x)) && (movement.ReadValue<Vector2>().x != 0);
        // Accelerate player towards 0 if they're not inputting or inputting in opposite direction
        if (turningAround || movement.ReadValue<Vector2>().x == 0)
        {
            if (Mathf.Abs(speed) >= 0.5) rb.velocity -= Mathf.Sign(rb.velocity.x) * movementVector;
            // If speed is low enough, round down to 0
            else rb.velocity = rb.velocity * Vector2.up;
        }

        // Ensure sprite is facing desired direction when skidding
        // haha xor go brrr
        if (rb.velocity.x > 0.01) spriteRenderer.flipX = true ^ turningAround;
        else if (rb.velocity.x < -0.01) spriteRenderer.flipX = false ^ turningAround;
    }

    void RunJump()
    {
        // Return if not jumping on this call
        if (!(canJump && !justJumped && jumpHeld)) return;

        // Jump will be slightly higher if moving
        rb.velocity = jumpVector * Mathf.Max((speed + 1) * jumpSpeedMultipier - 1, 1) + rb.velocity * Vector2.right;

        // Set a bunch of stuff 
        isGrounded = false;
        canJump = false;
        justJumped = true;
        jumpHeld = true;
        jumpTime = lowGravityFrames;
        lastGrounded = 0;
    }

    void UpdateAnimatorVariables()
    {
        animator.SetBool("Is Grounded", isGrounded);
        animator.SetBool("Is Moving", speed > 0.1);
        animator.SetBool("Turning Around", turningAround);
        animator.SetFloat("Speed", Mathf.Min(Mathf.Max(speed, 1f), 6f));
    }
}
