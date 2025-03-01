using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

class PlayerHealth : Health {
    public bool framInvincable {get; set;}

    public override void TakeDamage( float damage ) {
        if (!framInvincable) {
            base.TakeDamage( damage );
        }
    }

    protected override void Die()
    {
        print("Died");
        CurrentHealth = maxHealth;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GetComponent<PlayerMovement>().UnblockMovement();
    }
}