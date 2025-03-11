using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossScript : MonoBehaviour
{
    public GameObject player;
    public DefaultBoss boss;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision == player.GetComponent<BoxCollider2D>()) {
            if (PlayerPrefs.GetInt("TrainingBossDead") == 1) {
                return;
            }
            boss.isLocked = false;
            GameManager.instance.SetBossHealth(boss.GetComponent<EnemyHealth>(), "Training Boss");
        }
    }
}
