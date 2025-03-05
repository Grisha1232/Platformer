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

    /// <summary>
    /// Находится ли Теневой кот в тени
    /// </summary>
    [HideInInspector] public bool isInShadow {get; private set;}= false;


    /// <summary>
    /// This methods calls every time game is start
    /// </summary>
    private void Awake() {
        base.Start();
        attackScript = GetComponent<ShadowCatAttack>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    
    protected override void Update()
    {
        Patrol();
        
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
            indicatorAttack.SetActive(false);
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


    #endregion

    #region Help functions

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
