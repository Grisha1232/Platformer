
using System;

[Serializable]
public class Bow : Weapon {
    
    public Bow(string name = "Bow"): base(name) {
        IsRemoveble = false;
        IsRanged = true;
        Damage = 1f;
    }

    public override void UpgradeWeapon()
    {
        throw new System.NotImplementedException();
    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }
}