public class CrouchState : GroundedState
{
    public CrouchState(PlayerController player) : base(player){}

    public override void Enter()
    {
        base.Enter();
        player.Animator.SetBool("isCrouching", true);
        //player.Motor.SetCrouch(true);
    }
    public override void Exit() { base.Exit();
        player.Animator.SetBool("isCrouching", false);
       // player.Motor.SetCrouch(false);
    }
}