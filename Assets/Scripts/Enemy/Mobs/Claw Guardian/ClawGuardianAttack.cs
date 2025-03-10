using System;
using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class ClawGuardianAttack : DefaultAttack
{
    [SerializeField] private float rushSpeed = 10f; // Скорость рывка
    [SerializeField] private float rushDistance = 5f; // Дистанция рывка
    [SerializeField] private float timeBetweenAttacks = 0.5f; // Время между атаками в серии

    private bool isRushing = false; // Флаг для рывка
    private bool isAttackingSeries = false; // Флаг для серии атак
    private bool isFirstAttack = true; // Флаг для первой атаки

    private new void Start() {
        base.Start();
    }

    private void Update() {
        attackTimeCounter += Time.deltaTime;
        if (PlayerInAggroRange()) {
            ChaseToAttack();
            if (attackTimeCounter >= attackCooldown && !isAttackingSeries && !isRushing && Mathf.Abs(player.transform.position.y - transform.position.y) <= 1f) {
                StartAttackSequence();
            }
        } else {
            isFirstAttack = true;
        }
    }

    private void StartAttackSequence() {
        if (!isGrounded()) {
            return;
        }
        if (isFirstAttack) {
            // Первая атака начинается с рывка
            StartCoroutine(RushAttack());
            isFirstAttack = false; // Сбрасываем флаг первой атаки
            return;
        }

        Vector3 playerPosition = player.transform.position;

        
        if (!PlayerInAttackRange() && Mathf.Abs(transform.position.y - playerPosition.y) <= 0.1f) {
            StartCoroutine(RushAttack());
        } else {
            StartCoroutine(AttackSeries());
        }
    }

    private IEnumerator AttackSeries() {
        Debug.Log("Attack series");
        attackTimeCounter = 0;
        isAttackingSeries = true;

        // Серия из 3 атак
        for (int i = 0; i < 3; i++) {
            if (PlayerInAttackRange()) {
                // Наносим урон
                DealDamage();
                // animator.SetTrigger("Attack"); // Анимация атаки
            }
            yield return new WaitForSeconds(timeBetweenAttacks);
        }

        isAttackingSeries = false;
    }

    protected override IEnumerator Attack() {
        // Этот метод больше не используется, так как атака начинается с рывка
        yield break;
    }

    private IEnumerator RushAttack() {
        Debug.Log("rush");
        attackTimeCounter = 0;
        isRushing = true;
        if (Mathf.Sign(player.transform.position.x - transform.position.x) != Mathf.Sign(transform.localScale.x)) {
            Flip();
        }
        Vector2 rushDirection = (player.transform.position - transform.position).normalized;
        rushDirection.y = 0;
        float rushTime = rushDistance / rushSpeed;

        var temp = body.gravityScale;
        body.gravityScale = 0;
        // Рывок в сторону игрока
        float elapsedTime = 0f;
        while (elapsedTime < rushTime) {
            body.velocity = rushDirection * rushSpeed;
            elapsedTime += Time.deltaTime;
            DealDamage();
            yield return null;
        }

        player.gameObject.GetComponent<PlayerHealth>().HasTakenDamage = false;

        // Останавливаем рывок
        body.gravityScale = temp;
        body.velocity = Vector2.zero;
        isRushing = false;
        
        Debug.Log("stop rush");
    }

    private void DealDamage() {
        var hits = Physics2D.CircleCast(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);
        if (hits.collider != null && shouldBeDamaging) {
            // Наносим урон игроку
            PlayerHealth playerHealth = hits.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.HasTakenDamage) {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    public override void ChaseToAttack() {
        if ( isRushing && isAttackingSeries ) {
            return;
        }
        UpdatePath();
        followPath();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
        
        
        Vector3 from = new Vector2(transform.position.x - aggroRange, transform.position.y);
        Vector3 to = new Vector2(transform.position.x + aggroRange, transform.position.y);     
        Gizmos.DrawLine(from, to);

        Gizmos.color = Color.green;
        Pathfinder.instance.DrawPath(body.position);
    }
}