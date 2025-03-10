using UnityEngine;

public class ClawGuardianMovement : DefaultMovement {

    protected override void Update() {
        Patrol();
        if (attackScript.PlayerInAggroRange()) {
            if (Mathf.Sign(player.transform.position.x - transform.position.x) != Mathf.Sign(transform.localScale.x)) {
                Flip();
            }
        }
    }

    protected override void Patrol() {
        if (attackScript.PlayerInAggroRange()) {
            return;
        }
        // Поворот в пределах патрулирования
        if (Mathf.Abs(transform.position.x - initialPosition.x ) >= patrolRange && Mathf.Sign(transform.localScale.x) != Mathf.Sign(initialPosition.x - transform.position.x)) {
            Flip();
        }
        
        // animator.SetFloat("Speed", patrolSpeed);
        // Движение в пределах патрулирования
        float moveDirection = Mathf.Sign(transform.localScale.x);
        body.velocity = new Vector2(moveDirection * patrolSpeed, body.velocity.y);
        // Запуск анимации патрулирования
        // animator.SetBool("isMoving", true);
    }
}