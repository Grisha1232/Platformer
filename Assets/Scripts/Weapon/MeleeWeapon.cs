using System;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private float attackSpeed;
    private float lifeTime;
    protected override void Update()
    {
        if (hit) return;
        transform.Rotate(new Vector3(0, 0, (float)Math.Pow(attackSpeed, 4)) * Time.deltaTime);
        
        lifeTime += Time.deltaTime;
        if (lifeTime > 1) gameObject.SetActive(false);
    }
    
    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            return;
        }
        hit = true;
        boxCollider.enabled = false;
        Deactivate();
    }

    public override void SetDirection(float _direction)
    {
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