using UnityEngine;
using UnityEngine.UI;

public abstract class Health : MonoBehaviour {
    [SerializeField] public float maxHealth { get; protected set; } = 3f;
    [SerializeField] public Slider healthBar;
    [SerializeField] public Slider staminaManaBar;
    public float CurrentHealth {get; protected set;}

    public bool HasTakenDamage {get; set;} = false;

    protected virtual void Start() {
        HasTakenDamage = false;
        CurrentHealth = maxHealth;
        if (healthBar != null) {
            healthBar.value = 1;
        }
    }

    public virtual void TakeDamage( float damage ) {
        HasTakenDamage = true;

        CurrentHealth -= damage;
        if (healthBar != null) {
            healthBar.value = CurrentHealth / maxHealth;
        }
        print("Taken damage");
        if (CurrentHealth <= 0) {
            Die();
        }
    }

    public virtual void Die() {
        Destroy(gameObject);
    }

    public virtual void Heal( float amount ) {
        CurrentHealth += amount;
        if (CurrentHealth > maxHealth) {
            CurrentHealth = maxHealth;
        }
        healthBar.value = CurrentHealth / maxHealth;
        return;
    }
}