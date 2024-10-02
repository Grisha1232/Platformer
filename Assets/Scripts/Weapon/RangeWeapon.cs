using System;
using UnityEngine;

public class RangeWeapon : Weapon
{
    private float lifeTime;
    protected override void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(0, movementSpeed, 0);

        lifeTime += Time.deltaTime;
        if (lifeTime > 5) gameObject.SetActive(false);
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        print("hit!!");
        hit = true;
        boxCollider.enabled = false;
        Deactivate();
    }

    public override void SetDirection(float _direction) {
        lifeTime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleY = transform.localScale.y;
        if (Mathf.Sign(localScaleY) != _direction)
            localScaleY = -localScaleY;

        transform.localScale = new Vector3(transform.localScale.x, localScaleY, transform.localScale.z);
    }
}