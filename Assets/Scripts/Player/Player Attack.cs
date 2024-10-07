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
        playerInventory.EquippedWeapon = new Claws();
    }

    private void Update() {
        if (cooldownTimer + 0.05f > playerInventory.EquippedWeapon.AttackCooldown) {
            playerMovement.setCanMove(true);
        }
        if (Input.GetMouseButton(0) && 
            cooldownTimer > playerInventory.EquippedWeapon.AttackCooldown &&
            playerMovement.canAttack()) {
            Attack();
        }
        cooldownTimer += Time.deltaTime;
    }

    private void Attack() {
        cooldownTimer = 0;
        if (playerInventory.EquippedWeapon.IsRanged) {
        } else {
            playerMovement.Anim.SetTrigger("MeleeAttack");
            playerMovement.setCanMove(false);
        }
        playerInventory.EquippedWeapon.Attack();
    }

}