
using System;

[Serializable]
public abstract class Weapon : Item {
    protected Weapon(string name) : base(name)
    {
    }

    /// <summary>
    /// Determines wether this weapon is ranged
    /// </summary>
    public bool IsRanged { get; protected set; }

    /// <summary>
    /// How much damage you can deal with this weapon
    /// </summary>
    public float Damage { get; protected set; }

    public abstract void UpgradeWeapon();
}