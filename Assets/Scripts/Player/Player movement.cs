using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 15f;
    public float dashSpeed = 15f;
    public float dashTime = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator anim;
    private bool isDashing = false;
    private float dashTimer;
    private bool isMovementBlocked = false;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing) {
            Dash();
        }
        if (isDashing || isMovementBlocked)
        {
            return;  // Пока движение заблокировано атакой или рывком, игнорируем управление
        }

        Move();
        Jump();

        if (Input.GetKeyDown(KeyCode.LeftAlt) && !isDashing)
        {
            StartDash();
        }
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        // Поворот спрайта в зависимости от направления
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput) * 4, 4f, 4f);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashTime;
        anim.SetTrigger("Dash");
    }

    void Dash()
    {
        float dashDirection = Mathf.Sign(transform.localScale.x);
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            isDashing = false;
        }
    }

    // Остановка движения для блокировки его во время атак
    public void BlockMovement()
    {
        isMovementBlocked = true;
    }

    public void UnblockMovement()
    {
        isMovementBlocked = false;
    }

    public void isGrounded() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        if (raycastHit.collider != null) {
            anim.SetTrigger("isGrounded");
        }
    }
}
