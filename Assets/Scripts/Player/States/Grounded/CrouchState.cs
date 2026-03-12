using UnityEngine;

public class CrouchState : GroundedState
{
    public CrouchState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.Animator.SetBool(AnimatorHashes.crouchBool, true);

        // On réduit le collider pour passer sous des obstacles
        player.Motor.StartRollCollider();
    }

    public override void Update()
    {
        base.Update();

        // 1. Sortie de l'accroupissement
        // Si on appuie à nouveau sur Crouch ou si on Sprint
        if (player.Input.CrouchPressed || player.Input.SprintHeld)
        {
            Debug.Log("Exiting crouch state");
            player.Input.UseCrouchInput();
            player.StateMachine.ChangeState(PlayerStateType.Idle);
            return;
        }

        // 2. Gestion du mouvement (Sneak)
        Vector2 input = player.Input.MoveInput;
        player.Animator.SetFloat(AnimatorHashes.hHash, input.x, 0.1f, Time.deltaTime);
        player.Animator.SetFloat(AnimatorHashes.vHash, input.y, 0.1f, Time.deltaTime);
        player.Animator.SetFloat(AnimatorHashes.speedHash, input.magnitude, 0.2f, Time.deltaTime);

        // On oriente le personnage vers l'input comme dans MoveState
        if (input.sqrMagnitude > 0.1f)
        {
            player.Motor.RotateTowardsInput(input);
        }

    }
    // ON ÉTEINT LA LOGIQUE DU PARENT
    protected override void HandleCrouchInput()
    {
        // On laisse vide ou on met un commentaire. 
        // Cela empêche le parent de détecter le "CrouchPressed" pour re-entrer dans l'état.
    }
    public override void Exit()
    {
        base.Exit();
        player.Animator.SetBool(AnimatorHashes.crouchBool, false);

        // On remet le collider à sa taille normale
        player.Motor.EndRollCollider();
    }
}