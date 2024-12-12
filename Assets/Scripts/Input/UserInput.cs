using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;

    [HideInInspector] public Controls controls;
    [HideInInspector] public Vector2 moveInput;

    [HideInInspector] public Vector3 initialPosition {get; private set;}

    private void Awake() {
        if ( instance == null ) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        initialPosition = GameObject.FindGameObjectWithTag("Player").transform.position; // Поиск игрока по тегу

        controls = new Controls();
        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }
}
