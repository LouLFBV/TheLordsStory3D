using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerController player) : base(player) { }

    public override void Update()
    {
        if (player.Input.MoveInput == Vector2.zero)
            player.StateMachine.ChangeState(player.IdleState);

        if (player.Input.AttackPressed)
            player.StateMachine.ChangeState(player.AttackState);

        if (player.Input.RollPressed && player.Stamina.CanSpend(20))
            player.StateMachine.ChangeState(player.RollState);
    }

    public override void FixedUpdate()
    {
        player.Motor.Move(player.Input.MoveInput);
    }
}