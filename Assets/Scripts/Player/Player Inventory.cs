using System;

public class PlayerInventory {

    // private Weapon _equipedWeapon;
    // public Weapon EquipedWeapon {
    //     get {
    //         return _equipedWeapon;
    //     }
    //     private set {
    //         _equipedWeapon = value;
    //     }
    // }
    private Item[] holdsItems;

    private static PlayerInventory instance;

    private PlayerInventory()
    {
        // EquipedWeapon = new MeleeWeapon();
    }

    public static PlayerInventory getInstance() {
        if (instance == null) {
            instance = new PlayerInventory();
        }
        return instance;
    }
}