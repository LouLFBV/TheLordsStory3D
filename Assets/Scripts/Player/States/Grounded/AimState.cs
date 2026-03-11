using UnityEngine;

public class AimState : GroundedState
{
    private readonly int aimBool = Animator.StringToHash("Aim");

    public AimState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.Animator.SetBool(aimBool, true);
        UIManagerSystem.Instance.ShowCrosshair(true);
        ThirdPersonCameraController.Instance.SetAimState(true);
    }

    public override void Update()
    {
        base.Update();

        // Sortie de l'état si on relâche le bouton
        if (!player.Input.AimHeld)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
            return;
        }

        // --- GESTION DE L'ANIMATION ---
        // En visée (strafe), on envoie l'input X et Y brut à l'animator 
        // pour faire des pas de côté (Blend Tree 2D)
        player.Animator.SetFloat("H", player.Input.MoveInput.x, 0.1f, Time.deltaTime);
        player.Animator.SetFloat("V", player.Input.MoveInput.y, 0.1f, Time.deltaTime);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // 1. ROTATION : Toujours face à la caméra
        RotateTowardsCamera();

        // 2. DÉPLACEMENT : Utilise ta méthode existante pour avoir la direction relative cam
        Vector3 moveDir = player.Motor.GetDirectionFromInput(player.Input.MoveInput);
    }

    public override void Exit()
    {
        base.Exit();
        player.Animator.SetBool(aimBool, false);
        UIManagerSystem.Instance.ShowCrosshair(false);
        ThirdPersonCameraController.Instance.SetAimState(false);
    }

    private void RotateTowardsCamera()
    {
        // On récupère le forward de la caméra via ton controller
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        if (camForward != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(camForward);
            // On utilise Slerp pour une rotation fluide
            player.Rigidbody.MoveRotation(Quaternion.Slerp(player.Rigidbody.rotation, targetRot, 15f * Time.fixedDeltaTime));
        }
    }
}