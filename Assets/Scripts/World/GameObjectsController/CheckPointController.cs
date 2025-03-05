using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    public GameObject canvasTip;
    void Awake() {
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            canvasTip.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision){
        if (collision.gameObject.tag == "Player") {
            canvasTip.SetActive(false);
        }
    }

}
