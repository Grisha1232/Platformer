using System;
using UnityEditorInternal;

public class PlayerInventory {

    private PlayerInventory() {
        EquippedWeapon = new Claws();
    }
    private static PlayerInventory _instance;
    public static PlayerInventory GetInstance() {
        if (_instance == null) {
            _instance = new PlayerInventory();
        }
        return _instance;
    }
    public Item[] Items { get; protected set; }
    public Weapon EquippedWeapon { get; protected set; }


}