public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController controller) : base(controller) { }
    //public override void Enter()
    //{
    //    base.Enter();
    //    Controller.Animator.SetBool("isMoving", false);
    //    Controller.Agent.isStopped = true;
    //}
    //public override void Update()
    //{
    //    base.Update();
    //    // Si le joueur est dans la zone de détection, on passe à l'état de suivi
    //    if (Vector3.Distance(Controller.transform.position, Controller.Target.position) <= Controller.DetectionRadius)
    //    {
    //        StateMachine.ChangeState(Controller.FollowState);
    //    }
    //}
    //public override void Exit()
    //{
    //    base.Exit();
    //    Controller.Agent.isStopped = false;
    //}
}