using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public VisualElement root;
    public Button continueBtn;
    public Button newGameBtn;
    public Button settingsBtn;
    public Button exitBtn;

    private List<Button> btns;
    private int currentSelected = 0;

    public GameObject levelSelector;

    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;

        continueBtn = root.Q<Button>("ContinueBtn");
        continueBtn.clicked += Continue;

        newGameBtn = root.Q<Button>("NewGameBtn");
        newGameBtn.clicked += NewGame;
        
        settingsBtn = root.Q<Button>("SettingsBtn");
        settingsBtn.clicked += OpenSettings;

        exitBtn = root.Q<Button>("ExitBtn");
        exitBtn.clicked += Exit;
        btns = new()
        {
            continueBtn,
            newGameBtn,
            settingsBtn,
            exitBtn
        };
    }

    private void Update() {
        interactWithMenu();
    }

    private void Continue() {
        levelSelector.SetActive(true);
        gameObject.SetActive(false);
    }

    private void NewGame() {
        Continue();
    }

    private void OpenSettings() {
        Debug.Log("open settings");
    }

    private void Exit() {
        Application.Quit();
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif
    }

    private void interactWithMenu() {
        if (UserInput.instance.controls.UIinteractive.MenuDown.WasPressedThisFrame()) {
            currentSelected++;
        }
        if (UserInput.instance.controls.UIinteractive.MenuUp.WasPressedThisFrame()) {
            currentSelected--;
        }
        if (currentSelected < 0) {
            currentSelected = btns.Count - 1;
        }
        currentSelected %= btns.Count;

        btns[currentSelected].Focus();
    }
}
