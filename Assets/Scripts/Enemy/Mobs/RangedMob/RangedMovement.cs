using System;
using UnityEngine;

public class RangedMovement : DefaultMovement {

    private new void Start() {
        base.Start();
        attackScript = GetComponent<RangedAttack>();
    }

    protected override void Patrol()
    {
        if (attackScript.PlayerInAggroRange()) {
            return;
        }

        // Поворот в пределах патрулирования
        if (Mathf.Abs(transform.position.x - initialPosition.x ) >= patrolRange && Mathf.Sign(transform.localScale.x) != Mathf.Sign(initialPosition.x - transform.position.x))
        {
            Flip();
        }
        
        // animator.SetFloat("Speed", patrolSpeed);
        // Движение в пределах патрулирования
        float moveDirection = Math.Sign(transform.localScale.x);
        body.velocity = new Vector2(moveDirection * patrolSpeed, body.velocity.y);
        // Запуск анимации патрулирования
        // animator.SetBool("isMoving", true);
    }

    protected override void Update() {
        Patrol();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(new Vector2(initialPosition.x - patrolRange, initialPosition.y + 1), new Vector2(initialPosition.x + patrolRange, initialPosition.y + 1));
    }
}