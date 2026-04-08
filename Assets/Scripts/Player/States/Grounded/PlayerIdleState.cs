using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.Animator.applyRootMotion = true;
        player.Animator.SetFloat("Speed", 0f);
        player.Animator.SetBool("IsFalling", false);
    }

    public override void Update()
    {
        base.Update();
        if (player.Input.MoveInput != Vector2.zero)
            player.StateMachine.ChangeState(PlayerStateType.Move);
    }
}