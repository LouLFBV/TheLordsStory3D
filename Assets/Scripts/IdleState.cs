using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.Animator.applyRootMotion = true;
        player.Animator.SetFloat("Speed", 0f);
    }

    public override void Update()
    {
        if (player.Input.MoveInput != Vector2.zero)
            player.StateMachine.ChangeState(PlayerStateType.Move);

        else if (player.Input.AttackPressed)
            player.StateMachine.ChangeState(PlayerStateType.Attack);

        //else if (player.Input.RollPressed && player.Stamina.CanSpend(20))
        //    player.StateMachine.ChangeState(PlayerStateType.Roll);
    }
}