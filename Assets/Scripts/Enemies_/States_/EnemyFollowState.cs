using UnityEngine;

public class EnemyFollowState : EnemyState
{
    public EnemyFollowState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        agent.speed = 4f; // Vitesse de poursuite (plus rapide que walkSpeed)
        agent.isStopped = false;
    }

    public override void Update()
    {
        if (enemy.Target == null)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Idle);
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.Target.position);

        // 1. Si on est assez proche pour attaquer
        if (distance <= enemy.AttackRadius)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Attack);
            return;
        }

        // 2. Si le joueur s'est trop éloigné
        if (distance > enemy.DetectionRadius * 1.5f)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Idle);
            return;
        }

        // 3. Logique de transition vers l'Orbite
        // On ne change d'état QUE si on a la permission ET qu'on est à la bonne distance
        if (distance <= enemy.AIManager.OrbitDistance + 1f && enemy.AIManager.HasPermission(EnemyStateType.Orbit))
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Orbit);
            return; // On sort de l'Update pour ne pas exécuter le SetDestination ci-dessous
        }

        // 4. Si on n'est pas en orbite (Squelette ou trop loin), on fonce !
        agent.SetDestination(enemy.Target.position);

        // Animation
        enemy.Animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed, 0.1f, Time.deltaTime);
    }

    public override void Exit()
    {
        // On arrête l'agent proprement en quittant l'état
        if (agent.isActiveAndEnabled)
            agent.isStopped = true;
    }
}