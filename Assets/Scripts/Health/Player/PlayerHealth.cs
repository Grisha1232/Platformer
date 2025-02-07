using UnityEngine;

class PlayerHealth : Health {

    protected override void Die()
    {
        print("Died");
        CurrentHealth = maxHealth;
        GameObject.FindGameObjectWithTag("Player").transform.position = UserInput.instance.initialPosition;
    }
}