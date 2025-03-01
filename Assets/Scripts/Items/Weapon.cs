
using System;

[Serializable]
public abstract class Weapon : Item {

    /// <summary>
    /// Determines wether this weapon is ranged
    /// </summary>
    public bool IsRanged { get; protected set; }

    /// <summary>
    /// How much damage you can deal with this weapon
    /// </summary>
    public float Damage { get; protected set; }

    /// <summary>
    /// How far you can attack (radius)
    /// </summary>
    public float AttackRange { get; protected set; }

    /// <summary>
    /// How fast you can attack with this weapon
    /// </summary>
    public float AttackCooldown { get; protected set; } 

    public abstract void UpgradeWeapon();
    public abstract void Attack();
}