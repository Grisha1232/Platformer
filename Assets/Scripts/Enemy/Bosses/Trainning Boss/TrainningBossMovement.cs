using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainningBossMovement : DefaultBoss
{
    public List<GameObject> platforms; // Массив платформ, на которые может прыгать босс

    public Rigidbody2D platformRigid;

    private int currentPlatform = -1;

    public int groundPoundDamage = 10; // Урон от удара по земле

    public Transform attackTransform;

    public GroundProjectiles leftProj;
    public GroundProjectiles righttProj;

    public GameObject[] projectiles;
    public float projectileSpeed = 30f; // Скорость стрелы
    public float timeBetweenShots = 0.3f;
    public float projectileLifeDistance = 30f;

    public float meleeRange = 1.5f; // Дистанция для ближней атаки
    public int meleeDamage = 5; // Урон от ближней атаки

    private float extraHeight = 0.25f;

    private bool isJumpingAttack = false;

    private float timePlayerAbove;
    private bool isMovingToPlayer = false;

    private new void Start() {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        attackCooldownCounter = attackCooldown; // Чтобы босс мог атаковать сразу
    }

    void Update() {
        if (isLocked) {
            Debug.Log("locked");
            return;
        }

        TurnToPlayer();
        // Если босс движется к игроку, обновляем его позицию
        if (isMovingToPlayer)
        {
            MoveTowardsPlayer();
            return;
        }

        if (attackCooldownCounter >= attackCooldown)
        {
            DecideAttack();
            attackCooldownCounter = 0;
        }
        attackCooldownCounter += Time.deltaTime;
    }

    private void DecideAttack() {
        // Получаем позиции босса и игрока
        Vector3 bossPosition = transform.position;
        Vector3 playerPosition = player.transform.position;

        // Вычисляем расстояние до игрока
        float distanceToPlayer = Vector3.Distance(bossPosition, playerPosition);

        // 1. Атака в ближнем бою
        if (distanceToPlayer <= 10f && playerPosition.y <= bossPosition.y + 1f) {
            StartMeleeAttack();
            return; // Прерываем выполнение, так как атака выбрана
        }

        // 2. Атака в дальнем бою
        if (distanceToPlayer >= 10f && distanceToPlayer <= 25f) {
            ShootProjectile();
            return; // Прерываем выполнение, так как атака выбрана
        }

        // 3. Удар по земле
        if (Mathf.Abs(bossPosition.y - playerPosition.y) <= 1f) {
            GroundPound();
            return; // Прерываем выполнение, так как атака выбрана
        }


        // 4. Прыжок на платформу
        var checkPlatform = GetClosestPlatformOnSameLevel();
        if (checkPlatform != -1) {
            JumpToPlatform(checkPlatform);
            return; // Прерываем выполнение, так как атака выбрана
        }

        // Если ни одна атака не подошла, босс не атакует
        Debug.Log("Босс не нашел подходящей атаки. Рандом в деле");
        System.Random random = new();
        switch (random.Next(0, 4)) {
            case 0: {
                StartMeleeAttack();
                break;
            }
            case 1: {
                ShootProjectile();
                break;
            }
            case 2: {
                GroundPound();
                break;
            }
            case 3: {
                JumpToPlatform(random.Next(platforms.Count()));
                break;
            }
            default:
                break;
        }
    }

    #region Movement methods

    private void JumpToPlatform(int choosenPlatform) {
        if (!isGrounded()) {
            return;
        }
        currentPlatform = choosenPlatform;

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

    // Метод для начала ближней атаки
    private void StartMeleeAttack()
    {
        Debug.Log("Босс начинает ближнюю атаку.");
        isMovingToPlayer = true; // Начинаем движение к игроку
        timePlayerAbove = 0f; // Сбрасываем таймер
    }
    
    private void MoveTowardsPlayer() {
        Vector3 bossPosition = transform.position;
        Vector3 playerPosition = player.transform.position;

        // Если игрок ниже босса, спускаемся вниз
        if (playerPosition.y < bossPosition.y - 1f)
        {
            DisableCollisionWithPlatforms();
            StartCoroutine(DropDown());
            return;
        }

        // Двигаемся к игроку по оси X
        Vector3 direction = (playerPosition - bossPosition).normalized;
        direction.y = 0; // Игнорируем ось Y
        rb.
        transform.position += moveSpeed * Time.deltaTime * direction;

        // Проверяем, находится ли игрок выше босса
        if (playerPosition.y > bossPosition.y + 1f) {
            timePlayerAbove += Time.deltaTime;

            // Если игрок выше в течение jumpToPlatformTime, переключаемся на прыжок на платформу
            if (timePlayerAbove >= 1f) {
                var checkPlatform = GetClosestPlatformOnSameLevel();
                if (checkPlatform != -1) {
                    JumpToPlatform(checkPlatform);
                    isMovingToPlayer = false; // Останавливаем движение
                }
            }
        }
        else {
            timePlayerAbove = 0f; // Сбрасываем таймер, если игрок не выше
        }

        // Если босс достиг игрока, выполняем ближнюю атаку
        if (Vector3.Distance(bossPosition, playerPosition) <= meleeRange) {
            MeleeAttack();
            isMovingToPlayer = false; // Останавливаем движение
        }
    }

    private void MeleeAttack() {
        // Анимация ближней атаки
        // animator.SetTrigger("MeleeAttack");

        // TODO: Сделать анимацию на каждый фрейм анимации сделать метод который будет наносить урон если в радиусе атаки оказался игрок
        var hits = Physics2D.CircleCastAll(attackTransform.position, meleeRange, transform.right, 0f, playerLayer);
        
        for (int i = 0; i < hits.Length; i++) {
            PlayerHealth enemyHealth = hits[i].collider.gameObject.GetComponent<PlayerHealth>();
            if (enemyHealth != null) {
                enemyHealth.TakeDamage( meleeDamage );
                enemyHealth.HasTakenDamage = false;
            }
        }
    }

    private void GroundPound() {
        isJumpingAttack = false;

        // Анимация удара по земле
        // animator.SetTrigger("GroundPound");

        EnableFroundProjectiles();
    }

    private void ShootProjectile() {
        if (projectiles.Count() >= 3 && player != null) {
            StartCoroutine(ShootArrowWithDelay(0)); // Первая стрела
            StartCoroutine(ShootArrowWithDelay(timeBetweenShots)); // Вторая стрела с задержкой
            StartCoroutine(ShootArrowWithDelay(timeBetweenShots * 2)); // Третья стрела с задержкой
        }
    }

    private void TurnToPlayer() {
        var direction = -Mathf.Sign(transform.position.x - player.position.x);
        if ( direction != Mathf.Sign(transform.localScale.x) ) {
            Flip();
        }
    }

    private IEnumerator DropDown() {
        yield return new WaitUntil( () => {
            var collider = Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size * 1.1f, 0f, layerMask: platformLayer);
            return collider == null;
        });

        EnableCollisionWithPlatforms();
    }

    #endregion

    #region help methods
    
    private void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private bool SolveSystem(float[,] matrix, float[] rhs, out float a, out float b, out float c) {
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

    private void CalculateFlightParameters(Vector2 startPoint, Vector2 endPoint, Vector2 dropPoint, out float flightTime, out Vector2 initialVelocity) {
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
    void OnTriggerEnter2D(Collider2D col) {
        if (currentPlatform != -1 && col == platforms[currentPlatform].GetComponent<BoxCollider2D>()) {
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

    private IEnumerator ShootArrowWithDelay(float delay) {
        yield return new WaitForSeconds(delay);

        // Анимация стрельбы
        // animator.SetTrigger("Shoot");

        // Находим первую неактивную стрелу в массиве
        GameObject arrow = System.Array.Find(projectiles, a => !a.activeInHierarchy);
        if (arrow != null)
        {
            // Активируем стрелу и задаем её позицию
            arrow.SetActive(true);
            arrow.transform.position = transform.position;

            // Направление стрельбы (в сторону игрока)
            Vector2 direction = (player.transform.position - transform.position).normalized;
            // Поворачиваем стрелу в сторону игрока
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Вычисляем угол
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle); // Применяем поворот

            // Запускаем стрелу
            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.velocity = direction * projectileSpeed;
            }

            // Уничтожаем стрелу после прохождения расстояния
            StartCoroutine(DestroyArrowAfterDistance(arrow));
        }
    }

    private IEnumerator DestroyArrowAfterDistance(GameObject arrow) {
        Vector2 startPosition = arrow.transform.position;

        while (Vector2.Distance(startPosition, arrow.transform.position) < projectileLifeDistance)
        {
            yield return null; // Ждем каждый кадр
        }

        // Деактивируем стрелу
        arrow.SetActive(false);
    }

    // Метод для поиска ближайшего объекта на том же уровне по Y
    private int GetClosestPlatformOnSameLevel(float yThreshold = 1f) {
        int closestPlatform = -1;
        float closestDistance = Mathf.Infinity; // Начальное значение расстояния
        Vector3 playerPosition = player.transform.position;

        for (int i = 0; i < platforms.Count(); i++) {
            // Проверяем, находится ли объект примерно на том же уровне по Y
            if (Mathf.Abs(platforms[i].transform.position.y - playerPosition.y) <= yThreshold)
            {
                // Вычисляем расстояние до объекта (по оси X или в 2D/3D пространстве)
                float distance = Vector3.Distance(playerPosition, platforms[i].transform.position);

                // Если объект ближе, обновляем ближайший объект
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlatform = i;
                }
            }
        }

        return closestPlatform;
    }

    #endregion

    #region Draw methods

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.black;
        foreach (var platform in platforms) {
            Gizmos.DrawWireSphere(platform.transform.position, 0.5f);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackTransform.position, meleeRange);
    }

    #endregion

}
