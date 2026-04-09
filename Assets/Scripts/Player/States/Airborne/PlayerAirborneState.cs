using UnityEngine;
public class PlayerAirborneState : PlayerState
{
    public PlayerAirborneState(PlayerController player) : base(player) { }
    public override void Enter()
    {
        base.Enter();
        player.Animator.applyRootMotion = false; // On laisse le Rigidbody gérer la trajectoire
        player.Animator.SetBool("Grounded", false);
        player.Motor.SetFriction(false);
    }
    public override void Update()
    {
        base.Update();

        Vector2 input = player.Input.MoveInput;
        float airSpeed = 5f;

        // --- LOGIQUE DE MOUVEMENT HORIZONTAL ---
        if (input != Vector2.zero)
        {
            Vector3 moveDir = player.Motor.GetDirectionFromInput(input);
            Vector3 targetVelocity = moveDir * airSpeed;

            player.Rigidbody.linearVelocity = new Vector3(
                targetVelocity.x,
                player.Rigidbody.linearVelocity.y, // On préserve la chute
                targetVelocity.z
            );

            player.Motor.RotateTowardsInput(input);
        }
        else
        {
            // SI PAS D'INPUT : On stoppe net le mouvement horizontal (X et Z)
            // Mais on laisse la gravité (Y) agir normalement
            player.Rigidbody.linearVelocity = new Vector3(
                0,
                player.Rigidbody.linearVelocity.y,
                0
            );
        }

        // --- DÉTECTION DU SOL ---
        if (player.Motor.IsGrounded() && player.Rigidbody.linearVelocity.y <= 0.5f)
        {
            player.StateMachine.ChangeState(player.Input.MoveInput != Vector2.zero
                ? PlayerStateType.Move
                : PlayerStateType.Idle);
        }
    }

}