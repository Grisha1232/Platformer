public abstract class EnemyHealth : Health {
    public virtual void Reset() {
        CurrentHealth = maxHealth;
        if (healthBar != null) {
            healthBar.value = 1;
        }
    }
}