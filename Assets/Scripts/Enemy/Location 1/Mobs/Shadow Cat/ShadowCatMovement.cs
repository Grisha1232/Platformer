using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class ShadowCatMovement : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float patrolRange;
    [SerializeField] private float aggroRange;
    [SerializeField] private float exitChaseRange;

    [SerializeField] private float teleportDistance;
    [SerializeField] private float teleportCooldown;

    private bool isTeleporting = false;
    private bool isFacingRight = true;
    private bool isChasingPlayer = false;

    [SerializeField] private Vector3 startingPosition;
    private Transform player;
    private Rigidbody2D body;
    private Animator animator;

/// <summary>
/// This methods calls every time game is start
/// </summary>
    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {

       if (PlayerInAggroRange() && !isTeleporting)
        {
            // Если игрок в зоне агро — начинаем преследование
            ChasePlayer();
        }
        else if (PlayerOutOfChaseRange() && isChasingPlayer)
        {
            // Если игрок вышел за пределы зоны преследования — возвращаемся к патрулированию
            StopChasingPlayer();
        }
        else if (!isChasingPlayer)
        {
            // Если не преследуем игрока — патрулируем
            Patrol();
        }

        // Если не происходит телепортация — инициируем ее
        if (!isTeleporting)
        {
            StartCoroutine(Teleport());
        }
       
    }

    void Patrol()
    {
        // Движение в пределах патрулирования
        float moveDirection = isFacingRight ? 1 : -1;
        body.velocity = new Vector2(moveDirection * moveSpeed, body.velocity.y);

        // Поворот в пределах патрулирования
        if (Vector2.Distance(transform.position, startingPosition) >= patrolRange)
        {
            Flip();
        }

        // Запуск анимации патрулирования
        animator.SetBool("isMoving", true);
    }

    void ChasePlayer()
    {
        isChasingPlayer = true;
        // Движение в сторону игрока
        Vector2 direction = (player.position - transform.position).normalized;
        body.velocity = new Vector2(direction.x * chaseSpeed, body.velocity.y);

        // Запуск анимации преследования
        animator.SetBool("isMoving", true);

        // Поворот в сторону игрока
        if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
        {
            Flip();
        }
    }

     void StopChasingPlayer()
    {
        isChasingPlayer = false;
        body.velocity = Vector2.zero;

        // Возвращение к патрулированию
        transform.position = Vector2.MoveTowards(transform.position, startingPosition, moveSpeed * Time.deltaTime);

        // Запуск анимации возвращения
        animator.SetBool("isMoving", true);

        // Поворот в сторону начальной позиции
        if ((startingPosition.x > transform.position.x && !isFacingRight) || (startingPosition.x < transform.position.x && isFacingRight))
        {
            Flip();
        }
    }

    bool PlayerInAggroRange()
    {
        // Проверка, находится ли игрок в зоне агрессии
        return Vector2.Distance(transform.position, player.position) <= aggroRange;
    }

    bool PlayerOutOfChaseRange()
    {
        // Проверка, находится ли игрок за пределами зоны преследования
        return Vector2.Distance(transform.position, player.position) > exitChaseRange;
    }

     IEnumerator Teleport()
    {
        isTeleporting = true;

        // Случайное время до телепортации
        yield return new WaitForSeconds(Random.Range(3f, teleportCooldown));

        // Телепортируемся на случайное расстояние
        Vector3 teleportTarget = transform.position + new Vector3(teleportDistance * (isFacingRight ? 1 : -1), 0, 0);
        transform.position = teleportTarget;

        // Запуск анимации телепортации
        animator.SetTrigger("Teleport");

        // Изменение направления после телепортации
        Flip();

        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }

    void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
