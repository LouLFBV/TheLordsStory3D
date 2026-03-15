using UnityEngine;
public class PlayerAirborneState : PlayerState
{
    public PlayerAirborneState(PlayerController player) : base(player) { }
    public override void Enter()
    {
        base.Enter();
        player.Animator.applyRootMotion = false; // On laisse le Rigidbody gérer la trajectoire
        player.Animator.SetBool("Grounded", false);
    }
    public override void Update()
    {
        base.Update();

        Vector2 input = player.Input.MoveInput;
        if (input != Vector2.zero)
        {
            // On récupère la direction du mouvement basée sur la caméra
            Vector3 moveDir = player.Motor.GetDirectionFromInput(input);

            // On définit une vitesse horizontale (ex: 5f)
            float airSpeed = 5f;
            Vector3 targetVelocity = moveDir * airSpeed;

            // On applique au Rigidbody en gardant sa vitesse de chute (Y)
            player.Rigidbody.linearVelocity = new Vector3(
                targetVelocity.x,
                player.Rigidbody.linearVelocity.y,
                targetVelocity.z
            );

            // On fait pivoter le personnage vers cette direction
            player.Motor.RotateTowardsInput(input);
        }

        if (player.Motor.IsGrounded() && player.Rigidbody.linearVelocity.y <= 0.1f)
        {
            // Transition vers Idle ou Move selon l'input
            player.StateMachine.ChangeState(player.Input.MoveInput != Vector2.zero
                ? PlayerStateType.Move
                : PlayerStateType.Idle);
        }
    }
}