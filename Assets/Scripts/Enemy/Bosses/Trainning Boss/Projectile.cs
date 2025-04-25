using UnityEngine;

public class Projectile : MonoBehaviour {
    public int damage = 1; // Урон от стрелы
    public string playerTag = "Player"; // Тег игрока
    private Vector2 startPosition;

    private void Update() {
        CheckTravelDistance();
    }

    private void OnEnable() {
        startPosition = transform.position;
    }

    private void CheckTravelDistance() {
        if (Vector2.Distance(startPosition, transform.position) < 30f) {
            return;
        }
        // Деактивируем стрелу
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.name == "Map") {
            gameObject.SetActive(false);
        }
        if (collision.CompareTag(playerTag)) {
            // Наносим урон игроку
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.TakeDamage(damage);
            }

            // Деактивируем стрелу после попадания
            gameObject.SetActive(false);
        }
    }
}