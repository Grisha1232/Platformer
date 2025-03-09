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

    protected BoxCollider2D boxCollider;

    /// <summary>
    /// Аниматор врага
    /// </summary>
    protected Animator animator;

    protected DefaultAttack attackScript;


    
    protected void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        attackScript = GetComponent<DefaultAttack>();
        initialPosition = body.position; // Сохранение начальной позиции
        player = GameObject.FindGameObjectWithTag("Player").transform; // Поиск игрока по тегу
    }

    abstract protected void Update();

    abstract protected void Patrol();

}