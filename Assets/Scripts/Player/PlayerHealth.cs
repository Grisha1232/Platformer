using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    float IDamageable.Health { get; set; }
    bool IDamageable.CanBeDamaged { get; set; }
}
