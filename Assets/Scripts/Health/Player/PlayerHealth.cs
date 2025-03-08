
class PlayerHealth : Health {
    public bool framInvincable {get; set;}

    public override void TakeDamage( float damage ) {
        if (!framInvincable) {
            base.TakeDamage( damage );
        }
    }

    protected override void Die()
    {
        print("Died");
        CurrentHealth = maxHealth;
        healthBar.value = 1;
        GameManager.instance.ReturnToCheckpoint(true);
    }
}