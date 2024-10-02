using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class Weapon : Item {
    [SerializeField] protected float speed;
    protected float direction;
    protected bool hit;
    protected BoxCollider2D boxCollider;

    protected override void Awake()  {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    
    protected abstract void Update();

    protected abstract void OnTriggerEnter2D(Collider2D collision);

    public abstract void SetDirection(float _direction);

    protected void Deactivate() {
        print("destroyed");
        gameObject.SetActive(false);
    }
}