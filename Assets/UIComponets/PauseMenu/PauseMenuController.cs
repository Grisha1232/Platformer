using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    public VisualElement root;
    public Button inventoryBtn;
    private Label invetoryLabel;
    public Button statusBtn;
    private Label statusLabel;
    public Button MainMenuBtn;
    private Label MainMenuLabel;

    private List<Button> btns;
    private int currentSelected = 0;

    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;

        inventoryBtn = root.Q<VisualElement>("Inventory").Q<Button>("InventoryBtn");
        invetoryLabel = root.Q<VisualElement>("Inventory").Q<Label>();
        inventoryBtn.clicked += openInventory;

        statusBtn = root.Q<VisualElement>("Status").Q<Button>("StatusBtn");
        statusLabel = root.Q<VisualElement>("Status").Q<Label>();
        statusBtn.clicked += openStatus;
        
        MainMenuBtn = root.Q<VisualElement>("MainMenu").Q<Button>("MainMenuBtn");
        MainMenuLabel = root.Q<VisualElement>("MainMenu").Q<Label>();
        MainMenuBtn.clicked += openMainMenu;

        inventoryBtn.RegisterCallback<FocusInEvent>(OnFocusInInvetory);
        statusBtn.RegisterCallback<FocusInEvent>(OnFocusInStatus);
        MainMenuBtn.RegisterCallback<FocusInEvent>(OnFocusInMainMenu);

        inventoryBtn.RegisterCallback<FocusOutEvent>(OnFocusOutInventory);
        statusBtn.RegisterCallback<FocusOutEvent>(OnFocusOutStatus);
        MainMenuBtn.RegisterCallback<FocusOutEvent>(OnFocusOutMainMenu);

        btns = new()
        {
            inventoryBtn,
            statusBtn,
            MainMenuBtn
        };
    }

    private void Update() {
        interactWithMenu();
    }

    private void OnFocusInInvetory(FocusInEvent evt) {
        invetoryLabel.style.display = DisplayStyle.Flex;
    }

    private void OnFocusInStatus(FocusInEvent evt) {
        statusLabel.style.display = DisplayStyle.Flex;
    }

    private void OnFocusInMainMenu(FocusInEvent evt) {
        MainMenuLabel.style.display = DisplayStyle.Flex;
    }

    private void OnFocusOutInventory(FocusOutEvent evt) {
        invetoryLabel.style.display = DisplayStyle.None;
    }

    private void OnFocusOutStatus(FocusOutEvent evt) {
        statusLabel.style.display = DisplayStyle.None;
    }

    private void OnFocusOutMainMenu(FocusOutEvent evt) {
        MainMenuLabel.style.display = DisplayStyle.None;
    }

    private void openInventory() {
        Debug.Log("Inventory");
    }

    private void openStatus() {
        Debug.Log("Status");
    }

    private void openMainMenu() {
        Debug.Log("Main Menu");
    }

    private void interactWithMenu() {
        
        if (UserInput.instance.controls.UIinteractive.MenuRight.WasPressedThisFrame()) {
            currentSelected++;
        }
        if (UserInput.instance.controls.UIinteractive.MenuLeft.WasPressedThisFrame()) {
            currentSelected--;
        }


        if (currentSelected < 0) {
            currentSelected = btns.Count - 1;
        }
        currentSelected %= btns.Count;

        btns[currentSelected].Focus();
    }
}
