using UnityEngine;
public class PlayerJumpState : PlayerAirborneState
{
    private float jumpForce = 6f;

    public PlayerJumpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();

        // 1. On récupère la direction dans laquelle le joueur regarde
        Vector3 jumpDirection = player.transform.forward;

        // 2. On vérifie si le joueur est en train de bouger (input)
        float speed = player.Input.MoveInput.magnitude > 0.1f ? 5f : 0f; // 5f est ta vitesse de saut horizontal

        // 3. On applique l'impulsion (Verticale + Horizontale)
        Vector3 force = (jumpDirection * speed) + (Vector3.up * jumpForce);

        // On remplace la vélocité actuelle
        player.Rigidbody.linearVelocity = force;

        //player.Stamina.Spend(15f);
        player.Animator.SetTrigger("Jump_");
    }

    public override void Update()
    {
        base.Update();

        // Si on commence à redescendre, on passe en Fall
        if (player.Rigidbody.linearVelocity.y < -0.1f)
        {
            player.StateMachine.ChangeState(PlayerStateType.Fall);
        }
    }
}