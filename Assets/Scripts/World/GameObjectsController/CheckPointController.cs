using TMPro;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    public GameObject canvasTip;
    public TMP_Text text;
    void Awake() {
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            canvasTip.SetActive(true);
            text.text = "Press " + UserInput.instance.controls.GameInteraction.Interact.activeControl.path;
        }
    }

    void OnTriggerExit2D(Collider2D collision){
        if (collision.gameObject.tag == "Player") {
            canvasTip.SetActive(false);
        }
    }

}
