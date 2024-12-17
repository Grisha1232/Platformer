using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class ShadowCatMovement : DefaultMovement
{
    /// <summary>
    /// Маска земли
    /// </summary>
    [SerializeField] private LayerMask groundLayer;

    /// <summary>
    /// Находится ли Теневой кот в тени
    /// </summary>
    [HideInInspector] public bool isInShadow {get; private set;}= false;

    private float findPathCooldownCounter = 99f;

    /// <summary>
    /// This methods calls every time game is start
    /// </summary>
    private void Awake() {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        attackScript = GetComponent<ShadowCatAttack>();
        SetTarget(player.transform);
        
    }
    
    protected override void Update()
    {
        Patrol();
        if (findPathCooldownCounter > 1f) {
            findPathCooldownCounter = 0;
            SetSeeker(transform);
            SetTarget(player.transform);
            FindPath(jumpForce);
        }
        findPathCooldownCounter += Time.deltaTime;
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

        
        List<Node> foundPath = GetPath();
        if (foundPath != null) {
            Gizmos.color = Color.green;
            foreach (Node node in foundPath) {
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (NavigationGrid.instance.GetNodeSize() - 0.1f));
            }
        }
    }

    #endregion

}
