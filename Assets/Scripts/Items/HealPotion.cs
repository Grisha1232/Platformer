

using System;

[Serializable]
public class HealPotion : Item
{
    private Health health;
    private float rateOfHeal = 0.2f;
    private float amountOfHeal;

    public HealPotion(Health health) {
        this.health = health;
        amountOfHeal = health.CurrentHealth * rateOfHeal;
    }

    public void upgradeHeal() {
        rateOfHeal += 0.1f;
        amountOfHeal = health.CurrentHealth * rateOfHeal;
    }

    public override void Use()
    {
        health.Heal( amountOfHeal );
    }
}