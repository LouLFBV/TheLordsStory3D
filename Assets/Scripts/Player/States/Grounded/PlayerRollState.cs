using UnityEngine;

public class PlayerRollState : PlayerState
{
    private bool isRollFinished; 
    private Vector3 rollDirection;

    public PlayerRollState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        isRollFinished = false;

        // 1. Stamina
        player.Stamina.Spend(20f);

        // 2. Animation & Root Motion
        player.Animator.applyRootMotion = false;
        player.Animator.SetTrigger(AnimatorHashes.rollTrigger);

        // 3. Collider & Invincibilité
        player.Motor.StartRollCollider();
        player.Health.SetInvulnerable(true);

        // 4. Direction
        CalculateRollDirection();
    }

    public override void Update()
    {
        if (isRollFinished)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void FixedUpdate() // Utilise FixedUpdate pour la physique
    {
        base.FixedUpdate();

        // On applique une vélocité constante vers l'avant pendant la roulade
        // Cela permet au Rigidbody de détecter les murs correctement
        if (!isRollFinished)
        {
            Vector3 velocity = rollDirection * player.rollForce;
            velocity.y = player.Rigidbody.linearVelocity.y; // On garde la gravité actuelle
            player.Rigidbody.linearVelocity = velocity; 
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.Motor.EndRollCollider();
        player.Health.SetInvulnerable(false);
        player.Animator.applyRootMotion = true;
    }

    // Cette méthode sera appelée par le PlayerController via un Animation Event
    public void OnRollAnimationEnd()
    {
        isRollFinished = true;
    }

    private void CalculateRollDirection()
    {
        Vector2 input = player.Input.MoveInput;

        // Si le joueur ne touche pas au stick, on roule vers l'avant du perso
        rollDirection = input.sqrMagnitude > 0.1f
            ? player.Motor.GetDirectionFromInput(input)
            : player.transform.forward;

        // On oriente immédiatement le personnage
        player.transform.rotation = Quaternion.LookRotation(rollDirection);
    }
}