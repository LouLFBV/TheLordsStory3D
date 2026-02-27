using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public AttackSO lightAttack1;
    public PlayerGroundedState(PlayerController player) : base(player) { }
    public override void Enter()
    {
        // On force l'Animator à revenir sur l'Idle du Layer 0 
        // et on fait un fondu pour effacer l'attaque du Layer 10
        player.Animator.CrossFadeInFixedTime("Idle", 0.2f, 0);

        // Si tu utilises un Layer 10 avec du poids (Weight), 
        // il faut parfois "vider" le layer 10 pour qu'il ne fige pas le perso
        player.Animator.SetLayerWeight(10, 0f);
    }
    public override void Update()
    {// Si on perd le sol, on force le passage en FallState
        if (!player.Motor.IsGrounded())
        {
            player.StateMachine.ChangeState(PlayerStateType.Fall); // à créer
            return;
        }
        if (player.Input.AttackPressed && player.Stamina.HasStamina())
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