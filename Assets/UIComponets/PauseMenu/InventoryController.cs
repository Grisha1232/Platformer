using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryController : MonoBehaviour
{
    public VisualElement root;

    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;
        
    }

    void Update() {
        if (UserInput.instance.controls.UIinteractive.Back.WasPressedThisFrame()) {
            gameObject.SetActive(false);
        }
    }
}
