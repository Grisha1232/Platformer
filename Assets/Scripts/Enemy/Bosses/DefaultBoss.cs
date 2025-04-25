using UnityEngine;

public class DefaultBoss : MonoBehaviour {
    
    public LayerMask platformLayer;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    public GameObject checkpoint;

    protected float attackCooldown = 2f;
    protected float attackCooldownCounter;
    protected float moveSpeed = 5f;
    protected Transform player; // Ссылка на игрока
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;
    protected Animator animator;
    [HideInInspector] public bool isLocked = true;

    private Vector2 initialPosition;

    public void Start() {
        initialPosition = transform.position;
    }

    public void Reset() {
        GetComponent<EnemyHealth>().Reset();
        isLocked = true;
        transform.position = initialPosition;
    }

    public virtual void ShowCheckpoint() {}
}