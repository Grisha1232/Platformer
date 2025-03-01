using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainningBossMovement : MonoBehaviour
{
    public Transform[] platforms; // Массив платформ, на которые может прыгать босс
    public float jumpForce = 10f; // Сила прыжка
    public float groundPoundRadius = 4f; // Радиус урона при ударе по земле
    public int groundPoundDamage = 10; // Урон от удара по земле
    public GameObject projectilePrefab; // Префаб стрелы
    public float projectileSpeed = 10f; // Скорость стрелы
    public float attackCooldown = 2f; // Время между атаками
    public float meleeRange = 1.5f; // Дистанция для ближней атаки
    public int meleeDamage = 5; // Урон от ближней атаки

    private Transform player; // Ссылка на игрока
    private Rigidbody2D rb;
    private Animator animator;
    private float lastAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackCooldown; // Чтобы босс мог атаковать сразу
    }

    void Update()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            DecideAttack();
            lastAttackTime = Time.time;
        }
    }

    void DecideAttack()
    {
        int attackType = Random.Range(0, 4); // Выбираем случайную атаку

        switch (attackType)
        {
            case 0:
                JumpToPlatform();
                break;
            case 1:
                ShootProjectile();
                break;
            case 2:
                GroundPound();
                break;
            case 3:
                MeleeAttack();
                break;
        }
    }

    void JumpToPlatform()
    {
        Transform targetPlatform = platforms[Random.Range(0, platforms.Length)];
        Vector2 direction = (targetPlatform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * jumpForce, jumpForce);

        // Анимация прыжка
        // animator.SetTrigger("Jump");
    }

    void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

        // Анимация стрельбы
        // animator.SetTrigger("Shoot");
    }

    void GroundPound()
    {
        // Анимация удара по земле
        // animator.SetTrigger("GroundPound");

        // Определяем область под боссом
        Vector2 raycastOrigin = transform.position; // Начальная точка для Raycast
        float raycastDistance = groundPoundRadius; // Дистанция для проверки
        Vector2 directionL = Vector2.left; // Направление влево
        Vector2 directionR = Vector2.right; // Направление вправо

        // Используем Raycast для проверки попадания
        // RaycastHit2D[] hits = Physics2D.RaycastAll(raycastOrigin, direction, raycastDistance);

        // // Перебираем все объекты, которые попали под луч
        // foreach (RaycastHit2D hit in hits)
        // {
        //     if (hit.collider.CompareTag("Player"))
        //     {
        //         // Наносим урон игроку
        //         hit.collider.GetComponent<PlayerHealth>().TakeDamage(groundPoundDamage);
        //     }
        // }

        // Альтернативно, можно использовать BoxCast для более точной проверки области
        
        Vector2 boxSize = new Vector2(groundPoundRadius * 2, 1f); // Размер области проверки
        // RaycastHit2D[] boxHits = Physics2D.BoxCastAll(raycastOrigin, boxSize, 0f, direction, raycastDistance);

        // foreach (RaycastHit2D hit in boxHits)
        // {
        //     if (hit.collider.CompareTag("Player"))
        //     {
        //         hit.collider.GetComponent<PlayerHealth>().TakeDamage(groundPoundDamage);
        //     }
        // }
    }

    void MeleeAttack()
    {
        // Анимация ближней атаки
        // animator.SetTrigger("MeleeAttack");

        // Проверяем, находится ли игрок в радиусе атаки
        if (Vector2.Distance(transform.position, player.position) <= meleeRange)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(meleeDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Отображаем луч для удара по земле
        Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundPoundRadius);

        // Если используете BoxCast, отобразите область
       
        Vector2 boxSize = new Vector2(groundPoundRadius * 2, 1f);
        Gizmos.DrawWireCube(transform.position + Vector3.down, boxSize);
        
    }
}
