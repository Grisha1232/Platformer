using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BonefireController : MonoBehaviour
{
    private VisualElement root;

    private Button restBtn;
    private Button upgradeBtn;
    private Button travelBtn;
    private Button exitBtn;

    private GameObject upgradeUI;

    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;

        restBtn = root.Q<Button>("Rest");
        restBtn.clicked += Rest;

        upgradeBtn = root.Q<Button>("Upgrade");
        upgradeBtn.clicked += Upgrade;

        travelBtn = root.Q<Button>("Travel");
        travelBtn.clicked += Travel;

        exitBtn = root.Q<Button>("Exit");
        exitBtn.clicked += Exit;

        if (upgradeUI == null) {
            upgradeUI = GameManager.FindInactiveObjectByTag("UpgradeUI");
        }
    }

    private void Rest() {
        GameManager.instance.ReturnToCheckpoint();
    }

    private void Upgrade() {
        upgradeUI.SetActive(true);
        gameObject.SetActive(false);
    }

    private void Travel() {
        Debug.Log("Not Impemented yet");
    }

    private void Exit() {
        gameObject.SetActive(false);
        UserInput.instance.EnableForGame();
    }
}
