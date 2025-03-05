using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainningBossMovement : MonoBehaviour
{
    public List<GameObject> platforms; // Массив платформ, на которые может прыгать босс
    public Rigidbody2D platformRigid;
    private int currentPlatform = -1;
    public float jumpForce = 10f; // Сила прыжка
    public float groundPoundRadius = 4f; // Радиус урона при ударе по земле
    public int groundPoundDamage = 10; // Урон от удара по земле
    public GameObject projectilePrefab; // Префаб стрелы
    public float projectileSpeed = 10f; // Скорость стрелы
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
    private LineRenderer lineRenderer;
    public int resolution = 20; // Количество точек для отрисовки

    private Vector2 start;
    private Vector2 mid;
    private Vector2 end;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        // player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackCooldown; // Чтобы босс мог атаковать сразу
        
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
    }

    void Update()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            DecideAttack();
            lastAttackTime = Time.time;
        }
    }

    private void DecideAttack() {
        var random = new System.Random();
        currentPlatform = random.Next(platforms.Count);
        JumpToPlatform();
    }

    #region Movement methods

    void JumpToPlatform()
    {
        if (!isGrounded()) {
            return;
        }
        
        currentPlatform++;
        currentPlatform %= platforms.Count();

        Vector2 startPos = transform.position;
        Vector2 endPos = platforms[currentPlatform].transform.position;
        Vector2 midPos = new Vector2( (startPos.x + endPos.x) / 2f, (startPos.y + endPos.y) / 2f + Mathf.Abs(startPos.y - endPos.y) + 11f );
        Vector2 point1 = startPos;
        Vector2 point2 = endPos;
        Vector2 point3 = midPos;

        start = startPos;
        mid = midPos;
        end = endPos;
        
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
        
        // Находим вершину параболы
        float h = -b / (2 * a);
        float k = c - (b * b) / (4 * a);
        mid = new Vector2(h, k);

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

    void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

        // Анимация стрельбы
        // animator.SetTrigger("Shoot");
    }

    #endregion

    #region help methods
    // Метод для решения системы линейных уравнений
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
            // Debug.Log(col + "\n" + platforms[currentPlatform].GetComponent<BoxCollider2D>());
            EnableCollisionWithPlatforms();
        }
    }

    #endregion

    #region Draw methods

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        foreach (var platform in platforms) {
            Gizmos.DrawWireSphere(platform.transform.position, 0.5f);
        }

        Gizmos.color = Color.red;
        DrawArrow(transform.position, platforms[currentPlatform].transform.position);

        Gizmos.DrawWireSphere(start, 1f);
        Gizmos.DrawWireSphere(mid, 1f);
        Gizmos.DrawWireSphere(end, 1f);

        // Vector2 boxSize = new Vector2(groundPoundRadius * 2, 1f);
        // Gizmos.DrawWireCube(transform.position + Vector3.down, boxSize);
        
    }

    private void DrawArrow(Vector3 start, Vector3 end, float arrowHeadLength = 2f, float arrowHeadAngle = 90f)
    {
        // Рисуем основную линию стрелки
        Gizmos.DrawLine(start, end);

        // Вычисляем направление стрелки
        Vector3 direction = (end - start).normalized;

        // Рисуем правую часть наконечника стрелки
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Gizmos.DrawLine(end, end + right * arrowHeadLength);

        // Рисуем левую часть наконечника стрелки
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
        Gizmos.DrawLine(end, end + left * arrowHeadLength);
    }

    void DrawParabolaCurve()
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = platforms[currentPlatform].transform.position;
        Vector2 midPos = new Vector2( (startPos.x + endPos.x) / 2f, (startPos.y + endPos.y) / 2f + Mathf.Abs(startPos.y - endPos.y) + 10f );
        Vector2 point1 = startPos;
        Vector2 point2 = endPos;
        Vector2 point3 = midPos;

        // Получаем координаты точек
        float x1 = point1.x, y1 = point1.y;
        float x2 = point2.x, y2 = point2.y;
        float x3 = point3.x, y3 = point3.y;

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

        // Отрисовка параболы
        float minX = Mathf.Min(x1, x2, x3);
        float maxX = Mathf.Max(x1, x2, x3);

        for (int i = 0; i < resolution; i++)
        {
            float x = minX + (maxX - minX) * i / (resolution - 1);
            float y = a * x * x + b * x + c;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    #endregion

}
