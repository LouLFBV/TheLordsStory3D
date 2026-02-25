using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public AttackSO lightAttack1;
    public PlayerGroundedState(PlayerController player) : base(player) { }
    public override void Update()
    {// Si on perd le sol, on force le passage en FallState
        if (!player.Motor.IsGrounded())
        {
            player.StateMachine.ChangeState(PlayerStateType.Fall); // à créer
            return;
        }
        if (player.Input.AttackPressed && player.HasStamina())
        {
            if (player.StateMachine.CurrentState.GetType() != typeof(AttackState))
            {
                var attackState = (AttackState)player.StateMachine.GetState(PlayerStateType.Attack);
                attackState.SetAttack(player.defaultLightAttack);
                player.StateMachine.ChangeState(PlayerStateType.Attack);
            }
        }
    }
}