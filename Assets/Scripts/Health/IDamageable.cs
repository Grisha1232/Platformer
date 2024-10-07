public interface IDamageable {
    public float Health { get; protected set; }
    public bool CanBeDamaged { get; protected set; }
}