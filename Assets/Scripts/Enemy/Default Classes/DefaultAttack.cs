using System.Collections;
using UnityEngine;

public abstract class DefaultAttack : MonoBehaviour {

    /// <summary>
    /// Скорость преследования
    /// </summary>
    [SerializeField] protected float chaseSpeed = 5f;

    /// <summary>
    /// Радиус обнаружения игрока
    /// </summary>
    [SerializeField] protected float aggroRange = 10f;

    /// <summary>
    /// Количество урона
    /// </summary>
    [SerializeField] protected float damage = 1.0f;

    /// <summary>
    /// Дальность атаки
    /// </summary>
    [SerializeField] protected float attackRange = 1.0f; 

    /// <summary>
    /// Время перезарядки атаки
    /// </summary>
    [SerializeField] protected float attackCooldown = 2.0f;  

    /// <summary>
    /// Множитель урона при спец приемах
    /// </summary>
    [SerializeField] protected float multiplayerDamage = 2.0f;
    [SerializeField] protected Transform attackTransform;
    [SerializeField] protected LayerMask attackableLayer;
    [SerializeField] protected LayerMask obstacleLayer;

    /// <summary>
    /// Находится ли враг в атакующем режиме
    /// </summary>
    [HideInInspector] public bool isAttackMode {get; set;} = false;

    protected Rigidbody2D body;
    protected Animator animator;
    protected Transform player;
    protected float attackTimeCounter = 9f;

    
    protected bool shouldBeDamaging {get; set;}= true; 

    protected void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  
    }

    abstract protected IEnumerator Attack();
    abstract protected void ChaseToAttack();

    public virtual bool PlayerInAggroRange() {
        isAttackMode = Vector2.Distance(transform.position, player.position) <= aggroRange;
        return isAttackMode;
    }

    public virtual bool PlayerInAttackRange() {
        var hits = Physics2D.CircleCast(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);
        return hits.collider != null;
    }
    

}