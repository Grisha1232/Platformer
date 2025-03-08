using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossScript : MonoBehaviour
{
    public BoxCollider2D playerCollider;

    private void Start() {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log(collision + " " + playerCollider);
        if (collision == playerCollider) {
            TrainningBossMovement.isLocked = false;
        }
    }
}
