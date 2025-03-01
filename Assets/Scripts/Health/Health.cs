using UnityEngine;
using UnityEngine.UI;

public abstract class Health : MonoBehaviour {
    [SerializeField] protected float maxHealth = 3f;
    [SerializeField] public Slider healthBar;
    [SerializeField] public Slider staminaManaBar;
    public float CurrentHealth {get; protected set;}

    public bool HasTakenDamage {get; set;}

    protected virtual void Start() {
        CurrentHealth = maxHealth;
        healthBar.value = 1;
    }

    public virtual void TakeDamage( float damage ) {
        HasTakenDamage = true;

        CurrentHealth -= damage;
        if (healthBar != null) {
            healthBar.value = CurrentHealth / maxHealth;
        }
        print("Taken damage");
        if (CurrentHealth == 0) {
            Die();
        }
    }

    protected virtual void Die() {
        Destroy(gameObject);
    }

    public virtual void Heal( float amount ) {
        return;
    }
}