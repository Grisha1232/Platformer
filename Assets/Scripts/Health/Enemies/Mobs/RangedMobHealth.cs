public class RangedMobHealth : EnemyHealth {
    protected override void Start() {
        base.Start();
    }

    public override void TakeDamage( float damage ) {
        base.TakeDamage(damage);
    }

    public override void Die() {
        base.Die();
    }
}