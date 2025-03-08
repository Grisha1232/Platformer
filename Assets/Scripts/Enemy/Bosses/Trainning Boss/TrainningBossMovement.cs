using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class TrainningBossMovement : MonoBehaviour
{
    public List<GameObject> platforms; // Массив платформ, на которые может прыгать босс
    public Rigidbody2D platformRigid;
    private int currentPlatform = -1;
    public int groundPoundDamage = 10; // Урон от удара по земле
    public Transform attackTransform;
    public GameObject projectilePrefab; // Префаб стрелы
    public GroundProjectiles leftProj;
    public GroundProjectiles righttProj;
    public float projectileSpeed = 30f; // Скорость стрелы
    public float attackCooldown = 2f; // Время между атаками
    public float meleeRange = 1.5f; // Дистанция для ближней атаки
    public int meleeDamage = 5; // Урон от ближней атаки

    public LayerMask enemyLayer;
    public LayerMask platformLayer;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    private float extraHeight = 0.25f;

    private Transform player; // Ссылка на игрока
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private float lastAttackTime;
    private bool isJumpingAttack = false;
    [HideInInspector] public static bool isLocked = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackCooldown; // Чтобы босс мог атаковать сразу
    }

    void Update()
    {
        if (isLocked) {
            Debug.Log("locked");
            return;
        }
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            DecideAttack();
            lastAttackTime = Time.time;
        }
        TurnToPlayer();
    }

    private void DecideAttack() {
        var random = new System.Random();
        switch (random.Next(0, 2)) {
            case 0:
                Debug.Log("melee attack");
                currentPlatform = random.Next(platforms.Count);
                JumpToPlatform();
                break;
            case 1:
                Debug.Log("melee attack");
                MeleeAttack();
                break;
            case 2:
                Debug.Log("Ground pound");
                GroundPound();
                break;
            case 3:
                Debug.Log("3");
                break;
            case 4:
                Debug.Log("4");
                break;
            default:
                break;
            
        }
    }

    #region Movement methods

    private void JumpToPlatform()
    {
        if (!isGrounded()) {
            return;
        }

        Vector2 startPos = transform.position;
        Vector2 endPos = platforms[currentPlatform].transform.position;
        Vector2 midPos = new Vector2( (startPos.x + endPos.x) / 2f, (startPos.y + endPos.y) / 2f + Mathf.Abs(startPos.y - endPos.y) + 11f );
        Vector2 point1 = startPos;
        Vector2 point2 = endPos;
        Vector2 point3 = midPos;
        
        float x1 = startPos.x, y1 = startPos.y;
        float x2 = endPos.x, y2 = endPos.y;
        float x3 = midPos.x, y3 = midPos.y;

       // Решаем систему уравнений для a, b, c
        float[,] matrix = {
            { x1 * x1, x1, 1 },
            { x2 * x2, x2, 1 },
            { x3 * x3, x3, 1 }
        };

        float[] rhs = { y1, y2, y3 };

        if (!SolveSystem(matrix, rhs, out float a, out float b, out float c))
        {
            Debug.LogError("Точки не лежат на одной параболе!");
            return;
        }
        
        isJumpingAttack = true;
        
        // Находим вершину параболы
        float h = -b / (2 * a);
        float k = c - (b * b) / (4 * a);

        // Рассчитываем время полета (предполагаем, что начальная и конечная точки — point1 и point3)
        
        float deltaX = point3.x - point1.x;
        float deltaY = point3.y - point1.y;

        float flightTime;
        Vector2 initialVelocity;

        CalculateFlightParameters(startPos, endPos, new Vector2(h, k), out flightTime, out initialVelocity);

        // // Время полета
        // float t = Mathf.Sqrt(2 * Mathf.Abs(deltaY) / g);

        // // Начальная скорость
        // float vx = deltaX / t;
        // float vy = Mathf.Abs(deltaY) / t + 0.5f * g * t;

        DisableCollisionWithPlatforms();
        // Применяем скорость к Rigidbody2D
        rb.velocity = initialVelocity;

        // Анимация прыжка
        // animator.SetTrigger("Jump");
    }

    private void MeleeAttack()
    {
        // Анимация ближней атаки
        // animator.SetTrigger("MeleeAttack");

        // TODO: Сделать анимацию на каждый фрейм анимации сделать метод который будет наносить урон если в радиусе атаки оказался игрок
        
    }

    private void GroundPound() {
        isJumpingAttack = false;

        // Анимация удара по земле
        // animator.SetTrigger("GroundPound");

        EnableFroundProjectiles();
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

        // Анимация стрельбы
        // animator.SetTrigger("Shoot");
    }

    private void TurnToPlayer() {
        var direction = -Mathf.Sign(transform.position.x - player.position.x);
        if ( direction != Mathf.Sign(transform.localScale.x) ) {
            Flip();
        }
    }

    #endregion

    #region help methods
    
    private void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private bool SolveSystem(float[,] matrix, float[] rhs, out float a, out float b, out float c)
    {
        a = b = c = 0;

        // Определитель матрицы
        float det = matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1])
                  - matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
                  + matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);

        if (Mathf.Abs(det) < 0.0001f)
        {
            return false; // Система не имеет решения
        }

        // Находим a, b, c по правилу Крамера
        float detA = rhs[0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1])
                   - matrix[0, 1] * (rhs[1] * matrix[2, 2] - matrix[1, 2] * rhs[2])
                   + matrix[0, 2] * (rhs[1] * matrix[2, 1] - matrix[1, 1] * rhs[2]);

        float detB = matrix[0, 0] * (rhs[1] * matrix[2, 2] - matrix[1, 2] * rhs[2])
                   - rhs[0] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
                   + matrix[0, 2] * (matrix[1, 0] * rhs[2] - rhs[1] * matrix[2, 0]);

        float detC = matrix[0, 0] * (matrix[1, 1] * rhs[2] - rhs[1] * matrix[2, 1])
                   - matrix[0, 1] * (matrix[1, 0] * rhs[2] - rhs[1] * matrix[2, 0])
                   + rhs[0] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);

        a = detA / det;
        b = detB / det;
        c = detC / det;

        return true;
    }

    private void CalculateFlightParameters(Vector2 startPoint, Vector2 endPoint, Vector2 dropPoint, out float flightTime, out Vector2 initialVelocity)
    {
        float g = Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale;

        // Время подъема до точки, где тело начало падать
        float t_d = MathF.Sqrt(2 * (dropPoint.y - startPoint.y) / g);

        // Общее время полета (время подъема + время падения)
        flightTime = 2 * t_d;

        // Горизонтальная составляющая начальной скорости
        float v0x = (endPoint.x - startPoint.x) / flightTime;

        // Вертикальная составляющая начальной скорости
        float v0y = g * t_d;

        // Начальная скорость (в виде Vector2)
        initialVelocity = new Vector2(v0x, v0y);
    }

    private void EnableFroundProjectiles() {
        leftProj.GetComponent<Rigidbody2D>().velocity = new Vector2(-20f, 0);
        leftProj.Attack();
        righttProj.GetComponent<Rigidbody2D>().velocity = new Vector2(20f, 0);
        righttProj.Attack();
    }

    private bool isGrounded() {
        RaycastHit2D groundHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, groundLayer | platformLayer);
        
        return groundHit.collider != null;
    }
    
    private void DisableCollisionWithPlatforms() {
        Physics2D.IgnoreCollision(rb.GetComponent<BoxCollider2D>(), platformRigid.GetComponent<CompositeCollider2D>(), true);
    }

    private void EnableCollisionWithPlatforms() {
        Physics2D.IgnoreCollision(rb.GetComponent<BoxCollider2D>(), platformRigid.GetComponent<CompositeCollider2D>(), false);
    }

     // when the GameObjects collider arrange for this GameObject to travel to the left of the screen
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col == platforms[currentPlatform].GetComponent<BoxCollider2D>()) {
            if (isJumpingAttack) {
                StartCoroutine(CoroutineBeforeJumpAttack());
            }
            EnableCollisionWithPlatforms();
        }
    }

    private IEnumerator CoroutineBeforeJumpAttack() {
        yield return new WaitUntil( () => isGrounded() && rb.velocity.y == 0f );
        rb.sleepMode = RigidbodySleepMode2D.StartAsleep;
        GroundPound();
    }

    #endregion

    #region Draw methods

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        foreach (var platform in platforms) {
            Gizmos.DrawWireSphere(platform.transform.position, 0.5f);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackTransform.position, meleeRange);
    }

    #endregion

}
