using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private float damage = 1f;
    private RaycastHit2D[] hits;
    private Animator anim;

    public bool shouldBeDamaging {get; private set;}
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (PlayerMovement.isMovementBlocked) {
            return;
        }
        if (UserInput.instance.controls.Attacking.LightAttack.WasPressedThisFrame()) {
            StartCoroutine(Attack());
            anim.SetTrigger("MeleeAttack0");
        }

        // if (UserInput.instance.controls.Attacking.StrongAttack.WasReleasedThisFrame()) {
        //     print("Strong attack");
        // }
    }

    #region Attack functions

    public IEnumerator Attack()  {   
        List<EnemyHealth> listDamaged = new();
        shouldBeDamaging = true; 
        while(shouldBeDamaging) {            
            hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

            for (int i = 0; i < hits.Length; i++) {
                EnemyHealth enemyHealth = hits[i].collider.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null && !enemyHealth.HasTakenDamage) {
                    enemyHealth.TakeDamage( damage );
                    listDamaged.Add(enemyHealth);
                }
            }

            yield return null;
        }

        foreach( EnemyHealth enemyHealth in listDamaged ) {
            enemyHealth.HasTakenDamage = false;
        }
    }

    #endregion


    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }


    #region Animation Triggers

    private void endOfAttack() {
        shouldBeDamaging = false;
    }
    #endregion
}