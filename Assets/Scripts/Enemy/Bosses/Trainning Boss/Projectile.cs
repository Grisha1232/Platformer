using UnityEngine;

public class Projectile : MonoBehaviour {
    public int damage = 1; // Урон от стрелы
    public string playerTag = "Player"; // Тег игрока

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, столкнулась ли стрела с игроком
        if (collision.CompareTag(playerTag))
        {
            // Наносим урон игроку
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Деактивируем стрелу после попадания
            gameObject.SetActive(false);
        }
    }
}