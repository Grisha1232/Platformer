public interface IDamageable {
    public float Health { get; set; }
    public bool CanBeDamaged { get; set; }

    void TakeDamage(float damageTaken);
}