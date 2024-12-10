using UnityEngine;

abstract class Health : MonoBehaviour {
    [SerializeField] protected float maxHealth = 3f;
    public float CurrentHealth {get; protected set;}

    public bool HasTakenDamage {get; set;}

    protected virtual void Start() {
        CurrentHealth = maxHealth;
    }

    public virtual void TakeDamage( float damage ) {
        HasTakenDamage = true;

        CurrentHealth -= damage;
        if (CurrentHealth == 0) {
            Die();
        }
    }

    protected virtual void Die() {
        Destroy(gameObject);
    }
}