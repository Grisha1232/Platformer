

class TrainningBossHealth : EnemyHealth {
    protected override void Start() {
        base.Start();
    }

    public override void TakeDamage( float damage ) {
        base.TakeDamage(damage);
    }

    protected override void Die() {
        GetComponent<DefaultBoss>().isDead = true;
        GameManager.instance.BossDied();
        base.Die();

    }
}