using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class FlyingBossMovement : DefaultBoss {

    [Header("Points and Attack Settings")]
    public Transform[] wallPoints; // точки на стенах (слева, справа)
    public Transform[] verticalPoints; // точки на потолке и полу
    public Transform attackTransform;
    public float dashSpeed = 20f;
    public float flySpeed = 8f;
    public float shootCooldown = 3f;
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float meleeAttackSpeed = 6f;
    public float meleeRange = 1.5f;

    [Header("Damage Settings")]
    public int dashDamage = 20;
    public int flyDamage = 10;
    public float knockbackForce = 5f;
    private float damageCooldown = 0.5f;
    private float lastDamageTime = 0f;
    public float damageRadius = 4f; // расстояние на котором босс наносит урон
    private bool hasDealtFlyDamage = false;
    private bool hasDealtDashDamage = false;

    private Transform currentWallTarget;
    private bool isDashing = false;
    private bool isFlyingToWall = false;
    private float shootCooldownTimer;

    enum BossState {
        Idle,
        MovingToWall,
        Dashing,
        Shooting,
        MeleeAttacking
    }

    private BossState state = BossState.Idle;

    new void Start() {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootCooldownTimer = shootCooldown;
    }

    void Update() {
        if (isLocked) {
            return;
        } else {
            GetComponent<FlyingBossHealth>().canTakeDamage = true;
        }

        switch (state) {
            case BossState.Idle:
                ChooseNextAction();
                break;
            case BossState.MovingToWall:
                MoveToWall();
                break;
            case BossState.Shooting:
                HandleShooting();
                break;
            case BossState.MeleeAttacking:
                HandleMeleeAttack();
                break;
        }

        shootCooldownTimer -= Time.deltaTime;
    }

    void ChooseNextAction() {
        int randomAction = UnityEngine.Random.Range(0, 3);
        switch(randomAction) {
            case 0:
                state = BossState.MovingToWall;
                ChooseWallPointBasedOnPlayer();
                break;
            case 1:
                // state = BossState.Shooting;
                // HandleShooting();
                break;
            case 2:
                state = BossState.MeleeAttacking;
                HandleMeleeAttack();
                break;
        }
    }

    void ChooseWallPointBasedOnPlayer() {
        bool chooseSideWalls = UnityEngine.Random.value > 0.5f;

        if (chooseSideWalls) {
            var temp = Array.FindAll(wallPoints, p => Mathf.Abs(p.position.y - player.transform.position.y) < 2f);
            if (temp.Length != 0) {
                currentWallTarget = temp[UnityEngine.Random.Range(0, temp.Length)];
            } else {
                Debug.Log("random wall");
                currentWallTarget = wallPoints[UnityEngine.Random.Range(0, wallPoints.Length)];
            }
        } else {
            var temp = Array.FindAll(verticalPoints, p => Mathf.Abs(p.position.x - player.transform.position.x) < 5f);
            if (temp.Length != 0) {
                currentWallTarget = temp[UnityEngine.Random.Range(0, temp.Length)];
            } else {
                Debug.Log("random vertical");
                currentWallTarget = verticalPoints[UnityEngine.Random.Range(0, verticalPoints.Length)];
            }
        }

        isFlyingToWall = true;
    }

    void MoveToWall() {
        Vector2 targetPos = currentWallTarget.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, flySpeed * Time.deltaTime);
        isFlyingToWall = true;

        if (!hasDealtFlyDamage) {
            CheckPlayerCollision(flyDamage, knockbackForce * 0.5f);
        }

        if (Vector2.Distance(transform.position, targetPos) < 0.1f) {
            isFlyingToWall = false;
            hasDealtFlyDamage = false; // Сброс для следующего раза
            StartCoroutine(DashToOppositeWall());
        }
    }

    IEnumerator DashToOppositeWall() {
        state = BossState.Dashing;
        isDashing = true;
        hasDealtDashDamage = false; 

        Transform dashTarget = GetOppositePoint(currentWallTarget);

        Vector2 direction = (dashTarget.position - transform.position).normalized;
        animator.SetTrigger("Dash");

        while (true) {
            float step = dashSpeed * Time.fixedDeltaTime;
            float distance = Vector2.Distance(transform.position, dashTarget.position);
            if (step >= distance) {
                rb.MovePosition(dashTarget.position);
                break;
            }

            rb.MovePosition(rb.position + direction * step);
            if (!hasDealtDashDamage) {
                CheckPlayerCollision(dashDamage, knockbackForce); 
            }

            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
        isFlyingToWall = false;

        yield return new WaitForSeconds(attackCooldown);
        state = BossState.Idle;
    }

    Transform GetOppositePoint(Transform point) {
        if (Array.Exists(wallPoints, p => p == point)) {
            return Array.Find(wallPoints, p => p.position.x != point.position.x && p.position.y == point.position.y);
        } else {
            return Array.Find(verticalPoints, p => p.position.y != point.position.y && p.position.x == point.position.x);
        }
    }

    void HandleShooting() {
        if (shootCooldownTimer <= 0f) {
            animator.SetTrigger("Shoot");
            Vector2 direction = (player.position - shootPoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            shootPoint.rotation = Quaternion.Euler(0, 0, angle);

            Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
            shootCooldownTimer = shootCooldown;
            StartCoroutine(ReturnToIdleAfterDelay(attackCooldown));
        }
    }

    void HandleMeleeAttack() {
        StartCoroutine(ChaseAndMelee());
    }

    IEnumerator ChaseAndMelee() {
        animator.SetTrigger("MeleeChase");

        while(Vector2.Distance(transform.position, player.position) > meleeRange) {
            Vector2 target = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, target, flySpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetTrigger("MeleeAttack");
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackTransform.position, meleeRange, playerLayer);
        if (hitPlayer) {
            Debug.Log("Player hit by melee!");
        }

        yield return new WaitForSeconds(attackCooldown);
        state = BossState.Idle;
    }

    IEnumerator ReturnToIdleAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        state = BossState.Idle;
    }

    void CheckPlayerCollision(int damage, float knockback) {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= damageRadius) {
            Vector2 knockbackDirection = (player.position - transform.position).normalized;
            player.GetComponent<PlayerHealth>().TakeDamage(damage);
            
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb) {
                Debug.Log("KnockBack player");
                playerRb.AddForce(knockbackDirection * knockback, ForceMode2D.Impulse);
            }
            
            if (isDashing) hasDealtDashDamage = true;
            if (isFlyingToWall) hasDealtFlyDamage = true;     
        }
    }

    void DealDamageToPlayer(GameObject playerObj, int damage) {
        // Предположим у игрока есть метод TakeDamage(int)
        playerObj.GetComponent<PlayerHealth>()?.TakeDamage(damage);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackTransform.position, meleeRange);
    }

    public override void ShowCheckpoint() {
        if (checkpoint) checkpoint.SetActive(true);
    }
}
