
class PlayerHealth : Health {
    public bool frameInvincable {get; set;}

    public override void TakeDamage( float damage ) {
        if (!frameInvincable) {
            base.TakeDamage( damage );
        }
    }

    public override void Die()
    {
        print("Died");
        GameManager.instance.PlayDead();
    }

    public void Reset() {
         Start();
    }
}