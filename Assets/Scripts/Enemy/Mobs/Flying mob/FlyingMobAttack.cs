using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 public class FlyingMobAttack : DefaultAttack
 {

    public float attackDashSpeed = 15f;
    public float attackDashDuration = 0.5f;
    public float stunDuration = 1f;
    public float hitRadius = 1f;
    private FlyingMobMovement movementScript;
    private RaycastHit2D[] hits;

    private bool isAttacking = false;
    private List<Vector3> pathToAttackAgain;
    private int indexToFollow = 0;
 
    void Awake()
    {
        Start();
        movementScript = GetComponent<FlyingMobMovement>();
    }

    void Update()
    {

        if (counterAfterBug >= 0.6f)
        {
        counterAfterBug = 0;
        body.gravityScale = 9.8f;
        }

        attackTimeCounter += Time.deltaTime;
        if (PlayerInAggroRange())
        {
            if (PlayerInAttackRange() && attackTimeCounter >= attackCooldown)
            {
                print("attack");
                animator.SetTrigger("Attack");
                StartCoroutine(Attack());
                TryGetDistanceFromPLayer();
            } 
            if (PlayerInAttackRange() && !isAttacking) {
                FollowPath();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        //  Vector3 from = new Vector3(transform.position.x - aggroRange, transform.position.y, transform.position.z);
        //  Vector3 to = new Vector3(transform.position.x + aggroRange, transform.position.y, transform.position.z);
        //  Gizmos.DrawLine(from, to);

        if (player != null)
        {
            Vector3 from = transform.position;
            Vector3 to = player.position;
            Vector3 direction = (to - from).normalized;
            float distance = Vector3.Distance(from, to);

            // Ограничение длины
            float clampedDistance = Mathf.Min(distance, attackRange);
            Vector3 endPoint = from + direction * clampedDistance;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(from, endPoint);
        }
    }
 
     private void endOfAttack()
     {
         shouldBeDamaging = false;
     }

    protected override IEnumerator Attack()
    {
        if (isAttacking) yield break; // уже атакует
        isAttacking = true;

        // 1. Запоминаем позицию игрока
        Vector2 targetPosition = player.position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        float elapsed = 0f;
        body.velocity = direction * attackDashSpeed;

        while (elapsed < attackDashDuration)
        {
            elapsed += Time.deltaTime;

            // 2. Проверяем столкновение с игроком
            Collider2D playerHit = Physics2D.OverlapCircle(transform.position, hitRadius, attackableLayer);
            if (playerHit)
            {
                TryDealDamage(playerHit.gameObject);
                break; // прерываем рывок
            }

            // 3. Проверяем столкновение с препятствием
            Collider2D obstacleHit = Physics2D.OverlapCircle(transform.position, hitRadius, groundLayer);
            if (obstacleHit)
            {
                Debug.Log("Stunned after hitting obstacle");
                body.velocity = Vector2.zero;
                yield return new WaitForSeconds(stunDuration);
                isAttacking = false;
                yield break;
            }

            yield return null;
        }

        // 4. Завершаем атаку
        body.velocity = Vector2.zero;
        isAttacking = false;
    }

    private void TryDealDamage(GameObject target)
    {
        Debug.Log("Hit player! Dealing damage.");
        target.GetComponent<Health>().TakeDamage(damage);
    }

    private void TryGetDistanceFromPLayer() {
        isAttacking = false;
        float angle = UnityEngine.Random.Range(0f, Mathf.PI);
        float r = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f)) * aggroRange;
        float x = Mathf.Cos(angle) * r;
        float y = Mathf.Sin(angle) * r;

        Vector3 newPointToAttack = new(x, y);
        indexToFollow = 0;
        pathToAttackAgain = Pathfinder.instance.FindPath(body.position, newPointToAttack);
    }

    private void FollowPath() {
        if (Vector3.Distance(body.position, pathToFollow[indexToFollow]) < 0.06f) {
            indexToFollow++;
        }

        MoveTowards(pathToAttackAgain[indexToFollow], chaseSpeed);
    }

    
    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        body.velocity = direction * speed;
        animator.SetFloat("Speed", speed);
    }

    public override bool PlayerInAggroRange()
    {
        var hits = Physics2D.CircleCast(transform.position, aggroRange, transform.right, 0f, attackableLayer);
        return hits.collider != null;
    }

    public override void ChaseToAttack()
    {
        throw new NotImplementedException();
    }
}