using UnityEngine;

public class DefaultBoss : MonoBehaviour {
    
    public LayerMask enemyLayer;
    public LayerMask platformLayer;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    protected Transform player; // Ссылка на игрока
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;
    protected Animator animator;
    [HideInInspector] public static bool isLocked = true;
}