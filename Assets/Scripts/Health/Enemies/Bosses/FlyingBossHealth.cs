using UnityEngine;

class FlyingBossHealth : EnemyHealth {
    protected override void Start() {
        base.Start();
    }

    public override void TakeDamage( float damage ) {
        if (canTakeDamage) {
            base.TakeDamage(damage);
        }
    }

    public override void Die() {
        GameManager.instance.BossDied();
        base.Die();
        PlayerPrefs.SetInt("FlyingBossDead", 1);
        GetComponent<DefaultBoss>().ShowCheckpoint();
    }
}