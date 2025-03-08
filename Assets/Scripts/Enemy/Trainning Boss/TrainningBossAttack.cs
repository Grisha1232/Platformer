using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainningBossAttack : MonoBehaviour
{
    public float Attack1Cooldown = 2.0f;
    public float Attack1Range = 5f;
    public float Attack2Cooldown = 5.0f;
    public float Attack2Range = 10f;
    public float Attack3Cooldown = 5.0f;
    public float Attack3Range = 3f;

    private PlayerHealth playerHealth;
    private TrainningBossMovement movementScript;
    private Rigidbody2D body;
    private Transform player;
    private float cooldownTimer1 = 999f;
    private float cooldownTimer2 = 999f;
    private float cooldownTimer3 = 999f;
    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = GetComponent<PlayerHealth>();
        movementScript = GetComponent<TrainningBossMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer1 += Time.deltaTime;
        cooldownTimer2 += Time.deltaTime;
        cooldownTimer3 += Time.deltaTime;
        if (isAttacking) {
            return;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < Attack3Range && cooldownTimer3 > Attack3Cooldown) {
            StartCoroutine(performAttack3());
        } else if (distanceToPlayer < Attack1Range && cooldownTimer1 > Attack1Cooldown) {
            StartCoroutine(performAttack1());
        } else if (distanceToPlayer < Attack2Range && cooldownTimer2 > Attack2Cooldown) {
            StartCoroutine(performAttack2());
        }
    }

    // Обычная атака 
    IEnumerator performAttack1() {
        isAttacking = true;
        movementScript.setIsAttacking(isAttacking);
        print("performAttack1");
        // Заглушка пока нет аннимаций
        yield return new WaitForSeconds(2);
        endOfAttack1();
    }

    // Атака с выпадом
    IEnumerator performAttack2() {
        isAttacking = true;
        movementScript.setIsAttacking(isAttacking);
        print("performAttack2");
        yield return new WaitForSeconds(1);
        endOfAttack2();
    }

    // Атака с замахом сзади (полуокружностью)
    IEnumerator performAttack3() {
        isAttacking = true;
        movementScript.setIsAttacking(isAttacking);
        print("performAttack3");
        yield return new WaitForSeconds(3);
        endOfAttack3();
    }

    void endOfAttack1() {
        isAttacking = false;
        movementScript.setIsAttacking(isAttacking);
        cooldownTimer1 = 0;
    }

    void endOfAttack2() {
        isAttacking = false;
        movementScript.setIsAttacking(isAttacking);
        cooldownTimer2 = 0;
    }

    void endOfAttack3() {
        isAttacking = false;
        movementScript.setIsAttacking(isAttacking);
        cooldownTimer3 = 0;
    }

    public bool canAttack() {
        return  cooldownTimer1 > Attack1Cooldown || 
                cooldownTimer2 > Attack2Cooldown || 
                cooldownTimer3 > Attack3Cooldown;
    }
}
