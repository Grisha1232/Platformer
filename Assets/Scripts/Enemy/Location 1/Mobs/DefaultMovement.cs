using Unity.VisualScripting;
using UnityEngine;

public abstract class DefaultMovement : MonoBehaviour
{
    /// <summary>
    /// Скорость патрулирования
    /// </summary>
    [SerializeField] protected float patrolSpeed = 2f;

    /// <summary>
    /// Скорость преследования
    /// </summary>
    [SerializeField] protected float chaseSpeed = 4f;

    /// <summary>
    /// Радиус обнаружения игрока
    /// </summary>
    [SerializeField] protected float detectionRange = 5f;

    /// <summary>
    /// Дальность патрулирования
    /// </summary>
    [SerializeField] protected float patrolRange;

    /// <summary>
    /// Ссылка на игрока
    /// </summary>
    protected Transform player;

    /// <summary>
    /// Флаг, указывающий на то, преследует ли враг игрока
    /// </summary>
    protected bool isChasing = false;

    /// <summary>
    /// Начальная позиция врага
    /// </summary>
    protected Vector3 initialPosition;
    
    /// <summary>
    /// Тело врага
    /// </summary>
    protected Rigidbody2D body;

    /// <summary>
    /// Аниматор врага
    /// </summary>
    protected Animator animator;
    
    protected void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        initialPosition = body.position; // Сохранение начальной позиции
        player = GameObject.FindGameObjectWithTag("Player").transform; // Поиск игрока по тегу
    }

    abstract protected void Update();

    abstract protected void Patrol();
    abstract protected void ChasePlayer();

}