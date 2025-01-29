using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using static Pathfinder;

public class ShadowCatMovement : DefaultMovement
{
    /// <summary>
    /// Маска земли
    /// </summary>
    [SerializeField] private LayerMask groundLayer;
    private BoxCollider2D boxCollider;
    [SerializeField] private float  extraHeight = 0.25f;
    private RaycastHit2D groundHit;
    private bool isJumping = false;
    private bool jumpingPhase1 = false;
    private bool jumpingPhase2 = false;
    private bool isStopped = false;

    private float counterAfterBug = 0;

    /// <summary>
    /// Находится ли Теневой кот в тени
    /// </summary>
    [HideInInspector] public bool isInShadow {get; private set;}= false;


    /// <summary>
    /// This methods calls every time game is start
    /// </summary>
    private void Awake() {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        attackScript = GetComponent<ShadowCatAttack>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    
    protected override void Update()
    {
        // Patrol();
        
        if (counterAfterBug >= 0.6f) {
            counterAfterBug = 0;
            body.gravityScale = 9.8f;
            isJumping = false;
            jumpingPhase1= false;
            jumpingPhase2 = false;
        }
        if (body.gravityScale == 0) {
            counterAfterBug += Time.deltaTime;
        }
        UpdatePath();
        followPath();
        // print("----------");

        // print("++++++++++");
        // foreach (var pair in compressedPath) {
        //     Debug.Log(pair.vect + " Direction = " + pair.dir);
        // }
        // print("++++++++++");
    }


    #region Movement Functions

    protected override void Patrol()
    {
        if (attackScript.PlayerInAggroRange()) {
            return;
        }

        {
            indicatorPatrol.SetActive(true);
            indicatorAttack.SetActive(attackScript.PlayerInAggroRange());
            indicatorShadow.SetActive(isInShadow);
        }
        // Поворот в пределах патрулирования
        if (Math.Abs(transform.position.x - initialPosition.x ) >= patrolRange && Math.Sign(transform.localScale.x) != Math.Sign(initialPosition.x - transform.position.x))
        {
            Flip();
        }
        
        animator.SetFloat("Speed", patrolSpeed);
        // Движение в пределах патрулирования
        float moveDirection = Math.Sign(transform.localScale.x);
        body.velocity = new Vector2(moveDirection * patrolSpeed, body.velocity.y);
        // Запуск анимации патрулирования
        // animator.SetBool("isMoving", true);
    }

    
    protected override void followPath() 
    {
        if (pathToFollow.Count == 0) {
            return;
        }

        if (Pathfinder.instance.getPathLength(transform.position) <= 1) {
            animator.SetFloat("Speed", 0);
            return;
        }

        animator.SetFloat("Speed", patrolSpeed);

        // Получаем текущую и следующую точку пути
        Vector3 currentPoint = pathToFollow[0] + new Vector3(0.5f, 0.8f);
        Vector3 nextPoint = pathToFollow[1] + new Vector3(0.5f, 0.8f);
        Vector3 afterNextPoint = pathToFollow[2] + new Vector3(0.5f, 0.8f);
       
        if (Math.Abs(currentPoint.x - nextPoint.x) > 0) {
            // Двигаемся к следующей точке
            // Debug.Log("moving from " + transform.position + " towards " + nextPoint + " currentPoint " + currentPoint);
            transform.position = Vector2.MoveTowards(transform.position, nextPoint, patrolSpeed * Time.deltaTime);
        } else {
            Jump(currentPoint, nextPoint, afterNextPoint);
        }

    }

    protected void Jump(Vector2 from, Vector2 to, Vector2 after) {
        isJumping = true;
        if (Math.Abs(transform.position.x - from.x) > 0.1f && !jumpingPhase1){        
            // Debug.Log("prepare for jump " + transform.position + " towards " + from);
            transform.position = Vector2.MoveTowards(transform.position, from, patrolSpeed * Time.deltaTime);
            return;
        } 
        if ( Math.Abs(transform.position.y - to.y) > 0.1f && !jumpingPhase2) {
            jumpingPhase1 = true;
            body.gravityScale = 0;
            // Debug.Log("levitating from " + transform.position + " to " + to);
            transform.position = Vector2.MoveTowards(transform.position, to, jumpForce * Time.deltaTime);
        } else {
            jumpingPhase2 = true;
            // Debug.Log("ending the jump from" + transform.position + " to " + after);
            transform.position = Vector2.MoveTowards(transform.position, after, patrolSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, after) < 0.1f) {
                isJumping = false;
                jumpingPhase1 = false;
                jumpingPhase2 = false;
                body.gravityScale = 9.8f;
            }
            counterAfterBug += Time.deltaTime;
        }
    }

