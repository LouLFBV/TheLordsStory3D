public class FallState : PlayerState
{
    public FallState(PlayerController player) : base(player) { }
    public override void Enter()
    {
        base.Enter();
        // Ici, tu pourrais jouer une animation de chute ou déclencher des effets visuels
        player.Animator.SetTrigger("Fall");
    }
    public override void Update()
    {
        base.Update();
        // Vérifie si le joueur touche le sol pour revenir à l'état Idle ou Move
        if (player.Motor.IsGrounded())
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
    public override void Exit()
    {
    }
}