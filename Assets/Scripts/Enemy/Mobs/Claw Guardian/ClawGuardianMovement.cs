using UnityEngine;

public class ClawGuardianMovement : MonoBehaviour
{
    public float patrolSpeed = 2f; // Скорость патрулирования
    public float chaseSpeed = 4f; // Скорость преследования
    public float detectionRange = 5f; // Радиус обнаружения игрока
    public float attackRange = 1.5f; // Радиус атаки

    private Transform player; // Ссылка на игрока
    private ClawGuardianAttack attackScript; // Ссылка на скрипт атаки
    private bool isChasing = false; // Флаг, указывающий на то, преследует ли охранник игрока
    private Vector3 initialPosition; // Начальная позиция охранника

    void Start()
    {
        initialPosition = transform.position; // Сохранение начальной позиции
        player = GameObject.FindGameObjectWithTag("Player").transform; // Поиск игрока по тегу
        attackScript = GetComponent<ClawGuardianAttack>(); // Получение ссылки на скрипт атаки
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Проверка расстояния до игрока
        if (distanceToPlayer < detectionRange && !isChasing)
        {
            isChasing = true; // Начинаем преследование
        }
        else if (distanceToPlayer > detectionRange)
        {
            isChasing = false; // Возвращаемся к патрулированию
        }

        if (isChasing)
        {
            ChasePlayer(); // Преследование игрока
        }
        else
        {
            Patrol(); // Патрулирование
        }
    }

    void Patrol()
    {
        // Логика патрулирования (можно добавить движение между заданными точками)
        // Для примера просто движемся влево и вправо от начальной позиции
        float newPositionX = Mathf.PingPong(Time.time * patrolSpeed, 3f) - 1.5f;
        transform.position = new Vector3(initialPosition.x + newPositionX, transform.position.y, transform.position.z);
    }

    void ChasePlayer() {
        // Преследование игрока
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        // Проверка на атаку или рывок
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            attackScript.PerformAttack(); // Выполнение атаки
        }
        else if (distanceToPlayer <= attackRange * 2 && Time.time >= attackScript.nextDashTime)
        {
            attackScript.PerformDash(); // Выполнение рывка
        }
    }

}
