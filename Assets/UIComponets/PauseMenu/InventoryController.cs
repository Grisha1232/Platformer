using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryController : MonoBehaviour
{
    public VisualElement root;
    public VisualTreeAsset itemBtn;

    private VisualElement itemsList;
    private Label DescriptionText;
    private Dictionary<Button, Item> items;

    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;
        items = new();
        itemsList = root.Q<VisualElement>().Q<VisualElement>("MainContent").Q<VisualElement>("InventoryItems");
        itemsList.Clear();


        DescriptionText = root.Q<VisualElement>().Q<VisualElement>("MainContent").Q<Label>("descriptionText");
        DescriptionText.Clear();
        foreach (var item in PlayerInventory.instance.Items) {
            var itemHolder = itemBtn.Instantiate();
            Button btn = itemHolder.Q<Button>();
            btn.text = item.Name;
            btn.clicked += () => showDescription(btn);
            
            itemsList.Add(itemHolder);
            items[btn] = item;
        }
    }

    private void showDescription(Button sender) {
        DescriptionText.text = items[sender].Description;
    }

    void Update() {
        if (UserInput.instance.controls.UIinteractive.Back.WasPressedThisFrame()) {
            gameObject.SetActive(false);
        }
    }
}
