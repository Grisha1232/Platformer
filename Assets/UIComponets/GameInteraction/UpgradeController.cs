using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeController : MonoBehaviour {
    
    private VisualElement root;
    
    private GameObject bonefireUI;

    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;

        bonefireUI = GameManager.FindInactiveObjectByTag("BonefireUI");
        UserInput.instance.DisableForGame();
    }

    private void Update() {
        if (UserInput.instance.controls.UIinteractive.Back.WasPressedThisFrame()) {
            bonefireUI.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
