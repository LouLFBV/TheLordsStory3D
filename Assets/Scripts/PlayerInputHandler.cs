using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool RollPressed { get; private set; }
    public bool SprintHeld { get; private set; }

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

        input.actions["Roll"].performed += ctx => RollPressed = true;
        input.actions["Roll"].canceled += ctx => RollPressed = false;

        input.actions["Sprint"].performed += ctx => SprintHeld = true;
        input.actions["Sprint"].canceled += ctx => SprintHeld = false;
    }
}