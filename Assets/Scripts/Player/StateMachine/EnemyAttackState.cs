public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(EnemyController controller) : base(controller) { }
    //public override void Enter()
    //{
    //    base.Enter();
    //    Controller.Animator.SetTrigger("attack");
    //}
    //public override void Update()
    //{
    //    base.Update();
    //
    //    float distanceToPlayer = Vector3.Distance(Controller.transform.position, Controller.Target.position);
    //
    //    // Si le joueur sort de la zone d'attaque, on revient à l'état de suivi
    //    if (distanceToPlayer > Controller.AttackRadius)
    //    {
    //        StateMachine.ChangeState(Controller.FollowState);
    //    }
    //}
    public void OnAnimationFinished() { }        
}