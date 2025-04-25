using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InvisibleWallController : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Invisible Collision enter");
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Player") {
            GetComponent<TilemapRenderer>().enabled = false;
        }
    }
}
