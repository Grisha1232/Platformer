public abstract class EnemyHealth : Health {
    public virtual void Reset() {
        CurrentHealth = maxHealth;
        if (healthBar != null) {
            healthBar.value = 1;
        }
    }

    public override void Die() {
        base.Die();
        PlayerInventory.instance.AddToCurrentCurency(10);
    }
}