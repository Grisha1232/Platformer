using UnityEngine;

public class PlayerAttack: MonoBehaviour {
    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPoint;


    private float cooldownTimer = Mathf.Infinity;
    private PlayerMovement playerMovement;
    private PlayerInventory playerInventory;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerInventory = PlayerInventory.GetInstance();
    }

    private void Update() {
        if (Input.GetMouseButton(0) && cooldownTimer > playerInventory.EquippedWeapon.AttackCooldown) {
            Attack();
        }
    }

    private void Attack() {
        playerInventory.EquippedWeapon.Attack();
    }

}