using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    public Vector2 mouseLook { get; private set; }
    public Vector2 gamepadLook { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool RollPressed { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool CrouchHeld { get; private set; }
    public bool AimHeld { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool Weapon1Pressed { get; private set; }
    public bool Weapon2Pressed { get; private set; }
    public bool Object1Pressed { get; private set; }
    public bool Object2Pressed { get; private set; }

    private PlayerInput input;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        input.actions["Move"].performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        input.actions["Move"].canceled += ctx => MoveInput = Vector2.zero;

        input.actions["Attack"].performed += ctx => AttackPressed = true;
        input.actions["Attack"].canceled += ctx => AttackPressed = false;

        input.actions["ForwardRool"].performed += ctx => RollPressed = true;
        input.actions["ForwardRool"].canceled += ctx => RollPressed = false;

        input.actions["Sprint"].performed += ctx => SprintHeld = true;
        input.actions["Sprint"].canceled += ctx => SprintHeld = false;

        input.actions["Crouch"].performed += ctx => CrouchHeld = true;
        input.actions["Crouch"].canceled += ctx => CrouchHeld = false;

        input.actions["Jump"].performed += ctx => JumpPressed = true;
        input.actions["Jump"].canceled += ctx => JumpPressed = false;


        // Souris
        input.actions["LookMouse"].performed += ctx => mouseLook = ctx.ReadValue<Vector2>();
        input.actions["LookMouse"].canceled += _ => mouseLook = Vector2.zero;

        // Gamepad
        input.actions["LookGamepad"].performed += ctx => gamepadLook = ctx.ReadValue<Vector2>();
        input.actions["LookGamepad"].canceled += _ => gamepadLook = Vector2.zero;


        input.actions["Aim"].performed += ctx => AimHeld = true;
        input.actions["Aim"].canceled += ctx => AimHeld = false;

        //Palette
        input.actions["Weapon1"].performed += ctx => Weapon1Pressed = true;
        input.actions["Weapon1"].canceled += ctx => Weapon1Pressed = false;

        input.actions["Weapon2"].performed += ctx => Weapon2Pressed = true;
        input.actions["Weapon2"].canceled += ctx => Weapon2Pressed = false;

        input.actions["Object1"].performed += ctx => Object1Pressed = true;
        input.actions["Object1"].canceled += ctx => Object1Pressed = false;

        input.actions["Object2"].performed += ctx => Object2Pressed = true;
        input.actions["Object2"].canceled += ctx => Object2Pressed = false;
    }
}