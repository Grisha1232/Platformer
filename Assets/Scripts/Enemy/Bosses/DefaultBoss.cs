using UnityEngine;

public class DefaultBoss : MonoBehaviour {
    
    public LayerMask enemyLayer;
    public LayerMask platformLayer;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    protected float attackCooldown = 2f;
    protected float attackCooldownCounter;
    protected Transform player; // Ссылка на игрока
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;
    protected Animator animator;
    [HideInInspector] public bool isLocked = true;

    public bool isDead = false;

    private Vector2 initialPosition;

    public void Start() {
        if (isDead) {
            gameObject.SetActive(false);
            return;
        }
        initialPosition = transform.position;
    }

    public void Reset() {
        GetComponent<EnemyHealth>().Reset();
        isLocked = true;
        transform.position = initialPosition;
    }
}