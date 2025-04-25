using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowCatAttack : DefaultAttack
{
    
    private ShadowCatMovement movementScript;
    private RaycastHit2D[] hits;

    void Awake()
    {
        Start();
        movementScript = GetComponent<ShadowCatMovement>();
    }

    void Update()
    {
        
        if (counterAfterBug >= 0.6f) {
            counterAfterBug = 0;
            body.gravityScale = 9.8f;
            isJumping = false;
            jumpingPhase1= false;
            jumpingPhase2 = false;
        }
        if (body.gravityScale == 0) {
            counterAfterBug += Time.deltaTime;
        }

        attackTimeCounter += Time.deltaTime;
        if (PlayerInAggroRange()) {
            ChaseToAttack();
            if (PlayerInAttackRange() && attackTimeCounter >= attackCooldown) {
                print("attack");
                attackTimeCounter = 0;
                animator.SetTrigger("Attack");
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

    public override void ChaseToAttack() {
        UpdatePath();
        followPath();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
        
        
        Vector3 from = new Vector2(transform.position.x - aggroRange, transform.position.y);
        Vector3 to = new Vector2(transform.position.x + aggroRange, transform.position.y);     
        Gizmos.DrawLine(from, to);
    }

    private void startDamage() {
        StartCoroutine(Attack());
    }

    private void endOfAttack() {
        shouldBeDamaging = false;
    }

}
