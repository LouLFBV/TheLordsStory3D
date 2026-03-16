public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(EnemyController controller) : base(controller) { }
    //public override void Enter()
    //{
    //    base.Enter();
    //    Controller.Animator.SetBool("isMoving", true);
    //}
    //public override void Update()
    //{
    //    base.Update();
    //    // Logique de patrouille (ex: suivre un chemin prédéfini)
    //    // Si le joueur est dans la zone de détection, on passe à l'état de suivi
    //    if (Vector3.Distance(Controller.transform.position, Controller.Target.position) <= Controller.DetectionRadius)
    //    {
    //        StateMachine.ChangeState(Controller.FollowState);
    //    }
    //}
}