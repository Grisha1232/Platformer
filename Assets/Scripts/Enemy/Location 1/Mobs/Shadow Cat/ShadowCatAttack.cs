using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCatAttack : MonoBehaviour
{
    [SerializeField] public float attackRange = 1.0f;  // Дальность атаки
    [SerializeField] public float attackDamage = 10f;     // Урон
    [SerializeField] public float attackCooldown = 2.0f;  // Время перезарядки атаки

    private bool canAttack = true;
    private Animator animator;
    private Transform player;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (PlayerInRange() && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    // Проверка, находится ли игрок в зоне атаки
    bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;

        // Запуск анимации атаки
        animator.SetTrigger("Attack");

        // Задержка перед нанесением урона (например, в середине анимации атаки)
        yield return new WaitForSeconds(0.5f); 

        // Наносим урон игроку
        if (PlayerInRange())
        {
            player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }

        // Задержка между атаками
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
