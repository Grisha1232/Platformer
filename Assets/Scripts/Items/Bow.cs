
using System;

[Serializable]
public class Bow : Weapon {
    
    public Bow() {
        Name = "Bow";
        IsRemoveble = false;
        IsRanged = true;
        Damage = 50f;
        AttackRange = 50f;
        AttackCooldown = 1f;
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
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