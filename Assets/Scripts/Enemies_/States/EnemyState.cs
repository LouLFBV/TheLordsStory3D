public abstract class EnemyState : State
{
    protected EnemyController enemy;
    protected UnityEngine.AI.NavMeshAgent agent; // Petit bonus confort

    protected EnemyState(EnemyController enemy)
    {
        this.enemy = enemy;
        this.agent = enemy.Agent;
    }
}