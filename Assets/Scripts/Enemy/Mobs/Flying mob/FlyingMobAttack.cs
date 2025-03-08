using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingMobAttack : DefaultAttack
{
    private FlyingMobMovement movementScript;
    private RaycastHit2D[] hits;


    private bool isJumping = false;
    private bool jumpingPhase1 = false;
    private bool jumpingPhase2 = false;
    [SerializeField] protected float jumpForce = 10f;

    private float counterAfterBug = 0;
    protected List<Vector3Int> pathToFollow;
    protected int currentPointToFollow;

    void Awake()
    {
        Start();
        movementScript = GetComponent<FlyingMobMovement>();
    }

    void Update()
    {

        if (counterAfterBug >= 0.6f)
        {
            counterAfterBug = 0;
            body.gravityScale = 9.8f;
            isJumping = false;
            jumpingPhase1 = false;
            jumpingPhase2 = false;
        }
        if (body.gravityScale == 0)
        {
            counterAfterBug += Time.deltaTime;
        }

        attackTimeCounter += Time.deltaTime;
        if (PlayerInAggroRange())
        {
            UpdatePath();
            followPath();
            if (PlayerInAttackRange() && attackTimeCounter >= attackCooldown)
            {
                print("attack");
                animator.SetTrigger("Attack");
                StartCoroutine(Attack());
            }
        }
    }

    protected void followPath()
    {

        {
            indicatorPatrol.SetActive(false);
            indicatorAttack.SetActive(true);
        }

        if (pathToFollow.Count == 0)
        {
            return;
        }

        if (Pathfinder.instance.getPathLength(transform.position) <= 1)
        {
            animator.SetFloat("Speed", 0);
            return;
        }

        animator.SetFloat("Speed", chaseSpeed);

        // �������� ������� � ��������� ����� ����
        Vector3 currentPoint = pathToFollow[0] + new Vector3(0.5f, 0.8f);
        Vector3 nextPoint = pathToFollow[1] + new Vector3(0.5f, 0.8f);
        Vector3 afterNextPoint = pathToFollow[2] + new Vector3(0.5f, 0.8f);

        int dir = Math.Sign(currentPoint.x - nextPoint.x);
        if (Math.Abs(currentPoint.y - nextPoint.y) == 0 && dir == transform.localScale.x)
        {
            Flip();
        }

        if (Math.Abs(currentPoint.x - nextPoint.x) > 0)
        {
            // ��������� � ��������� �����
            // Debug.Log("moving from " + transform.position + " towards " + nextPoint + " currentPoint " + currentPoint);
            transform.position = Vector2.MoveTowards(transform.position, nextPoint, chaseSpeed * Time.deltaTime);
        }
        else
        {
            Jump(currentPoint, nextPoint, afterNextPoint);
        }

    }


    protected void Jump(Vector2 from, Vector2 to, Vector2 after)
    {
        isJumping = true;
        if (Math.Abs(transform.position.x - from.x) > 0.1f && !jumpingPhase1)
        {
            // Debug.Log("prepare for jump " + transform.position + " towards " + from);
            transform.position = Vector2.MoveTowards(transform.position, from, chaseSpeed * Time.deltaTime);
            return;
        }
        if (Math.Abs(transform.position.y - to.y) > 0.1f && !jumpingPhase2)
        {
            jumpingPhase1 = true;
            body.gravityScale = 0;
            // Debug.Log("levitating from " + transform.position + " to " + to);
            transform.position = Vector2.MoveTowards(transform.position, to, jumpForce * Time.deltaTime);
        }
        else
        {
            jumpingPhase2 = true;
            // Debug.Log("ending the jump from" + transform.position + " to " + after);
            transform.position = Vector2.MoveTowards(transform.position, after, chaseSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, after) < 0.1f)
            {
                isJumping = false;
                jumpingPhase1 = false;
                jumpingPhase2 = false;
                body.gravityScale = 9.8f;
            }
            counterAfterBug += Time.deltaTime;
        }
    }


    protected override IEnumerator Attack()
    {
        attackTimeCounter = 0;
        List<PlayerHealth> listDamaged = new();
        shouldBeDamaging = true;
        while (shouldBeDamaging)
        {
            hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                PlayerHealth enemyHealth = hits[i].collider.gameObject.GetComponent<PlayerHealth>();
                if (enemyHealth != null && !enemyHealth.HasTakenDamage)
                {
                    enemyHealth.TakeDamage(damage);
                    listDamaged.Add(enemyHealth);
                }
            }

            yield return null;
        }

        foreach (PlayerHealth enemyHealth in listDamaged)
        {
            enemyHealth.HasTakenDamage = false;
        }
    }

    protected override void ChaseToAttack()
    {
        if (!PlayerInAggroRange())
        {
            return;
        }

        // �������� � ������� ������
        Vector2 direction = (player.position - transform.position).normalized;

        animator.SetFloat("Speed", Math.Abs(direction.x));
        // ������� � ������� ������
        if ((direction.x > 0 && Math.Sign(transform.localScale.x) == -1) || (direction.x < 0 && Math.Sign(transform.localScale.x) == 1))
        {
            Flip();
        }
        body.velocity = new Vector2(direction.x * chaseSpeed, body.velocity.y);

        // ������ �������� �������������
        // animator.SetBool("isMoving", true);

    }

    private void UpdatePath()
    {
        if (isJumping)
        {
            return;
        }
        var path = Pathfinder.instance.getNextThreeTiles(transform.position);
        if (path == null || path.Count == 0)
        {
            return;
        }
        pathToFollow = path;
    }


    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);


        Vector3 from = new Vector3(transform.position.x - aggroRange, transform.position.y, transform.position.z);
        Vector3 to = new Vector3(transform.position.x + aggroRange, transform.position.y, transform.position.z);
        Gizmos.DrawLine(from, to);
    }

    private void endOfAttack()
    {
        shouldBeDamaging = false;
    }
}
