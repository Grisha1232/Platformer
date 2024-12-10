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
    [SerializeField] protected float attackDamage = 1.0f;

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

    
    [SerializeField] protected GameObject indicatorShadow;
    [SerializeField] protected GameObject indicatorAttack;
    [SerializeField] protected GameObject indicatorPatrol;

    /// <summary>
    /// Находится ли враг в атакующем режиме
    /// </summary>
    [HideInInspector] public bool isAttackMode {get; set;} = false;

    protected Rigidbody2D body;
    protected Animator animator;
    protected Transform player;

    protected void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  
    }

    abstract protected void Attack();
    abstract protected void ChaseToAttack();

    public virtual bool PlayerInAggroRange() {
        isAttackMode = Vector2.Distance(transform.position, player.position) <= aggroRange;
        return isAttackMode;
    }
    

}