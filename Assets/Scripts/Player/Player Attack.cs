using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack: MonoBehaviour {
    [SerializeField] private float attackSwordCooldown;
    [SerializeField] private float attackProjectileCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject[] rangedWeapons;


    private float cooldownTimer = Mathf.Infinity;
    private PlayerMovement playerMovement;
    private PlayerInventory playerInventory;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerInventory = PlayerInventory.getInstance();
    }

    private void Update() {
        if (Input.GetMouseButton(0) && cooldownTimer > attackProjectileCooldown && playerMovement.canAttack()) {
            Attack();
        }
        cooldownTimer += Time.deltaTime;
    }

    private void Attack() {
        cooldownTimer = 0;
        print(playerInventory.EquipedWeapon.GetType() == typeof(RangeWeapon) ? "Range" : "Melee");

        var weapon = GetWeapon(playerInventory.EquipedWeapon.GetType() == typeof(RangeWeapon));
        if (weapon == null) {
            return;
        }
        weapon.transform.position = attackPoint.position;
        
        weapon.GetComponent<Weapon>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private GameObject GetWeapon(bool isRanged) {
        if (isRanged) {
            for (int i = 0; i < rangedWeapons.Length; i++) {
                if (!rangedWeapons[i].activeInHierarchy) {
                    return rangedWeapons[i];
                }
            }
        } 
        return null;
    }
}