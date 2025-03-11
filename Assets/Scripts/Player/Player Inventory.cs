using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditorInternal;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    public List<Item> Items { get; set; }
    public List<Item> QuickItems { get; set; }
    public Weapon EquippedWeapon { get; set; }
    public Item   CurrentItem { get; set; }
    public int    Currency { get; set; }
    private HealPotion healPotion;

    public TMP_Text currentCurrency;
    public TMP_Text healRemaining;

    public static PlayerInventory instance;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        Items = new();
        EquippedWeapon = new Claws();
    }

    void Update() {
        if (UserInput.instance.controls.GameInteraction.UseHeal.WasPressedThisFrame()) {
            UseHeal();
        }
        if (UserInput.instance.controls.GameInteraction.UseItem.WasPressedThisFrame()) {
            UseItem();
        }
    }

    private void OnEnable() {
        healPotion = new HealPotion(GetComponent<PlayerHealth>());
        LoadItems();
        healRemaining.text = healPotion.amountOfUseRamaining.ToString();
    }

    private void OnDisable() {
        SaveItems();
    }

    public void SwitchWeapon() {
        if (EquippedWeapon.IsRanged) {
            EquippedWeapon = new Claws();
        } else {
            EquippedWeapon = new Bow();
        }
    }

    public void AddToCurrentCurency(int amount) {
        Currency += amount;
        currentCurrency.text = Currency.ToString();
    }

    private void UseHeal() {
        healPotion.Use();
        healRemaining.text = healPotion.amountOfUseRamaining.ToString();
    }

    private void UseItem() {
        CurrentItem.Use();
    }

    private void SaveItems() {
        string items = JsonUtility.ToJson(Items);
        string quickItems = JsonUtility.ToJson(QuickItems);

        PlayerPrefs.SetString("Items", items);
        PlayerPrefs.SetString("QuickItems", quickItems);
        PlayerPrefs.Save();
    }

    private void LoadItems() {
        string items = PlayerPrefs.GetString("Items");
        string quickItems = PlayerPrefs.GetString("QuickItems");

        if (string.IsNullOrEmpty(items)) {
            Items.Add(healPotion);
        } else {
            Items = JsonUtility.FromJson<List<Item>>(items);
            var check = Items.OfType<HealPotion>().ToList();
            if (check.Count() > 1) {
                Debug.Log("Два healPotion!!!!!!!");
            }
            if (check.Count == 0) {
                Items.Add(healPotion);
            } else {
                healPotion = check[0];
            }
            healPotion.setHealth(GetComponent<PlayerHealth>());
        }
         if (string.IsNullOrEmpty(quickItems)) {
        } else {
            QuickItems = JsonUtility.FromJson<List<Item>>(quickItems);
        }
    }

    public void ResetHealPotions() {
        healPotion.Reset();
        healRemaining.text = healPotion.amountOfUseRamaining.ToString();
    }
}