using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolkitInput : MonoBehaviour
{
    public InputActionAsset controls;

    private InputAction move;
    private InputAction look;
    private InputAction jump;

    void Awake()
    {
        move = controls.FindAction("Move");
        look = controls.FindAction("Look");
        jump = controls.FindAction("Jump");
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    public Vector2 Move => move.ReadValue<Vector2>();
    public Vector2 Look => look.ReadValue<Vector2>();
    public bool JumpPressed => jump.WasPressedThisFrame();
}