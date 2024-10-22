public class Claws: Weapon {

    public Claws() {
        Name = "Claws";
        IsRemoveble = false;
        IsRanged = false;
        Damage = 20f;
        AttackRange = 2f;
        AttackCooldown = 0.5f;
    }

    public override void Attack()
    {
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