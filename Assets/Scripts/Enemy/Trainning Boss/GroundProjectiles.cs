using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.Callbacks;
using UnityEngine;

public class GroundProjectiles : MonoBehaviour
{
    public bool isLeft;
    public LayerMask groundLayer;
    public Transform initialPosition;

    private CircleCollider2D circleCollider;
    private BoxCollider2D playerCollider;
    private Rigidbody2D rb;
    private float extraHeight = 0.25f;
    private float velocityConst = 20f;

    public void Attack() {
        if (circleCollider == null) {
            circleCollider = GetComponent<CircleCollider2D>();
        }
        if (rb == null) {
            rb = GetComponent<Rigidbody2D>();
        }
        if (playerCollider == null) {
            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
        }
        rb.transform.position = initialPosition.position;
        gameObject.SetActive(true);
    }

    void OnEnable() {
        Vector2 velocity = new Vector2(0, 0);
        if (isLeft) {
            velocity.x = -velocityConst;
        } else {
            velocity.x = velocityConst;
        }
        rb.velocity = velocity;
    }

    void OnDisable() {
        rb.transform.position = initialPosition.position;
    }

    private void Update() {
        if (!isAlive()) {
            gameObject.SetActive(false);
        }
    }

    private bool isAlive() {
        var downHits =  Physics2D.CircleCast(circleCollider.bounds.center, circleCollider.bounds.size.x / 2, Vector2.down, extraHeight, groundLayer);

       return downHits.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("collision");
        if (collision == playerCollider) {
            playerCollider.gameObject.GetComponent<PlayerHealth>().TakeDamage(0.5f);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(circleCollider.bounds.center, circleCollider.bounds.size.x / 2);
    }

}
