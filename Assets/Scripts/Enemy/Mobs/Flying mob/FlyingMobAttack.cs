using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 public class FlyingMobAttack : DefaultAttack
 {

    public float attackDashSpeed = 20f;
    public float attackDashDuration = 0.5f;
    public float stunDuration = 1f;
    public float hitRadius = 0.25f;
    private FlyingMobMovement movementScript;

 
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
        if (PlayerInAttackRange() && attackTimeCounter >= attackCooldown)
        {
            // animator.SetTrigger("Attack");
            print("Attack");
            StartCoroutine(Attack());
        }
    }
 
     private void endOfAttack()
     {
         shouldBeDamaging = false;
     }

    protected override IEnumerator Attack()
    {
        attackTimeCounter = 0;

        if (Mathf.Sign(player.transform.position.x - transform.position.x) != Mathf.Sign(transform.localScale.x)) {
            Flip();
        }
        
        Vector2 tempPosition = body.position;

        // 1. Запоминаем позицию игрока
        Vector2 targetPosition = player.position;
        Vector3 rushDirection = (targetPosition - (Vector2)transform.position).normalized;

        float rushTime = Vector3.Distance(player.transform.position, body.position) / attackDashSpeed;

        // Рывок в сторону игрока
        float elapsedTime = 0f;
        while (elapsedTime < rushTime) {
            body.velocity = rushDirection * attackDashSpeed;
            elapsedTime += Time.deltaTime;
            if (!DealDamage()) {
                body.gravityScale = 9.8f;
                yield return new WaitForSeconds(stunDuration);
                body.gravityScale = 0;

                player.gameObject.GetComponent<PlayerHealth>().HasTakenDamage = false;
                MoveTowards(tempPosition, chaseSpeed);
                yield break;
            }
            yield return null;
        }

        player.gameObject.GetComponent<PlayerHealth>().HasTakenDamage = false;

        // Останавливаем рывок
        body.velocity = Vector2.zero;

        MoveTowards(tempPosition, chaseSpeed);
    }

    private bool DealDamage()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(transform.position, hitRadius, attackableLayer);
        if (playerHit)
        {
            Debug.Log("Hit player! Dealing damage.");
            if (!playerHit.gameObject.GetComponent<Health>().HasTakenDamage) {
                playerHit.gameObject.GetComponent<Health>().TakeDamage(damage);
            }
        }

        Collider2D obstacleHit = Physics2D.OverlapCircle(transform.position, hitRadius, groundLayer);
        if (obstacleHit)
        {
            Debug.Log("Hit obstacle! Stunned.");
            return false;
        }
        return true;
    }
    
    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        body.velocity = direction * speed;
        animator.SetFloat("Speed", speed);
    }

    public override void ChaseToAttack()
    {
        throw new NotImplementedException();
    }

    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hitRadius);

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
}