using System;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int[] attackDamage = {10, 15, 25};  // Урон для каждой атаки в серии
    public float chargedAttackDamage = 50;      // Урон для мощной атаки
    public float comboResetTime = 1f;         // Время сброса комбо

    private Animator anim;
    private int comboStep = 0;
    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Запуск мощной атаки
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                PowerfulAttack();
            }
        }
        // Выполнить обычную атаку (комбо)
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (comboStep < 3) {
                MeleeComboAttack();
            } else {
                comboStep = 0;
            }
        }

        // Сброс комбо, если прошло слишком много времени с последней атаки
        if (comboStep >= 0 && Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }
    }

    void MeleeComboAttack()
    {
        if (isAttacking) {
            return;
        }
        print("attack " + comboStep);
        // Остановка движения
        isAttacking = true;
        GetComponent<PlayerMovement>().enabled = false;

        // Запуск атаки в зависимости от текущего шага комбо
        anim.SetTrigger("MeleeAttack" + comboStep); // Используем разные анимации для каждой атаки в серии
        DealDamage(attackDamage[comboStep]);
        comboStep++;
        lastAttackTime = Time.time;
    }

    void PowerfulAttack()
    {
        if (isAttacking) return;

        // Остановка движения
        isAttacking = true;
        GetComponent<PlayerMovement>().enabled = false;

        anim.SetTrigger("PowerfulAttack"); // Анимация мощной атаки
        DealDamage(chargedAttackDamage);
    }

    void DealDamage(float damage)
    {
        // Определение врагов в зоне удара
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<IEnemy>().TakeDamage(damage);
        }
    }

    // Вызов из анимации, чтобы разблокировать движения после окончания атаки
    public void EndAttack()
    {
        isAttacking = false;
        GetComponent<PlayerMovement>().enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}