    #endregion

    #region Help functions

    private void UpdatePath() {
        if (isJumping) {
            return;
        }
        var path = Pathfinder.instance.getNextThreeTiles(transform.position);
        if (path == null || path.Count == 0) {
            return;
        }
        pathToFollow = path;
    }

    void Flip() {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

     private void OnDrawGizmosSelected() {
        Vector3 from = new Vector3(initialPosition.x - patrolRange, initialPosition.y + 1, initialPosition.z);
        Vector3 to = new Vector3(initialPosition.x + patrolRange, initialPosition.y + 1, initialPosition.z);     
        Gizmos.DrawLine(from, to);

        if (Pathfinder.instance != null) {
            // var path = Pathfinder.instance.getPath2( body.position );
            // var size = new Vector2(1f, 1f);
            // Gizmos.color = Color.red;
            // for (int i = 0; i < path.Count - 1; i++) {
            //     var check = path[i] - path[i + 1];
            //     Direction dir;
            //     if (check.y == 0 && check.x > 0) {
            //         dir = Direction.Left;
            //     } else if (check.y == 0 && check.x < 0) {
            //         dir = Direction.Right;
            //     } else if (check.x == 0 && check.y > 0) {
            //         dir = Direction.Down;
            //     } else {
            //         dir = Direction.Up;
            //     }
            //     DrawArrow(Pathfinder.instance.map.CellToWorld(path[i]) + new Vector3(0.5f, 0.5f), size, dir);
            // }
            DrawPathToTarget();
        }

    }

    private void DrawPathToTarget() {
        Gizmos.color = Color.blue;
        Pathfinder.instance.DrawPath(body.position);
    }

    
    private void DrawArrow(Vector3 center, Vector2 size, Direction direction)
    {
        // Центр квадрата
        Vector3 start = center;

        // Длина стрелки и ее размер относительно стороны квадрата
        float arrowLength = Mathf.Min(size.x, size.y) * 0.5f;
        float arrowWidth = arrowLength * 0.2f;

        // Направление стрелки
        Vector3 end = center;
        Vector3 leftWing, rightWing;

        switch (direction)
        {
            case Direction.Up:
                end += Vector3.up * arrowLength;
                leftWing = end + (Vector3.left + Vector3.down) * arrowWidth;
                rightWing = end + (Vector3.right + Vector3.down) * arrowWidth;
                break;

            case Direction.Down:
                end += Vector3.down * arrowLength;
                leftWing = end + (Vector3.left + Vector3.up) * arrowWidth;
                rightWing = end + (Vector3.right + Vector3.up) * arrowWidth;
                break;

            case Direction.Left:
                end += Vector3.left * arrowLength;
                leftWing = end + (Vector3.up + Vector3.right) * arrowWidth;
                rightWing = end + (Vector3.down + Vector3.right) * arrowWidth;
                break;

            case Direction.Right:
                end += Vector3.right * arrowLength;
                leftWing = end + (Vector3.up + Vector3.left) * arrowWidth;
                rightWing = end + (Vector3.down + Vector3.left) * arrowWidth;
                break;

            default:
                return;
        }

        // Рисуем стрелку
        Gizmos.DrawLine(start, end);
        Gizmos.DrawLine(end, leftWing);
        Gizmos.DrawLine(end, rightWing);
    }

    #endregion

}
