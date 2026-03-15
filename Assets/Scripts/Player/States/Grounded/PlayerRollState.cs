using UnityEngine;

public class PlayerRollState : PlayerState
{
    private bool isRollFinished;

    public PlayerRollState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        isRollFinished = false;

        // 1. Stamina
        player.Stamina.Spend(20f);

        // 2. Animation & Root Motion
        player.Animator.applyRootMotion = true;
        player.Animator.SetTrigger(AnimatorHashes.rollTrigger);

        // 3. Collider & Invincibilité
        player.Motor.StartRollCollider();
        player.Health.SetInvulnerable(true);

        // 4. Direction
        RotateRollDirection();
    }

    public override void Update()
    {
        if (isRollFinished)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.Motor.EndRollCollider();
        player.Health.SetInvulnerable(false);
    }

    // Cette méthode sera appelée par le PlayerController via un Animation Event
    public void OnRollAnimationEnd()
    {
        isRollFinished = true;
    }

    private void RotateRollDirection()
    {
        Vector2 input = player.Input.MoveInput;
        Vector3 rollDir = input.sqrMagnitude > 0.1f
            ? player.Motor.GetDirectionFromInput(input)
            : player.transform.forward;

        player.transform.rotation = Quaternion.LookRotation(rollDir);
    }
}