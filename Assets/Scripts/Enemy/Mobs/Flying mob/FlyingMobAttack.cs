using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 public class FlyingMobAttack : DefaultAttack
 {
     private FlyingMobMovement movementScript;
     private RaycastHit2D[] hits;
 
 
     private bool isJumping = false;
     private bool jumpingPhase1 = false;
     private bool jumpingPhase2 = false;
     [SerializeField] protected float jumpForce = 10f;
 
     private float counterAfterBug = 0;
     protected List<Vector3Int> pathToFollow;
     protected int currentPointToFollow;

     private Vector2 directionToPlayer;
     private Vector2 avoidanceDirection;
 
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
             FollowThePlayer();
             if (PlayerInAttackRange() && attackTimeCounter >= attackCooldown)
             {
                 print("attack");
                 animator.SetTrigger("Attack");
                 StartCoroutine(Attack());
             }
         }
     }
 
     protected void FollowThePlayer() {
         if (player == null)
        {
            Debug.LogWarning("Игрок не назначен!");
            return;
        }

        // Вычисляем направление к игроку
        directionToPlayer = (player.position - transform.position).normalized;

        // Проверяем, есть ли препятствие на пути к игроку
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, 2f, obstacleLayer);

        if (hit.collider != null)
        {
            // Если препятствие обнаружено, вычисляем направление для обхода
            avoidanceDirection = Vector2.Perpendicular(hit.normal).normalized;

            // Двигаемся в сторону обхода
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + avoidanceDirection, chaseSpeed * Time.deltaTime);
        }
        else
        {
            // Если препятствий нет, двигаемся прямо к игроку
            transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        }
     }
 
 
     protected override IEnumerator Attack()
     {
         attackTimeCounter = 0;
         List<PlayerHealth> listDamaged = new();
         shouldBeDamaging = true;
         while (shouldBeDamaging)
         {
             hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);
 
             for (int i = 0; i < hits.Length; i++)
             {
                 PlayerHealth enemyHealth = hits[i].collider.gameObject.GetComponent<PlayerHealth>();
                 if (enemyHealth != null && !enemyHealth.HasTakenDamage)
                 {
                     enemyHealth.TakeDamage(damage);
                     listDamaged.Add(enemyHealth);
                 }
             }
 
             yield return null;
         }
 
         foreach (PlayerHealth enemyHealth in listDamaged)
         {
             enemyHealth.HasTakenDamage = false;
         }
     }
 
     protected override void ChaseToAttack()
     {
         if (!PlayerInAggroRange())
         {
             return;
         }
 
         // �������� � ������� ������
         Vector2 direction = (player.position - transform.position).normalized;
 
         animator.SetFloat("Speed", Math.Abs(direction.x));
         // ������� � ������� ������
         if ((direction.x > 0 && Math.Sign(transform.localScale.x) == -1) || (direction.x < 0 && Math.Sign(transform.localScale.x) == 1))
         {
             Flip();
         }
         body.velocity = new Vector2(direction.x * chaseSpeed, body.velocity.y);
 
         // ������ �������� �������������
         // animator.SetBool("isMoving", true);
 
     }
 
 
     void Flip()
     {
         Vector3 scale = transform.localScale;
         scale.x *= -1;
         transform.localScale = scale;
     }
 
     private void OnDrawGizmosSelected()
     {
         Gizmos.DrawWireSphere(attackTransform.position, attackRange);
 
 
         Vector3 from = new Vector3(transform.position.x - aggroRange, transform.position.y, transform.position.z);
         Vector3 to = new Vector3(transform.position.x + aggroRange, transform.position.y, transform.position.z);
         Gizmos.DrawLine(from, to);

         // Визуализация луча для обнаружения препятствий
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)directionToPlayer * 2f);
        }
     }
 
     private void endOfAttack()
     {
         shouldBeDamaging = false;
     }
 }