using System;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    private float lifeTime;
    protected override void Update()
    {
        throw new NotImplementedException();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        throw new NotImplementedException();
    }

    public override void SetDirection(float _direction)
    {
        throw new NotImplementedException();
    }
}