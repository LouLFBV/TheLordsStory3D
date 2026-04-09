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
        // On garde la base
        Vector2 input = player.Input.MoveInput;
        float airSpeed = 5f;

        if (input != Vector2.zero)
        {
            Vector3 moveDir = player.Motor.GetDirectionFromInput(input);
            Vector3 targetVelocity = moveDir * airSpeed;

            // --- DÉTECTION DE MUR INTELLIGENTE ---
            // On check devant
            if (Physics.Raycast(player.transform.position + Vector3.up * 1f, moveDir, out RaycastHit wallHit, 0.8f, ~0, QueryTriggerInteraction.Ignore))
            {
                // 1. Calcul de la direction de glisse (on projette le mouvement sur le plan du mur)
                // Cela empêche la force de pousser "vers le haut" ou "à travers"
                Vector3 frictionFreeMove = Vector3.ProjectOnPlane(targetVelocity, wallHit.normal);

                // 2. On force la direction vers le bas ou l'horizontale, jamais vers le haut !
                float verticalVel = player.Rigidbody.linearVelocity.y;
                if (verticalVel > 0) verticalVel = 0;

                player.Rigidbody.linearVelocity = new Vector3(frictionFreeMove.x, verticalVel, frictionFreeMove.z);
            }
            else
            {
                // Mouvement aérien normal
                player.Rigidbody.linearVelocity = new Vector3(targetVelocity.x, player.Rigidbody.linearVelocity.y, targetVelocity.z);
            }

            player.Motor.RotateTowardsInput(input);
        }
        else
        {
            // Freinage progressif
            Vector3 currentVel = player.Rigidbody.linearVelocity;
            player.Rigidbody.linearVelocity = new Vector3(currentVel.x * 0.9f, currentVel.y, currentVel.z * 0.9f);
        }

        // --- SORTIE D'ÉTAT ---
        if (player.Motor.IsGrounded())
        {
            player.StateMachine.ChangeState(player.Input.MoveInput != Vector2.zero ? PlayerStateType.Move : PlayerStateType.Idle);
        }
    }

}