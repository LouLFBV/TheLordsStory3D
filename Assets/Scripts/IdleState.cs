using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Update()
    {
        if (player.Input.MoveInput != Vector2.zero)
            player.StateMachine.ChangeState(player.MoveState);

        else if (player.Input.AttackPressed)
            player.StateMachine.ChangeState(player.AttackState);

        else if (player.Input.RollPressed && player.Stamina.CanSpend(20))
            player.StateMachine.ChangeState(player.RollState);
    }
}