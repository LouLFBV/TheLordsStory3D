public class AimState : GroundedState
{
    public AimState(PlayerController player) : base(player) { }
    public override void Enter()
    {
        base.Enter();
        // Activer l'animation de visée
        player.Animator.SetBool("IsAiming", true);
    }
    public override void Update()
    {
        base.Update();
        // Rester en AimState tant que le bouton de visée est maintenu
        if (!player.Input.AimHeld)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
    public override void Exit()
    {
        base.Exit();
        // Désactiver l'animation de visée
        player.Animator.SetBool("IsAiming", false);
    }
}