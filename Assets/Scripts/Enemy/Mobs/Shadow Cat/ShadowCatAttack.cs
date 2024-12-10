using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCatAttack : DefaultAttack
{
    
    private ShadowCatMovement movementScript;

    void Awake()
    {
        Start();
        movementScript = GetComponent<ShadowCatMovement>();
    }

    void Update()
    {
        if (PlayerInAggroRange()) {
            ChaseToAttack();
            Attack();
        }
    }



    protected override void Attack()
    {
        throw new System.NotImplementedException();
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
}
