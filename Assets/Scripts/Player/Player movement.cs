using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    public Animator Anim { get; private set; }
    [SerializeField] private LayerMask groundLayer;
    
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private float wallJumpCooldown;
    private float horizontalInput;
    private bool canMove;

/// <summary>
/// This methods calls every time game is start
/// </summary>
    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
        canMove = true;
    }

    private void Update() {
        
        horizontalInput = Input.GetAxis("Horizontal");
        if (!canMove) {
            return;
        }
        
        if (horizontalInput > 0.0f) {
            transform.localScale = new  Vector3(4, 4, 4);
        } else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector3(-4, 4, 4);
        }

        Anim.SetBool("Run", horizontalInput != 0);
        if (isGrounded()) {
            Anim.SetBool("grounded", true);
        } else {
            Anim.SetBool("grounded", false);
        }


        if (wallJumpCooldown > 0.4f) {

            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (isOnWall() && !isGrounded()) {
                body.gravityScale = 1;
                body.velocity = Vector2.zero;
                Anim.SetBool("grounded", false);
                Anim.SetBool("OnWall", true);
            } else {
                body.gravityScale = 3;
                Anim.SetBool("OnWall", false);
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
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        if ( isGrounded() ) {
            print("jump of the ground");
            Anim.SetBool("Jump", true);
            body.velocity = new Vector2(body.velocity.x, jumpPower);
        } else if ( isOnWall() && !isGrounded() ) {
            if (horizontalInput == 0) {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * speed, jumpPower / 2f);
                Anim.SetBool("OnWall", false);
                Anim.SetBool("grounded", false);
                //transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            } else {
                body.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * speed, jumpPower);
                Anim.SetBool("OnWall", false);
                Anim.SetBool("grounded", false);
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
        return !isOnWall() && isGrounded();
    }

    public void setCanMove(bool val) {
        if (!val) {
            body.velocity = new Vector2(0, body.velocity.y);
        }
        canMove = val;
    }

}
