using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkyboxLogic : MonoBehaviour
{

    private Collider2D skyboxCollider;

    // Start is called before the first frame update
    private void Start() {
        skyboxCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.name == "Player") {
            collision.gameObject.GetComponent<PlayerHealth>().Die();
        }
    }

}
