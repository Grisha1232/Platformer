using UnityEngine;

class TrainningBossHealth : EnemyHealth {
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
        PlayerPrefs.SetInt("TrainingBossDead", 1);
        GetComponent<DefaultBoss>().ShowCheckpoint();
    }
}