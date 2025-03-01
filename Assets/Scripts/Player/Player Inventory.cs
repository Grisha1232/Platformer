using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    public List<Item> Items { get; protected set; }
    public Weapon EquippedWeapon { get; set; }
    public Item   CurrentItem {get; set;}
    private HealPotion healPotion;

    void Awake() {
        healPotion = new HealPotion(GetComponent<PlayerHealth>());
        Items = new();
        Items.Append(healPotion);

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