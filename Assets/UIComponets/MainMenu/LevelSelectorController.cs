using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelSelectorController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement rootLevels;
    public VisualTreeAsset buttonLevel;

    private Button backBtn;
    private List<Button> levelBtns;
    private int currentSelectedLevels = 0;
    private bool focusedOnBack = false;

    public GameObject MainMenu;


    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;

        rootLevels = root.Q<VisualElement>("LevelsContainer");
        rootLevels.Clear();

        var scenes = EditorBuildSettings.scenes;

        levelBtns = new();
        foreach (var scene in scenes)
        {
            // Извлекаем имя сцены из пути
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
            CreateButton(rootLevels, sceneName);
        }
        backBtn = root.Q<Button>("BackBtn");
        backBtn.clicked += Back;
    }

    private void Update() {
        interactWithLevels();
    }

    
    private void CreateButton(VisualElement root, string buttonText)
    {
        // Создаем новую кнопку
        var button = buttonLevel.Instantiate();
        var btn = button.Q<Button>("button");
        levelBtns.Add(btn);
        btn.text = buttonText;

        // Добавляем обработчик события нажатия
        btn.clicked += () => onButtonClicked(btn);

        // Добавляем кнопку в rootVisualElement
        root.Add(button);
    }

    
    private void onButtonClicked(Button button) {
        print(button.text);
        SceneManager.LoadScene(button.text);
    }

    private void Back() {
        MainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void interactWithLevels() {
        if (UserInput.instance.controls.UIinteractive.MenuDown.WasPressedThisFrame()) {
            backBtn.Focus();
            focusedOnBack = true;
            return;
        }
        if (UserInput.instance.controls.UIinteractive.MenuUp.WasPressedThisFrame()) {
            levelBtns[currentSelectedLevels].Focus();
            focusedOnBack = false;
            return;
        }

        if (focusedOnBack) {
            return;
        }
        
        if (UserInput.instance.controls.UIinteractive.MenuRight.WasPressedThisFrame()) {
            currentSelectedLevels++;
        }
        if (UserInput.instance.controls.UIinteractive.MenuLeft.WasPressedThisFrame()) {
            currentSelectedLevels--;
        }


        if (currentSelectedLevels < 0) {
            currentSelectedLevels = levelBtns.Count - 1;
        }
        currentSelectedLevels %= levelBtns.Count;

        levelBtns[currentSelectedLevels].Focus();
    }
}
