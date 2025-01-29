using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Pathfinder;

public abstract class DefaultMovement : MonoBehaviour
{
    /// <summary>
    /// Скорость патрулирования
    /// </summary>
    [SerializeField] protected float patrolSpeed = 2f;

    /// <summary>
    /// Дальность патрулирования
    /// </summary>
    [SerializeField] protected float patrolRange = 4f;

    [SerializeField] protected float jumpForce = 10f;

    [SerializeField] protected GameObject indicatorShadow;
    [SerializeField] protected GameObject indicatorAttack;
    [SerializeField] protected GameObject indicatorPatrol;


    /// <summary>
    /// Ссылка на игрока
    /// </summary>
    protected Transform player;

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

    protected DefaultAttack attackScript;

    protected List<Vector3Int> pathToFollow;
    protected int currentPointToFollow;

    
    protected void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        attackScript = GetComponent<DefaultAttack>();
        initialPosition = body.position; // Сохранение начальной позиции
        player = GameObject.FindGameObjectWithTag("Player").transform; // Поиск игрока по тегу
    }

    abstract protected void Update();

    abstract protected void Patrol();

    abstract protected void followPath();

    
    protected class Pair {
        public Vector3 vect;
        public Direction dir;

        public Pair(Vector3 vect, Direction dir) {
            this.vect = vect;
            this.dir = dir;
        }
    }

}