using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;

    [HideInInspector] public Controls controls;
    [HideInInspector] public Vector2 moveInput;

    private void Awake() {
        if ( instance == null ) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        controls = new Controls();
        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        if ( controls != null ) {
            controls.Disable();
        }
    }

    public Dictionary<string, List<string>> GetAllBindings() {
        var bindingMap = new Dictionary<string, List<string>>();

        foreach (var actionMap in controls.asset.actionMaps) {
            foreach (var action in actionMap.actions) {
                List<string> bindings = new();
                foreach(var binding in action.bindings) {
                    bindings.Add(binding.effectivePath);
                }
                bindingMap[action.name] = bindings;
            }
        }

        return bindingMap;
    }

    public void PrintAllBindings()
    {
        var allBindings = GetAllBindings();

        foreach (var actionName in allBindings.Keys)
        {
            Debug.Log($"Action: {actionName}");
            foreach (var binding in allBindings[actionName])
            {
                Debug.Log($"  Binding: {binding}");
            }
        }
    }
}
