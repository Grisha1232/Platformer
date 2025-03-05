using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    public List<Item> Items { get; set; }
    public List<Item> quickItems { get; set; }
    public Weapon EquippedWeapon { get; set; }
    public Item   CurrentItem { get; set; }
    public int    Currency { get; set; }
    private HealPotion healPotion;

    public static PlayerInventory instance;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        healPotion = new HealPotion(GetComponent<PlayerHealth>());
        Items = new();
    }

    void Update() {
        if (UserInput.instance.controls.GameInteraction.UseHeal.WasPressedThisFrame()) {
            UseHeal();
        }
        if (UserInput.instance.controls.GameInteraction.UseItem.WasPressedThisFrame()) {
            UseItem();
        }
    }

    private void UseHeal() {
        healPotion.Use();
    }

    private void UseItem() {
        CurrentItem.Use();
    }
}