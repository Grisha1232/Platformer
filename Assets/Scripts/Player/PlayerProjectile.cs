using UnityEngine;

public class PlayerProjectile : MonoBehaviour {
    public float damage = 1; // Урон от стрелы
    public Vector2 startPosition;

    private void Update() {
        CheckTravelDistance();
    }
    
    void OnDisable() {
        transform.position = startPosition;
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
        var enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null) {
            enemyHealth.TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }
}