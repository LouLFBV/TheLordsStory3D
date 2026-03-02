using UnityEngine;

public class MoveState : GroundedState
{
    private int hHash = Animator.StringToHash("H");
    private int vHash = Animator.StringToHash("V");
    private int speedHash = Animator.StringToHash("Speed");

    private Vector2 cachedInput;

    // Vitesses
    private float walkSpeed = 2f;
    private float runSpeed = 4f;
    private float sprintSpeed = 7f;

    private bool isSprinting;
    private bool changedFOV;

    public MoveState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        Debug.Log("Enter: Move");
        base.Enter();
        player.Animator.applyRootMotion = true;
    }

    public override void Update()
    {
        base.Update();
        Vector2 input = player.Input.MoveInput;
        cachedInput = input;

        if (input == Vector2.zero)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
            return;
        }

        // Rotation vers l'input
        player.Motor.RotateTowardsInput(input);

        // Sprint
        isSprinting = CanSprint(input);

        float animSpeed = isSprinting ? sprintSpeed : runSpeed;

        // FOV sprint
        if (isSprinting && !changedFOV)
        {
            ThirdPersonCameraController.Instance.SetFOV(ThirdPersonCameraController.Instance.SprintFOV);
            changedFOV = true;
        }
        else if (!isSprinting && changedFOV)
        {
            ThirdPersonCameraController.Instance.ResetFOV();
            changedFOV = false;
        }

        // Paramètres Animator pour Root Motion
        player.Animator.SetFloat(hHash, input.x, 0.1f, Time.deltaTime);
        player.Animator.SetFloat(vHash, input.y, 0.1f, Time.deltaTime);
        player.Animator.SetFloat(speedHash, input.magnitude * (animSpeed / sprintSpeed), 0.1f, Time.deltaTime);

        // Stamina
        if (isSprinting)
            player.Stamina.Spend(player.Stamina.consommationRate * Time.deltaTime);

        if (player.Input.SprintHeld) // Le joueur VEUT sprinter
        {
            if (isSprinting)
            {
                // Consommation normale
                player.Stamina.Spend(player.Stamina.consommationRate * Time.deltaTime);
            }
            else if (!player.Stamina.HasStamina())
            {
                // Le joueur appuie mais HasStamina est faux (épuisé ou vide)
                // On force l'appel à Spend(0) ou une méthode de feedback
                player.Stamina.RequestEmptyFeedback();
            }
        }
    }

    public override void FixedUpdate()
    {
        // On continue à orienter le personnage vers l'input
        player.Motor.RotateTowardsInput(cachedInput);
    }

    private bool CanSprint(Vector2 input)
    {
        return input.magnitude > 0.1f
               && player.Input.SprintHeld
               && player.Stamina.HasStamina();
    }
}