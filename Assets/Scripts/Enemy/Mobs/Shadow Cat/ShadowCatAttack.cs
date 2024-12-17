using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowCatAttack : DefaultAttack
{
    
    [SerializeField] private Transform attackTransform;
    [SerializeField] private LayerMask attackableLayer;
    private ShadowCatMovement movementScript;
    private RaycastHit2D[] hits;

    void Awake()
    {
        Start();
        movementScript = GetComponent<ShadowCatMovement>();
    }

    void Update()
    {
        attackTimeCounter += Time.deltaTime;
        if (PlayerInAggroRange()) {
            ChaseToAttack();
            if (PlayerInAttackRange() && attackTimeCounter >= attackCooldown) {
                animator.SetTrigger("Attack");
                StartCoroutine(Attack());
            }
        }
    }



    protected override IEnumerator Attack()  { 
        attackTimeCounter = 0;
        List<PlayerHealth> listDamaged = new();
        shouldBeDamaging = true; 
        while(shouldBeDamaging) {            
            hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);
            
            for (int i = 0; i < hits.Length; i++) {
                PlayerHealth enemyHealth = hits[i].collider.gameObject.GetComponent<PlayerHealth>();
                if (enemyHealth != null && !enemyHealth.HasTakenDamage) {
                    enemyHealth.TakeDamage( damage );
                    listDamaged.Add(enemyHealth);
                }
            }

            yield return null;
        }

        foreach( PlayerHealth enemyHealth in listDamaged ) {
            enemyHealth.HasTakenDamage = false;
        }
    }

    protected override void ChaseToAttack()
    {
        if (!PlayerInAggroRange()) {
            return;
        }

        {
            indicatorPatrol.SetActive(false);
            indicatorAttack.SetActive(PlayerInAggroRange());
            indicatorShadow.SetActive(movementScript.isInShadow);
        }
        // Движение в сторону игрока
        Vector2 direction = (player.position - transform.position).normalized;
        
        animator.SetFloat("Speed", Math.Abs(direction.x));
        // Поворот в сторону игрока
        if ((direction.x > 0 && Math.Sign(transform.localScale.x) == -1) || (direction.x < 0 && Math.Sign(transform.localScale.x) == 1))
        {
            Flip();
        }
        body.velocity = new Vector2(direction.x * chaseSpeed, body.velocity.y);

        // Запуск анимации преследования
        // animator.SetBool("isMoving", true);

    }

    
    void Flip() {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected() {
        // Gizmos.DrawWireSphere(attackTransform.position, attackRange);
        
        
        Vector3 from = new Vector3(transform.position.x - aggroRange, transform.position.y, transform.position.z);
        Vector3 to = new Vector3(transform.position.x + aggroRange, transform.position.y, transform.position.z);     
        Gizmos.DrawLine(from, to);
    }

    private void endOfAttack() {
        shouldBeDamaging = false;
    }

}
