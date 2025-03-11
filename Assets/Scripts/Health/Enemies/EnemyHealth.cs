public abstract class EnemyHealth : Health {
    
    public bool canTakeDamage = false;

    public virtual void Reset() {
        canTakeDamage = false;
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