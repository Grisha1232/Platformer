using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float jumpTime = 0.5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 0.1f;

    [Header("Envirement")]
    [SerializeField] private float  extraHeight = 0.25f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator anim;

    private bool isJumping;
    private bool isDashing;
    private float jumpTimeCounter;
    private float dashTimeCounter;
    private float dashCooldownTimer;

    private RaycastHit2D groundHit;
    private RaycastHit2D enemyHit;

    static public bool isMovementBlocked {get; private set;}

    void Start()
    {
        isJumping = false;
        jumpTimeCounter = 0f;
        dashCooldownTimer = 99;
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        dashCooldownTimer += Time.deltaTime;
        dashTimeCounter -= Time.deltaTime;
        if (isMovementBlocked) {
            return;
        }
        Move();
        Jump();
        Dash();
    }

    #region Movement Functions
    void Move()
    {
        float moveInput = UserInput.instance.moveInput.x;
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        anim.SetFloat("Speed", Math.Abs(moveInput));

        // Поворот спрайта в зависимости от направления
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput) * 4, 4f, 4f);
        }
    }

    void Jump()
    {
        if (UserInput.instance.controls.Jumping.Jump.WasPressedThisFrame() && isGrounded()) {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTimeCounter = jumpTime;
            anim.SetTrigger("Jump");
        }

        if (UserInput.instance.controls.Jumping.Jump.IsPressed() && isJumping) {
            if (jumpTimeCounter > 0 && isJumping) {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        if (UserInput.instance.controls.Jumping.Jump.WasReleasedThisFrame() && isJumping) {
            isJumping = false;
        }
    }

    void Dash()
    {
        if (UserInput.instance.controls.Dashing.Dash.WasPressedThisFrame() && !isDashing && dashCooldownTimer > dashCooldown) {
            isMovementBlocked = true;
            isDashing = true;
            anim.SetTrigger("Dash");
            rb.velocity = new Vector2(dashSpeed * Math.Sign(transform.localScale.x), 0);
            rb.gravityScale = 0;
            dashTimeCounter = dashTime;
        }
    }

    private void Dashing() {
        if (dashTimeCounter > 0 && isDashing) {
            isMovementBlocked = true;
            rb.velocity = new Vector2(dashSpeed * Math.Sign(transform.localScale.x), 0);
            rb.gravityScale = 0;
        }

        if (dashTimeCounter <= 0 && isDashing) {
            isMovementBlocked = false;
            isDashing = false;
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 6;
            dashCooldownTimer = 0;
            anim.SetTrigger("StopDash");
        }
    }

    #endregion

    #region GroundCheck

    private bool isGrounded() {
        groundHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, groundLayer);
        enemyHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, enemyLayer);
        
        return groundHit.collider != null || enemyHit.collider != null;
    }

    private void isGroundedForAnim() {
        groundHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, groundLayer);
        enemyHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, enemyLayer);
        
        if (groundHit.collider != null || enemyHit.collider != null) {
            anim.SetTrigger("isGrounded");
        }
    }

    #endregion

    #region Animation Triggers

    private void BlockMovement() {
        isMovementBlocked = true;
    }

    private void UnblockMovement() {
        isMovementBlocked = false;
    }

    #endregion
}
