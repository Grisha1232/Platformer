using System;

[Serializable]
public class HealPotion : Item {
    private Health health;
    private float rateOfHeal = 0.2f;
    private float amountOfHeal;

    public int amountOfUse { get; private set; }
    public int amountOfUseRamaining { get; set; }

    public HealPotion(Health health, string name = "HealthPotion"): base(name) {
        this.health = health;
        amountOfHeal = health.maxHealth * rateOfHeal;
        amountOfUse = 3;
        amountOfUseRamaining = amountOfUse;
    }

    public void setHealth(Health health) {
        this.health = health;
        amountOfHeal = health.maxHealth * rateOfHeal;
        amountOfUseRamaining = amountOfUse;
    }

    public void upgradeHeal() {
        amountOfUse++;
        rateOfHeal += 0.1f;
        amountOfHeal = health.maxHealth * rateOfHeal;
    }

    public void Reset() {
        amountOfUseRamaining = amountOfUse;
    }

    public override void Use() {
        if (amountOfUseRamaining > 0) {
            health.Heal( amountOfHeal );
            amountOfUseRamaining--;
        }
    }
}