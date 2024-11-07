using UnityEngine;

public class ClawGuardianAttack : MonoBehaviour
{
    public float attackCooldown = 1f; // Время между атаками
    public int maxComboAttacks = 3; // Максимальное количество атак в комбо
    public float dashDistance = 2f; // Дистанция рывка
    public float dashCooldown = 3f; // Время перезарядки рывка
    public float nextDashTime = 0f; // Время, когда можно выполнить следующий рывок

    private Animator animator; // Ссылка на Animator
    private float nextAttackTime = 0f; // Время, когда можно атаковать
    private int currentCombo = 0; // Текущий счетчик комбо
    private Transform player; // Ссылка на игрока

    void Start()
    {
        animator = GetComponent<Animator>(); // Получение компонента Animator
        player = GameObject.FindGameObjectWithTag("Player").transform; // Поиск игрока по тегу
    }

    public void PerformAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            if (currentCombo < maxComboAttacks)
            {
                Attack();
                currentCombo++; // Увеличиваем счетчик комбо
            }
            else
            {
                // Если достигли максимального количества атак в комбо, сбрасываем счетчик
                currentCombo = 0;
            }
        }
    }

    void Attack()
    {
        nextAttackTime = Time.time + attackCooldown; // Установка времени следующей атаки
        Debug.Log("Когтистый охранник атакует!");

        // Триггер анимации атаки
        animator.SetTrigger("Attack");

        // Логика нанесения урона игроку
        // Здесь можно добавить код для нанесения урона, если игрок в радиусе атаки

        // Пример: вызов мощной атаки при третьем ударе
        if (currentCombo == 2) // Если это третий удар (индекс 2)
        {
            PerformPowerAttack();
        }
    }

    void PerformPowerAttack()
    {
        Debug.Log("Когтистый охранник выполняет мощную атаку!");
        animator.SetTrigger("PowerAttack");

        // Логика мощной атаки (например, дополнительные эффекты)
    }

    public void PerformDash()
    {
        if (Time.time >= nextDashTime)
        {
            nextDashTime = Time.time + dashCooldown; // Установка времени следующего рывка
            Debug.Log("Когтистый охранник выполняет рывок на игрока!");

            // Анимация рывка
            animator.SetTrigger("Dash");

            // Расчет направления рывка
            Vector3 dashDirection = (player.position - transform.position).normalized; 
            transform.position += dashDirection * dashDistance; // Движение в направлении игрока
        }
    }

    public void ResetCombo()
    {
        currentCombo = 0; // Сброс комбо при выходе из радиуса атаки или после атаки
    }
}
