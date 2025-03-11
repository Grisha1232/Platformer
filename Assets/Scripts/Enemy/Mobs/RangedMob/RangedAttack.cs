using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : DefaultAttack {

    [SerializeField] public GameObject[] projectiles;

    public float projectileSpeed = 10f;
    private float projectileLifeDistance = 25f;

    private new void Start() {
        base.Start();
        attackTimeCounter = attackCooldown;
    }

    private void Update() {
        attackTimeCounter += Time.deltaTime;
        if (!PlayerInAttackRange()) {
            return;
        }
        // Получаем позиции босса и игрока
        Vector3 bossPosition = transform.position;
        Vector3 playerPosition = player.transform.position;

        // Вычисляем расстояние до игрока
        float distanceToPlayer = Vector3.Distance(bossPosition, playerPosition);

         // 2. Атака в дальнем бою
        if (distanceToPlayer <= aggroRange && attackTimeCounter >= attackCooldown) {
            Shoot();
            attackTimeCounter = 0;
        }
    }

    private void Shoot() {
        GameObject arrow = System.Array.Find(projectiles, a => !a.activeInHierarchy);
        if (arrow != null)
        {
            // Активируем стрелу и задаем её позицию
            arrow.SetActive(true);
            arrow.transform.position = transform.position;

            // Направление стрельбы (в сторону игрока)
            Vector2 direction = (player.transform.position - transform.position).normalized;
            // Поворачиваем стрелу в сторону игрока
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Вычисляем угол
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle); // Применяем поворот

            // Запускаем стрелу
            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.velocity = direction * projectileSpeed;
            }
        }
    }

    protected override IEnumerator Attack() {
        throw new System.NotImplementedException();
    }

    public override void ChaseToAttack() {
        throw new System.NotImplementedException();
    }

    #region Gizmos draw

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    #endregion
}