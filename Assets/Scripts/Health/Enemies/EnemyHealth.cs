public abstract class EnemyHealth : Health {
    public virtual void Reset() {
        CurrentHealth = maxHealth;
        healthBar.value = 1;
    }
}