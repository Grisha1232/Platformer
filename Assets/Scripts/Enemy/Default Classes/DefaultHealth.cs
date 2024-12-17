using Unity.VisualScripting;
using UnityEngine;

abstract class DefaultHealth : MonoBehaviour {
    [SerializeField] protected float maxHealth = 3f;

    protected float currentHealth;

    protected void Start() {
        currentHealth = maxHealth;
    }

    protected virtual void GetDamage( float damage ) {
        currentHealth -= damage;
    }
}