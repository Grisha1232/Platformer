using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private GameObject[] arrows;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private UnityEngine.UI.Image weaponImage;
    private float damage;
    private RaycastHit2D[] hits;
    private Animator anim;

    public bool shouldBeDamaging {get; private set;}

    private Weapon currentWeapon;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        currentWeapon = PlayerInventory.instance.EquippedWeapon;
        damage = currentWeapon.Damage;
    }

    void Update() {
        SwitchWeapon();

        if (PlayerMovement.isMovementBlocked) {
            return;
        }
        if (UserInput.instance.controls.Attacking.LightAttack.WasPressedThisFrame() && !currentWeapon.IsRanged) {
            StartCoroutine(MeleeAttack());
            anim.SetTrigger("MeleeAttack0");
        }
        if (UserInput.instance.controls.Attacking.LightAttack.WasPressedThisFrame() && currentWeapon.IsRanged) {
            RangeAttack();
            // anim.SetTrigger("RangeAttack");
        }

        // if (UserInput.instance.controls.Attacking.StrongAttack.WasReleasedThisFrame()) {
        //     print("Strong attack");
        // }
    }

    #region Attack functions

    private IEnumerator MeleeAttack()  {   
        List<EnemyHealth> listDamaged = new();
        shouldBeDamaging = true; 
        while(shouldBeDamaging) {            
            hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

            for (int i = 0; i < hits.Length; i++) {
                EnemyHealth enemyHealth = hits[i].collider.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null && !enemyHealth.HasTakenDamage) {
                    enemyHealth.TakeDamage( damage );
                    listDamaged.Add(enemyHealth);
                }
            }

            yield return null;
        }

        foreach( EnemyHealth enemyHealth in listDamaged ) {
            enemyHealth.HasTakenDamage = false;
        }
    }

    private void RangeAttack() {
        GameObject arrow = Array.Find(arrows, a => !a.activeInHierarchy);
        if (arrow != null)
        {
            // Активируем стрелу и задаем её позицию
            arrow.SetActive(true);
            arrow.GetComponent<PlayerProjectile>().startPosition = transform.position;
            arrow.transform.localScale = new Vector3(
                Mathf.Sign(transform.localScale.x) * arrow.transform.localScale.x,
                arrow.transform.localScale.y,
                arrow.transform.localScale.z
            );
            arrow.transform.position = transform.position;

            Vector2 direction = new Vector2(transform.localScale.x, 0).normalized;

            // Запускаем стрелу
            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
            if (arrowRb != null) {
                arrowRb.velocity = direction * projectileSpeed;
            }
        }
    }

    private void SwitchWeapon() {
        if (UserInput.instance.controls.GameInteraction.SwitchWeapon.WasPressedThisFrame()) {
            PlayerInventory.instance.SwitchWeapon();
            currentWeapon = PlayerInventory.instance.EquippedWeapon;
            damage = currentWeapon.Damage;
            if (currentWeapon.IsRanged) {
                weaponImage.GetComponentInChildren<TMP_Text>().text = "Ranged";
            } else {
                weaponImage.GetComponentInChildren<TMP_Text>().text = "Melee";
            }
        }
    }

    #endregion

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }


    #region Animation Triggers

    private void endOfAttack() {
        shouldBeDamaging = false;
    }
    
    #endregion
}