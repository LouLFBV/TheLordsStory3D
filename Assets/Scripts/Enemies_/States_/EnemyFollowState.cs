public class EnemyFollowState : EnemyState
{
    public EnemyFollowState(EnemyController controller) : base(controller) { }
    //public override void Enter()
    //{
    //    base.Enter();
    //    Controller.Animator.SetBool("isMoving", true);
    //}
    //public override void Update()
    //{
    //    base.Update();
    //    // Suivre le joueur
    //    Controller.Agent.SetDestination(Controller.Target.position);
    //
    //    float distanceToPlayer = Vector3.Distance(Controller.transform.position, Controller.Target.position);
    //
    //    // Si le joueur est dans la zone d'attaque, on passe à l'état d'attaque
    //    if (distanceToPlayer <= Controller.AttackRadius)
    //    {
    //        StateMachine.ChangeState(Controller.AttackState);
    //    }
    //}
}