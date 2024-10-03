using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private Animator anim;
    [SerializeField] private LayerMask groundLayer;
    
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private float wallJumpCooldown;
    private float horizontalInput;

/// <summary>
/// This methods calls every time game is start
/// </summary>
    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        
        if (horizontalInput > 0.0f) {
            transform.localScale = new  Vector3(4, 4, 4);
        } else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector3(-4, 4, 4);
        }

        print("horizontalInput = "  + horizontalInput);
        anim.SetBool("Run", horizontalInput != 0);
        if (isGrounded()) {
            anim.SetBool("Jump", false);
        } else {
            anim.SetBool("Jump", true);
        }


        if (wallJumpCooldown > 0.2f) {

            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (isOnWall() && !isGrounded()) {
                body.gravityScale = 1;
                body.velocity = Vector2.zero;
            } else {
                body.gravityScale = 3;
            }
            
            if ( Input.GetKey(KeyCode.Space) ) {
                print("performing jump");
                Jump();
            }

        } else {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump() {
        if ( isGrounded() ) {
            print("jump of the ground");
            anim.SetBool("Jump", true);
            body.velocity = new Vector2(body.velocity.x, jumpPower);
        } else if ( isOnWall() && !isGrounded() ) {
            if (horizontalInput == 0) {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * speed, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            } else {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, jumpPower);
            }
            
            wallJumpCooldown = 0;
        }
    }

    private bool isOnWall() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    private bool isGrounded() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack() {
        return !isOnWall();
    }
}
