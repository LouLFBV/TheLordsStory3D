using UnityEngine;

public class EnemyFollowState : EnemyState
{
    public EnemyFollowState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        agent.speed = enemy.enemyData.chaseSpeed;
        agent.isStopped = false;

        // TRÈS IMPORTANT : On remet la distance d'arrêt à presque 0 
        // pour qu'il accepte de foncer au contact du joueur.
        agent.stoppingDistance = 1.2f;
    }

    public override void Update()
    {
        if (enemy.target == null)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Idle);
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);



        if (enemy.AIManager.HasPermission(EnemyStateType.Orbit) && distance <= enemy.AIManager.OrbitDistance + 2f)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Orbit);
            return;
        }

        // 1. Si on est assez proche pour attaquer
        if (enemy.PeekBestAttack() != null)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Attack);
            return;
        }

        // 2. Si le joueur s'est trop éloigné
        if (distance > enemy.enemyData.visionRange * 1.5f)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Idle);
            return;
        }

        // 4. Si on n'est pas en orbite (Squelette ou trop loin), on fonce !
        agent.SetDestination(enemy.target.position);

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