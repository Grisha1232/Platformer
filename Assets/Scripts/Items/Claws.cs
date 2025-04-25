
using System;

[Serializable]
public class Claws : Weapon
{

    public Claws(string name = "Claws") : base(name) {
        IsRemoveble = false;
        IsRanged = false;
        Damage = 2f;
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