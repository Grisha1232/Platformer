using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class ShadowCatMovement : DefaultMovement
{
    /// <summary>
    /// Маска земли
    /// </summary>
    [SerializeField] private LayerMask groundLayer;
    /// <summary>
    /// Скорость передвижения в патрулированиии
    /// </summary>
    [SerializeField] private float aggroRange;
    [SerializeField] private float exitChaseRange;

    [SerializeField] private float teleportDistance;
    [SerializeField] private float teleportCooldown;

    [SerializeField] private GameObject indicatorShadow;
    [SerializeField] private GameObject indicatorAttack;
    [SerializeField] private GameObject indicatorPatrol;
    [SerializeField] private GameObject indicatorChase;

    /// <summary>
    /// Находится ли Теневой кот в тени
    /// </summary>
    private bool isInShadow = false;

    /// <summary>
    /// Повернут ли Теневой кот вправо
    /// </summary>
    private bool isFacingRight = true;

    /// <summary>
    /// This methods calls every time game is start
    /// </summary>
    private void Awake() {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    
    protected override void Update()
    {
        // Проверка на погоню
        if ( PlayerInAggroRange() && !isChasing ) {
            isChasing = true; // Начинаем преследование
        }
        else if ( PlayerOutOfChaseRange() ) {
            isChasing = false; // Возвращаемся к патрулированию
        }

        print(isChasing);

        if ( isChasing ) {
            ChasePlayer(); // Преследование игрока
        }
        else {
            Patrol(); // Патрулирование
        }
    }

     void StopChasingPlayer()
    {
        print("stop chase");
        isChasing = false;
        body.velocity = Vector2.zero;

        // Возвращение к патрулированию
        transform.position = Vector2.MoveTowards(transform.position, initialPosition, patrolSpeed * Time.deltaTime);

        // Запуск анимации возвращения
        // animator.SetBool("isMoving", true);

        // Поворот в сторону начальной позиции
        if ((initialPosition.x > transform.position.x && !isFacingRight) || (initialPosition.x < transform.position.x && isFacingRight))
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
        return Vector2.Distance(transform.position, initialPosition) > exitChaseRange;
    }

    void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected override void Patrol()
    {
        {
            indicatorChase.SetActive(isChasing);
            indicatorPatrol.SetActive(true);
            indicatorAttack.SetActive(false);
            indicatorShadow.SetActive(isInShadow);
        }
        // Поворот в пределах патрулирования
        if (Math.Abs(transform.position.x - initialPosition.x ) >= patrolRange && Math.Sign(transform.localScale.x) != Math.Sign(initialPosition.x - transform.position.x))
        {
            Flip();
        }
        // Движение в пределах патрулирования
        float moveDirection = isFacingRight ? 1 : -1;
        body.velocity = new Vector2(moveDirection * patrolSpeed, body.velocity.y);
        
        // Запуск анимации патрулирования
        // animator.SetBool("isMoving", true);
    }

    protected override void ChasePlayer()
    {
        
        {
            indicatorChase.SetActive(isChasing);
            indicatorPatrol.SetActive(false);
            indicatorAttack.SetActive(false);
            indicatorShadow.SetActive(isInShadow);
        }

        if (PlayerOutOfChaseRange()) {
            StopChasingPlayer();
            return;
        }
        isChasing = true;
        // Движение в сторону игрока
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Поворот в сторону игрока
        if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
        {
            Flip();
        }
        body.velocity = new Vector2(direction.x * chaseSpeed, body.velocity.y);

        // Запуск анимации преследования
        // animator.SetBool("isMoving", true);

    }

}
