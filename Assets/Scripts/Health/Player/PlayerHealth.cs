using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

class PlayerHealth : Health {

    protected override void Die()
    {
        print("Died");
        CurrentHealth = maxHealth;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GetComponent<PlayerMovement>().UnblockMovement();
    }
}