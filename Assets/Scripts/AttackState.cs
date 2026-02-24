//public class AttackState : PlayerState
//{
//    private bool attackFinished;

//    public AttackState(PlayerController player) : base(player) { }
//    public override void Enter()
//    {
//        player.Combat.PerformAttack();
//        attackFinished = false;
//    }
//    public override void Exit()
//    {
//        player.Combat.EndAttack();
//    }

//    public override void Update()
//    {
//        if (player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
//        {
//            player.StateMachine.ChangeState(player.IdleState);
//        }
//        if (player.Input.AttackPressed)
//            player.Combat.TryQueueNextAttack();
//    }

//    public void OnAttackFinished()
//    {
//        attackFinished = true;
//    }
//}