using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainningBossMovement : MonoBehaviour
{
    public float MovementSpeed = 2.0f;
    public float MovementSpeedBackwards = 1.0f;
    public float JumpCooldown = 3;

    private PlayerMovement playerMovement;
    private TrainningBossAttack attackScript;
    private Rigidbody2D body;
    private Transform player;
    private bool isAttacking;
    private float JumpCooldownTimer = 99f;

    // Start is called before the first frame update
    void Start()
    {
        attackScript = GetComponent<TrainningBossAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Поиск игрока по тегу
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking) {
            if (player.position.y > body.position.y && JumpCooldownTimer > JumpCooldown) {
                jumpTowardsPlayerPlatform();
            } else if (attackScript.canAttack()){
                moveTowardsPlayer();
            } else {
                moveBackwardsFromPlayer();
            }
        }
        JumpCooldownTimer += Time.deltaTime;
    }

    public void setIsAttacking(bool isAttacking) {
        this.isAttacking = isAttacking;
    }

    void moveTowardsPlayer() {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        body.velocity = new Vector3(direction * MovementSpeed, body.velocity.y);
        transform.localScale = new Vector3(direction * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void moveBackwardsFromPlayer() {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        body.velocity = new Vector3( -direction * MovementSpeedBackwards, body.velocity.y);
        transform.localScale = new Vector3(direction * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void jumpTowardsPlayerPlatform() {
        JumpCooldownTimer = 0;
        print("jumping to player's platform");
    }
}
