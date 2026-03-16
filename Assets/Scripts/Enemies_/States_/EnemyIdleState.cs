public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        agent.isStopped = true;
        enemy.Animator.SetFloat("Speed", 0);
    }

    public override void Update() { } // Le changement vers Follow est géré par le Controller pour l'instant

    public override void Exit() { }
}