using UnityEngine;

class PlayerHealth : MonoBehaviour, IDamageable
{
    public float Health { get; set; }
    public bool CanBeDamaged { get; set; }

    public void TakeDamage(float damageTaken)
    {
        Health -= damageTaken;
    }

}
