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
    public bool InventoryPressed { get; private set; }
    public bool MenuPressed { get; private set; }
    public Vector2 NavigationInput { get; private set; }
    public bool SubmitPressed { get; private set; }
    public bool CancelPressed { get; private set; }
    public bool CloseMenuPressed { get; private set; }
    public bool CloseInventoryPressed { get; private set; }

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

        input.actions["Inventory"].performed += ctx => InventoryPressed = true;
        input.actions["Inventory"].canceled += ctx => InventoryPressed = false;

        input.actions["Menu"].performed += ctx => MenuPressed = true;
        input.actions["Menu"].canceled += ctx => MenuPressed = false;


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

        // UI

        input.actions["Navigate"].performed += ctx => NavigationInput = ctx.ReadValue<Vector2>(); ;
        input.actions["Navigate"].canceled += ctx => NavigationInput = Vector2.zero;

        input.actions["Submit"].performed += ctx => SubmitPressed = true;
        input.actions["Submit"].canceled += ctx => SubmitPressed = false;

        input.actions["Cancel"].performed += ctx => CancelPressed = true;
        input.actions["Cancel"].canceled += ctx => CancelPressed = false;

        input.actions["CloseMenu"].performed += ctx => CloseMenuPressed = true;
        input.actions["CloseMenu"].canceled += ctx => CloseMenuPressed = false;

        input.actions["CloseInventory"].performed += ctx => CloseInventoryPressed = true;
        input.actions["CloseInventory"].canceled += ctx => CloseInventoryPressed = false;
    }

    // --- LA MÉTHODE PRO ---
    public void SwitchActionMap(string mapName)
    {
        input.SwitchCurrentActionMap(mapName);

        // Reset des valeurs pour éviter que le perso continue de courir 
        // si on ouvre l'inventaire en plein sprint
        MoveInput = Vector2.zero;
        AttackPressed = false;
        RollPressed = false;
        SprintHeld = false;
        CrouchHeld = false;
        AimHeld = false;
        JumpPressed = false;
        Weapon1Pressed = false;
        Weapon2Pressed = false;
        Object1Pressed = false;
        Object2Pressed = false;
    }
    public void UseInventoryInput() => InventoryPressed = false;
    public void UseMenuInput() => MenuPressed = false;
    public void UseCloseMenuInput() => CloseMenuPressed = false;
    public void UseCloseInventoryInput() => CloseInventoryPressed = false;
    public void UseWeapon1Pressed() => Weapon1Pressed = false;
    public void UseWeapon2Pressed() => Weapon2Pressed = false;
    public void UseObject1Pressed() => Object1Pressed = false;
    public void UseObject2Pressed() => Object2Pressed = false;
}