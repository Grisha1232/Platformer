using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class ShadowCatMovement : DefaultMovement
{
    /// <summary>
    /// Маска земли
    /// </summary>
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float exitChaseRange = 20f;


    /// <summary>
    /// Находится ли Теневой кот в тени
    /// </summary>
    [HideInInspector] public bool isInShadow {get; private set;}= false;

    /// <summary>
    /// Повернут ли Теневой кот вправо
    /// </summary>
    private bool isFacingRight = true;

    /// <summary>
    /// This methods calls every time game is start
    /// </summary>
    private void Awake() {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        attackScript = GetComponent<ShadowCatAttack>();
        
    }
    
    protected override void Update()
    {
        Patrol();
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
        // Движение в пределах патрулирования
        float moveDirection = isFacingRight ? 1 : -1;
        body.velocity = new Vector2(moveDirection * patrolSpeed, body.velocity.y);
        
        // Запуск анимации патрулирования
        // animator.SetBool("isMoving", true);
    }

    #endregion

    #region Help functions
    
    void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    #endregion

}
