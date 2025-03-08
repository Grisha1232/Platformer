using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{
    public VisualElement root;

    private VisualElement settingsView;
    private Button backBtn;
    public GameObject MainMenu;

    public VisualTreeAsset lineEdit;
    public VisualTreeAsset header;

    void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;
        settingsView = root.Q<VisualElement>("treeView");

        backBtn = root.Q<Button>();
        backBtn.clicked += OnBack;
        LoadBindings();
    }

    private void LoadBindings() {
        var saveData = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(saveData))
        {
            UserInput.instance.controls.LoadBindingOverridesFromJson(saveData);
        }
        settingsView.Clear();

       var allBindings = UserInput.instance.GetAllBindings();
       settingsView.Add(header.Instantiate());

        // Создаем строки для каждой записи
        foreach (var actionName in allBindings.Keys)
        {
            if (actionName == "Move" || actionName.Contains("Menu")) {
                continue;
            }
            // Получаем привязки для клавиатуры и геймпада
            string keyboardBinding = GetBindingForDevice(allBindings[actionName], "Keyboard");
            if (keyboardBinding == "None") {
                keyboardBinding = GetBindingForDevice(allBindings[actionName], "Mouse");
            }
            string gamepadBinding = GetBindingForDevice(allBindings[actionName], "Gamepad");

            // keyboardBinding = keyboardBinding.Split("/")[1];
            // gamepadBinding = gamepadBinding.Split("/")[1];

            // Создаем строку
            var rowInst = lineEdit.Instantiate();

            // Колонка с названием действия
            var actionLabel = rowInst.Q<Label>();
            actionLabel.text = actionName;

            // Кнопка для изменения привязки клавиатуры
            var keyboardButton = rowInst.Q<Button>("Keyboard");
            keyboardButton.clicked += () => RebindKeyboard(actionName);
            keyboardButton.text = keyboardBinding;

            // Кнопка для изменения привязки геймпада
            var gamepadButton = rowInst.Q<Button>("Gamepad");
            gamepadButton.clicked += () => RebindGamepad(actionName);
            gamepadButton.text = gamepadBinding;

            // Добавляем строку в контейнер
            settingsView.Add(rowInst);
        }
    }

    private void OnBack() {
        MainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
    private string GetBindingForDevice(List<string> bindings, string device)
    {
        foreach (var binding in bindings)
        {
            if (binding.Contains(device))
            {
                return binding;
            }
        }
        return "None";
    }

    // Метод для изменения привязки клавиатуры
    private void RebindKeyboard(string actionName)
    {
        InputAction action = UserInput.instance.controls.FindAction(actionName, true);
        action.Disable();
        action.PerformInteractiveRebinding()
        .WithTargetBinding(0)
        .OnMatchWaitForAnother(0.1f)
        .OnComplete(operation => {
            action.Enable();
            SaveBindings();
            LoadBindings();
            operation.Dispose();
        }).Start();

    }

    // Метод для изменения привязки геймпада
    private void RebindGamepad(string actionName)
    {
        var action = UserInput.instance.controls.FindAction(actionName);
        action.Disable();
        action.PerformInteractiveRebinding()
        .WithTargetBinding(1)
        .OnMatchWaitForAnother(0.1f)
        .OnComplete(operation =>
        {

            action.Enable();
            SaveBindings();
            LoadBindings();
            operation.Dispose();
        })
        .OnCancel(operation =>
        {
            // Включаем действие обратно, если переназначение отменено
            action.Enable();

            // Освобождаем ресурсы
            operation.Dispose();
        })
        .Start();
    }
    private void SaveBindings() {
        var rebinds = UserInput.instance.controls.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
